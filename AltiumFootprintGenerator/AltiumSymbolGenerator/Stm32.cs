using System.Xml;
using System.Xml.Linq;
using AltiumFootprintGenerator;
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumSymbolGenerator;

public class Stm32 : ISymbol
{
    public enum PinType
    {
        Power,
        Io,
        Input,
        Output
    }

    public class PinInfo
    {
        public PinType Type { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public List<string> Functions { get; set; } = new();

        [JsonIgnore] public bool IsVcc => Type == PinType.Power && (Name == "VCC" || Name == "VDD" || Name == "VDDA");
        [JsonIgnore] public bool IsGnd => Type == PinType.Power && (Name == "VSS" || Name == "GND" || Name == "VSSA");

        [JsonIgnore]
        public SchPin Pin
        {
            get
            {
                var res = new SchPin()
                {
                    Electrical = PinElectricalType.Power,
                    Name = Name,
                    Designator = Location,
                    IsNameVisible = true,
                    IsDesignatorVisible = true,
                    PinLength = Coord.FromMils(200),
                };
                Functions.ForEach(res.Functions.Add);
                return res;
            }
        }
    }

    public string Name { get; set; }
    public string Package { get; set; }
    public List<PinInfo> Pins { get; set; } = new List<PinInfo>();

    public static Stm32 FromXml(string xml)
    {
        var res = new Stm32();
        var doc = XDocument.Parse(xml);

        res.Name = doc.Root.Attribute("RefName").Value;
        res.Package = doc.Root.Attribute("Package").Value;
        
        var pinNodes = doc.Root.Elements().Where(x => x.Name.LocalName == "Pin");

        foreach (var pin in pinNodes)
        {
            var info = new PinInfo();
            info.Location = pin.Attribute("Position").Value;
            

            switch (pin.Attribute("Type").Value)
            {
                case "Power":
                    info.Type = PinType.Power;
                    break;
                case "I/O":
                    info.Type = PinType.Io;
                    break;
                case "Reset":
                    info.Type = PinType.Input;
                    break;
                default:
                    throw new ArgumentException($"Unknown pin type {pin.Attribute("Type").Value}");
            }

            info.Functions = pin.Elements().Where(x => x.Name.LocalName == "Signal")
                .Select(x => x.Attribute("Name").Value).ToList();
            
            var name = pin.Attribute("Name").Value;

            if (name.Contains("/"))
            {
                var nParts = name.Split("/");
                name = nParts[0];
                info.Functions.AddRange(nParts.Skip(1));
            }

            info.Name = name;
            
            res.Pins.Add(info);
        }
        
        

        return res;
    }

    public string Description => $"{Name} in {Package} package";


    private void RenderPower(SchComponent comp)
    {
        comp.AddPart();

        var pwrPins = Pins.Where(x => x.Type == PinType.Power);

        var powerPins = pwrPins.Where(x => x.IsVcc).ToList();
        var gndPins = pwrPins.Where(x => x.IsGnd).ToList();
        var miscPins = pwrPins.Where(x => !x.IsGnd && !x.IsVcc).ToList();

        var sidePins = Math.Max(powerPins.Count + miscPins.Count + 1, gndPins.Count);

        var step = 100;
        var pos = sidePins / 2 * step;
        var width = 800;

        var lPos = pos;
        var rPos = pos;

        comp.Rect(0, sidePins % 2 == 0 ? step / 2 : 0, width, (sidePins + 1) * step);
        
        foreach (var pin in miscPins)
        {
            var p = pin.Pin;
            p.Location = CoordPoint.FromMils(-width / 2, lPos);
            p.Orientation = TextOrientations.Flipped;
            lPos -= step;
            comp.Add(p);
        }
        
        lPos -= step;
        
        foreach (var pin in powerPins)
        {
            var p = pin.Pin;
            p.Location = CoordPoint.FromMils(-width / 2, lPos);
            p.Orientation = TextOrientations.Flipped;
            lPos -= step;
            comp.Add(p);
        }
        
        foreach (var pin in gndPins)
        {
            var p = pin.Pin;
            p.Location = CoordPoint.FromMils(width / 2, rPos);
            rPos -= step;
            comp.Add(p);
        }
    }
    
    
    public SchComponent Component
    {
        get
        {
            var comp = new SchComponent();
            comp.Designator.Name = "U?";
            comp.LibReference = Name;
            comp.ComponentDescription = Description;
            
            RenderPower(comp);
            

            return comp;
        }
    }
}