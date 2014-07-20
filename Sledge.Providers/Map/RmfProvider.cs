using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;
using Path = Sledge.DataStructures.MapObjects.Path;

namespace Sledge.Providers.Map
{
    public class RmfProvider : MapProvider
    {
        protected override IEnumerable<MapFeature> GetFormatFeatures()
        {
            return new[]
            {
                MapFeature.Worldspawn,
                MapFeature.Solids,
                MapFeature.Entities,
                MapFeature.Groups,

                MapFeature.Colours,
                MapFeature.SingleVisgroups,
                MapFeature.Cameras
            };
        }

        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".rmf", true, CultureInfo.InvariantCulture)
                || filename.EndsWith(".rmx", true, CultureInfo.InvariantCulture);
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            var map = new DataStructures.MapObjects.Map();
            var br = new BinaryReader(stream);

            // Only RMF version 2.2 is supported for the moment.
            var version = Math.Round(br.ReadSingle(), 1);
            if (Math.Abs(version - 2.2) > 0.01)
            {
                throw new ProviderException("Incorrect RMF version number. Expected 2.2, got " + version + ".");
            }
            map.Version = (decimal) version;

            // RMF header test
            var header = br.ReadFixedLengthString(Encoding.UTF8, 3);
            if (header != "RMF")
            {
                throw new ProviderException("Incorrect RMF header. Expected 'RMF', got '" + header + "'.");
            }

            // Visgroups
            var visgroups = ReadVisgroups(br);
            map.Visgroups.AddRange(visgroups);

            // Map Objects
            map.IDGenerator.Reset(); // So the world will be ID = 1
            var worldspawn = ReadWorldSpawn(br, map.Visgroups, map.IDGenerator);
            map.WorldSpawn = worldspawn;

            // DOCINFO string check
            var docinfo = br.ReadFixedLengthString(Encoding.UTF8, 8);
            if (docinfo != "DOCINFO")
            {
                throw new ProviderException("Incorrect RMF format. Expected 'DOCINFO', got '" + docinfo + "'.");
            }
            
            // Cameras
            br.ReadSingle(); // Appears to be a version number for camera data. Unused.
            var activeCamera = br.ReadInt32();
            var cameras = ReadCameras(br);
            map.Cameras.AddRange(cameras);
            if (activeCamera >= 0 && activeCamera < map.Cameras.Count)
            {
                map.ActiveCamera = map.Cameras[activeCamera];
            }

