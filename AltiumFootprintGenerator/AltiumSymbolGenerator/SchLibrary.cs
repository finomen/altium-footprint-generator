using OriginalCircuit.AltiumSharp;

namespace AltiumSymbolGenerator;

public class SchLibrary
{
    private SchLib _schLib = new SchLib();
    
   
    public void Add(ISymbol symbol)
    {
        _schLib.Add(symbol.Component);
    }

    public void Save(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        using (var writer = new SchLibWriter())
        {
            writer.Write(_schLib, path);
        }
    }
}