using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Editing.Components.Compile.Specification
{
    public class CompileParameter
    {
        public string Name { get; set; }
        public string Flag { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string Value { get; set; }
        public CompileParameterType Type { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public int Precision { get; set; }
        public List<string> Options { get; set; }
        public List<string> OptionValues { get; set; }
        public string Filter { get; set; }

        public decimal DecimalValue
        {
            get
            {
                decimal d;
                return Decimal.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0;
            }
        }

        public string ChoiceValue
        {
            get
            {
                if (!Options.Any()) return null;
                var idx = Options.FindIndex(x => String.Equals(x, Value, StringComparison.InvariantCultureIgnoreCase));
                return Options[idx < 0 || idx >= Options.Count ? 0 : idx];
            }
        }

        public Color ColourValue
        {
            get
            {
                var spl = (Value ?? "").Split(' ');
                float r, g, b;
                if (spl.Length == 3 && float.TryParse(spl[0], out r) && float.TryParse(spl[1], out g) && float.TryParse(spl[2], out b))
                {
                    if (Type == CompileParameterType.ColourFloat)
                    {
                        r *= 255;
                        g *= 255;
                        b *= 255;
                    }
                    return Color.FromArgb((int) r, (int) g, (int) b);
                }
                return Color.Black;
            }
        }

        public static CompileParameter Parse(SerialisedObject gs)
        {
            var param = new CompileParameter
            {
                Name = gs.Get("Name", ""),
                Flag = gs.Get("Flag", ""),
                Description = gs.Get("Description", ""),
                Enabled = gs.Get("Enabled", true),
                Value = gs.Get("Value", ""),
                Type = gs.Get("Type", CompileParameterType.Checkbox),
                Min = gs.Get("Min", Decimal.MinValue),
                Max = gs.Get("Max", Decimal.MaxValue),
                Precision = gs.Get("Precision", 1),
                Options = gs.Get("Options", "").Split(',').ToList(),
                OptionValues = gs.Get("OptionValues", "").Split(',').ToList(),
                Filter = gs.Get("Filter", "")
            };
            return param;
        }

        public string GetDefaultArgumentString()
        {
            if (!Enabled) return "";
            var arg = Flag;
            if (!String.IsNullOrWhiteSpace(Value) && Type != CompileParameterType.Checkbox) arg += " " + Value;
            return arg;
        }
    }
}