using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sledge.DataStructures.GameData;

namespace Sledge.Providers.GameData
{
    public class FgdProvider
    {
        private String CurrentFile { get; set; }

        public DataStructures.GameData.GameData OpenFile(string filename)
        {
            return GetFromFile(filename);
        }

        private DataStructures.GameData.GameData GetFromFile(string filename)
        {
            if (!File.Exists(filename)) throw new ProviderException("File does not exist: " + filename);
            CurrentFile = filename;

            DataStructures.GameData.GameData parsed;

            using (var s = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                parsed = GetFromStream(s);
            }

            CurrentFile = null;
            return parsed;
        }

        private DataStructures.GameData.GameData GetFromStream(Stream stream)
        {
            var lex = Lex(new StreamReader(stream));
            return Parse(lex.Where(l => l.Type != LexType.Comment).ToList());
        }

        private DataStructures.GameData.GameData Parse(IEnumerable<LexObject> lex)
        {
            var gd = new DataStructures.GameData.GameData();
            var iterator = lex.GetEnumerator();
            while (true)
            {
                if (!iterator.MoveNext()) break;
                if (iterator.Current.Type == LexType.At)
                {
                    ParseAt(gd, iterator);
                }
            }
            return gd;
        }

        private void ParseAt(DataStructures.GameData.GameData gd, IEnumerator<LexObject> iterator)
        {
            iterator.MoveNext();
            var type = iterator.Current.Value;
            if (type.Equals("include", StringComparison.InvariantCultureIgnoreCase))
            {
                Expect(iterator, LexType.String);
                if (CurrentFile != null)
                {
                    var filename = iterator.Current.GetValue();
                    var path = Path.GetDirectoryName(CurrentFile) ?? "";
                    var incfile = Path.Combine(path, filename);

                    var current = CurrentFile;
                    var incgd = GetFromFile(incfile);
                    CurrentFile = current;

                    if (!gd.Includes.Any(x => String.Equals(x, filename, StringComparison.InvariantCultureIgnoreCase))) gd.Includes.Add(filename);

                    // Merge the included gamedata into the current one
                    gd.MapSizeHigh = Math.Max(incgd.MapSizeHigh, gd.MapSizeHigh);
                    gd.MapSizeLow = Math.Min(incgd.MapSizeLow, gd.MapSizeLow);
                    gd.Includes.AddRange(incgd.Includes.Where(x => !gd.Includes.Contains(x)));
                    gd.Classes.AddRange(incgd.Classes.Where(x => !gd.Classes.Any(y => String.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase))));
                    gd.AutoVisgroups.AddRange(incgd.AutoVisgroups.Where(x => !gd.AutoVisgroups.Any(y => String.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase))));
                    gd.MaterialExclusions.AddRange(incgd.MaterialExclusions.Where(x => !gd.MaterialExclusions.Any(y => String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase))));
                }
                else
                {
                    throw new ProviderException("Unable to include a file when not reading from a file.");
                }
            }
            else if (type.Equals("mapsize", StringComparison.InvariantCultureIgnoreCase))
            {
                Expect(iterator, LexType.OpenParen);
                Expect(iterator, LexType.Value);
                gd.MapSizeLow = Int32.Parse(iterator.Current.Value);
                Expect(iterator, LexType.Comma);
                Expect(iterator, LexType.Value);
                gd.MapSizeHigh = Int32.Parse(iterator.Current.Value);
                Expect(iterator, LexType.CloseParen);
            }
            else if (type.Equals("materialexclusion", StringComparison.InvariantCultureIgnoreCase))
            {
                Expect(iterator, LexType.OpenBracket);
                iterator.MoveNext();
                while (iterator.Current.Type != LexType.CloseBracket)
                {
                    Assert(iterator.Current, iterator.Current.IsValueOrString(), "Expected value type, got " + iterator.Current.Type + ".");
                    var exclusion = iterator.Current.GetValue();
                    gd.MaterialExclusions.Add(exclusion);
                    iterator.MoveNext();
                }
            }
            else if (type.Equals("autovisgroup", StringComparison.InvariantCultureIgnoreCase))
            {
                Expect(iterator, LexType.Equals);

                iterator.MoveNext();
                Assert(iterator.Current, iterator.Current.IsValueOrString(), "Expected value type, got " + iterator.Current.Type + ".");
                var sectionName = iterator.Current.GetValue();
                var sect = new AutoVisgroupSection {Name = sectionName};

                Expect(iterator, LexType.OpenBracket);
                iterator.MoveNext();
                while (iterator.Current.Type != LexType.CloseBracket)
                {
                    Assert(iterator.Current, iterator.Current.IsValueOrString(), "Expected value type, got " + iterator.Current.Type + ".");
                    var groupName = iterator.Current.GetValue();
                    var grp = new AutoVisgroup {Name = groupName};

                    Expect(iterator, LexType.OpenBracket);
                    iterator.MoveNext();
                    while (iterator.Current.Type != LexType.CloseBracket)
                    {
                        Assert(iterator.Current, iterator.Current.IsValueOrString(), "Expected value type, got " + iterator.Current.Type + ".");
                        var entity = iterator.Current.GetValue();
                        grp.EntityNames.Add(entity);
                        iterator.MoveNext();
                    }

                    sect.Groups.Add(grp);
                }

                gd.AutoVisgroups.Add(sect);
            }
            else
            {
                // Parsing:
                // @TypeClass name(param, param) name()
                var ct = ParseClassType(type, iterator.Current);
                var gdo = new GameDataObject("", "", ct);
                iterator.MoveNext();
                while (iterator.Current.Type == LexType.Value)
                {
                    // Parsing:
                    // @TypeClass {name(param, param) name()}
                    var name = iterator.Current.Value;
                    var bh = new Behaviour(name);
                    iterator.MoveNext();
                    if (iterator.Current.Type == LexType.Value)
                    {
                        // Allow for the following (first seen in hl2 base):
                        // @PointClass {halfgridsnap} base(Targetname)
                        continue;
                    }
                    Assert(iterator.Current, iterator.Current.Type == LexType.OpenParen, "Unexpected " + iterator.Current.Type);
                    iterator.MoveNext();
                    while (iterator.Current.Type != LexType.CloseParen)
                    {
                        // Parsing:
                        // name({param, param})
                        if (iterator.Current.Type != LexType.Comma)
                        {
                            Assert(iterator.Current, iterator.Current.Type == LexType.Value || iterator.Current.Type == LexType.String,
                                "Unexpected " + iterator.Current.Type + ".");
                            var value = iterator.Current.Value;
                            if (iterator.Current.Type == LexType.String) value = value.Trim('"');
                            bh.Values.Add(value);
                        }
                        iterator.MoveNext();
                    }
                    Assert(iterator.Current, iterator.Current.Type == LexType.CloseParen, "Unexpected " + iterator.Current.Type);
                    // Treat base behaviour as a special case
                    if (bh.Name == "base")
                    {
                        gdo.BaseClasses.AddRange(bh.Values);
                    }
                    else
                    {
                        gdo.Behaviours.Add(bh);
                    }
                    iterator.MoveNext();
                }
                // = class_name : "Descr" + "iption" : "Additional information" [
                Assert(iterator.Current, iterator.Current.Type == LexType.Equals, "Expected equals, got " + iterator.Current.Type);
                Expect(iterator, LexType.Value);
                gdo.Name = iterator.Current.Value;
                iterator.MoveNext();
                if (iterator.Current.Type == LexType.Colon)
                {
                    // Parsing:
                    // : {"Descr" + "iption"} [
                    iterator.MoveNext();
                    gdo.Description = ParsePlusString(iterator);
                }
                if (iterator.Current.Type == LexType.Colon)
                {
                    // Check for additional information
                    iterator.MoveNext();
                    gdo.AdditionalInformation = ParsePlusString(iterator);
                }

                // Handle case where the class has no definition and the next token starts a new class
                if (iterator.Current.Type == LexType.At)
                {
                    gd.Classes.Add(gdo);
                    return;
                }

                Assert(iterator.Current, iterator.Current.Type == LexType.OpenBracket, "Unexpected " + iterator.Current.Type);

                // Parsing:
                // name(type) : "Desc" : "Default" : "Long Desc" = [ ... ]
                // input name(type) : "Description"
                // output name(type) : "Description"
                iterator.MoveNext();
                while (iterator.Current.Type != LexType.CloseBracket)
                {
                    Assert(iterator.Current, iterator.Current.Type == LexType.Value, "Unexpected " + iterator.Current.Type);
                    var pt = iterator.Current.Value;
                    if (pt == "input" || pt == "output") // IO
                    {
                        // input name(type) : "Description"
                        var io = new IO();
                        Expect(iterator, LexType.Value);
                        io.IOType = (IOType) Enum.Parse(typeof (IOType), pt, true);
                        io.Name = iterator.Current.Value;
                        Expect(iterator, LexType.OpenParen);
                        Expect(iterator, LexType.Value);
                        io.VariableType = ParseVariableType(iterator.Current);
                        Expect(iterator, LexType.CloseParen);
                        iterator.MoveNext(); // if not colon, this will be the value of the next io/property, or close
                        if (iterator.Current.Type == LexType.Colon)
                        {
                            iterator.MoveNext();
                            io.Description = ParsePlusString(iterator);
                        }
                        gdo.InOuts.Add(io);
                    }
                    else // Property
                    {
                        Expect(iterator, LexType.OpenParen);
                        Expect(iterator, LexType.Value);
                        var vartype = ParseVariableType(iterator.Current);
                        Expect(iterator, LexType.CloseParen);
                        var prop = new Property(pt, vartype);
                        iterator.MoveNext();
                            // if not colon or equals, this will be the value of the next io/property, or close
                        if (iterator.Current.Type == LexType.Value)
                        {
                            // Check for additional flags on the property
                            // e.g.: name(type) readonly : "This is a read only value"
                            //       name(type) report   : "This value will show in the entity report"
                            switch (iterator.Current.Value)
                            {
                                case "readonly":
                                    prop.ReadOnly = true;
                                    iterator.MoveNext();
                                    break;
                                case "report":
                                    prop.ShowInEntityReport = true;
                                    iterator.MoveNext();
                                    break;
                            }
                        }
                        do // Using do/while(false) so I can break out - reduces nesting.
                        {
                            // Short description
                            if (iterator.Current.Type != LexType.Colon) break;
                            iterator.MoveNext();
                            prop.ShortDescription = ParsePlusString(iterator);

                            // Default value
                            if (iterator.Current.Type != LexType.Colon) break;
                            iterator.MoveNext();
                            if (iterator.Current.Type != LexType.Colon) // Allow for ': :' structure (no default)
                            {
                                if (iterator.Current.Type == LexType.String)
                                {
                                    prop.DefaultValue = iterator.Current.Value.Trim('"');
                                }
                                else
                                {
                                    Assert(iterator.Current, iterator.Current.Type == LexType.Value, "Unexpected " + iterator.Current.Type);
                                    prop.DefaultValue = iterator.Current.Value;
                                }
                                iterator.MoveNext();
                            }

                            // Long description
                            if (iterator.Current.Type != LexType.Colon) break;
                            iterator.MoveNext();
                            prop.Description = ParsePlusString(iterator);
                        } while (false);
                        if (iterator.Current.Type == LexType.Equals)
                        {
                            Expect(iterator, LexType.OpenBracket);
                            // Parsing property options:
                            // value : description
                            // value : description : 0
                            // value : description : 0 : "long description"
                            // value : description : "long description"
                            iterator.MoveNext();
                            while (iterator.Current.IsValueOrString())
                            {
                                var opt = new Option
                                {
                                    Key = iterator.Current.GetValue()
                                };
                                Expect(iterator, LexType.Colon);

                                // Some FGDs use values for property descriptions instead of strings
                                iterator.MoveNext();
                                Assert(iterator.Current, iterator.Current.IsValueOrString(), "Choices value must be value or string type.");
                                if (iterator.Current.Type == LexType.String)
                                {
                                    opt.Description = ParsePlusString(iterator);
                                }
                                else
                                {
                                    opt.Description = iterator.Current.GetValue();
                                    iterator.MoveNext(); // ParsePlusString moves next once it's complete, need to do the same here
                                }

                                prop.Options.Add(opt);

                                if (iterator.Current.Type != LexType.Colon)
                                {
                                    continue;
                                }

                                // There's a default value, a long description, or both
                                iterator.MoveNext();
                                if (iterator.Current.Type == LexType.Value)
                                {
                                    // If we have a value, it's the flag default
                                    opt.On = iterator.Current.Value == "1";
                                    iterator.MoveNext();

                                    // If we find another colon, there's a long description present as well
                                    // Otherwise we're finished here
                                    if (iterator.Current.Type == LexType.Colon) iterator.MoveNext();
                                    else continue;
                                }
                                // Assert without moving next
                                if (iterator.Current.Type != LexType.String)
                                {
                                    throw new ProviderException("Unable to parse FGD. Expected " + LexType.String + ", got " + iterator.Current.Type + ".\n" +
                                                                "On line " + iterator.Current.LineNumber + ", character " + iterator.Current.CharacterNumber);
                                }
                                opt.LongDescription = iterator.Current.GetValue();
                                iterator.MoveNext();
                            }
                            Assert(iterator.Current, iterator.Current.Type == LexType.CloseBracket, "Unexpected " + iterator.Current.Type);
                            iterator.MoveNext();
                        }
                        gdo.Properties.Add(prop);
                    }
                }
                Assert(iterator.Current, iterator.Current.Type == LexType.CloseBracket, "Unexpected " + iterator.Current.Type);
                gd.Classes.Add(gdo);
            }
        }

        /// <summary>
        /// Parse the iterator's tokens until the current token is not a plus or a string.
        /// </summary>
        /// <param name="iterator">A token iterator, the current value should be the start of the string to parse.</param>
        /// <returns>The string result.</returns>
        private static string ParsePlusString(IEnumerator<LexObject> iterator)
        {
            var result = "";
            var plustime = false;
            while (iterator.Current.Type == LexType.String || iterator.Current.Type == LexType.Plus)
            {
                if (iterator.Current.Type != LexType.Plus)
                {
                    if (plustime) break;
                    Assert(iterator.Current, iterator.Current.Type == LexType.String, "Unexpected " + iterator.Current.Type);
                    result += iterator.Current.Value.Trim('"');
                }
                else
                {
                    if (!plustime) break;
                }
                plustime = !plustime;
                iterator.MoveNext();
            }
            return result;
        }

        private static ClassType ParseClassType(string type, LexObject obj)
        {
            type = type.ToLower().Replace("class", "");
            ClassType ct;
            if (Enum.TryParse(type, true, out ct))
            {
                return ct;
            }
            throw new ProviderException("Unable to parse FGD. Invalid class type: " + type + ".\n" +
                                        "On line " + obj.LineNumber + ", character " + obj.CharacterNumber);
        }

        private static VariableType ParseVariableType(LexObject obj)
        {
            var type = obj.Value.ToLower().Replace("_", "");
            VariableType vt;
            if (Enum.TryParse(type, true, out vt))
            {
                return vt;
            }
            throw new ProviderException("Unable to parse FGD. Invalid variable type: " + type + ".\n" +
                                        "On line " + obj.LineNumber + ", character " + obj.CharacterNumber);
        }

        private static void Expect(IEnumerator<LexObject> iterator, LexType lexType)
        {
            iterator.MoveNext();
            if (iterator.Current.Type != lexType)
            {
                throw new ProviderException("Unable to parse FGD. Expected " + lexType + ", got " + iterator.Current.Type + ".\n" +
                                            "On line " + iterator.Current.LineNumber + ", character " + iterator.Current.CharacterNumber);
            }
        }

        private static void Assert(LexObject obj, bool value, string error)
        {
            if (!value)
            {
                throw new ProviderException("Unable to parse FGD. " + error.Trim() + "\n" +
                                            "On line " + obj.LineNumber + ", character " + obj.CharacterNumber);
            }
        }

        private enum LexType
        {
            At,             // @
            Equals,         // =
            Colon,          // :
            OpenBracket,    // [
            CloseBracket,   // ]
            OpenParen,      // (
            CloseParen,     // )
            Plus,           // +
            Comma,          // ,
            Value,          // Any unquoted string not in the above list
            String,         // Any quoted string (including quotes)
            Comment
        }

        private class LexObject
        {
            public LexType Type { get; private set; }
            public string Value { get; set; }
            public int LineNumber { get; private set; }
            public int CharacterNumber { get; private set; }

            public LexObject(int lineNum, int charNum, LexType type, string value = "")
            {
                LineNumber = lineNum;
                CharacterNumber = charNum;
                Type = type;
                Value = value;
            }

            public bool IsValueOrString()
            {
                return Type == LexType.String || Type == LexType.Value;
            }

            public string GetValue()
            {
                return Type == LexType.String ? Value.Trim('"') : Value;
            }
        }

        private static IEnumerable<LexObject> Lex(TextReader reader)
        {
            var lineNum = 1;
            var charNum = 0;
            int i;
            LexObject current = null;
            while ((i = reader.Read()) >= 0)
            {
                var c = Convert.ToChar(i);
                if (c == '\n')
                {
                    lineNum++;
                    charNum = 0;
                }
                else
                {
                    charNum++;
                }
                if (current == null)
                {
                    current = LexNew(lineNum, charNum, c);
                }
                else
                {
                    var le = LexExisting(lineNum, charNum, c, current);
                    if (le != current)
                    {
                        yield return current;
                        current = le;
                    }
                }
            }
            if (current != null)
            {
                yield return current;
            }
        }

        private static LexObject LexNew(int lineNum, int charNum, char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                return null;
            }
            if (c == '@')
            {
                return new LexObject(lineNum, charNum, LexType.At);
            }
            if (c == '=')
            {
                return new LexObject(lineNum, charNum, LexType.Equals);
            }
            if (c == ':')
            {
                return new LexObject(lineNum, charNum, LexType.Colon);
            }
            if (c == '[')
            {
                return new LexObject(lineNum, charNum, LexType.OpenBracket);
            }
            if (c == ']')
            {
                return new LexObject(lineNum, charNum, LexType.CloseBracket);
            }
            if (c == '(')
            {
                return new LexObject(lineNum, charNum, LexType.OpenParen);
            }
            if (c == ')')
            {
                return new LexObject(lineNum, charNum, LexType.CloseParen);
            }
            if (c == '+')
            {
                return new LexObject(lineNum, charNum, LexType.Plus);
            }
            if (c == ',')
            {
                return new LexObject(lineNum, charNum, LexType.Comma);
            }
            if (c == '/')
            {
                return new LexObject(lineNum, charNum, LexType.Comment);
            }
            if (c == '"')
            {
                return new LexObject(lineNum, charNum, LexType.String, c.ToString(CultureInfo.InvariantCulture));
            }
            return new LexObject(lineNum, charNum, LexType.Value, c.ToString(CultureInfo.InvariantCulture));
        }

        private static readonly char[] NonValueCharacters =
        {
            '@', '=', ':', '[', ']', '(', ')', '+', ','
        };

        private static LexObject LexExisting(int lineNum, int charNum, char c, LexObject existing)
        {
            switch (existing.Type)
            {
                case LexType.Value:
                    if (Char.IsWhiteSpace(c))
                    {
                        return null;
                    }
                    if (NonValueCharacters.Contains(c))
                    {
                        return LexNew(lineNum, charNum, c);
                    }
                    existing.Value += c.ToString(CultureInfo.InvariantCulture);
                    return existing;
                case LexType.String:
                    existing.Value += c.ToString(CultureInfo.InvariantCulture);
                    return c == '"' ? null : existing;
                case LexType.Comment:
                    return c == '\n' ? null : existing;
                default:
                    return LexNew(lineNum, charNum, c);
            }
        }
    }
}
