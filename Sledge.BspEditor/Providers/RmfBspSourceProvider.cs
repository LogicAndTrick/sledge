using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Extensions;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures;
using Path = Sledge.BspEditor.Primitives.MapObjectData.Path;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class RmfBspSourceProvider : IBspSourceProvider
    {
        const int MaxVariableStringLength = 127;

        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            // Map Object types
            typeof(Root),
            typeof(Group),
            typeof(Solid),
            typeof(Entity),

            // Map Data types
            typeof(Visgroup),
            typeof(Camera),

            // Map Object Data types
            typeof(VisgroupID),
            typeof(EntityData),
            typeof(ObjectColor),
        };

        public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; } = new[]
        {
            new FileExtensionInfo("Worldcraft map formats", ".rmf", ".rmx"), 
        };

        public async Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var br = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new BspFileLoadResult();

                    // Only RMF version 2.2 is supported for the moment.
                    var version = Math.Round(br.ReadSingle(), 1);
                    if (Math.Abs(version - 2.2) > 0.01)
                    {
                        throw new NotSupportedException("Incorrect RMF version number. Expected 2.2, got " + version + ".");
                    }

                    // RMF header test
                    var header = br.ReadFixedLengthString(Encoding.ASCII, 3);
                    if (header != "RMF")
                    {
                        throw new NotSupportedException("Incorrect RMF header. Expected 'RMF', got '" + header + "'.");
                    }

                    var map = new Map();

                    ReadVisgroups(map, br);
                    ReadWorldspawn(map, br);

                    // Some RMF files might not have the DOCINFO block so we check if we're at the end of the stream
                    if (stream.Position < stream.Length)
                    {
                        // DOCINFO string check
                        var docinfo = br.ReadFixedLengthString(Encoding.ASCII, 8);
                        if (docinfo != "DOCINFO")
                        {
                            throw new NotSupportedException("Incorrect RMF format. Expected 'DOCINFO', got '" + docinfo + "'.");
                        }

                        ReadCameras(map, br);
                    }

                    result.Map = map;
                    return result;
                }
            });
        }

        #region Reading

        private void ReadVisgroups(Map map, BinaryReader br)
        {
            var list = new List<Visgroup>();
            var numVisgroups = br.ReadInt32();
            for (var i = 0; i < numVisgroups; i++)
            {
                var vis = new Visgroup
                {
                    Name = br.ReadFixedLengthString(Encoding.ASCII, 128),
                    Colour = br.ReadRGBAColour(),
                    ID = br.ReadInt32(),
                    Visible = br.ReadBoolean()
                };
                vis.Colour = Color.FromArgb(255, vis.Colour);
                map.NumberGenerator.Seed("Visgroup", vis.ID);
                br.ReadBytes(3);
                list.Add(vis);
            }

            // Get rid of zero groups
            foreach (var vg in list.Where(x => x.ID == 0))
            {
                vg.ID = map.NumberGenerator.Next("Visgroup");
            }

            map.Data.AddRange(list);
        }

        private void ReadWorldspawn(Map map, BinaryReader br)
        {
            var root = ReadObject(map, br);
            map.Root.Unclone(root);
        }

        private IMapObject ReadObject(Map map, BinaryReader br)
        {
            var type = br.ReadCString();
            switch (type)
            {
                case "CMapWorld":
                    return ReadRoot(map, br);
                case "CMapGroup":
                    return ReadGroup(map, br);
                case "CMapSolid":
                    return ReadSolid(map, br);
                case "CMapEntity":
                    return ReadEntity(map, br);
                default:
                    throw new ArgumentOutOfRangeException("Unknown RMF map object: " + type);
            }
        }

        private void ReadMapBase(Map map, IMapObject obj, BinaryReader br)
        {
            var visgroupId = br.ReadInt32();
            if (visgroupId > 0)
            {
                obj.Data.Add(new VisgroupID(visgroupId));
            }

            var c = br.ReadRGBColour();
            obj.Data.Add(new ObjectColor(c));

            var numChildren = br.ReadInt32();
            for (var i = 0; i < numChildren; i++)
            {
                var child = ReadObject(map, br);
                if (child != null) child.Hierarchy.Parent = obj;
            }
        }

        private Root ReadRoot(Map map, BinaryReader br)
        {
            var wld = new Root(map.NumberGenerator.Next("MapObject"));
            ReadMapBase(map, wld, br);
            wld.Data.Add(ReadEntityData(br));
            var numPaths = br.ReadInt32();
            for (var i = 0; i < numPaths; i++)
            {
                wld.Data.Add(ReadPath(br));
            }
            return wld;
        }

        private Path ReadPath(BinaryReader br)
        {
            var path = new Path
            {
                Name = br.ReadFixedLengthString(Encoding.ASCII, 128),
                Type = br.ReadFixedLengthString(Encoding.ASCII, 128),
                Direction = (Path.PathDirection) br.ReadInt32()
            };
            var numNodes = br.ReadInt32();
            for (var i = 0; i < numNodes; i++)
            {
                var node = new Path.PathNode
                {
                    Position = br.ReadVector3(),
                    ID = br.ReadInt32(),
                    Name = br.ReadFixedLengthString(Encoding.ASCII, 128)
                };

                var numProps = br.ReadInt32();
                for (var j = 0; j < numProps; j++)
                {
                    var key = br.ReadCString();
                    var value = br.ReadCString();
                    node.Properties[key] = value;
                }
                path.Nodes.Add(node);
            }
            return path;
        }

        private Group ReadGroup(Map map, BinaryReader br)
        {
            var grp = new Group(map.NumberGenerator.Next("MapObject"));
            ReadMapBase(map, grp, br);
            return grp;
        }

        private Solid ReadSolid(Map map, BinaryReader br)
        {
            var sol = new Solid(map.NumberGenerator.Next("MapObject"));
            ReadMapBase(map, sol, br);
            var numFaces = br.ReadInt32();
            for (var i = 0; i < numFaces; i++)
            {
                var face = ReadFace(map, br);
                sol.Data.Add(face);
            }
            return sol;
        }

        private IMapObject ReadEntity(Map map, BinaryReader br)
        {
            var ent = new Entity(map.NumberGenerator.Next("MapObject"));
            ReadMapBase(map, ent, br);
            ent.Data.Add(ReadEntityData(br));
            br.ReadBytes(2); // Unused
            ent.Origin = br.ReadVector3();
            br.ReadBytes(4); // Unused
            return ent;
        }

        private static readonly string[] ExcludedKeys = { "spawnflags", "classname", "origin", "wad", "mapversion" };

        private EntityData ReadEntityData(BinaryReader br)
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
                var key = br.ReadCString();
                var value = br.ReadCString();
                if (key == null || ExcludedKeys.Contains(key.ToLower())) continue;
                data.Set(key, value);
            }

            br.ReadBytes(12); // More unused bytes

            return data;
        }

        private Face ReadFace(Map map, BinaryReader br)
        {
            var face = new Face(map.NumberGenerator.Next("Face"));
            var textureName = br.ReadFixedLengthString(Encoding.ASCII, 256);
            br.ReadBytes(4); // Unused
            face.Texture.Name = textureName;
            face.Texture.UAxis = br.ReadVector3();
            face.Texture.XShift = br.ReadSingle();
            face.Texture.VAxis = br.ReadVector3();
            face.Texture.YShift = br.ReadSingle();
            face.Texture.Rotation = br.ReadSingle();
            face.Texture.XScale = br.ReadSingle();
            face.Texture.YScale = br.ReadSingle();
            br.ReadBytes(16); // Unused
            var numVerts = br.ReadInt32();
            for (var i = 0; i < numVerts; i++)
            {
                face.Vertices.Add(br.ReadVector3());
            }
            face.Plane = br.ReadPlane();
            return face;
        }

        private void ReadCameras(Map map, BinaryReader br)
        {
            br.ReadSingle(); // Appears to be a version number for camera data. Unused.
            var activeCamera = br.ReadInt32();

            var num = br.ReadInt32();
            for (var i = 0; i < num; i++)
            {
                map.Data.Add(new Camera
                {
                    EyePosition = br.ReadVector3(),
                    LookPosition = br.ReadVector3(),
                    IsActive = activeCamera == i
                });
            }
        }

        #endregion

        public Task Save(Stream stream, Map map)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var bw = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    // RMF 2.2 header
                    bw.Write(2.2f);
                    bw.WriteFixedLengthString(Encoding.ASCII, 3, "RMF");

                    // Body
                    WriteVisgroups(map, bw);
                    WriteWorldspawn(map, bw);

                    // Only write docinfo if there's cameras in the document
                    if (map.Data.Get<Camera>().Any())
                    {
                        // Docinfo footer
                        bw.WriteFixedLengthString(Encoding.ASCII, 8, "DOCINFO");
                        WriteCameras(map, bw);
                    }
                }
            });
        }

        #region Writing

        private void WriteVisgroups(Map map, BinaryWriter bw)
        {
            var vis = map.Data.Get<Visgroup>().ToList();
            bw.Write(vis.Count);
            foreach (var visgroup in vis)
            {
                bw.WriteFixedLengthString(Encoding.ASCII, 128, visgroup.Name);
                bw.WriteRGBAColour(visgroup.Colour);
                bw.Write((int) visgroup.ID);
                bw.Write(visgroup.Visible);
                bw.Write(new byte[3]); // Unused
            }
        }

        private void WriteWorldspawn(Map map, BinaryWriter bw)
        {
            WriteObject(map.Root, bw);
        }

        private void WriteObject(IMapObject obj, BinaryWriter bw)
        {
            foreach (var o in obj.Decompose(SupportedTypes))
            {
                if (o is Root r) WriteRoot(r, bw);
                else if (o is Group g) WriteGroup(g, bw);
                else if (o is Solid s) WriteSolid(s, bw);
                else if (o is Entity e) WriteEntity(e, bw);
                else throw new ArgumentOutOfRangeException("Unsupported RMF map object: " + o.GetType());
            }
        }

        private void WriteMapBase(IMapObject obj, BinaryWriter bw)
        {
            bw.Write((int) (obj.Data.OfType<VisgroupID>().FirstOrDefault()?.ID ?? 0));
            bw.WriteRGBColour(obj.Data.GetOne<ObjectColor>()?.Color ?? Color.White);
            bw.Write(obj.Hierarchy.NumChildren);
            foreach (var child in obj.Hierarchy)
            {
                WriteObject(child, bw);
            }
        }

        private void WriteRoot(Root root, BinaryWriter bw)
        {
            bw.WriteCString("CMapWorld", MaxVariableStringLength);
            WriteMapBase(root, bw);
            WriteEntityData(root.Data.GetOne<EntityData>(), bw);
            var paths = root.Data.OfType<Path>().ToList();
            bw.Write(paths.Count);
            foreach (var path in paths)
            {
                WritePath(bw, path);
            }
        }

        private void WritePath(BinaryWriter bw, Path path)
        {
            bw.WriteFixedLengthString(Encoding.ASCII, 128, path.Name);
            bw.WriteFixedLengthString(Encoding.ASCII, 128, path.Type);
            bw.Write((int) path.Direction);
            bw.Write(path.Nodes.Count);
            foreach (var node in path.Nodes)
            {
                bw.WriteVector3(node.Position);
                bw.Write(node.ID);
                bw.WriteFixedLengthString(Encoding.ASCII, 128, node.Name);
                bw.Write(node.Properties.Count);
                foreach (var property in node.Properties)
                {
                    bw.WriteCString(property.Key, MaxVariableStringLength);
                    bw.WriteCString(property.Value, MaxVariableStringLength);
                }
            }
        }

        private void WriteGroup(Group group, BinaryWriter bw)
        {
            bw.WriteCString("CMapGroup", MaxVariableStringLength);
            WriteMapBase(group, bw);
        }

        private void WriteSolid(Solid solid, BinaryWriter bw)
        {
            bw.WriteCString("CMapSolid", MaxVariableStringLength);
            WriteMapBase(solid, bw);
            var faces = solid.Faces.ToList();
            bw.Write(faces.Count);
            foreach (var face in faces)
            {
                WriteFace(face, bw);
            }
        }

        private void WriteEntity(Entity entity, BinaryWriter bw)
        {
            bw.WriteCString("CMapEntity", MaxVariableStringLength);
            WriteMapBase(entity, bw);
            WriteEntityData(entity.EntityData, bw);
            bw.Write(new byte[2]); // Unused
            bw.WriteVector3(entity.Origin);
            bw.Write(new byte[4]); // Unused
        }

        private void WriteEntityData(EntityData data, BinaryWriter bw)
        {
            if (data == null) data = new EntityData();
            bw.WriteCString(data.Name, MaxVariableStringLength);
            bw.Write(new byte[4]); // Unused
            bw.Write(data.Flags);

            var props = data.Properties.Where(x => !String.IsNullOrWhiteSpace(x.Key) && !ExcludedKeys.Contains(x.Key.ToLower())).ToList();
            bw.Write(props.Count);
            foreach (var p in props)
            {
                bw.WriteCString(p.Key, MaxVariableStringLength);
                bw.WriteCString(p.Value, MaxVariableStringLength);
            }
            bw.Write(new byte[12]); // Unused
        }

        private void WriteFace(Face face, BinaryWriter bw)
        {
            bw.WriteFixedLengthString(Encoding.ASCII, 256, face.Texture.Name);
            bw.Write(new byte[4]);
            bw.WriteVector3(face.Texture.UAxis);
            bw.Write(face.Texture.XShift);
            bw.WriteVector3(face.Texture.VAxis);
            bw.Write(face.Texture.YShift);
            bw.Write(face.Texture.Rotation);
            bw.Write(face.Texture.XScale);
            bw.Write(face.Texture.YScale);
            bw.Write(new byte[16]);
            bw.Write(face.Vertices.Count);
            foreach (var vertex in face.Vertices)
            {
                bw.WriteVector3(vertex);
            }
            bw.WritePlane(face.Vertices.ToArray());
        }

        private void WriteCameras(Map map, BinaryWriter bw)
        {
            bw.Write(0.2f); // Unused

            var cams = map.Data.Get<Camera>().ToList();
            var active = Math.Max(0, cams.FindIndex(x => x.IsActive));

            bw.Write(active);
            bw.Write(cams.Count);
            foreach (var cam in cams)
            {
                bw.WriteVector3(cam.EyePosition);
                bw.WriteVector3(cam.LookPosition);
            }
        }
        
        #endregion
    }
}