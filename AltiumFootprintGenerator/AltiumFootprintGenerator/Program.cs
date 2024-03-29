using OriginalCircuit.AltiumSharp;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

var prms = new ParameterCollection();
var hdr = new PcbLibHeader();

using (var reader = new PcbLibReader())
{
    // Read the file.
    var pcbLib = reader.Read("test-in.PcbLib");

    // Iterate through each component in the library.
    foreach (var component in pcbLib)
    {
        // Print information about the component.
        Console.WriteLine($"Name: {component.Pattern}");
        Console.WriteLine($"Number of Pads: {component.Pads}");
        Console.WriteLine($"Number of Primitives: {component.Primitives.Count()}");
    }

    pcbLib.Header.ExportToParameters(prms);
    hdr = pcbLib.Header;
    // Retrieve settings from the header.
    //_displayUnit = pcbLib.Header.DisplayUnit;
    //_snapGridSize = pcbLib.Header.SnapGridSize;
}


using (var writer = new PcbLibWriter())
{


    var pcbLib = new PcbLib();
    //pcbLib.Header.ImportFromParameters(prms);
    var comp = new PcbComponent();


    comp.Pattern = "PName";
    comp.Description = "PDesc";
    comp.Height = Coord.FromMMs(0.5);
    //comp.Pads = 2;

    var p = new PcbPad(PcbPadTemplate.SmtTop);
    p.CornerRadius = 50;
    p.CornerRadiusBot = 50;
    p.CornerRadiusMid = 50;
    p.CornerRadiusTop = 50;
    p.Designator = "1";
    p.Flags = PcbFlags.Unlocked; // Unknown 8
    p.HoleSize = Coord.FromMMs(0);
    p.IsFabricationBottom = false;
    p.IsFabricationTop = false;
    p.IsKeepOut = false;
    p.IsLocked = false;
    p.IsPlated = true;
    p.IsTentingBottom = false;
    p.IsTentingTop = false;
    //p.IsVisible = true;
    p.Layer = Layer.TopLayer;
    p.Location = CoordPoint.FromMMs(-1, 0);
    p.OffsetFromHoleCenter = CoordPoint.FromMMs(0, 0);
    p.PasteMaskExpansionManual = false;
    p.Rotation = 0; // 90?
    p.Shape = PcbPadShape.RoundedRectangle;
    p.ShapeTop = PcbPadShape.RoundedRectangle;
    p.Size = CoordPoint.FromMMs(0.5, 0.6);
    p.StackMode = PcbStackMode.Simple;

    comp.Add(p);

    p = new PcbPad(PcbPadTemplate.SmtTop);
    p.CornerRadius = 50;
    p.CornerRadiusBot = 50;
    p.CornerRadiusMid = 50;
    p.CornerRadiusTop = 50;
    p.Designator = "2";
    p.Flags = PcbFlags.Unlocked; // Unknown 8
    p.HoleSize = Coord.FromMMs(0);
    p.IsFabricationBottom = false;
    p.IsFabricationTop = false;
    p.IsKeepOut = false;
    p.IsLocked = false;
    p.IsPlated = true;
    p.IsTentingBottom = false;
    p.IsTentingTop = false;
    //p.IsVisible = true;
    p.Layer = Layer.TopLayer;
    p.Location = CoordPoint.FromMMs(1, 0);
    p.OffsetFromHoleCenter = CoordPoint.FromMMs(0, 0);
    p.PasteMaskExpansionManual = false;
    p.Rotation = 0; // 90?
    p.Shape = PcbPadShape.RoundedRectangle;
    p.ShapeTop = PcbPadShape.RoundedRectangle;
    p.Size = CoordPoint.FromMMs(0.5, 0.6);
    p.StackMode = PcbStackMode.Simple;
    comp.Add(p);


    var t = new PcbTrack();
    t.Start = CoordPoint.FromMMs(-1, -0.5);
    t.End = CoordPoint.FromMMs(1, -0.5);
    t.Layer = Layer.Mechanical6;
    t.Width = Coord.FromMMs(0.05);
    comp.Add(t);

    t = new PcbTrack();
    t.Start = CoordPoint.FromMMs(-1, 0.5);
    t.End = CoordPoint.FromMMs(1, 0.5);
    t.Layer = Layer.Mechanical6;
    t.Width = Coord.FromMMs(0.05);
    comp.Add(t);

    t = new PcbTrack();
    t.Start = CoordPoint.FromMMs(-1, 0.5);
    t.End = CoordPoint.FromMMs(-1, -0.5);
    t.Layer = Layer.Mechanical6;
    t.Width = Coord.FromMMs(0.05);
    comp.Add(t);

    t = new PcbTrack();
    t.Start = CoordPoint.FromMMs(1, -0.5);
    t.End = CoordPoint.FromMMs(1, 0.5);
    t.Layer = Layer.Mechanical6;
    t.Width = Coord.FromMMs(0.05);
    comp.Add(t);

    pcbLib.Add(comp);

    //pcbLib.Header.LayerV8 = hdr.LayerV8;

    writer.Write(pcbLib, "test-out.PcbLib");

}