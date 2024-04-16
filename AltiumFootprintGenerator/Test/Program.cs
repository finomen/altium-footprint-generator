using AltiumFootprintGenerator;
using AltiumFootprintGenerator.footprints;



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

lib.Save("Test.PcbLib");


