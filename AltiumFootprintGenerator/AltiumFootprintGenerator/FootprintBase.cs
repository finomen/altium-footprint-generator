using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator;

public abstract class FootprintBase : IFootprint
{
    public abstract bool Equals(IFootprint? other);
    public abstract string Name { get; }
    public abstract string Description { get; set; }
    public string Variation { get; set; } = "";

    protected abstract PcbComponent Build(Density density);

    public List<string> Vairants => Enum.GetValues<Density>().Select(x => Name + Variation + x.Suffix()).ToList();

    [JsonIgnore]
    public PcbComponent Least => Build(Density.Least);
    [JsonIgnore]
    public PcbComponent Nominal => Build(Density.Nominal);
    [JsonIgnore]
    public PcbComponent Most => Build(Density.Most);
    
    [JsonIgnore]
    protected abstract StepModel StepModel { get;  }
    protected void AddBody(PcbComponent comp, string name)
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
}