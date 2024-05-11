using AltiumFootprintGenerator.step;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator.footprints;

public class Bga : FootprintBase
{
    public Bga()
    {
        _stepModel = new Lazy<StepModel>(this.MakeStep);
    }
    
    public override bool Equals(IFootprint? other)
    {
        if (other is Bga bga)
        {
            if (Name != bga.Name)
            {
                return false;
            }

            if (Description != bga.Description)
            {
                return false;
            }

            if (PresentPins is null || PresentPins.Count == 0)
            {
                if (bga.PresentPins is null || bga.PresentPins.Count == 0)
                {
                    return true;
                }
            }
            else
            {
                if (bga.PresentPins is not null && bga.PresentPins.Count != 0)
                {
                    return PresentPins.SequenceEqual(bga.PresentPins);
                }
            }
        }
        return false;
    }
    
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int Pins => Rows * Columns;
    public double Pitch { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Thickness { get; set; }
    public double StandoffHeight { get; set; }
    public double BallDiameter { get; set; }
    // TODO: calculate
    public double PadSize { get; set; }

    public bool CollapsingBalls { get; set; } = false;

    public string NameBase { get; set; } = "BGA";
    
    public List<string>? PresentPins { get; set; }

    public string RemovedPinSpec
    {
        get
        {
            if (PresentPins is null || PresentPins.Count == 0)
            {
                return "";
            }

            return $"-{Pins}_{PresentPins.Count}";
        }
    }
    
    public override string Name => $"{NameBase}{Pins}{(CollapsingBalls ? "C" : "N")}{Pitch:0.00}P{Columns}X{Rows}_{Length:0.0}X{Width:0.0}X{Thickness:0.0}{RemovedPinSpec}";
    public override string Description { get; set; }

    private static string BallName(int row, int column)
    {
        string name = "";
        do
        {
            name = name + (char)('A' + column % ('Z' - 'A'));
            column /= 'Z' - 'A';
        } while (column > 'Z' - 'A');

        var arr = name.ToCharArray();
        Array.Reverse(arr);
        name = new string(arr);

        name = name + $"{row + 1}";
        return name;
    }

    public List<(string Designator, double x, double y)> Balls
    {
        get
        {
            List<(string Designator, double x, double y)> allPins = 
                Enumerable.Range(0, Rows).SelectMany(row => Enumerable.Range(0, Columns).Select(col => (BallName(row, col), col * Pitch - (Columns - 1) * Pitch / 2,  (Rows - 1) * Pitch / 2 - row * Pitch ))).ToList();
            
            if (PresentPins is not null && PresentPins.Count > 0)
            {
                return allPins.Where(x => PresentPins.Contains(x.Designator)).ToList();
            }

            return allPins;
        }
    }
    
    void RenderComponentCenter(PcbComponent comp, double size)
    {
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, 0, -size / 2, 0, size / 2);
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, -size / 2, 0, size / 2, 0);
    }

    private void Outline(PcbComponent comp, Layer layer, double offset)
    {
        comp.Rect(layer, GlobalParameters.Silk.MinimumWidth, 0, 0, Width + offset * 2, Length + offset * 2);
    }

    private void RenderCourtyard(PcbComponent comp, double offset)
    {
        Outline(comp, PcbLibrary.Courtyard.Top, offset);
    }

    private void RenderAssembly(PcbComponent comp)
    {
        Outline(comp, PcbLibrary.Assembly.Top, 0);
    }
    
    private void RenderSilk(PcbComponent comp)
    {
        Outline(comp, Layer.TopOverlay, GlobalParameters.Silk.PadClearance);
        
        comp.FullCircle(Layer.TopOverlay, -Width / 2 - Pitch, Length / 2 + Pitch, GlobalParameters.Silk.MinimumWidth);
    }

    private void RenderPads(PcbComponent comp)
    {
        foreach (var ball in Balls)
        {
            var pad = new PcbPad()
            {
                Layer = Layer.TopLayer,
                Shape = PcbPadShape.Round,
                Size = CoordPoint.FromMMs(PadSize, PadSize),
                Location = CoordPoint.FromMMs(ball.x, ball.y),
                Designator = ball.Designator
            };
            comp.Add(pad);
        }
    }
    private void RenderFootprint(PcbComponent comp,  SolderGoals goal)
    {
        comp.Height = Coord.FromMMs(Thickness);
        
        RenderPads(comp);
        RenderComponentCenter(comp, Math.Min(Width, Length) / 4);
        RenderCourtyard(comp, goal.Courtyard);
        RenderSilk(comp);
        RenderAssembly(comp);
    }
    
    private SolderGoals Goals(Density density)
    {
        switch (density)
        {
            case Density.Least:
                return new SolderGoals(0.04, 0, 0, 0.1);
            case Density.Nominal:
                return new SolderGoals(0.04, 0, 0, 0.2);
            case Density.Most:
                return new SolderGoals(0.04, 0, 0, 0.3);
            default:
                throw new ArgumentOutOfRangeException(nameof(density), density, null);
        }
    }
    
    protected override PcbComponent Build(Density density)
    {
        var comp = new PcbComponent();

        comp.ItemGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        comp.Pattern = Name + density.Suffix();
        comp.Description = Description;
        
        RenderFootprint(comp, Goals(density));
        AddBody(comp, comp.Pattern);
        
        return comp;
    }

    protected override StepModel StepModel => _stepModel.Value;

    private Lazy<StepModel> _stepModel;

}