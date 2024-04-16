namespace AltiumFootprintGenerator;

public class Tolerances
{
    public double Fabrication { get; set; }
    public double Placement { get; set; }
}

public class Silk
{
    public double MinimumWidth { get; set; }
    public double PadClearance { get; set; }
}

public class Cornersize
{
    public double Relative { get; set; }
    public double Limit { get; set; }
}

public static class GlobalParameters
{
    public static Tolerances Tolerances { get; } = new Tolerances()
    {
        Fabrication = 0,
        Placement = 0,
    };

    public static Silk Silk { get; } = new Silk()
    {
        MinimumWidth = 0.16,
        PadClearance = 0.2,
    };

    public static Cornersize Cornersize { get; } = new Cornersize()
    {
        Relative = 0.25,
        Limit = 0.25
    };
}