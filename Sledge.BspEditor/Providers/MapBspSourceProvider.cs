using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class MapBspSourceProvider : IBspSourceProvider
    {
        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            // Map Object types
            typeof(Solid),
            typeof(Entity),

            // Map Object Data types
            typeof(VisgroupID),
            typeof(EntityData),
        };

        public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; } = new[]
        {
            new FileExtensionInfo("Quake map formats", ".map", ".max"), 
        };

        public async Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var reader = new StreamReader(stream, Encoding.ASCII, true, 1024, false))
                {
                    var result = new BspFileLoadResult();

                    var map = new Map();
                    var entities = ReadAllEntities(reader, map.NumberGenerator, result);

                    var worldspawn = entities.FirstOrDefault(x => x.EntityData?.Name == "worldspawn")
                                     ?? new Entity(0) { Data = { new EntityData { Name = "worldspawn" } } };
                    entities.Remove(worldspawn);

                    map.Root.Data.Replace(worldspawn.EntityData);
                    foreach (var ch in worldspawn.Hierarchy.ToList())
                    {
                        ch.Hierarchy.Parent = map.Root;
                    }

                    foreach (var entity in entities)
                    {
                        entity.Hierarchy.Parent = map.Root;
                    }

                    foreach (var obj in worldspawn.Hierarchy.ToList())
                    {
                        obj.Hierarchy.Parent = map.Root;
                    }

                    map.Root.DescendantsChanged();

                    result.Map = map;
                    return result;
                }
            });
        }

        #region Reading
        
        private static string CleanLine(string line)
        {
            if (line == null) return null;
            var ret = line;
            if (ret.Contains("//")) ret = ret.Substring(0, ret.IndexOf("//", StringComparison.Ordinal)); // Comments
            return ret.Trim();
        }

        private void Assert(bool b, string message = "Malformed file.")
        {
            if (!b) throw new Exception(message);
        }

        /// <summary>
        /// Same as Plane.GetClosestAxisToNormal(), but prioritises the axes Z, X, Y.
        /// </summary>
        /// <param name="plane">Input plane</param>
        /// <returns>Vector3.UnitX, Vector3.UnitY, or Vector3.UnitZ depending on the plane's normal</returns>
        private static Vector3 QuakeEdClosestAxisToNormal(Plane plane)
        {
            var norm = plane.Normal.Absolute();

            if (norm.Z >= norm.X && norm.Z >= norm.Y) return Vector3.UnitZ;
            if (norm.X >= norm.Y) return Vector3.UnitX;
            return Vector3.UnitY;
        }

        /// <summary>
        /// Set the initial texture axes for a face in the old-style .MAP format.
        /// Same as Face.AlignTextureToWorld(), except uses QuakeEdClosestAxisToNormal() instead of Plane.GetClosestAxisToNormal().
        /// </summary>
        /// <param name="face">Face to set Texture.UAxis and Texture.VAxis to their default values for the old-style .MAP format</param>
        private static void QuakeEdAlignTextureToWorld(Face face)
        {
            var direction = QuakeEdClosestAxisToNormal(face.Plane);
            face.Texture.UAxis = direction == Vector3.UnitX ? Vector3.UnitY : Vector3.UnitX;
            face.Texture.VAxis = direction == Vector3.UnitZ ? -Vector3.UnitY : -Vector3.UnitZ;
        }

        private Face ReadFace(string line, UniqueNumberGenerator generator)
        {
            const NumberStyles ns = NumberStyles.Float;

            var parts = line.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            
            Assert(parts[0] == "(");
            Assert(parts[4] == ")");
            Assert(parts[5] == "(");
            Assert(parts[9] == ")");
            Assert(parts[10] == "(");
            Assert(parts[14] == ")");

            var face = new Face(generator.Next("Face"))
            {
                Plane = new Plane(
                    NumericsExtensions.Parse(parts[1], parts[2], parts[3], ns, CultureInfo.InvariantCulture),
                    NumericsExtensions.Parse(parts[6], parts[7], parts[8], ns, CultureInfo.InvariantCulture),
                    NumericsExtensions.Parse(parts[11], parts[12], parts[13], ns, CultureInfo.InvariantCulture)
                ),
                Texture = {Name = parts[15]}
            };

            // Cater for older-style map formats
            // TODO Quake 3: when the MAP face has 24 parts, the last three parts are: content_flags, surface_flags, value
            if (parts.Count == 21 || parts.Count == 24)
            {
                QuakeEdAlignTextureToWorld(face);

                var xshift = float.Parse(parts[16], ns, CultureInfo.InvariantCulture);
                var yshift = float.Parse(parts[17], ns, CultureInfo.InvariantCulture);
                var rotate = float.Parse(parts[18], ns, CultureInfo.InvariantCulture);
                var xscale = float.Parse(parts[19], ns, CultureInfo.InvariantCulture);
                var yscale = float.Parse(parts[20], ns, CultureInfo.InvariantCulture);

                face.Texture.Rotation = -rotate;
                face.Texture.Rotation = rotate;
                face.Texture.XScale = xscale;
                face.Texture.YScale = yscale;
                face.Texture.XShift = xshift;
                face.Texture.YShift = yshift;
            }
            else
            {
                Assert(parts[16] == "[");
                Assert(parts[21] == "]");
                Assert(parts[22] == "[");
                Assert(parts[27] == "]");

                face.Texture.UAxis = NumericsExtensions.Parse(parts[17], parts[18], parts[19], ns, CultureInfo.InvariantCulture);
                face.Texture.XShift = float.Parse(parts[20], ns, CultureInfo.InvariantCulture);
                face.Texture.VAxis = NumericsExtensions.Parse(parts[23], parts[24], parts[25], ns, CultureInfo.InvariantCulture);
                face.Texture.YShift = float.Parse(parts[26], ns, CultureInfo.InvariantCulture);
                face.Texture.Rotation = float.Parse(parts[28], ns, CultureInfo.InvariantCulture);
                face.Texture.XScale = float.Parse(parts[29], ns, CultureInfo.InvariantCulture);
                face.Texture.YScale = float.Parse(parts[30], ns, CultureInfo.InvariantCulture);
            }

            return face;
        }

        private Solid ReadSolid(StreamReader rdr, UniqueNumberGenerator generator, BspFileLoadResult result)
        {
            var faces = new List<Face>();
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                if (line == "}")
                {
                    if (!faces.Any()) return null;

                    var poly = new Polyhedron(faces.Select(x => x.Plane));
                    var ret = new Solid(generator.Next("MapObject"));
                    ret.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));

                    foreach (var face in faces)
                    {
                        var pg = poly.Polygons.FirstOrDefault(x => x.Plane.Normal.EquivalentTo(face.Plane.Normal, 0.0075f)); // Magic number that seems to match VHE
                        if (pg == null)
                        {
                            result.InvalidObjects.Add(poly);
                            return null;
                        }
                        face.Vertices.AddRange(pg.Vertices);
                    }
                    ret.Data.AddRange(faces);
                    ret.DescendantsChanged();
                    return ret;
                }
                else if (line == "patchDef2")
                {
                    // Quake 3 has bezier faces
                    // TODO: support bezier faces...
                    while (CleanLine(rdr.ReadLine()) != "}")
                    {
                        // Skip...
                    }
                }
                else if (line == "brushDef")
                {
                    throw new Exception("Maps containing the 'brushDef' structure are not currently supported");
                }
                else
                {
                    faces.Add(ReadFace(line, generator));
                }
            }
            return null;
        }

        private static readonly string[] ExcludedKeys = new[] { "spawnflags", "classname", "origin", "wad", "mapversion" };

        private static void ReadProperty(Entity ent, string line)
        {
            // Quake id1 map sources use tabs between keys and values
            var split = line.Split(new char[] { ' ', '\t' });
            var key = split[0].Trim('"');

            var val = String.Join(" ", split.Skip(1)).Trim('"');

            if (key == "classname")
            {
                ent.EntityData.Name = val;
            }
            else if (key == "spawnflags")
            {
                ent.EntityData.Flags = int.Parse(val);
            }
            else if (key == "origin")
            {
                var osp = val.Split(' ');
                ent.Origin = NumericsExtensions.Parse(osp[0], osp[1], osp[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            else if (!ExcludedKeys.Contains(key.ToLower()))
            {
                ent.EntityData.Set(key, val);
            }
        }

        private Entity ReadEntity(StreamReader rdr, UniqueNumberGenerator generator, BspFileLoadResult result)
        {
            var ent = new Entity(generator.Next("Face"))
            {
                Data =
                {
                    new EntityData(),
                    new ObjectColor(Colour.GetRandomBrushColour())
                }
            };
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                if (line[0] == '"') ReadProperty(ent, line);
                else if (line[0] == '{')
                {
                    var s = ReadSolid(rdr, generator, result);
                    if (s != null) s.Hierarchy.Parent = ent;
                }
                else if (line[0] == '}') break;
            }
            ent.DescendantsChanged();
            return ent;
        }

        private List<Entity> ReadAllEntities(StreamReader rdr, UniqueNumberGenerator generator, BspFileLoadResult result)
        {
            var list = new List<Entity>();
            string line;
            while ((line = CleanLine(rdr.ReadLine())) != null)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;
                if (line == "{") list.Add(ReadEntity(rdr, generator, result));
            }
            return list;
        }


        #endregion

        public Task Save(Stream stream, Map map)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true))
                {
                    WriteWorld(writer, map.Root);
                }
            });
        }

        #region Writing


        private string FormatVector3(Vector3 c)
        {
            return c.X.ToString("0.000", CultureInfo.InvariantCulture)
                   + " " + c.Y.ToString("0.000", CultureInfo.InvariantCulture)
                   + " " + c.Z.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void CollectSolids(List<Solid> solids, IMapObject parent)
        {
            foreach (var obj in parent.Hierarchy.SelectMany(x => x.Decompose(SupportedTypes)))
            {
                if (obj is Solid s) solids.Add(s);
                else if (obj is Group) CollectSolids(solids, obj);
            }
        }

        private void CollectEntities(List<Entity> entities, IMapObject parent)
        {
            foreach (var obj in parent.Hierarchy.SelectMany(x => x.Decompose(SupportedTypes)))
            {
                if (obj is Entity e) entities.Add(e);
                else if (obj is Group) CollectEntities(entities, obj);
            }
        }

        private void WriteFace(StreamWriter sw, Face face)
        {
            // ( -128 64 64 ) ( -64 64 64 ) ( -64 0 64 ) AAATRIGGER [ 1 0 0 0 ] [ 0 -1 0 0 ] 0 1 1
            var strings = face.Vertices.Take(3).Select(x => "( " + FormatVector3(x) + " )").ToList();
            strings.Add(String.IsNullOrWhiteSpace(face.Texture.Name) ? "AAATRIGGER" : face.Texture.Name);
            strings.Add("[");
            strings.Add(FormatVector3(face.Texture.UAxis));
            strings.Add(face.Texture.XShift.ToString("0.000", CultureInfo.InvariantCulture));
            strings.Add("]");
            strings.Add("[");
            strings.Add(FormatVector3(face.Texture.VAxis));
            strings.Add(face.Texture.YShift.ToString("0.000", CultureInfo.InvariantCulture));
            strings.Add("]");
            strings.Add(face.Texture.Rotation.ToString("0.000", CultureInfo.InvariantCulture));
            strings.Add(face.Texture.XScale.ToString("0.000", CultureInfo.InvariantCulture));
            strings.Add(face.Texture.YScale.ToString("0.000", CultureInfo.InvariantCulture));
            sw.WriteLine(String.Join(" ", strings));
        }

        private void WriteSolid(StreamWriter sw, Solid solid)
        {
            sw.WriteLine("{");
            foreach (var face in solid.Faces)
            {
                WriteFace(sw, face);
            }
            sw.WriteLine("}");
        }

        private void WriteProperty(StreamWriter sw, string key, string value)
        {
            sw.WriteLine('"' + key + "\" \"" + value + '"');
        }

        private void WriteEntity(StreamWriter sw, Entity ent)
        {
            var solids = new List<Solid>();
            CollectSolids(solids, ent);

            sw.WriteLine("{");
            WriteProperty(sw, "classname", ent.EntityData.Name);

            if (ent.EntityData.Flags > 0)
            {
                // VHE doesn't write the spawnflags when they are zero
                WriteProperty(sw, "spawnflags", ent.EntityData.Flags.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var prop in ent.EntityData.Properties)
            {
                if (prop.Key == "classname" || prop.Key == "spawnflags" || prop.Key == "origin") continue;

                // todo? VHE doesn't write empty or zero values to the .map file
                // var gameDataProp = obj?.Properties.FirstOrDefault(x => String.Equals(x.Name, prop.Key, StringComparison.InvariantCultureIgnoreCase));
                // if (gameDataProp != null)
                // {
                //     var emptyGd = String.IsNullOrWhiteSpace(gameDataProp.DefaultValue) || gameDataProp.DefaultValue == "0";
                //     var emptyProp = String.IsNullOrWhiteSpace(prop.Value) || prop.Value == "0";
                // 
                //     // The value hasn't changed from the default, don't write if it's an empty value
                //     if (emptyGd && emptyProp) continue;
                // }
                WriteProperty(sw, prop.Key, prop.Value);
            }

            if (solids.Any()) solids.ForEach(x => WriteSolid(sw, x)); // Brush entity
            else WriteProperty(sw, "origin", FormatVector3(ent.Origin)); // Point entity

            sw.WriteLine("}");
        }

        private void WriteWorld(StreamWriter sw, Root world)
        {
            var solids = new List<Solid>();
            var entities = new List<Entity>();
            CollectSolids(solids, world);
            CollectEntities(entities, world);

            sw.WriteLine("{");

            var ed = world.Data.GetOne<EntityData>() ?? new EntityData();

            WriteProperty(sw, "classname", ed.Name);
            WriteProperty(sw, "spawnflags", ed.Flags.ToString(CultureInfo.InvariantCulture));
            WriteProperty(sw, "mapversion", "220");
            foreach (var prop in ed.Properties)
            {
                if (prop.Key == "classname" || prop.Key == "spawnflags" || prop.Key == "mapversion") continue;
                WriteProperty(sw, prop.Key, prop.Value);
            }
            solids.ForEach(x => WriteSolid(sw, x));

            sw.WriteLine("}");

            foreach (var entity in entities)
            {
                WriteEntity(sw, entity);
            }
        }

        #endregion
    }
}