
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumSymbolGenerator;

public interface ISymbol
{
    string Name { get; set; }
    
    string Description { get; set;}
    
    [JsonIgnore]
    SchComponent Component { get; }
}

public abstract class SymbolBase : ISymbol
{
    public abstract string Name { get; set;}
    public abstract string Description { get; set;}
    public abstract SchComponent Component { get; }
}
