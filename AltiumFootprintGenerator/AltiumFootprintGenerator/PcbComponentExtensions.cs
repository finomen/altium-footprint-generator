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
    
    public static void Polygon(this PcbComponent comp, Layer layer, double lw, List<(double x, double y)> points)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            comp.Line(layer, lw, points[i].x, points[i].y, points[(i + 1) % points.Count].x, points[(i + 1) % points.Count].y);
        }
    }

    public static void FullCircle(this PcbComponent comp, Layer layer, double x, double y, double r)
    {
        var c = new PcbArc();
        c.Location = CoordPoint.FromMMs(x, y);
        c.Radius = Coord.FromMMs(r / 2);
        c.Width = Coord.FromMMs(r);
        c.StartAngle = 0;
        c.EndAngle = 360;
        c.Layer = layer;
        comp.Add(c);
    }
}