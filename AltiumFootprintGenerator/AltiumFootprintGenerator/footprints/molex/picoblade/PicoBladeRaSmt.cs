using AltiumFootprintGenerator.step.molex.picoblade;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator.footprints.molex.picoblade;

public class PicoBladeRaSmt : FootprintBase
{
    public override bool Equals(IFootprint? other)
    {
        if (other is PicoBladeRaSmt pb)
        {
            return Color == pb.Color && Coating == pb.Coating && Circuits == pb.Circuits && Name == pb.Name;
        }

        return false;
    }
    
    public Coating Coating { get; set; }
    public HousingColor Color { get; set; } = HousingColor.White;
    public string PartNumber { get; set; }
    public override string Name => PartNumber;
    public int Circuits { get; set; }

    public override string Description => $"Molex PicoBlade {Circuits}-Pin Right-Angle Connector with {Coating} Coating";

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

    public double B => (Circuits - 1) * 1.25;
    public double A => B + 6.4;
    public double C => B + 6;
    public double D => B + 3;
    public double H => 3.4;
    
    void RenderComponentCenter(PcbComponent comp, double size)
    {
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, 0, -size / 2, 0, size / 2);
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, -size / 2, 0, size / 2, 0);
    }

    void RenderPads(PcbComponent comp)
    {
        var mhW = 2.1;
        var mhL = 3;
        var totalW = 3.6 * 2 + B;
        var mhSpacing = totalW - mhW;
        
        for (int i = 0; i < 2; ++i)
        {
            var p = new PcbPad(PcbPadTemplate.SmtTop);
            p.CornerRadius = (byte)Math.Round(100 * GlobalParameters.Cornersize.Limit / 2.1);
            p.Designator = $"MH{i + 1}";
            p.Layer = Layer.TopLayer;
            p.Location = CoordPoint.FromMMs(mhSpacing / 2 - mhSpacing * i,  1.1 );
            p.Shape = PcbPadShape.RoundedRectangle;
            p.ShapeTop = PcbPadShape.RoundedRectangle;
            p.Size = CoordPoint.FromMMs(mhW, mhL);
            p.StackMode = PcbStackMode.Simple;

            comp.Add(p);
        }
        
        for (int i = 0; i < Circuits; ++i)
        {
            var p = new PcbPad(PcbPadTemplate.SmtTop);
            p.CornerRadius = (byte)Math.Round(100 * GlobalParameters.Cornersize.Limit / 0.8);
            p.Designator = $"{i + 1}";
            p.Layer = Layer.TopLayer;
            p.Location = CoordPoint.FromMMs(B/2 - 1.25 * i,  -1.8 );
            p.Shape = PcbPadShape.RoundedRectangle;
            p.ShapeTop = PcbPadShape.RoundedRectangle;
            p.Size = CoordPoint.FromMMs(0.8, 1.6);
            p.StackMode = PcbStackMode.Simple;

            comp.Add(p);
        }
    }

    private void RenderKeepout(PcbComponent comp)
    {
        var sx = 1;
        var sy = 1.8;
        var dx = D - sx;
        var dy = 5 - 1.6;
        var cy = 1.1;
        
        comp.Add(new PcbFill()
        {
            Layer = Layer.TopLayer,
            IsKeepOut = true,
            Corner1 = CoordPoint.FromMMs(-D/2, 3.6),
            Corner2 = CoordPoint.FromMMs(-D/2 + sx, 3.6 - sy),
        });
        comp.Add(new PcbFill()
        {
            Layer = Layer.TopLayer,
            IsKeepOut = true,
            Corner1 = CoordPoint.FromMMs(D/2, 3.6),
            Corner2 = CoordPoint.FromMMs(D/2 - sx, 3.6 - sy),
        });
    
        
        comp.Add(new PcbFill()
        {
            Layer = Layer.TopLayer,
            IsKeepOut = true,
            Corner1 = CoordPoint.FromMMs(-D/2, 3.6 - 5),
            Corner2 = CoordPoint.FromMMs(-D/2 + sx, 3.6  - 5 + sy),
        });
        comp.Add(new PcbFill()
        {
            Layer = Layer.TopLayer,
            IsKeepOut = true,
            Corner1 = CoordPoint.FromMMs(D/2, 3.6 - 5),
            Corner2 = CoordPoint.FromMMs(D/2 - sx, 3.6 - 5 + sy),
        });
    }

    private void Outline(PcbComponent comp, Layer layer, double offset)
    {
        var points = new List<(double x, double y)>()
        {
            (D / 2 + offset, 3.6 + offset),
            (D / 2 + offset, 2.8 + offset),
            (3.6 + B / 2 + offset, 2.8 + offset),
            (3.6 + B / 2 + offset, 1.1 - 1.5 - offset),
            (D / 2 + offset, 1.1 - 1.5 - offset),
            (D / 2 + offset, -1.4 - offset - 1),
            (B / 2 + 0.8 / 2 + offset, -1.4 - offset - 1),
            (B / 2 + 0.8 / 2 + offset, -1.6 - offset - 1),
        };
        var mirror = points.Select(x => (-x.x, x.y)).ToList();
        mirror.Reverse();
        points.AddRange(mirror);
        comp.Polygon(layer, GlobalParameters.Silk.MinimumWidth, points);
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
        
        comp.FullCircle(Layer.TopOverlay, B/2 + 2.5, -1.8, GlobalParameters.Silk.MinimumWidth);
    }
    
    private void RenderFootprint(PcbComponent comp,  SolderGoals goal)
    {
        comp.Height = Coord.FromMMs(H);
        
        RenderPads(comp);
        RenderComponentCenter(comp, 3);
        RenderKeepout(comp);
        RenderCourtyard(comp, goal.Courtyard);
        RenderSilk(comp);
        RenderAssembly(comp);
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

    public PicoBladeRaSmt()
    {
        _stepModel = new Lazy<StepModel>(this.MakeStep);
    }
}