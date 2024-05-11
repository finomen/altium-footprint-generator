using AltiumFootprintGenerator;
using AltiumFootprintGenerator.footprints;
using AltiumFootprintGenerator.footprints.molex.picoblade;
using AltiumSymbolGenerator;
using OriginalCircuit.AltiumSharp.Records;

/*
var lib = new PcbLibrary();


lib.Add(new PicoBladeRaSmt()
{
    Coating = Coating.Gold,
    PartNumber = "TEST PART",
    Circuits = 5,
});

lib.Save("Test.PcbLib");
*/
var slib = new SchLibrary();

slib.Add(new IntegratedCircuit()
{
    Name = "TestIC",
    Description = "Some test IC",
    Parts = new List<Part>()
    {
        new Part()
        {
            PinGroups = new List<PinGroup>()
            {
                new PinGroup()
                {
                    Position = Position.Left,
                    Pins = new List<PinInfo>()
                    {
                        new PinInfo()
                        {
                            Name = "Left 1",
                            Designator = "L1",
                            Type = PinElectricalType.Input
                        },
                        new PinInfo()
                        {
                            Name = "Left 2",
                            Designator = "L2",
                            Type = PinElectricalType.Input
                        },
                        new PinInfo()
                        {
                            Name = "Left 2 With long name",
                            Designator = "L3",
                            Type = PinElectricalType.Input
                        }
                    }
                },
                new PinGroup()
                {
                    Position = Position.Right,
                    Pins = new List<PinInfo>()
                    {
                        new PinInfo()
                        {
                            Name = "Right 1",
                            Designator = "R1",
                            Type = PinElectricalType.InputOutput
                        },
                        new PinInfo()
                        {
                            Name = "Right 2",
                            Designator = "R2",
                            Type = PinElectricalType.InputOutput
                        }
                    }
                },
                new PinGroup()
                {
                    Position = Position.Right,
                    Pins = new List<PinInfo>()
                    {
                        new PinInfo()
                        {
                            Name = "Right A1",
                            Designator = "RA1",
                            Type = PinElectricalType.Output
                        },
                        new PinInfo()
                        {
                            Name = "Right A2",
                            Designator = "RA2",
                            Type = PinElectricalType.Output
                        }
                    }
                },
                new PinGroup()
                {
                    Position = Position.Top,
                    Pins = new List<PinInfo>()
                    {
                        new PinInfo()
                        {
                            Name = "Top 1",
                            Designator = "T1",
                            Type = PinElectricalType.Power
                        },
                        new PinInfo()
                        {
                            Name = "Top 2",
                            Designator = "T2",
                            Type = PinElectricalType.Power
                        }
                    }
                },
                new PinGroup()
                {
                    Position = Position.Bottom,
                    Pins = new List<PinInfo>()
                    {
                        new PinInfo()
                        {
                            Name = "Bottom 1",
                            Designator = "B1",
                            Type = PinElectricalType.Power
                        }
                    }
                }
            }
        }
    }
});

slib.Save("Test.SchLib");