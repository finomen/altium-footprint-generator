using System.Xml;
using System.Xml.Linq;
using AltiumFootprintGenerator;
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumSymbolGenerator;

public class PinInfo
{
    public string Designator { get; set; }
    public string Name { get; set; }
    public List<string> Functions { get; set; } = new();
    public PinElectricalType Type { get; set; }
    public string Description { get; set; }
    public double PackageLength { get; set; } = 0;
    public double Propagationdelay { get; set; } = 0;


    public SchPin SchPin
    {
        get
        {
            var pin = new SchPin()
            {
                Electrical = Type,
                Name = Name,
                Designator = Designator,
                Description = Description,
                PinPropagationDelay = Propagationdelay,
                //TODO: pin length?
                IsNameVisible = true,
                IsDesignatorVisible = true,
                PinLength = Coord.FromMils(300),
            };
            Functions.ForEach(pin.Functions.Add);
            return pin;
        }
    }

    public int MaximumNameLength => Functions.Select(x => x.Length).Union(new []{Name.Length}).Max() * 50;
}

public enum Position
{
    Left,
    Right,
    Top,
    Bottom,
}

public class PinGroup
{
    public Position Position { get; set; }
    public List<PinInfo> Pins { get; set; }
}

public class Part
{
    public List<PinGroup> PinGroups { get; set; }
}

public class IntegratedCircuit : SymbolBase
{
    public override bool Equals(object? obj)
    {
        if (obj is IntegratedCircuit ic)
        {
            //TODO: make proper comparator
            return Name == ic.Name && Description == ic.Description;
        }

        return false;
    }

    public override string Name { get; set; }
    public override string Description { get; set; }
    public List<Part> Parts { get; set; }

    
    
    private void RenderPart(SchComponent comp, Part part)
    {
        var leftGroups = part.PinGroups.Where(x => x.Position == Position.Left).ToList();
        var rightGroups = part.PinGroups.Where(x => x.Position == Position.Right).ToList();
        var topGroups = part.PinGroups.Where(x => x.Position == Position.Top).ToList();
        var bottomGroups = part.PinGroups.Where(x => x.Position == Position.Bottom).ToList();

        var leftWidth = leftGroups.SelectMany(x => x.Pins.Select(x => x.MaximumNameLength)).Max();
        var rightWidth = rightGroups.SelectMany(x => x.Pins.Select(x => x.MaximumNameLength)).Max();
        
        var topHeight = topGroups.SelectMany(x => x.Pins.Select(x => x.MaximumNameLength)).Max();
        var bottomHeight = bottomGroups.SelectMany(x => x.Pins.Select(x => x.MaximumNameLength)).Max();

        var step = 100;

        var leftHeight = leftGroups.Select(x => (x.Pins.Count - 1) * step).Sum() + (leftGroups.Count - 1) * step;
        var rightHeight = rightGroups.Select(x => (x.Pins.Count - 1) * step).Sum() + (rightGroups.Count - 1) * step;
        
        var topWidth = topGroups.Select(x => (x.Pins.Count - 1) * step).Sum() + (topGroups.Count - 1) * step;
        var bottomWidth = bottomGroups.Select(x => (x.Pins.Count - 1) * step).Sum() + (bottomGroups.Count - 1) * step;

        var width = Math.Max(topWidth, bottomWidth) + leftWidth + rightWidth + step;
        var height = new[] { leftHeight, rightHeight, topHeight + bottomHeight + step }.Max() + step * 2;
        
        comp.Rect(0, 0, width, height);

        var pIdx = 0;
        foreach (var group in leftGroups)
        {
            foreach (var pin in group.Pins)
            {
                var sPin = pin.SchPin;
                sPin.Orientation = TextOrientations.Flipped;
                sPin.Location = CoordPoint.FromMils(-width / 2, height / 2 - step - pIdx * step);
                comp.Add(sPin);
                ++pIdx;
            }

            ++pIdx;
        }
        
        pIdx = 0;
        foreach (var group in rightGroups)
        {
            foreach (var pin in group.Pins)
            {
                var sPin = pin.SchPin;
                sPin.Location = CoordPoint.FromMils(width / 2,  height / 2 - step - pIdx * step);
                comp.Add(sPin);
                ++pIdx;
            }

            ++pIdx;
        }
        
        pIdx = 0;
        foreach (var group in topGroups)
        {
            foreach (var pin in group.Pins)
            {
                var sPin = pin.SchPin;
                sPin.Orientation = TextOrientations.Rotated;
                sPin.Location = CoordPoint.FromMils( pIdx * step - width/2 + leftWidth, height / 2);
                comp.Add(sPin);
                ++pIdx;
            }

            ++pIdx;
        }
        
        pIdx = 0;
        foreach (var group in bottomGroups)
        {
            foreach (var pin in group.Pins)
            {
                var sPin = pin.SchPin;
                sPin.Orientation = TextOrientations.Rotated | TextOrientations.Flipped;
                sPin.Location = CoordPoint.FromMils( pIdx * step - width/2 + leftWidth, -height / 2);
                comp.Add(sPin);
                ++pIdx;
            }

            ++pIdx;
        }
        

    }
    
    private void Render(SchComponent comp)
    {
        for (int i = 0; i < Parts.Count; ++i)
        {
            if (i != 0)
            {
                comp.AddPart();
            }
            RenderPart(comp, Parts[i]);
        }
    }
    
    
    public override SchComponent Component
    {
        get
        {
            var comp = new SchComponent();
            comp.Designator.Text = "U?";
            comp.LibReference = Name;
            comp.Comment.Text = Name;
            comp.ComponentDescription = Description;
            
            Render(comp);
            

            return comp;
        }
    }
}