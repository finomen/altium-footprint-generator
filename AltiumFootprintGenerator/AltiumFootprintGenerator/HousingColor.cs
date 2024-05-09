using System.Drawing;

namespace AltiumFootprintGenerator;

public enum HousingColor
{
    Black,
    Natural,
    White,
}

public static class HousingColorExtensions
{
    public static Color Color(this HousingColor color)
    {
        switch (color)
        {
            case HousingColor.Black:
                return System.Drawing.Color.Black;
                break;
            case HousingColor.White:
                return System.Drawing.Color.White;
                break;
            case HousingColor.Natural:
                return System.Drawing.Color.Beige;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}