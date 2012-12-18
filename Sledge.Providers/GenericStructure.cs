using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.Providers
{
	/// <summary>
	/// Holds values parsed from a generic Valve structure. It holds a single entity:
	/// entity_name
	/// {
	/// 	"property" "value"
	/// 	subentity_name
	/// 	{
	/// 		"property" "value
	/// 		subentity_name
	/// 		{
	/// 			...
	/// 		}
	/// 	}
	/// }
	/// </summary>
	public class GenericStructure
	{
	    public string Name { get; private set; }
	    public Dictionary<string, string> Properties { get; set; }
	    public List<GenericStructure> Children { get; private set; }

        public string this[string key]
	    {
            get { return Properties.ContainsKey(key) ? Properties[key] : null; }
            set { Properties[key] = value; }
	    }

		public GenericStructure(string name)
		{
		    Name = name;
            Properties = new Dictionary<string, string>();
            Children = new List<GenericStructure>();
		}

        public int PropertyInteger(string name, int defaultValue = 0)
        {
            var prop = this[name];
            int d;
            if (int.TryParse(prop, out d))
            {
                return d;
            }
            return defaultValue;
        }

        public decimal PropertyDecimal(string name, decimal defaultValue = 0)
        {
            var prop = this[name];
            decimal d;
            if (decimal.TryParse(prop, out d))
            {
                return d;
            }
            return defaultValue;
        }

        public decimal[] PropertyDecimalArray(string name, int count)
        {
            var prop = this[name];
            var defaultValue = Enumerable.Range(0, count).Select(i => 0m).ToArray();
            if (prop == null || prop.Count(c => c == ' ') != (count - 1)) return defaultValue;
            var split = prop.Split(' ');
            for (var i = 0; i < count; i++)
            {
                decimal d;
                if (decimal.TryParse(split[i], out d))
                {
                    defaultValue[i] = d;
                }
            }
            return defaultValue;
        }

        public Plane PropertyPlane(string name)
        {
            var prop = this[name];
            var defaultValue = new Plane(Coordinate.UnitZ, 0);
            if (prop == null || prop.Count(c => c == ' ') != 8) return defaultValue;
            var split = prop.Replace("(", "").Replace(")", "").Split(' ');
            decimal x1, x2, x3, y1, y2, y3, z1, z2, z3;
            if (decimal.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x1)
                && decimal.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y1)
                && decimal.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z1)
                && decimal.TryParse(split[3], NumberStyles.Float, CultureInfo.InvariantCulture, out x2)
                && decimal.TryParse(split[4], NumberStyles.Float, CultureInfo.InvariantCulture, out y2)
                && decimal.TryParse(split[5], NumberStyles.Float, CultureInfo.InvariantCulture, out z2)
                && decimal.TryParse(split[6], NumberStyles.Float, CultureInfo.InvariantCulture, out x3)
                && decimal.TryParse(split[7], NumberStyles.Float, CultureInfo.InvariantCulture, out y3)
                && decimal.TryParse(split[8], NumberStyles.Float, CultureInfo.InvariantCulture, out z3))
            {
                return new Plane(
                    new Coordinate(x1, y1, z1).Round(),
                    new Coordinate(x2, y2, z2).Round(),
                    new Coordinate(x3, y3, z3).Round());
            }
            return defaultValue;
        }

        public Coordinate PropertyCoordinate(string name)
        {
            var prop = this[name];
            var defaultValue = Coordinate.Zero;
            if (prop == null || prop.Count(c => c == ' ') != 2) return defaultValue;
            var split = prop.Replace("[", "").Replace("]", "").Split(' ');
            decimal x, y, z;
            if (decimal.TryParse(split[0], out x)
                && decimal.TryParse(split[1], out y)
                && decimal.TryParse(split[2], out z))
            {
                return new Coordinate(x, y, z);
            }
            return defaultValue;
        }

        public Coordinate[] PropertyCoordinateArray(string name, int count)
        {
            var prop = this[name];
            var defaultValue = Enumerable.Range(0, count).Select(i => Coordinate.Zero).ToArray();
            if (prop == null || prop.Count(c => c == ' ') != (count * 3 - 1)) return defaultValue;
            var split = prop.Split(' ');
            for (var i = 0; i < count; i++)
            {
                decimal x, y, z;
                if (decimal.TryParse(split[i * 3], out x)
                    && decimal.TryParse(split[i * 3 + 1], out y)
                    && decimal.TryParse(split[i * 3 + 2], out z))
                {
                    defaultValue[i] = new Coordinate(x, y, z);
                }
            }
            return defaultValue;
        }

        public Tuple<Coordinate, decimal, decimal> PropertyTextureAxis(string name)
        {
            var prop = this[name];
            var defaultValue = Tuple.Create(Coordinate.UnitX, 0m, 1m);
            if (prop == null || prop.Count(c => c == ' ') != 4) return defaultValue;
            var split = prop.Replace("[", "").Replace("]", "").Split(' ');
            decimal x, y, z, sh, sc;
            if (decimal.TryParse(split[0], out x)
                && decimal.TryParse(split[1], out y)
                && decimal.TryParse(split[2], out z)
                && decimal.TryParse(split[3], out sh)
                && decimal.TryParse(split[4], out sc))
            {
                return Tuple.Create(new Coordinate(x, y, z), sh, sc);
            }
            return defaultValue;
        }

        public Color PropertyColour(string name, Color defaultValue)
        {
            var prop = this[name];
            if (prop == null || prop.Count(x => x == ' ') != 2) return defaultValue;
            var split = prop.Split(' ');
            int r, g, b;
            if (int.TryParse(split[0], out r)
                && int.TryParse(split[1], out g)
                && int.TryParse(split[2], out b))
            {
                return Color.FromArgb(r, g, b);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the immediate children of this structure
        /// </summary>
        /// <param name="name">Optional name filter</param>
        /// <returns>A list of children</returns>
        public IEnumerable<GenericStructure> GetChildren(string name = null)
        {
            return Children.Where(x => name == null || x.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Gets all descendants of this structure recursively
        /// </summary>
        /// <param name="name">Optional name filter</param>
        /// <returns>A list of descendants</returns>
        public IEnumerable<GenericStructure> GetDescendants(string name = null)
        {
            return Children.Where(x => name == null || x.Name.ToLower() == name.ToLower())
                .Union(Children.SelectMany(x => x.GetDescendants(name)));
        }

        #region Printer
        public override string ToString()
        {
            var sb = new StringBuilder();
            Print(sb);
            return sb.ToString();
        }

        private void Print(StringBuilder sb, int tabs = 0)
        {
            var preTabStr = new string(' ', tabs * 4);
            var postTabStr = new string(' ', (tabs + 1) * 4);
            sb.Append(preTabStr).AppendLine(Name);
            sb.Append(preTabStr).AppendLine("{");
            foreach (var kv in Properties)
            {
                sb.Append(postTabStr)
                    .Append('"').Append(kv.Key).Append('"')
                    .Append(' ')
                    .Append('"').Append(kv.Value).Append('"')
                    .AppendLine();
            }
            foreach (var child in Children)
            {
                child.Print(sb, tabs + 1);
            }
            sb.Append(preTabStr).AppendLine("}");
        }
        #endregion

        #region Parser
        /// <summary>
        /// Parse a structure from a file
        /// </summary>
        /// <param name="filePath">The file to parse from</param>
        /// <returns>The parsed structure</returns>
        public static IEnumerable<GenericStructure> Parse(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parse a structure from a stream
        /// </summary>
        /// <param name="reader">The TextReader to parse from</param>
        /// <returns>The parsed structure</returns>
        public static IEnumerable<GenericStructure> Parse(TextReader reader)
        {
            string line;
            while ((line = CleanLine(reader.ReadLine())) != null)
            {
                if (ValidStructStartString(line))
                {
                    yield return ParseStructure(reader, line);
                }
            }
        }

        /// <summary>
        /// Remove comments and excess whitespace from a line
        /// </summary>
        /// <param name="line">The unclean line</param>
        /// <returns>The cleaned line</returns>
        private static string CleanLine(string line)
        {
            if (line == null) return null;
            var ret = line;
            if (ret.Contains("//")) ret = ret.Substring(0, ret.IndexOf("//")); // Comments
            return ret.Trim();
        }

	    /// <summary>
	    /// Split a string, but don't split within quoted values.
	    /// </summary>
	    /// <param name="line">The line to split</param>
	    /// <param name="splitTest">Optional split test. Defaults to whitespace test.</param>
	    /// <param name="quoteChar">Optional quote character. Defaults to double quote.</param>
	    /// <returns>An array of split values</returns>
	    private static string[] SplitWithQuotes(string line, Func<char, bool> splitTest = null, char quoteChar = '"')
        {
            if (splitTest == null) splitTest = Char.IsWhiteSpace;
            var result = new List<string>();
            var index = 0;
            var inQuote = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (splitTest(c) && index == i)
                {
                    index = i + 1;
                }
                else if (c == quoteChar)
                {
                    inQuote = !inQuote;
                }
                else if (splitTest(c) && !inQuote)
                {
                    result.Add(line.Substring(index, i - index).Trim(quoteChar));
                    index = i + 1;
                }
                if (i != line.Length - 1) continue;
                result.Add(line.Substring(index, (i + 1) - index).Trim(quoteChar));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Parse a structure, given the name of the structure
        /// </summary>
        /// <param name="reader">The TextReader to read from</param>
        /// <param name="name">The structure's name</param>
        /// <returns>The parsed structure</returns>
		private static GenericStructure ParseStructure(TextReader reader, string name)
        {
            var gs = new GenericStructure(SplitWithQuotes(name)[0]);
	        var line = CleanLine(reader.ReadLine());
			if (line != "{") {
				return gs;
			}
            while ((line = CleanLine(reader.ReadLine())) != null)
			{
				if (line == "}") break;

				if (ValidStructPropertyString(line)) ParseProperty(gs, line);
				else if (ValidStructStartString(line)) gs.Children.Add(ParseStructure(reader, line));
			}
			return gs;
		}

        /// <summary>
        /// Check if the given string is a valid structure name
        /// </summary>
        /// <param name="s">The string to test</param>
        /// <returns>True if this is a valid structure name, false otherwise</returns>
	    private static bool ValidStructStartString(string s)
		{
			if (string.IsNullOrEmpty(s)) return false;
            var split = SplitWithQuotes(s);
            return split.Length == 1;
		}

        /// <summary>
        /// Check if the given string is a valid property string in the format: "key" "value"
        /// </summary>
        /// <param name="s">The string to test</param>
        /// <returns>True if this is a valid property string, false otherwise</returns>
		private static bool ValidStructPropertyString(string s)
		{
			if (string.IsNullOrEmpty(s)) return false;
            var split = SplitWithQuotes(s);
            return split.Length == 2;
		}

        /// <summary>
        /// Parse a property string in the format: "key" "value", and add it to the structure
        /// </summary>
        /// <param name="gs">The structure to add the property to</param>
        /// <param name="prop">The property string to parse</param>
		private static void ParseProperty(GenericStructure gs, string prop)
		{
            var split = SplitWithQuotes(prop);
            gs[split[0]] = split[1];
		}
        #endregion
	}
}
