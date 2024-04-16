using OriginalCircuit.AltiumSharp.BasicTypes;

namespace AltiumFootprintGenerator;

public class StepModel
{
    public string Model { get; set; }
    public List<CoordPoint> Outline { get; set; }
    public Coord Height { get;  set;}
    public Coord StandoffHeight { get; set; }
}