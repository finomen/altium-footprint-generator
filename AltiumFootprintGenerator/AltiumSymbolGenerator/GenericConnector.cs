﻿using System.Xml;
using System.Xml.Linq;
using AltiumFootprintGenerator;
using Newtonsoft.Json;
using OriginalCircuit.AltiumSharp.BasicTypes;
using OriginalCircuit.AltiumSharp.Records;

namespace AltiumSymbolGenerator;

public class GenericConnector : ISymbol
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public int Pins { get; set; }

    public int MountingHoles { get; set; } = 0;

    private void RenderPower(SchComponent comp)
    {
        var vSpace = Pins;
        if (MountingHoles > 0)
        {
            vSpace += MountingHoles + 1;
        }
        
        var step = 100;
        var pos = vSpace / 2 * step;
        var width = 400;

        comp.Rect(0, vSpace % 2 == 0 ? step / 2 : 0, width, (vSpace + 1) * step);

        for (var i = 1; i <= Pins; ++i)
        {
            var p = new SchPin()
            {
                Electrical = PinElectricalType.Passive,
                Name = $"{i}",
                Designator = $"{i}",
                IsNameVisible = true,
                IsDesignatorVisible = true,
                PinLength = Coord.FromMils(250),
            };
            p.Location = CoordPoint.FromMils(-width / 2, pos);
            p.Orientation = TextOrientations.Flipped;
            pos -= step;
            comp.Add(p);
        }
        
         pos -= step;
         
         for (var i = 1; i <= MountingHoles; ++i)
         {
             var p = new SchPin()
             {
                 Electrical = PinElectricalType.Power,
                 Name = $"MH{i}",
                 Designator = $"MH{i}",
                 IsNameVisible = true,
                 IsDesignatorVisible = true,
                 PinLength = Coord.FromMils(250),
             };
             p.Location = CoordPoint.FromMils(-width / 2, pos);
             p.Orientation = TextOrientations.Flipped;
             pos -= step;
             comp.Add(p);
         }
    }
    
    
    public SchComponent Component
    {
        get
        {
            var comp = new SchComponent();
            comp.Designator.Text = "J?";
            comp.LibReference = Name;
            comp.Comment.Text = Name;
            comp.ComponentDescription = Description;
            
            RenderPower(comp);
            

            return comp;
        }
    }
}