using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common.Extensions;

namespace Sledge.Common.Transport
{
    /// <summary>
    /// Handles serialisation of objects using Valve's common definition format.
    /// </summary>
    [Export]
    public class SerialisedObjectFormatter
    {
        /// <summary>
        /// Serialise an array of objects
        /// </summary>
        /// <param name="serializationStream">The stream to serialise into</param>
        /// <param name="objects">The objects to serialise</param>
        public void Serialize(Stream serializationStream, params SerialisedObject[] objects)
        {
            Serialize(serializationStream, objects.AsEnumerable());
        }

        /// <summary>
        /// Serialise an array of objects
        /// </summary>
        /// <param name="serializationStream">The stream to serialise into</param>
        /// <param name="objects">The objects to serialise</param>
        public void Serialize(Stream serializationStream, IEnumerable<SerialisedObject> objects)
        {
            using (var writer = new StreamWriter(serializationStream, Encoding.UTF8, 1024, true))
            {
                foreach (var obj in objects.Where(x => x != null))
                {
                    Print(obj, writer);
                }
            }
        }

        /// <summary>
        /// Deserialise an array of objects from a stream
        /// </summary>
        /// <param name="serializationStream">The stream to deserialise from</param>
        /// <returns>The deserialised objects</returns>
        public IEnumerable<SerialisedObject> Deserialize(Stream serializationStream)
        {
            using (var reader = new StreamReader(serializationStream, Encoding.UTF8, true, 1024, true))
            {
                return Parse(reader);
            }
        }

        #region Printer
        
        /// <summary>
        /// Ensure a string doesn't exceed a length limit.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <param name="limit">The length limit</param>
        /// <returns>The string, truncated to the limit if it was exceeded</returns>
        private static string LengthLimit(string str, int limit)
        {
            return str.Length >= limit ? str.Substring(0, limit - 1) : str;
        }

        /// <summary>
        /// Print the structure to a stream
        /// </summary>
        /// <param name="obj">The object to print</param>
        /// <param name="tw">The output stream to write to</param>
        /// <param name="tabs">The number of tabs to indent this value to</param>
        private static void Print(SerialisedObject obj, TextWriter tw, int tabs = 0)
        {
            var preTabStr = new string(' ', tabs * 4);
            var postTabStr = new string(' ', (tabs + 1) * 4);
            tw.Write(preTabStr);
            tw.WriteLine(obj.Name);
            tw.Write(preTabStr);
            tw.WriteLine("{");
            foreach (var kv in obj.Properties)
            {
                tw.Write(postTabStr);
                tw.Write('"');
                tw.Write(LengthLimit(kv.Key, 1024));
                tw.Write('"');
                tw.Write(' ');
                tw.Write('"');
                tw.Write(LengthLimit((kv.Value ?? "").Replace('"', '`'), 1024));
                tw.Write('"');
                tw.WriteLine();
            }
            foreach (var child in obj.Children)
            {
                Print(child, tw, tabs + 1);
            }
            tw.Write(preTabStr);
            tw.WriteLine("}");
        }

        #endregion

        #region Parser

        /// <summary>
        /// Parse a structure from a stream
        /// </summary>
        /// <param name="reader">The TextReader to parse from</param>
        /// <returns>The parsed structure</returns>
        public static IEnumerable<SerialisedObject> Parse(TextReader reader)
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
            if (ret.Contains("//")) ret = ret.Substring(0, ret.IndexOf("//", StringComparison.Ordinal)); // Comments
            return ret.Trim();
        }

        /// <summary>
        /// Parse a structure, given the name of the structure
        /// </summary>
        /// <param name="reader">The TextReader to read from</param>
        /// <param name="name">The structure's name</param>
        /// <returns>The parsed structure</returns>
        private static SerialisedObject ParseStructure(TextReader reader, string name)
        {
            var spl = name.SplitWithQuotes();
            var gs = new SerialisedObject(spl[0]);
            string line;
            if (spl.Length != 2 || spl[1] != "{")
            {
                do
                {
                    line = CleanLine(reader.ReadLine());
                } while (String.IsNullOrWhiteSpace(line));
                if (line != "{")
                {
                    return gs;
                }
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
            var split = s.SplitWithQuotes();
            return split.Length == 1 || (split.Length == 2 && split[1] == "{");
        }

        /// <summary>
        /// Check if the given string is a valid property string in the format: "key" "value"
        /// </summary>
        /// <param name="s">The string to test</param>
        /// <returns>True if this is a valid property string, false otherwise</returns>
        private static bool ValidStructPropertyString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            var split = s.SplitWithQuotes();
            return split.Length == 2;
        }

        /// <summary>
        /// Parse a property string in the format: "key" "value", and add it to the structure
        /// </summary>
        /// <param name="gs">The structure to add the property to</param>
        /// <param name="prop">The property string to parse</param>
        private static void ParseProperty(SerialisedObject gs, string prop)
        {
            var split = prop.SplitWithQuotes();
            gs.Properties.Add(new KeyValuePair<string, string>(split[0], (split[1] ?? "").Replace('`', '"')));
        }
        #endregion
    }
}