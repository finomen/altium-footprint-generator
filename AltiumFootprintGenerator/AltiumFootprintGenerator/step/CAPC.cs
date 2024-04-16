using System.Drawing;
using AltiumFootprintGenerator.footprints;
using OCCTS;
using OriginalCircuit.AltiumSharp.BasicTypes;
using Point = OCCTS.Point;

namespace AltiumFootprintGenerator.step;

public static class CAPC
{
    public static StepModel MakeStep(this Capc capc)
    {
        var substrate = Shape.Box(capc.L.Value - 2 * capc.B.Value, capc.W.Value, capc.H.Value)
            .Move(new Vector(-capc.L.Value / 2 + capc.B.Value, -capc.W.Value / 2, -capc.H.Value / 2));

        var terminal = () =>
        {
            var terminal = Shape.Box(capc.B.Value, capc.W.Value, capc.H.Value)
                .Move(new Vector(-capc.B.Value / 2, -capc.W.Value / 2, -capc.H.Value / 2));

            var edges = terminal.Edges.Where(x =>
            {
                var pts = x.Points.ToList();
                if (Math.Abs(pts[0].X - pts[1].X) > double.Epsilon)
                {
                    return false;
                }

                if (pts[0].X > 0)
                {
                    return false;
                }

                return true;
            });

            terminal = terminal.Fillet(edges, capc.B.Value / 5);

            return terminal;
        };


        return new StepModel()
        {
            Outline = new List<CoordPoint>()
            {
                CoordPoint.FromMMs(-capc.L.Value / 2, -capc.W.Value / 2),
                CoordPoint.FromMMs(capc.L.Value / 2, -capc.W.Value / 2),
                CoordPoint.FromMMs(capc.L.Value / 2, capc.W.Value / 2),
                CoordPoint.FromMMs(-capc.L.Value / 2, capc.W.Value / 2),
            },
            Height = Coord.FromMMs(capc.H.Value),
            StandoffHeight = Coord.FromMMs(0),
            Model = new Assembly()
                .Add(substrate.Move(new Vector(0, 0, capc.H.Value / 2)), "substrate", Color.Orange)
                .Add(terminal().Move(new Vector(-capc.L.Value / 2 + capc.B.Value / 2, 0, capc.H.Value / 2)),
                    "terminal1", Color.Gray)
                .Add(terminal().Rotate(new Point(0, 0, 0), new Vector(0, 0, 1), Math.PI).Move(new Vector(
                    capc.L.Value / 2 - capc.B.Value / 2,
                    0, capc.H.Value / 2)), "terminal2", Color.Gray)
                .MakeStep()
        };
    }
}