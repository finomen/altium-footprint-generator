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