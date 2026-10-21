using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.UI;

namespace BonjourMtl;

/// <summary>Bundled, credited photography. No remote image requests at runtime.</summary>
public static class Photography
{
    public static ImageBrush Brush(string name) => new()
    {
        ImageSource = new BitmapImage(new Uri($"ms-appx:///Assets/Photography/{name}.jpg")),
        Stretch = Stretch.UniformToFill,
        AlignmentX = AlignmentX.Center,
        AlignmentY = AlignmentY.Center
    };

    public static string ForCategory(string id) => id switch
    {
        "bagels" or "bakery" => "bagels",
        "coffee" => "coffee",
        "poutine" => "poutine",
        "music" => "music",
        "parks" or "garden" or "wellness" => "parks",
        "learning" => "biosphere",
        "housing" or "books" => "neighbourhood",
        "auto" or "services" => "street",
        _ => "city"
    };

    public static LinearGradientBrush Shade(bool mobile)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = mobile ? new Point(.3, 1) : new Point(1, .15)
        };
        brush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(mobile ? (byte)140 : (byte)255, 21, 21, 21), Offset = 0 });
        brush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(mobile ? (byte)210 : (byte)245, 21, 21, 21), Offset = mobile ? .65 : .35 });
        brush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(mobile ? (byte)250 : (byte)30, 21, 21, 21), Offset = 1 });
        return brush;
    }
}
