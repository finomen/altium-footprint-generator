using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AltiumFootprintGenerator
{
    public class Dimension
    {
        public double Value { get; set; }
        public double Tolerance => Math.Max(Max  - Value, Value - Min);

        public double Min { get; set; }

        public double Max { get; set; }
    }

}
