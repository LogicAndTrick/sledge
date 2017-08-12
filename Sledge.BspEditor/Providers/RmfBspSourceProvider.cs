using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Extensions;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class RmfBspSourceProvider : IBspSourceProvider
    {
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

        public async Task<Map> Load(Stream stream)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var br = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    // Only RMF version 2.2 is supported for the moment.
                    var version = Math.Round(br.ReadSingle(), 1);
                    if (Math.Abs(version - 2.2) > 0.01)
                    {
                        throw new NotSupportedException("Incorrect RMF version number. Expected 2.2, got " + version + ".");
                    }

                    // RMF header test
                    var header = br.ReadFixedLengthString(Encoding.UTF8, 3);
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
                        var docinfo = br.ReadFixedLengthString(Encoding.UTF8, 8);
                        if (docinfo != "DOCINFO")
                        {
                            throw new NotSupportedException("Incorrect RMF format. Expected 'DOCINFO', got '" + docinfo + "'.");
                        }

                        ReadCameras(map, br);
                    }

                    return map;
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
                    Name = br.ReadFixedLengthString(Encoding.UTF8, 128),
                    Colour = br.ReadRGBAColour(),
                    ID = br.ReadInt32(),
                    Visible = br.ReadBoolean(),
                    Parent = 0
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
            wld.Data.Add(ReadEntityData(map, br));
            var numPaths = br.ReadInt32();
            for (var i = 0; i < numPaths; i++)
            {
                throw new NotImplementedException("Paths are not supported yet");
                //wld.Paths.Add(ReadPath(br));
            }
            return wld;
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
            ent.Data.Add(ReadEntityData(map, br));
            br.ReadBytes(2); // Unused
            ent.Origin = br.ReadCoordinate();
            br.ReadBytes(4); // Unused
            return ent;
        }

        private static readonly string[] ExcludedKeys = { "spawnflags", "classname", "origin", "wad", "mapversion" };

        private EntityData ReadEntityData(Map map, BinaryReader br)
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
                if (ExcludedKeys.Contains(key.ToLower())) continue;
                data.Set(key, value);
            }

            br.ReadBytes(12); // More unused bytes

            return data;
        }

        private Face ReadFace(Map map, BinaryReader br)
        {
            var face = new Face(map.NumberGenerator.Next("Face"));
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
                face.Vertices.Add(br.ReadCoordinate());
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
                    EyePosition = br.ReadCoordinate(),
                    LookPosition = br.ReadCoordinate(),
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

                    // Docinfo footer
                    bw.WriteFixedLengthString(Encoding.ASCII, 8, "DOCINFO");
                    WriteCameras(map, bw);
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
                bw.WriteFixedLengthString(Encoding.UTF8, 128, visgroup.Name);
                bw.WriteRGBAColour(visgroup.Colour);
                bw.Write(visgroup.ID);
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
            bw.WriteCString("CMapWorld");
            WriteMapBase(root, bw);
            WriteEntityData(root.Data.GetOne<EntityData>(), bw);
            var paths = new object[0]; // Root.Data.Get<Path> etc/
            bw.Write(paths.Length);
            foreach (var path in paths)
            {
                throw new NotImplementedException("Paths are not supported yet");
                // WritePath(path, bw);
            }
        }

        private void WriteGroup(Group group, BinaryWriter bw)
        {
            bw.WriteCString("CMapGroup");
            WriteMapBase(group, bw);
        }

        private void WriteSolid(Solid solid, BinaryWriter bw)
        {
            bw.WriteCString("CMapSolid");
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
            bw.WriteCString("CMapEntity");
            WriteMapBase(entity, bw);
            WriteEntityData(entity.EntityData, bw);
            bw.Write(new byte[2]); // Unused
            bw.WriteCoordinate(entity.Origin);
            bw.Write(new byte[4]); // Unused
        }

        private void WriteEntityData(EntityData data, BinaryWriter bw)
        {
            if (data == null) data = new EntityData();
            bw.WriteCString(data.Name);
            bw.Write(new byte[4]); // Unused
            bw.Write(data.Flags);

            var props = data.Properties.Where(x => !ExcludedKeys.Contains(x.Key.ToLower())).ToList();
            bw.Write(props.Count);
            foreach (var p in props)
            {
                bw.WriteCString(p.Key);
                bw.WriteCString(p.Value);
            }
            bw.Write(new byte[12]); // Unused
        }

        private void WriteFace(Face face, BinaryWriter bw)
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
                bw.WriteCoordinate(vertex);
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
                bw.WriteCoordinate(cam.EyePosition);
                bw.WriteCoordinate(cam.LookPosition);
            }
        }
        
        #endregion
    }
}