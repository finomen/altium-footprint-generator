using System.Drawing;
using AltiumFootprintGenerator.footprints;
using OCCTS;
using OriginalCircuit.AltiumSharp.BasicTypes;
using Point = OCCTS.Point;

namespace AltiumFootprintGenerator.step;

public static class RESC
{
    public static StepModel MakeStep(this Resc resc)
    {
         double d = Math.Min(resc.L1.Value, resc.L2.Value) / 5;

                var substrate = Shape.Box(resc.L.Value - 2 * d, resc.W.Value, resc.H.Value - 2 * d)
                    .Move(new Vector(-resc.L.Value / 2 + d, -resc.W.Value / 2, -resc.H.Value / 2 + d));
                var overcoat = Shape.Box(resc.L.Value - 2 * resc.L1.Value, resc.W.Value, d)
                    .Move(new Vector(-resc.L.Value / 2 + resc.L1.Value, -resc.W.Value / 2, -d / 2));

                var terminal = () =>
                {
                    var points = new List<Point>()
                    {
                        new(0, 0, -resc.H.Value / 2),
                        new(resc.L2.Value, 0, -resc.H.Value / 2),
                        new(resc.L2.Value, 0, -resc.H.Value / 2 + d),
                        new(d, 0, -resc.H.Value / 2 + d),
                        new(d, 0, resc.H.Value / 2 - d),
                        new(resc.L1.Value, 0, resc.H.Value / 2 - d),
                        new(resc.L1.Value, 0, resc.H.Value / 2),
                        new(0, 0, resc.H.Value / 2),
                    };

                    var poly = Wire.Polygon(points);
                    var face = new Face(poly);
                    var terminal = Shape.Prism(face, new Vector(0, resc.W.Value, 0));

                    var edges = terminal.Edges.Where(x =>
                    {
                        var pts = x.Points.ToList();
                        if (Math.Abs(pts[0].Y - pts[1].Y) < double.Epsilon)
                        {
                            return false;
                        }

                        if (Math.Abs(pts[0].X) > double.Epsilon)
                        {
                            return false;
                        }

                        return true;
                    });

                    terminal = terminal.Fillet(edges, d / 4);

                    terminal = terminal.Cut(Shape.Box(resc.L1.Value / 3, resc.W.Value / 8, 5)
                        .Move(new Vector(resc.L1.Value - resc.L1.Value / 3, 0, resc.H.Value / 2 - d)));
                    terminal = terminal.Cut(Shape.Box(resc.L1.Value / 3, resc.W.Value / 8, 5)
                        .Move(new Vector(resc.L1.Value - resc.L1.Value / 3, resc.W.Value - resc.W.Value / 8, resc.H.Value / 2 - d)));
                    return terminal;
                };


                return new StepModel()
                {
                    Outline = new List<CoordPoint>()
                    {
                        CoordPoint.FromMMs(-resc.L.Value / 2, -resc.W.Value / 2),
                        CoordPoint.FromMMs(resc.L.Value / 2, -resc.W.Value / 2),
                        CoordPoint.FromMMs(resc.L.Value / 2, resc.W.Value / 2),
                        CoordPoint.FromMMs(-resc.L.Value / 2, resc.W.Value / 2),
                    },
                    Height = Coord.FromMMs(resc.H.Value),
                    StandoffHeight = Coord.FromMMs(0),
                    Model = new Assembly()
                        .Add(substrate.Move(new Vector(0, 0, resc.H.Value / 2)), "substrate", Color.Beige)
                        .Add(overcoat.Move(new Vector(0, 0, resc.H.Value / 2 - d / 2)).Move(new Vector(0, 0, resc.H.Value / 2)),
                            "overcoat", Color.Black)
                        .Add(terminal().Move(new Vector(-resc.L.Value / 2, -resc.W.Value / 2, 0)).Move(new Vector(0, 0, resc.H.Value / 2)),
                            "terminal1", Color.Gray)
                        .Add(terminal().Rotate(new Point(0, 0, 0), new Vector(0, 0, 1), Math.PI).Move(new Vector(
                            resc.L.Value / 2,
                            resc.W.Value / 2, 0)).Move(new Vector(0, 0, resc.H.Value / 2)), "terminal2", Color.Gray)
                        .MakeStep()
                };
    }
}