            return map;
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            var bw = new BinaryWriter(stream, Encoding.UTF8);
            bw.Write(2.2f); // RMF version number
            bw.WriteFixedLengthString(Encoding.UTF8, 3, "RMF");
            WriteVisgroups(bw, map.Visgroups);
            WriteMapObject(bw, map.WorldSpawn);
            bw.WriteFixedLengthString(Encoding.UTF8, 8, "DOCINFO");
            bw.Write(0.2f); // Unused
            bw.Write(map.Cameras.IndexOf(map.ActiveCamera));
            WriteCameras(bw, map.Cameras);
        }

        private static Property ReadProperty(BinaryReader br)
        {
            return new Property
                {
                    Key = br.ReadCString(),
                    Value = br.ReadCString()
                };
        }

        public static void WriteProperty(BinaryWriter bw, Property p)
        {
            bw.WriteCString(p.Key);
            bw.WriteCString(p.Value);
        }

        private static IEnumerable<Camera> ReadCameras(BinaryReader br)
        {
            var num = br.ReadInt32();
            for (var i = 0; i < num; i++)
            {
                yield return new Camera
                {
                    EyePosition = br.ReadCoordinate(),
                    LookPosition = br.ReadCoordinate()
                };
            }
        }

        public static void WriteCameras(BinaryWriter bw, List<Camera> cameras)
        {
            bw.Write(cameras.Count);
            foreach (var camera in cameras)
            {
                bw.WriteCoordinate(camera.EyePosition);
                bw.WriteCoordinate(camera.LookPosition);
            }
        }

        private static readonly string[] ExcludedKeys = new[] { "spawnflags", "classname", "origin", "wad", "mapversion" };

        private static EntityData ReadEntityData(BinaryReader br)
        {
            var data = new EntityData
                {
                    Name = br.ReadCString()
                };

            br.ReadBytes(4); // Unused bytes

            data.Flags = br.ReadInt32();

            var numProperties = br.ReadInt32();
            for (var i = 0; i < numProperties; i++)
            {
                var prop = ReadProperty(br);
                if (ExcludedKeys.Contains(prop.Key.ToLower())) continue;
                data.SetPropertyValue(prop.Key, prop.Value);
            }

            br.ReadBytes(12); // More unused bytes

            return data;
        }

        private static void WriteEntityData(BinaryWriter bw, EntityData data)
        {
            bw.WriteCString(data.Name);
            bw.Write(new byte[4]); // Unused
            bw.Write(data.Flags);
            bw.Write(data.Properties.Count);
            foreach (var property in data.Properties)
            {
                WriteProperty(bw, property);
            }
            bw.Write(new byte[12]); // Unused
        }

        private static World ReadWorldSpawn(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            return (World)ReadMapObject(br, visgroups, generator);
        }

        private static MapObject ReadMapObject(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            var type = br.ReadCString();
            switch (type)
            {
                case "CMapWorld":
                    return ReadMapWorld(br, visgroups, generator);
                case "CMapGroup":
                    return ReadMapGroup(br, visgroups, generator);
                case "CMapSolid":
                    return ReadMapSolid(br, visgroups, generator);
                case "CMapEntity":
                    return ReadMapEntity(br, visgroups, generator);
                default:
                    throw new ProviderException("Unknown RMF map object: " + type);
            }
        }

        private static void WriteMapObject(BinaryWriter bw, MapObject mo)
        {
            if (mo is World) WriteMapWorld(bw, (World)mo);
            else if (mo is Group) WriteMapGroup(bw, (Group)mo);
            else if (mo is Solid) WriteMapSolid(bw, (Solid)mo);
            else if (mo is Entity) WriteMapEntity(bw, (Entity)mo);
        }

        private static void ReadMapBase(BinaryReader br, MapObject obj, List<Visgroup> visgroups, IDGenerator generator)
        {
            var visgroupId = br.ReadInt32();
            if (visgroupId > 0 && visgroups.Any(x => x.ID == visgroupId))
            {
                obj.Visgroups.Add(visgroupId);
                obj.IsVisgroupHidden = !visgroups.First(x => x.ID == visgroupId).Visible;
            }

            obj.Colour = br.ReadRGBColour();

            var numChildren = br.ReadInt32();
            for (var i = 0; i < numChildren; i++)
            {
                var child = ReadMapObject(br, visgroups, generator);
                child.SetParent(obj);
            }
        }

        private static void WriteMapBase(BinaryWriter bw, MapObject obj)
        {
            bw.Write(obj.Visgroups.Except(obj.AutoVisgroups).FirstOrDefault());
            bw.WriteRGBColour(obj.Colour);
            bw.Write(obj.ChildCount);
            foreach (var mo in obj.GetChildren())
            {
                WriteMapObject(bw, mo);
            }
        }

        private static Entity ReadMapEntity(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            var ent = new Entity(generator.GetNextObjectID());
            ReadMapBase(br, ent, visgroups, generator);
            ent.EntityData = ReadEntityData(br);
            br.ReadBytes(2); // Unused
            ent.Origin = br.ReadCoordinate();
            br.ReadBytes(4); // Unused
            ent.UpdateBoundingBox(false);
            return ent;
        }

        private static void WriteMapEntity(BinaryWriter bw, Entity ent)
        {
            bw.WriteCString("CMapEntity");
            WriteMapBase(bw, ent);
            WriteEntityData(bw, ent.EntityData);
            bw.Write(new byte[2]); // Unused
            bw.WriteCoordinate(ent.Origin);
            bw.Write(new byte[4]); // Unused
        }

        private static Face ReadFace(BinaryReader br, IDGenerator generator)
        {
            var face = new Face(generator.GetNextFaceID());
            var textureName = br.ReadFixedLengthString(Encoding.UTF8, 256);
            br.ReadBytes(4); // Unused
            face.Texture.Name = textureName;
            face.Texture.UAxis = br.ReadCoordinate();
            face.Texture.XShift = br.ReadSingleAsDecimal();
            face.Texture.VAxis = br.ReadCoordinate();
            face.Texture.YShift = br.ReadSingleAsDecimal();
            face.Texture.Rotation = br.ReadSingleAsDecimal();
            face.Texture.XScale = br.ReadSingleAsDecimal();
            face.Texture.YScale = br.ReadSingleAsDecimal();
            br.ReadBytes(16); // Unused
            var numVerts = br.ReadInt32();
            for (var i = 0; i < numVerts; i++)
            {
                face.Vertices.Add(new Vertex(br.ReadCoordinate(), face));
            }
            face.Plane = br.ReadPlane();
            face.UpdateBoundingBox();
            return face;
        }

        private static void WriteFace(BinaryWriter bw, Face face)
        {
            bw.WriteFixedLengthString(Encoding.UTF8, 256, face.Texture.Name);
            bw.Write(new byte[4]);
            bw.WriteCoordinate(face.Texture.UAxis);
            bw.WriteDecimalAsSingle(face.Texture.XShift);
            bw.WriteCoordinate(face.Texture.VAxis);
            bw.WriteDecimalAsSingle(face.Texture.YShift);
            bw.WriteDecimalAsSingle(face.Texture.Rotation);
            bw.WriteDecimalAsSingle(face.Texture.XScale);
            bw.WriteDecimalAsSingle(face.Texture.YScale);
            bw.Write(new byte[16]);
            bw.Write(face.Vertices.Count);
            foreach (var vertex in face.Vertices)
            {
                bw.WriteCoordinate(vertex.Location);
            }
            bw.WritePlane(face.Vertices.Select(v => v.Location).ToArray());
        }

        private static Solid ReadMapSolid(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            var sol = new Solid(generator.GetNextObjectID());
            ReadMapBase(br, sol, visgroups, generator);
            var numFaces = br.ReadInt32();
            for (var i = 0; i < numFaces; i++)
            {
                var face = ReadFace(br, generator);
                face.Parent = sol;
                face.Colour = sol.Colour;
                sol.Faces.Add(face);
            }
            sol.UpdateBoundingBox(false);
            return sol;
        }

        private static void WriteMapSolid(BinaryWriter bw, Solid s)
        {
            bw.WriteCString("CMapSolid");
            WriteMapBase(bw, s);
            bw.Write(s.Faces.Count);
            foreach (var face in s.Faces)
            {
                WriteFace(bw, face);
            }
        }

        private static Group ReadMapGroup(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            var grp = new Group(generator.GetNextObjectID());
            ReadMapBase(br, grp, visgroups, generator);
            grp.UpdateBoundingBox(false);
            return grp;
        }

        private static void WriteMapGroup(BinaryWriter bw, Group g)
        {
            bw.WriteCString("CMapGroup");
            WriteMapBase(bw, g);
        }

        private static PathNode ReadPathNode(BinaryReader br)
        {
            var node = new PathNode
                {
                    Position = br.ReadCoordinate(),
                    ID = br.ReadInt32(),
                    Name = br.ReadFixedLengthString(Encoding.UTF8, 128)
                };
            var numProps = br.ReadInt32();
            for (var i = 0; i < numProps; i++)
            {
                node.Properties.Add(ReadProperty(br));
            }
            return node;
        }

        private static void WritePathNode(BinaryWriter bw, PathNode node)
        {
            bw.WriteCoordinate(node.Position);
            bw.Write(node.ID);
            bw.WriteFixedLengthString(Encoding.UTF8, 128, node.Name);
            bw.Write(node.Properties.Count);
            foreach (var property in node.Properties)
            {
                WriteProperty(bw, property);
            }
        }

        private static Path ReadPath(BinaryReader br)
        {
            var path = new Path
                {
                    Name = br.ReadFixedLengthString(Encoding.UTF8, 128),
                    Type = br.ReadFixedLengthString(Encoding.UTF8, 128),
                    Direction = (PathDirection) br.ReadInt32()
                };
            var numNodes = br.ReadInt32();
            for (var i = 0; i < numNodes; i++)
            {
                var node = ReadPathNode(br);
                node.Parent = path;
                path.Nodes.Add(node);
            }
            return path;
        }

        private static void WritePath(BinaryWriter bw, Path path)
        {
            bw.WriteFixedLengthString(Encoding.UTF8, 128, path.Name);
            bw.WriteFixedLengthString(Encoding.UTF8, 128, path.Type);
            bw.Write((int) path.Direction);
            bw.Write(path.Nodes.Count);
            foreach (var node in path.Nodes)
            {
                WritePathNode(bw, node);
            }
        }

        private static MapObject ReadMapWorld(BinaryReader br, List<Visgroup> visgroups, IDGenerator generator)
        {
            var wld = new World(generator.GetNextObjectID());
            ReadMapBase(br, wld, visgroups, generator);
            wld.EntityData = ReadEntityData(br);
            var numPaths = br.ReadInt32();
            for (var i = 0; i < numPaths; i++)
            {
                wld.Paths.Add(ReadPath(br));
            }
            return wld;
        }

        private static void WriteMapWorld(BinaryWriter bw, World w)
        {
            bw.WriteCString("CMapWorld");
            WriteMapBase(bw, w);
            WriteEntityData(bw, w.EntityData);
            bw.Write(w.Paths.Count);
            foreach (var path in w.Paths)
            {
                WritePath(bw, path);
            }
        }

        private static IEnumerable<Visgroup> ReadVisgroups(BinaryReader br)
        {
            var list = new List<Visgroup>();
            var numVisgroups = br.ReadInt32();
            for (var i = 0; i < numVisgroups; i++)
            {
                var vis = new Visgroup
                    {
                        Name = br.ReadFixedLengthString(Encoding.UTF8, 128),
                        Colour = br.ReadRGBAColour(),
                        ID = br.ReadInt32(),
                        Visible = br.ReadBoolean()
                    };
                vis.Colour = Color.FromArgb(255, vis.Colour);
                br.ReadBytes(3);
                list.Add(vis);
            }
            // Get rid of zero groups
            foreach (var vg in list.Where(x => x.ID == 0))
            {
                vg.ID = list.Max(x => x.ID) + 1;
            }
            return list;
        }

        private static void WriteVisgroups(BinaryWriter bw, IEnumerable<Visgroup> visgroups)
        {
            var vis = visgroups.Where(x => !x.IsAutomatic).ToList();
            bw.Write(vis.Count);
            foreach (var visgroup in vis)
            {
                bw.WriteFixedLengthString(Encoding.UTF8, 128, visgroup.Name);
                bw.WriteRGBAColour(visgroup.Colour);
                bw.Write(visgroup.ID);
                bw.Write(visgroup.Visible);
                bw.Write(new byte[3]); // Unused
            }
        }
    }
}
