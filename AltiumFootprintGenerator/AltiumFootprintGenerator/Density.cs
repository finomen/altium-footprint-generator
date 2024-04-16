namespace AltiumFootprintGenerator;

public enum Density
{
    Least,
    Nominal,
    Most
}

public static class DensityTraits
{
    public static string Suffix(this Density d)
    {
        switch (d)
        {
            case Density.Least: return "L";
            case Density.Nominal: return "N";
            case Density.Most: return "M";
            default: throw new ArgumentException("Unknown density");
        }
    }
}