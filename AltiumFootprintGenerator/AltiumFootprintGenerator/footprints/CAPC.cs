using AltiumFootprintGenerator.step;

namespace AltiumFootprintGenerator.footprints;

public class Capc : SmtChip
{
    public override string Name => $"CAPC{(int)(Length.Value * 10):00}{(int)(Width.Value * 10):00}X{(int)(Height.Value * 100):000}";
    public override string Description => "Capacitors, Chip, Non-polarized";

    protected override StepModel StepModel => _stepModel.Value;

    private Lazy<StepModel> _stepModel;

    public Capc()
    {
        _stepModel = new Lazy<StepModel>(this.MakeStep);
    }

    protected override Dimension TerminalSize => B;
    protected override Dimension Height => H;
    protected override Dimension Width => W;
    protected override Dimension Length => L;
    public Dimension L { get; set; }
    public Dimension W { get; set; }
    public Dimension H { get; set; }
    public Dimension B { get; set; }
}