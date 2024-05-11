using AltiumFootprintGenerator.step;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator.footprints;

public class Qfp : FootprintBase
{
    public Qfp()
    {
        //_stepModel = new Lazy<StepModel>(this.MakeStep);
        _stepModel = new Lazy<StepModel>(() => new StepModel()
        {
            Outline = new List<CoordPoint>(),
            Height = Coord.FromMMs(1),
            Model = "",
        });
    }
    
    public override bool Equals(IFootprint? other)
    {
        if (other is Qfp qfn)
        {
            if (Name != qfn.Name)
            {
                return false;
            }

            if (Description != qfn.Description)
            {
                return false;
            }

            if (HPins == qfn.HPins && VPins == qfn.VPins)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    
    public int HPins { get; set; }
    public int VPins { get; set; }
    public int Pins => (HPins + VPins) * 2;
    public double Pitch { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    
    public double BodyLength { get; set; }
    public double BodyWidth { get; set; }
    public double Thickness { get; set; }
    
    public double PadWidth { get; set; }
    public double PadLength { get; set; }
    
    public string NameBase { get; set; } = "QFP";
     
    public override string Name => $"{NameBase}{Pitch:0.00}P{Length:0.0}X{Width:0.0}X{Thickness:0.0}-{Pins}";
    public override string Description { get; set; }
    
    void RenderComponentCenter(PcbComponent comp, double size)
    {
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, 0, -size / 2, 0, size / 2);
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, -size / 2, 0, size / 2, 0);
    }

    private void Outline(PcbComponent comp, Layer layer, double offset)
    {
        //TODO: outline including pads
        comp.Rect(layer, GlobalParameters.Silk.MinimumWidth, 0, 0, BodyWidth + offset * 2, BodyLength + offset * 2);
    }

    private void RenderCourtyard(PcbComponent comp, double offset)
    {
        Outline(comp, PcbLibrary.Courtyard.Top, offset);
    }

    private void RenderAssembly(PcbComponent comp)
    {
        Outline(comp, PcbLibrary.Assembly.Top, 0);
    }
    
    private void RenderSilk(PcbComponent comp, double offset)
    {
        //Outline(comp, Layer.TopOverlay, GlobalParameters.Silk.PadClearance + offset);
        comp.FullCircle(Layer.TopOverlay, -Width / 2 - offset - GlobalParameters.Silk.PadClearance - GlobalParameters.Silk.MinimumWidth * 3, Length / 2 , GlobalParameters.Silk.MinimumWidth);
    }

    private void RenderPads(PcbComponent comp, SolderGoals goal)
    {
        //TODO: calculate properly!
        var xs = PadLength + goal.JHeel + goal.JToe;
        var ys = PadWidth + goal.JSide * 2;
        var pad = (int idx) =>
        {
            var p = new PcbPad(PcbPadTemplate.SmtTop);
            var minDim = Math.Min(xs, ys);
            p.CornerRadius = (byte)Math.Round(100 *
                Math.Min(GlobalParameters.Cornersize.Limit, GlobalParameters.Cornersize.Relative * minDim) / minDim);
            p.Designator = $"{idx + 1}";
            p.Layer = Layer.TopLayer;
            p.Shape = PcbPadShape.RoundedRectangle;
            p.ShapeTop = PcbPadShape.RoundedRectangle;
            p.Size = CoordPoint.FromMMs(xs, ys);
            p.StackMode = PcbStackMode.Simple;
            return p;
        };
        
        for (int i = 0; i < VPins; ++i)
        {
            var p = pad(i);
            p.Location = CoordPoint.FromMMs(-Width / 2 + xs / 2- goal.JToe, (VPins - 1) / 2.0 * Pitch - i * Pitch);
            comp.Add(p);
            p = pad(i + VPins + HPins);
            p.Location = CoordPoint.FromMMs(Width / 2 - xs /2 + goal.JToe, -(VPins - 1) / 2.0 * Pitch + i * Pitch);
            comp.Add(p);
        }
        
        for (int i = 0; i < HPins; ++i)
        {
            var p = pad(i + VPins);
            p.Size = CoordPoint.FromMMs(ys, xs);
            
            p.Location = CoordPoint.FromMMs(-(HPins  - 1) / 2.0 * Pitch + i * Pitch, -Length / 2 + xs / 2 - goal.JToe);
            comp.Add(p);
            
            p = pad(i + VPins * 2 + HPins);
            p.Size = CoordPoint.FromMMs(ys, xs);
            p.Location = CoordPoint.FromMMs((HPins  - 1) / 2.0 * Pitch - i * Pitch, Length / 2 - xs / 2 + goal.JToe);
            comp.Add(p);
        }

    }
    private void RenderFootprint(PcbComponent comp,  SolderGoals goal)
    {
        comp.Height = Coord.FromMMs(Thickness);
        
        RenderPads(comp, goal);
        RenderComponentCenter(comp, Math.Min(Width, Length) / 4);
        RenderCourtyard(comp, goal.Courtyard + goal.JToe);
        RenderSilk(comp, goal.JToe);
        RenderAssembly(comp);
    }
    
    private SolderGoals Goals(Density density)
    {
        switch (density)
        {
            //FIXME: ipc suggests side fillet -0.04
            case Density.Least:
                return new SolderGoals(0.15, 0.25, -0, 0.1);
            case Density.Nominal:
                return new SolderGoals(0.35, 0.35, -0, 0.25);
            case Density.Most:
                return new SolderGoals(0.55, 0.45, -0, 0.5);
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