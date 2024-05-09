using System.Drawing;
using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumFootprintGenerator;

public static class SchComponentExtensions
{
    public static void Rect(this SchComponent comp, double x, double y, double w, double h)
    {
        var r = new SchRectangle();
        
        r.Location = CoordPoint.FromMils(x + w/2, y + h/2);
        r.Corner = CoordPoint.FromMils(x -w/2, y-h/2);
        r.LineWidth = LineWidth.Smallest;
        r.AreaColor = Color.FromArgb(255, 255, 255, 176);
        r.Color = Color.FromArgb(255, 128, 0, 0);
        comp.Add(r);
    }
}