using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;

namespace AltiumFootprintGenerator;

public class PcbLibrary
{
    public class LayerPair(Layer top, Layer bottom, string kind)
    {
        public Layer Top { get; } = top;
        public Layer Bottom { get; } = bottom;
        public string Kind { get; } = kind;

        public string MechKind => Kind.Replace(" ", "");
    }

    public static readonly LayerPair ComponentCenter = new(Layer.Mechanical2, Layer.Mechanical3, "Component Center");
    public static readonly LayerPair Courtyard = new(Layer.Mechanical4, Layer.Mechanical5, "Courtyard");
    public static readonly LayerPair Assembly = new(Layer.Mechanical6, Layer.Mechanical7, "Assembly");
    public static readonly LayerPair Body3D = new(Layer.Mechanical9, Layer.Mechanical10, "3D Body");
    
    private PcbLib _pcbLib = new PcbLib();
    
    private void AddLayerPair(LayerPair pair)
    {
        var l = _pcbLib.Header.Layer[pair.Top.ToByte()  - 1];
        l.MechEnabled = true;
        l.Name = "Top " + pair.Kind;
        l.MechKind = pair.MechKind + "Top";
        _pcbLib.Header.Layer[pair.Top.ToByte() - 1] = l;


        l = _pcbLib.Header.Layer[pair.Bottom.ToByte() - 1];
        l.MechEnabled = true;
        l.Name = "Bottom " + pair.Kind;
        l.MechKind = pair.MechKind + "Bottom";
        _pcbLib.Header.Layer[pair.Bottom.ToByte() - 1] = l;

        _pcbLib.Header.MechanicalPair.Add((pair.Top.Name.Replace(" ", "").ToUpper(), pair.Bottom.Name.Replace(" ", "").ToUpper()));
    }
    
    private void InitLayers()
    {
        AddLayerPair(ComponentCenter);
        AddLayerPair(Courtyard);
        AddLayerPair(Assembly);
        AddLayerPair(Body3D);
    }

    public PcbLibrary()
    {
        InitLayers();
    }

    public void Add(IFootprint footprint)
    {
        _pcbLib.Add(footprint.Least);
        _pcbLib.Add(footprint.Nominal);
        _pcbLib.Add(footprint.Most);
    }

    public void Save(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        using (var writer = new PcbLibWriter())
        {
            writer.Write(_pcbLib, path);
        }
    }
}