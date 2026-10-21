using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace BonjourMtl.Models;

public record Nominee(string Id, string Name);
public record Category(string Id, string Group, string English, string French, string Description,
    string DescriptionFr, string Art, string Color, Nominee[] Nominees)
{
    public string Name(bool french) => french ? French : English;
}
/// <summary>A local-only ballot, never an official vote.</summary>
public sealed class Ballot
{
    public Dictionary<string, string> Choices { get; set; } = new();
    public string? SubmittedOn { get; set; }
    public Dictionary<string, string> Receipt { get; set; } = new();
    public string Language { get; set; } = "en";
    public NominationDraft? Nomination { get; set; }
    public bool IsSubmitted(DateOnly day) => SubmittedOn == day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    public void Choose(string categoryId, string nomineeId, DateOnly day)
    {
        if (IsSubmitted(day)) throw new InvalidOperationException("Demo ballot already saved today.");
        var category = Catalog.Categories.SingleOrDefault(c => c.Id == categoryId) ?? throw new ArgumentException("Unknown category.");
        if (!category.Nominees.Any(n => n.Id == nomineeId)) throw new ArgumentException("Unknown nominee.");
        Choices[categoryId] = nomineeId;
    }
    public void Remove(string categoryId, DateOnly day)
    {
        if (IsSubmitted(day)) throw new InvalidOperationException("Demo ballot already saved today.");
        Choices.Remove(categoryId);
    }
    public void SubmitDemo(DateOnly day)
    {
        if (Choices.Count == 0) throw new InvalidOperationException("Choose at least one place.");
        if (IsSubmitted(day)) throw new InvalidOperationException("Demo ballot already saved today.");
        Receipt = new(Choices);
        SubmittedOn = day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
    public void RestartDemo() { SubmittedOn = null; Receipt.Clear(); }
    public string Serialize() => JsonSerializer.Serialize(this, BallotJsonContext.Default.Ballot);
    public static Ballot Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new();
        try
        {
            var state = JsonSerializer.Deserialize(json, BallotJsonContext.Default.Ballot) ?? new();
            state.Choices ??= new(); state.Receipt ??= new();
            state.Choices = state.Choices.Where(ValidChoice).ToDictionary(x => x.Key, x => x.Value);
            state.Receipt = state.Receipt.Where(ValidChoice).ToDictionary(x => x.Key, x => x.Value);
            state.Language = state.Language == "fr" ? "fr" : "en";
            return state;
        }
        catch (JsonException) { return new(); }
    }
    private static bool ValidChoice(KeyValuePair<string,string> item) =>
        Catalog.Categories.Any(c => c.Id == item.Key && c.Nominees.Any(n => n.Id == item.Value));
    public static DateOnly MontrealToday(DateTimeOffset now) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById("America/Toronto")).DateTime);
    public static string SearchKey(string input) =>
        string.Concat(input.Normalize(NormalizationForm.FormD).Where(c =>
            CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        .Normalize(NormalizationForm.FormC).ToLowerInvariant();
}
public record NominationDraft(string Name, string City, string Category, string Website);

[JsonSerializable(typeof(Ballot))]
internal partial class BallotJsonContext : JsonSerializerContext;
