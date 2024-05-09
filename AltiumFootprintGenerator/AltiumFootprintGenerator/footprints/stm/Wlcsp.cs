using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;
using AltiumFootprintGenerator.step.stm;

namespace AltiumFootprintGenerator.footprints.stm;

public class Wlcsp : FootprintBase
{
    public override bool Equals(IFootprint? other)
    {
        throw new NotImplementedException();
    }
    
    
    void RenderComponentCenter(PcbComponent comp)
    {
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, 0, -Pitch.Value, 0, Pitch.Value);
        comp.Line(PcbLibrary.ComponentCenter.Top, 0.1, -Pitch.Value, 0, Pitch.Value, 0);
    }

    void RenderAssembly(PcbComponent comp)
    {
        comp.Rect(PcbLibrary.Assembly.Top, 0.1, 0, 0, Width.Value, Length.Value);
        var t = new PcbText();
        t.Layer = Layer.Mechanical6;
        t.Text = ".Designator";
        t.TextKind = PcbTextKind.TrueType;

        t.Corner1 = CoordPoint.Zero;
        t.Height = Coord.FromMMs(1);
        comp.Add(t);
    }
    void RenderCourtyard(PcbComponent comp, double courtyard)
    {
        comp.Rect(PcbLibrary.Courtyard.Top, 0.1, 0, 0, Width.Max + 2 * courtyard, Length.Max + 2 * courtyard);
    }

    void RenderSilk(PcbComponent comp)
    {
        comp.Rect(Layer.TopOverlay, GlobalParameters.Silk.MinimumWidth, 0, 0, Width.Max, Length.Max);
        comp.FullCircle(Layer.TopOverlay, -Width.Max / 2 - GlobalParameters.Silk.PadClearance , Length.Max / 2 + GlobalParameters.Silk.PadClearance , GlobalParameters.Silk.MinimumWidth);
    }

    void RenderPads(PcbComponent comp)
    {
        var num = (int)Math.Sqrt(Pins);
        if (num * num != Pins)
        {
            throw new ArgumentException("Unable do render non-square WLCSP");
        }

        for (int i = 0; i < num; ++i)
        {
            for (int j = 0; j < num; ++j)
            {
                var p = new PcbPad(PcbPadTemplate.SmtTop);
                p.Designator = $"{(char)('A' + j)}{i + 1}";
                p.Layer = Layer.TopLayer;
                p.Location = CoordPoint.FromMMs(i * Pitch.Value - (num - 1) * Pitch.Value / 2,   (num - 1) * Pitch.Value / 2 - j * Pitch.Value);
                p.Shape = PcbPadShape.Round;
                p.ShapeTop = PcbPadShape.Round;
                p.Size = CoordPoint.FromMMs(PadDiameter.Value, PadDiameter.Value);
                p.StackMode = PcbStackMode.Simple;
                comp.Add(p);
            } 
        }
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

    protected StepModel StepModel => _stepModel.Value;

    private Lazy<StepModel> _stepModel;

    public Wlcsp()
    {
        _stepModel = new Lazy<StepModel>(this.MakeStep);
    }
    private void RenderFootprint(PcbComponent comp,  double courtyard)
    {
        comp.Height = Coord.FromMMs(MaximumHeight.Max);
        
        RenderPads(comp);
        RenderComponentCenter(comp);
        RenderCourtyard(comp, courtyard);
        RenderSilk(comp);
        RenderAssembly(comp);
    }
    
    private PcbComponent Build(Density density)
    {
        var comp = new PcbComponent();
        comp.ItemGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        comp.Pattern = Name + density.Suffix();
        comp.Description = Description;

        switch (density)
        {
        case Density.Least:
            RenderFootprint(comp, 0.1);
            break;
        case Density.Nominal:
            RenderFootprint(comp, 0.25);
            break;
        case Density.Most:
            RenderFootprint(comp, 0.5);
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(density), density, null);
        }
        
        AddBody(comp, comp.Pattern);
        
        return comp;
    }

    [JsonIgnore]
    public override PcbComponent Least => Build(Density.Least);
    [JsonIgnore]
    public override PcbComponent Nominal => Build(Density.Nominal);
    [JsonIgnore]
    public override PcbComponent Most => Build(Density.Most);
    
    public override string Name => $"STM_WLCSP{Pins}";

    public override string Description => "Wafer Level Chip Scale Package (WLCSP)";
    
    public override string Variation { get; set; }
    
    public override List<string> Vairants => Enum.GetValues<Density>().Select(x => Name + Variation + x.Suffix()).ToList();
    
    public int Pins { get; set; }
    public Dimension Width { get; set; }
    public Dimension Length { get; set; }
    public Dimension MaximumHeight { get; set; }
    public Dimension Pitch { get; set; }
    public Dimension BallDiameter { get; set; }
    public Dimension PadDiameter { get; set; }
    public Dimension ChipThickness { get; set; }
}