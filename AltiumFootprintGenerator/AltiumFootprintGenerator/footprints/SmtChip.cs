using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator.footprints;

public abstract class SmtChip : FootprintBase
{
    public abstract override string Name { get; }

    public override string Variation { get; set; } = "";
    public abstract override string Description { get; }
    [JsonIgnore]
    protected abstract StepModel StepModel { get;  }
    protected abstract Dimension TerminalSize { get; }
    protected abstract Dimension Height { get; }
    protected abstract Dimension Width { get; }
    protected abstract Dimension Length { get; }
    
    private void RenderPassivePads(PcbComponent comp, double zMax, double gMin, double xMax)
    {
        for (int i = 0; i < 2; ++i)
        {
            var p = new PcbPad(PcbPadTemplate.SmtTop);
            var xs = (zMax - gMin) / 2;
            var minDim = Math.Min(xs, xMax);
            p.CornerRadius = (byte)Math.Round(100 * Math.Min(GlobalParameters.Cornersize.Limit, GlobalParameters.Cornersize.Relative * minDim) / minDim);
            p.Designator = $"{i + 1}";
            p.Layer = Layer.TopLayer;
            p.Location = CoordPoint.FromMMs((zMax + gMin) / 4  * (i * 2 - 1), 0);
            p.Shape = PcbPadShape.RoundedRectangle;
            p.ShapeTop = PcbPadShape.RoundedRectangle;
            p.Size = CoordPoint.FromMMs(xs, xMax);
            p.StackMode = PcbStackMode.Simple;

            comp.Add(p);
        }
    }
    void RenderComponentCenter(PcbComponent comp, double size)
    {
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, 0, -size / 2, 0, size / 2);
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, -size / 2, 0, size / 2, 0);
    }

    void RenderAssembly(PcbComponent comp)
    {
        comp.Rect(PcbLibrary.Assembly.Top, 0.1, 0, 0, Length.Value, Width.Value);
        var t = new PcbText();
        t.Layer = Layer.Mechanical6;
        t.Text = ".Designator";
        t.TextKind = PcbTextKind.TrueType;

        t.Corner1 = CoordPoint.Zero;
        t.Height = Coord.FromMMs(1);
        comp.Add(t);
    }
    void RenderCourtyard(PcbComponent comp, double zMax, double xMax, double clearance )
    {
        comp.Rect(PcbLibrary.Courtyard.Top, 0.1, 0, 0, zMax + clearance * 2, xMax + clearance * 2);
    }

    void RenderSilk(PcbComponent comp, double zMax, double xMax)
    {
        comp.Rect(Layer.TopOverlay, GlobalParameters.Silk.MinimumWidth, 0, 0, zMax + GlobalParameters.Silk.PadClearance * 2, xMax + GlobalParameters.Silk.PadClearance * 2);
    }

    private double ZMax(SolderGoals goal)
    {
        var zMax = Length.Min + 2 * goal.JToe + Math.Sqrt(Math.Pow(Length.Tolerance, 2) + Math.Pow(GlobalParameters.Tolerances.Fabrication, 2) + Math.Pow(GlobalParameters.Tolerances.Placement, 2));
        zMax = Math.Round(zMax / 2, 2) * 2;
        return zMax;
    }

    private double GMin(SolderGoals goal)
    {
        var gMin = Length.Max - TerminalSize.Min * 2 - 2 * goal.JHeel - Math.Sqrt(Math.Pow(Length.Tolerance + TerminalSize.Tolerance, 2) + Math.Pow(GlobalParameters.Tolerances.Fabrication, 2) + Math.Pow(GlobalParameters.Tolerances.Placement, 2));
        gMin = Math.Round(gMin / 2, 2) * 2;
        return gMin;
    }

    private double XMax(SolderGoals goal)
    {
        var xMax = Width.Min + 2 * goal.JSide + Math.Sqrt(Math.Pow(Width.Tolerance, 2) + Math.Pow(GlobalParameters.Tolerances.Fabrication, 2) + Math.Pow(GlobalParameters.Tolerances.Placement, 2));
        xMax = Math.Round(xMax, 1);
        return xMax;
    }
    
    private void RenderFootprint(PcbComponent comp,  SolderGoals goal)
    {
        comp.Height = Coord.FromMMs(Height.Max);

        var zMax = ZMax(goal);
        var gMin = GMin(goal);
        var xMax = XMax(goal);
        
        RenderPassivePads(comp,  zMax, gMin, xMax);
        RenderComponentCenter(comp, Math.Min(gMin, xMax) / 2);
        RenderCourtyard(comp, zMax, xMax, goal.Courtyard);
        RenderSilk(comp, zMax, xMax);
        RenderAssembly(comp);
    }
    
    
    private void AddBody(PcbComponent comp, string name)
    {
        var b = new PcbComponentBody();
        b.ModelId = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        b.ArcResolution = Coord.FromMils(0.5);
        b.Identifier = name;
        b.StepModel = StepModel.Model;

        b.Layer = Layer.Mechanical9;
        b.V7Layer = Layer.Mechanical9.Name.ToUpper();
        b.Model2DLocation = new CoordPoint(0, 0);
        b.Model2DRotation = 0;
        b.Model3DDz = Coord.FromMMs(0);

        b.TextureCenter = new CoordPoint(0, 0);
        b.TextureSize = new CoordPoint(0, 0);
                
        b.OverallHeight = StepModel.Height;
        b.StandOffHeight = StepModel.StandoffHeight;
        b.TextureSize = CoordPoint.FromMils(0.0001, 0.0001);

        b.Outline.AddRange(StepModel.Outline);
        
        comp.Add(b);
    }


    private SolderGoals Goals(Density density)
    {
        switch (density)
        {
            case Density.Least:
                if (Length.Value > 4.75)
                {
                    return new SolderGoals(0.4, 0, 0, 0.1);
                }
                if (Length.Value > 3.85)
                {
                    return new SolderGoals(0.3, 0, 0, 0.1);
                }
                if (Length.Value > 2.85)
                {
                    return new SolderGoals(0.25, 0, 0, 0.1);
                }
                if (Length.Value > 1.3)
                {
                    return new SolderGoals(0.2, 0, 0, 0.1);
                }
                if (Length.Value > 0.75)
                {
                    return new SolderGoals(0.15, 0, 0, 0.1);
                }
                if (Length.Value > 0.5)
                {
                    return new SolderGoals(0.08, 0, 0, 0.1);
                }
                return new SolderGoals(0.04, 0, 0, 0.1);
            case Density.Nominal:
                if (Length.Value > 4.75)
                {
                    return new SolderGoals(0.5, 0, 0, 0.2);
                }
                if (Length.Value > 3.85)
                {
                    return new SolderGoals(0.4, 0, 0, 0.2);
                }
                if (Length.Value > 2.85)
                {
                    return new SolderGoals(0.35, 0, 0, 0.2);
                }
                if (Length.Value > 1.3)
                {
                    return new SolderGoals(0.3, 0, 0, 0.2);
                }
                if (Length.Value > 0.75)
                {
                    return new SolderGoals(0.2, 0, 0, 0.15);
                }
                if (Length.Value > 0.5)
                {
                    return new SolderGoals(0.1, 0, 0, 0.15);
                }
                return new SolderGoals(0.05, 0, 0, 0.15);
            case Density.Most:
                if (Length.Value > 4.75)
                {
                    return new SolderGoals(0.6, 0, 0, 0.4);
                }
                if (Length.Value > 3.85)
                {
                    return new SolderGoals(0.5, 0, 0, 0.4);
                }
                if (Length.Value > 2.85)
                {
                    return new SolderGoals(0.45, 0, 0, 0.4);
                }
                if (Length.Value > 1.3)
                {
                    return new SolderGoals(0.4, 0, 0, 0.4);
                }
                if (Length.Value > 0.75)
                {
                    return new SolderGoals(0.25, 0, 0, 0.2);
                }
                if (Length.Value > 0.5)
                {
                    return new SolderGoals(0.12, 0, 0, 0.2);
                }
                return new SolderGoals(0.06, 0, 0, 0.2);
            default:
                throw new ArgumentOutOfRangeException(nameof(density), density, null);
        }
    }
    
    private PcbComponent Build(Density density)
    {
        var comp = new PcbComponent();
        comp.ItemGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        comp.Pattern = Name + density.Suffix();
        comp.Description = Description;
        
        RenderFootprint(comp, Goals(density));
        AddBody(comp, comp.Pattern);
        
        return comp;
    }

    public override List<string> Vairants => Enum.GetValues<Density>().Select(x => Name + Variation + x.Suffix()).ToList();

    [JsonIgnore]
    public override PcbComponent Least => Build(Density.Least);
    [JsonIgnore]
    public override PcbComponent Nominal => Build(Density.Nominal);
    [JsonIgnore]
    public override PcbComponent Most => Build(Density.Most);
    public override bool Equals(IFootprint? other)
    {
        if (GetType() != other?.GetType())
        {
            return false;
        }
        
        var oChip = other as SmtChip;
        if (oChip is null)
        {
            return false;
        }

        if (!Name.Equals(other.Name))
        {
            return false;
        }

        foreach (var g in Enum.GetValues<Density>().Select(Goals)) 
        {
            if (Math.Abs(ZMax(g) - oChip.ZMax(g)) >= 0.01)
            {
                return false;
            }
            
            if (Math.Abs(GMin(g) - oChip.GMin(g)) >= 0.01)
            {
                return false;
            }
            
            if (Math.Abs(XMax(g) - oChip.XMax(g)) >= 0.01)
            {
                return false;
            }
        }

        return true;
    }
}