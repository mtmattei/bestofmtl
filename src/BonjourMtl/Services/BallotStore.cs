using BonjourMtl.Models;
using Windows.Storage;
namespace BonjourMtl.Services;
public sealed class BallotStore
{
    private const string Key = "bonjour-mtl-ballot-v1";
    public Ballot Load() => Ballot.Deserialize(ApplicationData.Current.LocalSettings.Values[Key] as string);
    public bool Save(Ballot ballot)
    {
        try { ApplicationData.Current.LocalSettings.Values[Key] = ballot.Serialize(); return true; }
        catch (Exception e) { System.Diagnostics.Debug.WriteLine($"Could not save ballot: {e.Message}"); return false; }
    }
}
