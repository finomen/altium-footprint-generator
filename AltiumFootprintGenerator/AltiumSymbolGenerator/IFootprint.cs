
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumSymbolGenerator;

public interface ISymbol
{
    string Name { get; }
    
    string Description { get; }
    
    [JsonIgnore]
    SchComponent Component { get; }
}

