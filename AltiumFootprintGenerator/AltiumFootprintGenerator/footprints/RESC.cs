using AltiumFootprintGenerator.step;

namespace AltiumFootprintGenerator.footprints;

public class Resc : SmtChip
{
    public override string Name => $"RESC{(int)(Length.Value * 10):00}{(int)(Width.Value * 10):00}X{(int)(Height.Value * 100):000}";
    public override string Description => "Resistors, Chip";

    protected override StepModel StepModel => _stepModel.Value;

    private Lazy<StepModel> _stepModel;

    public Resc()
    {
        _stepModel = new Lazy<StepModel>(this.MakeStep);
    }

    protected override Dimension TerminalSize => L2;
    protected override Dimension Height => H;
    protected override Dimension Width => W;
    protected override Dimension Length => L;
    public Dimension L { get; set; }
    public Dimension W { get; set; }
    public Dimension H { get; set; }
    public Dimension L1 { get; set; } // Top
    public Dimension L2 { get; set; } // Bottom
}