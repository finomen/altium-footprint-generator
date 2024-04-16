using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator;

public static class PcbComponentExtensions
{
    public static void Line(this PcbComponent comp, Layer layer, double w, double xs, double ys, double xe, double ye)
    {
        var t = new PcbTrack();
        t.Start = CoordPoint.FromMMs(xs, ys);
        t.End = CoordPoint.FromMMs(xe, ye);
        t.Layer = layer;
        t.Width = Coord.FromMMs(w);
        comp.Add(t);
    }

    public static void Rect(this PcbComponent comp, Layer layer, double lw, double x, double y, double w, double h)
    {
        comp.Line(layer, lw, x -w / 2, y - h / 2, x + w / 2, y -h / 2);
        comp.Line( layer, lw, x + w / 2, y - h / 2, x + w / 2, y + h / 2);
        comp.Line( layer, lw, x + w / 2, y + h / 2, x - w / 2, y + h / 2);
        comp.Line( layer, lw, x - w / 2, y + h / 2, x - w / 2, y - h / 2);
    }
}