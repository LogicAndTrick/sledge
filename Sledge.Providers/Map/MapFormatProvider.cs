using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    public class MapFormatProvider : MapProvider
    {
        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".map");
        }

        private string CleanLine(string line)
        {
            if (line == null) return null;
            var ret = line;
            if (ret.Contains("//")) ret = ret.Substring(0, ret.IndexOf("//")); // Comments
            return ret.Trim();
        }

        private void Assert(bool b, string message = "Malformed file.")
        {
            if (!b) throw new Exception(message);
        }

        private Face ReadFace(string line)
        {
            const NumberStyles ns = NumberStyles.Float;

            var parts = line.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();

            Assert(parts[0] == "(");
            Assert(parts[4] == ")");
            Assert(parts[5] == "(");
            Assert(parts[9] == ")");
            Assert(parts[10] == "(");
            Assert(parts[14] == ")");
            Assert(parts[16] == "[");
            Assert(parts[21] == "]");
            Assert(parts[22] == "[");
            Assert(parts[27] == "]");

            return new Face
                       {
                           Plane = new Plane(Coordinate.Parse(parts[1], parts[2], parts[3]),
                                             Coordinate.Parse(parts[6], parts[7], parts[8]),
                                             Coordinate.Parse(parts[11], parts[12], parts[13])),
                           Texture =
                               {
                                   Name = parts[15],
                                   UAxis = Coordinate.Parse(parts[17], parts[18], parts[19]),
                                   XShift = decimal.Parse(parts[20], ns),
                                   VAxis = Coordinate.Parse(parts[23], parts[24], parts[25]),
                                   YShift = decimal.Parse(parts[26], ns),
                                   Rotation = decimal.Parse(parts[28], ns),
                                   XScale = decimal.Parse(parts[29], ns),
                                   YScale = decimal.Parse(parts[30], ns)
                               }
                       };
        }

        private Solid ReadSolid(StreamReader rdr)
        {
            var faces = new List<Face>();
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (line == "}")
                {
                    var ret = Solid.CreateFromIntersectingPlanes(faces.Select(x => x.Plane));
                    ret.Colour = Colour.GetRandomBrushColour();
                    foreach (var face in ret.Faces)
                    {
                        var f = faces.FirstOrDefault(x => x.Plane.Normal.EquivalentTo(face.Plane.Normal));
                        if (f == null)
                        {
                            // TODO: Report invalid solids
                            Debug.WriteLine("Invalid solid!");
                            return null;
                        }
                        face.Texture = f.Texture;
                        face.Parent = ret;
                        face.Colour = ret.Colour;
                    }
                    ret.UpdateBoundingBox(false);
                    return ret;
                }
                faces.Add(ReadFace(line));
            }
            return null;
        }

        private static void ReadProperty(Entity ent, string line)
        {
            var split = line.Split(' ');
            var key = split[0].Trim('"');
            if (key == "wad" || key == "mapversion") { return; }

            var val = split[1].Trim('"');

            if (key == "classname")
            {
                ent.EntityData.Name = val;
            }
            else if (key == "flags")
            {
                ent.EntityData.Flags = int.Parse(val);
            }
            else if (key == "origin")
            {
                var osp = val.Split(' ');
                ent.Origin = Coordinate.Parse(osp[0], osp[1], osp[2]);
            }
            else
            {
                ent.EntityData.Properties.Add(new Property { Key = key, Value = val });
            }
        }

        private Entity ReadEntity(StreamReader rdr)
        {
            var ent = new Entity();
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (line[0] == '"') ReadProperty(ent, line);
                else if (line[0] == '{') ent.Children.Add(ReadSolid(rdr));
                else if (line[0] == '}') break;
            }
            return ent;
        }

        private List<Entity> ReadAllEntities(StreamReader rdr)
        {
            var list = new List<Entity>();
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (line == "{") list.Add(ReadEntity(rdr));
            }
            return list;
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var map = new DataStructures.MapObjects.Map();
                var allentities = ReadAllEntities(reader);
                var worldspawn = allentities.FirstOrDefault(x => x.EntityData.Name == "worldspawn")
                                 ?? new Entity {EntityData = {Name = "worldspawn"}};
                allentities.Remove(worldspawn);
                map.WorldSpawn = new World {EntityData = worldspawn.EntityData};
                map.WorldSpawn.Children.AddRange(allentities);
                return map;
            }
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            throw new NotImplementedException();
        }
    }
}
