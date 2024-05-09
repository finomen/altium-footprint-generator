using System.Drawing;
using AltiumFootprintGenerator.footprints.molex.picoblade;
using OCCTS;
using OriginalCircuit.AltiumSharp.BasicTypes;
using Point = OCCTS.Point;

namespace AltiumFootprintGenerator.step.molex.picoblade;

public static class PicoBladeRaSmtExtensions
{
    private static Shape LoadStep(string name)
    {
        using (Stream stream = typeof(PicoBladeRaSmt).Assembly.GetManifestResourceStream($"AltiumFootprintGenerator.assets.molex.picoblade.{name}.STEP"))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return Shape.LoadStep(result);
            }
        }
    }
    public static StepModel MakeStep(this PicoBladeRaSmt fp)
    {
        var mh = LoadStep("SMT-RA-MH");
        var pin = LoadStep("SMT-RA-Pin");
        var rep = LoadStep("SMT-RA-REP");
        var side = LoadStep("SMT-RA-SIDE");

        var asm = new Assembly();

        asm.Add(mh
            .Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
            .Move(new Vector(-fp.B / 2 - 2 + 1.48 - 1.23, 0, 0)), "MH1", Color.Gray);
        asm.Add(mh
            .Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
            .Rotate(new Point(0,0,0), new Vector(0,0,1), Double.Pi)
            .Move(new Vector(fp.B / 2 + 2 - 1.48 + 1.23, 2.2, 0)), "MH2", Color.Gray);
        
        var body = side
            .Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
            .Rotate(new Point(0,0,0), new Vector(0,0,1), Double.Pi)
            .Move(new Vector(fp.B / 2 + 1.25 / 2 - 0.015 /* compensate math error*/, -2 + 1.11, 0));
        
        

        for (int i = 0; i < fp.Circuits; ++i)
        {
            asm.Add(pin.Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
                    .Rotate(new Point(0,0,0), new Vector(0,0,1), Double.Pi)
                    .Move(new Vector(fp.B / 2 - i * 1.25, -2, 0)), $"Pin{i}", fp.Coating.Color());
            
            body = body.Merge(rep.Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
                    .Rotate(new Point(0,0,0), new Vector(0,0,1), Double.Pi)
                    .Move(new Vector(fp.B / 2 - i * 1.25, -2 + 1.11, 0.2)));
        }
        
        body = body.Merge(side
            .Rotate(new Point(0,0,0), new Vector(1,0,0), Double.Pi/2)
            .Rotate(new Point(0,0,0), new Vector(0,0,1), Double.Pi)
            .Mirror(new Point(0,0,0), new Vector(1,0,0))
            .Move(new Vector(-fp.B / 2 - 1.25/2, -2 + 1.11, 0)));
        

        var cutout = Shape.Prism(new Face(Wire.Polygon(new List<Point>()
        {
            new(-0.15, 3.2, 0.0),
            new(0.15, 3.2, 0.0),
            new(0.15, 3.2 - 1.45, 0.0),
            new(1.25 / 2, 3.2 - 1.45, 0.0),
            new(1.25 / 2, 3.2 - 1.45 - 1.15, 0.0),
            new(-1.25 / 2, 3.2 - 1.45 - 1.15, 0.0),
            new(-1.25 / 2, 3.2 - 1.45, 0.0),
            new(-0.15, 3.2 - 1.45, 0.0),
        })), new Vector(0, 0, 0.8));

        if (fp.Circuits <= 3)
        {
            body = body.Cut(cutout);
        }
        else
        {
            body = body.Cut(cutout.Move(new Vector(-fp.B/2 + 1.25/2, 0, 0)));
            body = body.Cut(cutout.Move(new Vector(fp.B/2 - 1.25/2, 0, 0)));
        }

        asm.Add(body, "Body", fp.Color.Color());
        
        return new StepModel()
        {
            Outline = new List<CoordPoint>()
            {
                CoordPoint.FromMMs(-fp.A / 2, 3.2),
                CoordPoint.FromMMs(fp.A / 2, 3.2),
                CoordPoint.FromMMs(fp.A / 2, -2),
                CoordPoint.FromMMs(-fp.A / 2, -2),
            },
            Height = Coord.FromMMs(fp.H),
            StandoffHeight = Coord.FromMMs(0),
            Model = asm.MakeStep(),
        };
    }
}