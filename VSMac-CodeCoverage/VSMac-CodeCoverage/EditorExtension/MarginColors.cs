using Gdk;
using MonoDevelop.Components;

namespace CodeCoverage
{
  struct MarginColors
  {
    public Color Foreground { get; }
    public Color BackgroundCovered { get; }
    public Color BackgroundUncovered { get; }

    public static MarginColors DefaultColors = new MarginColors(
      new Color(255, 255, 255),
      new Color(46, 125, 51),
      new Color(125, 51, 46)
    );

    public MarginColors(Color foreground, Color backgroundCovered, Color backgroundUncovered)
    {
      Foreground = foreground;
      BackgroundCovered = backgroundCovered;
      BackgroundUncovered = backgroundUncovered;
    }

    public static bool TryParse(string s, out MarginColors colors)
    {
      colors = default;

      var parts = s.Split(',');
      if (parts.Length != 3) return false;

      Color foreground = Color.Zero;
      Color backgroundCovered = Color.Zero;
      Color backgroundUncovered = Color.Zero;

      if (!Color.Parse(parts[0], ref foreground) ||
        !Color.Parse(parts[1], ref backgroundCovered) ||
        !Color.Parse(parts[2], ref backgroundUncovered))
        return false;

      colors = new MarginColors(foreground, backgroundCovered, backgroundUncovered);
      return true;
    }

    public override string ToString()
    {
      return $"{Foreground.GetHex()},{BackgroundCovered.GetHex()},{BackgroundUncovered.GetHex()}";
    }
  }
}
