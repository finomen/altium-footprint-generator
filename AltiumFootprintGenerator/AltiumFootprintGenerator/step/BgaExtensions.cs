using System.Drawing;
using AltiumFootprintGenerator.footprints;
using OCCTS;
using OriginalCircuit.AltiumSharp.BasicTypes;
using Point = OCCTS.Point;

namespace AltiumFootprintGenerator.step;

public static class BgaExtensions
{
    public static StepModel MakeStep(this Bga bga)
    {

        var body = Shape.Box(bga.Width, bga.Length, bga.Thickness - bga.StandoffHeight).Move(new Vector(-bga.Width/2, -bga.Length / 2,  bga.StandoffHeight));
        var marking = Shape.Cylinder(bga.BallDiameter / 2, 0.1)
            .Move(new Vector(-bga.Width / 2 + bga.BallDiameter * 2, bga.Length / 2 - bga.BallDiameter * 2,
                bga.Thickness - 0.09));
        
        // For unknown reason cutting marking from body breaks color rendering in altium

        var ball = Shape.Sphere(bga.BallDiameter / 2);
        
        var assy = new Assembly();
        assy.Add(body, "Body", Color.Black);
        
        assy.Add(marking, "Marking", Color.White);
        
        foreach (var b in bga.Balls)
        {
            assy.Add(ball.Move(new Vector(b.x, b.y, bga.BallDiameter / 2)), b.Designator, Color.Gray);
        }

        return new StepModel()
        {
            Outline = new List<CoordPoint>()
            {
                CoordPoint.FromMMs(-bga.Width / 2, -bga.Length / 2),
                CoordPoint.FromMMs(bga.Width / 2, -bga.Length / 2),
                CoordPoint.FromMMs(bga.Width / 2, bga.Length / 2),
                CoordPoint.FromMMs(-bga.Width / 2, bga.Length / 2),
            },
            Height = Coord.FromMMs(bga.Thickness),
            StandoffHeight = Coord.FromMMs(0),
            Model = assy.MakeStep()
        };
    }
}