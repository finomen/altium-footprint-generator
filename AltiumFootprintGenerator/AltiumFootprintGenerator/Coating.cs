using System.Drawing;

namespace AltiumFootprintGenerator;

public enum Coating
{
    Tin,
    Gold
}

public static class CoatingExtensions
{
    public static Color Color(this Coating coating)
    {
        switch (coating)
        {
            case Coating.Tin:
                return System.Drawing.Color.Gray;
                break;
            case Coating.Gold:
                return System.Drawing.Color.Gold;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(coating), coating, null);
        }
    }
}