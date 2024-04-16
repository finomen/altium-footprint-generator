using AltiumFootprintGenerator.footprints;
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp;

namespace AltiumFootprintGenerator;

public interface IFootprint : IEquatable<IFootprint>
{
    [JsonIgnore]
    PcbComponent Least { get; }
    [JsonIgnore]
    PcbComponent Nominal { get; }
    [JsonIgnore]
    PcbComponent Most { get; }
    
    string Name { get; }
    
    string Description { get; }
    
    string Variation { get; set; }
    
    List<string> Vairants { get; }
}

public abstract class FootprintBase : IFootprint
{
    public abstract bool Equals(IFootprint? other);
    public abstract PcbComponent Least { get; }
    public abstract PcbComponent Nominal { get; }
    public abstract PcbComponent Most { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Variation { get; set; }
    public abstract List<string> Vairants { get; }
}