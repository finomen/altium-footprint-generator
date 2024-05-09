using AltiumFootprintGenerator;
using AltiumFootprintGenerator.footprints;
using AltiumFootprintGenerator.footprints.molex.picoblade;
using AltiumSymbolGenerator;


var lib = new PcbLibrary();


lib.Add(new PicoBladeRaSmt()
{
    Coating = Coating.Gold,
    PartNumber = "TEST PART",
    Circuits = 5,
});

lib.Save("Test.PcbLib");

var slib = new SchLibrary();

slib.Add(new GenericConnector()
{
    Name = "532614015",
    Description = "1.25mm Pitch, PicoBlade PCB Header, Single Row, Right-Angle, Surface Mount, Gold (Au) Flash Plating, Friction Lock, 15 Circuits, Natural, Tape and Reel",
    Pins = 15,
    MountingHoles = 2,
});

slib.Save("Test.SchLib");