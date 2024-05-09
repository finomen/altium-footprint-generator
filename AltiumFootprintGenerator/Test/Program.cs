using AltiumFootprintGenerator;
using AltiumFootprintGenerator.footprints;
using AltiumFootprintGenerator.footprints.stm;
using AltiumFootprintGenerator.step.stm;
using AltiumSymbolGenerator;


var lib = new PcbLibrary();

lib.Add(new Resc()
{
    L = new Dimension() { Value = 6.35, Tolerance = 0.1},
    W = new Dimension() { Value = 3.1, Tolerance = 0.15},
    H = new Dimension() { Value = 0.55, Tolerance = 0.1},
    L1 = new Dimension() { Value = 0.6, Tolerance = 0.2},
    L2 = new Dimension() { Value = 0.6, Tolerance = 0.2}
});

lib.Add(new Capc()
{
    L = new Dimension() { Value = 1.55, Tolerance = 0.05},
    W = new Dimension() { Value = 0.85, Tolerance = 0.05},
    H = new Dimension() { Value = 0.45, Tolerance = 0.05},
    B = new Dimension() { Value = 0.35, Tolerance = 0.15},
});

lib.Add(new Wlcsp()
{
    Pins = 49,
    Width = new Dimension() {Value = 3.141, Min = 3.106, Max = 3.176},
    Length = new Dimension() {Value = 3.127, Min = 3.092, Max = 3.162},
    MaximumHeight  = new Dimension() {Value = 0.38, Tolerance = 0},
    PadDiameter = new Dimension() {Value = 0.21, Tolerance = 0},
    Pitch  = new Dimension() {Value = 0.35, Tolerance = 0},
    BallDiameter  = new Dimension() {Value = 0.22, Tolerance = 0.03},
    ChipThickness  = new Dimension() {Value = 0.22, Tolerance = 0.03},
});

lib.Save("Test.PcbLib");

var slib = new SchLibrary();

using (StreamReader file = new StreamReader("pinout.xml"))
{
    slib.Add(Stm32.FromXml(file.ReadToEnd()));
}

slib.Save("Test.SchLib");