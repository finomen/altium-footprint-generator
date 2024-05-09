using System.Drawing;
using AltiumFootprintGenerator.footprints.stm;
using OCCTS;
using OriginalCircuit.AltiumSharp.BasicTypes;
using Point = OCCTS.Point;

namespace AltiumFootprintGenerator.step.stm;

public static class WLCSP
{
    public static StepModel MakeStep(this Wlcsp wlcsp)
    {
        
        
        
        var assy = new Assembly();
        
        var num = (int)Math.Sqrt(wlcsp.Pins);
        if (num * num != wlcsp.Pins)
        {
            throw new ArgumentException("Unable do render non-square WLCSP");
        }
        
        var marking = () =>
        {
            return Shape.Cylinder(wlcsp.PadDiameter.Value, wlcsp.ChipThickness.Value / 2)
                .Move(new Vector(-wlcsp.Width.Value / 2 + wlcsp.PadDiameter.Value * 2,
                    wlcsp.Length.Value / 2 - wlcsp.PadDiameter.Value * 2,
                    wlcsp.MaximumHeight.Value - wlcsp.ChipThickness.Value / 2));
        };

        var body = () =>
        {
            var body = Shape.Box(wlcsp.Width.Value, wlcsp.Length.Value, wlcsp.ChipThickness.Value)
                .Move(new Vector(-wlcsp.Width.Value / 2, -wlcsp.Length.Value / 2,
                    wlcsp.MaximumHeight.Value - wlcsp.ChipThickness.Value));

            //FIXME: for unknown reason this makes altium render body white body = body.Cut(marking());
            return body;
        };

        assy.Add(body(), "body", Color.Black);
        assy.Add(marking().Move(new Vector(0, 0, 0.001)), "pin1", Color.White);
        
        for (int i = 0; i < num; ++i)
        {
            for (int j = 0; j < num; ++j)
            {
                var s = Shape.Sphere(wlcsp.BallDiameter.Value / 2).Move(new Vector(i * wlcsp.Pitch.Value - (num - 1) * wlcsp.Pitch.Value / 2,
                    (num - 1) * wlcsp.Pitch.Value / 2 - j * wlcsp.Pitch.Value, wlcsp.BallDiameter.Value / 2));
                assy.Add(s, $"{(char)('A' + j)}{i + 1}", Color.Gray);
            } 
        }
        

        return new StepModel()
        {
            Outline = new List<CoordPoint>()
            {
                CoordPoint.FromMMs(-wlcsp.Width.Value / 2, -wlcsp.Length.Value / 2),
                CoordPoint.FromMMs(wlcsp.Width.Value / 2, -wlcsp.Length.Value / 2),
                CoordPoint.FromMMs(wlcsp.Width.Value / 2, wlcsp.Length.Value / 2),
                CoordPoint.FromMMs(-wlcsp.Width.Value / 2, wlcsp.Length.Value / 2),
            },
            Height = Coord.FromMMs(wlcsp.MaximumHeight.Value),
            StandoffHeight = Coord.FromMMs(0),
            Model = assy.MakeStep()
        };
    }
}