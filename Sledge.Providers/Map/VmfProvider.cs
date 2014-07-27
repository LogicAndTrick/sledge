using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    public class VmfProvider : MapProvider
    {
        protected override IEnumerable<MapFeature> GetFormatFeatures()
        {
            return new[]
            {
                MapFeature.Worldspawn,
                MapFeature.Solids,
                MapFeature.Entities,
                MapFeature.Groups,

                MapFeature.Displacements,
                MapFeature.Instances,

                MapFeature.Colours,
                MapFeature.SingleVisgroups,
                MapFeature.MultipleVisgroups,
                MapFeature.Cameras,
                MapFeature.CordonBounds,
                MapFeature.ViewSettings
            };
        }

        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".vmf", true, CultureInfo.InvariantCulture)
                || filename.EndsWith(".vmx", true, CultureInfo.InvariantCulture);
        }

        private static long GetObjectID(GenericStructure gs, IDGenerator generator)
        {
            var id = gs.PropertyLong("id");
            if (id == 0) id = generator.GetNextObjectID();
            return id;
        }

        private static void FlattenTree(MapObject parent, List<Solid> solids, List<Entity> entities, List<Group> groups)
        {
            foreach (var mo in parent.GetChildren())
            {
                if (mo is Solid)
                {
                    solids.Add((Solid) mo);
                }
                else if (mo is Entity)
                {
                    entities.Add((Entity) mo);
                }
                else if (mo is Group)
                {
                    groups.Add((Group) mo);
                    FlattenTree(mo, solids, entities, groups);
                }
            }
        }

        private static string FormatCoordinate(Coordinate c)
        {
            return c.X.ToString("0.00000000", CultureInfo.InvariantCulture)
                + " " + c.Y.ToString("0.00000000", CultureInfo.InvariantCulture)
                + " " + c.Z.ToString("0.00000000", CultureInfo.InvariantCulture);
        }

        private static string FormatColor(Color c)
        {
            return c.R.ToString(CultureInfo.InvariantCulture)
                + " " + c.G.ToString(CultureInfo.InvariantCulture)
                + " " + c.B.ToString(CultureInfo.InvariantCulture);
        }

        private static readonly string[] ExcludedKeys = new[] {"id", "spawnflags", "classname", "origin", "wad", "mapversion"};

        private static EntityData ReadEntityData(GenericStructure structure)
        {
            var ret = new EntityData();
            foreach (var key in structure.GetPropertyKeys())
            {
                if (ExcludedKeys.Contains(key.ToLower())) continue;
                ret.SetPropertyValue(key, structure[key]);
            }
            ret.Name = structure["classname"];
            ret.Flags = structure.PropertyInteger("spawnflags");
            return ret;
        }

        private static void WriteEntityData(GenericStructure obj, EntityData data)
        {
            foreach (var property in data.Properties.OrderBy(x => x.Key))
            {
                obj[property.Key] = property.Value;
            }
            obj["spawnflags"] = data.Flags.ToString(CultureInfo.InvariantCulture);
        }

        private static GenericStructure WriteEditor(MapObject obj)
        {
            var editor = new GenericStructure("editor");
            editor["color"] = FormatColor(obj.Colour);
            foreach (var visgroup in obj.Visgroups.Except(obj.AutoVisgroups).OrderBy(x => x)) 
            {
                editor.AddProperty("visgroupid", visgroup.ToString(CultureInfo.InvariantCulture));
            }
            editor["visgroupshown"] = "1";
            editor["visgroupautoshown"] = "1";
            if (obj.Parent is Group) editor["groupid"] = obj.Parent.ID.ToString(CultureInfo.InvariantCulture);
            if (obj.Parent != null) editor["parentid"] = obj.Parent.ID.ToString(CultureInfo.InvariantCulture);
            return editor;
        }

        private static Displacement ReadDisplacement(long id, GenericStructure dispinfo)
        {
            var disp = new Displacement(id);
            // power, startposition, flags, elevation, subdiv, normals{}, distances{},
            // offsets{}, offset_normals{}, alphas{}, triangle_tags{}, allowed_verts{}
            disp.SetPower(dispinfo.PropertyInteger("power", 3));
            disp.StartPosition = dispinfo.PropertyCoordinate("startposition");
            disp.Elevation = dispinfo.PropertyDecimal("elevation");
            disp.SubDiv = dispinfo.PropertyInteger("subdiv") > 0;
            var size = disp.Resolution + 1;
            var normals = dispinfo.GetChildren("normals").FirstOrDefault();
            var distances = dispinfo.GetChildren("distances").FirstOrDefault();
            var offsets = dispinfo.GetChildren("offsets").FirstOrDefault();
            var offsetNormals = dispinfo.GetChildren("offset_normals").FirstOrDefault();
            var alphas = dispinfo.GetChildren("alphas").FirstOrDefault();
            //var triangleTags = dispinfo.GetChildren("triangle_tags").First();
            //var allowedVerts = dispinfo.GetChildren("allowed_verts").First();
            for (var i = 0; i < size; i++)
            {
                var row = "row" + i;
                var norm = normals != null ? normals.PropertyCoordinateArray(row, size) : Enumerable.Range(0, size).Select(x => Coordinate.Zero).ToArray();
                var dist = distances != null ? distances.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                var offn = offsetNormals != null ? offsetNormals.PropertyCoordinateArray(row, size) : Enumerable.Range(0, size).Select(x => Coordinate.Zero).ToArray();
                var offs = offsets != null ? offsets.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                var alph = alphas != null ? alphas.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                for (var j = 0; j < size; j++)
                {
                    disp.Points[i, j].Displacement = new Vector(norm[j], dist[j]);
                    disp.Points[i, j].OffsetDisplacement = new Vector(offn[j], offs[j]);
                    disp.Points[i, j].Alpha = alph[j];
                }
            }
            return disp;
        }

        private static GenericStructure WriteDisplacement(Displacement disp)
        {
            throw new NotImplementedException();
        }

        private static Face ReadFace(GenericStructure side, IDGenerator generator)
        {
            var id = side.PropertyLong("id");
            if (id == 0) id = generator.GetNextFaceID();
            var dispinfo = side.GetChildren("dispinfo").FirstOrDefault();
            var ret = dispinfo != null ? ReadDisplacement(id, dispinfo) : new Face(id);
            // id, plane, material, uaxis, vaxis, rotation, lightmapscale, smoothing_groups
            var uaxis = side.PropertyTextureAxis("uaxis");
            var vaxis = side.PropertyTextureAxis("vaxis");
            ret.Texture.Name = side["material"];
            ret.Texture.UAxis = uaxis.Item1;
            ret.Texture.XShift = uaxis.Item2;
            ret.Texture.XScale = uaxis.Item3;
            ret.Texture.VAxis = vaxis.Item1;
            ret.Texture.YShift = vaxis.Item2;
            ret.Texture.YScale = vaxis.Item3;
            ret.Texture.Rotation = side.PropertyDecimal("rotation");
            ret.Plane = side.PropertyPlane("plane");

            var verts = side.Children.FirstOrDefault(x => x.Name == "vertex");
            if (verts != null)
            {
                var count = verts.PropertyInteger("count");
                for (var i = 0; i < count; i++)
                {
                    ret.Vertices.Add(new Vertex(verts.PropertyCoordinate("vertex"+i), ret));
                }
            }

            return ret;
        }

        private static GenericStructure WriteFace(Face face)
        {
            var ret = new GenericStructure("side");
            ret["id"] = face.ID.ToString(CultureInfo.InvariantCulture);
            ret["plane"] = String.Format("({0}) ({1}) ({2})",
                                         FormatCoordinate(face.Vertices[0].Location),
                                         FormatCoordinate(face.Vertices[1].Location),
                                         FormatCoordinate(face.Vertices[2].Location));
            ret["material"] = face.Texture.Name;
            ret["uaxis"] = String.Format(CultureInfo.InvariantCulture, "[{0} {1}] {2}", FormatCoordinate(face.Texture.UAxis), face.Texture.XShift, face.Texture.XScale);
            ret["vaxis"] = String.Format(CultureInfo.InvariantCulture, "[{0} {1}] {2}", FormatCoordinate(face.Texture.VAxis), face.Texture.YShift, face.Texture.YScale);
            ret["rotation"] = face.Texture.Rotation.ToString(CultureInfo.InvariantCulture);
            // ret["lightmapscale"]
            // ret["smoothing_groups"]

            var verts = new GenericStructure("vertex");
            verts["count"] = face.Vertices.Count.ToString(CultureInfo.InvariantCulture);
            for (var i = 0; i < face.Vertices.Count; i++)
            {
                verts["vertex" + i] = FormatCoordinate(face.Vertices[i].Location);
            }
            ret.Children.Add(verts);

            var disp = face as Displacement;
            if (disp != null)
            {
                ret.Children.Add(WriteDisplacement(disp));
            }

            return ret;
        }

        private static Solid ReadSolid(GenericStructure solid, IDGenerator generator)
        {
            var editor = solid.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
            var faces = solid.GetChildren("side").Select(x => ReadFace(x, generator)).ToList();

            Solid ret;

            if (faces.All(x => x.Vertices.Count >= 3))
            {
                // Vertices were stored in the VMF
                ret = new Solid(GetObjectID(solid, generator));
                ret.Faces.AddRange(faces);
            }
            else
            {
                // Need to grab the vertices using plane intersections
                var idg = new IDGenerator(); // No need to increment the id generator if it doesn't have to be
                ret = Solid.CreateFromIntersectingPlanes(faces.Select(x => x.Plane), idg);
                ret.ID = GetObjectID(solid, generator);

                for (var i = 0; i < ret.Faces.Count; i++)
                {
                    var face = ret.Faces[i];
                    var f = faces.FirstOrDefault(x => x.Plane.Normal.EquivalentTo(ret.Faces[i].Plane.Normal));
                    if (f == null)
                    {
                        // TODO: Report invalid solids
                        Debug.WriteLine("Invalid solid! ID: " + solid["id"]);
                        return null;
                    }
                    face.Texture = f.Texture;

                    var disp = f as Displacement;
                    if (disp == null) continue;

                    disp.Plane = face.Plane;
                    disp.Vertices = face.Vertices;
                    disp.Texture = f.Texture;
                    disp.AlignTextureToWorld();
                    disp.CalculatePoints();
                    ret.Faces[i] = disp;
                }
            }

            ret.Colour = editor.PropertyColour("color", Colour.GetRandomBrushColour());
            ret.Visgroups.AddRange(editor.GetAllPropertyValues("visgroupid").Select(int.Parse).Where(x => x > 0));
            foreach (var face in ret.Faces)
            {
                face.Parent = ret;
                face.Colour = ret.Colour;
                face.UpdateBoundingBox();
            }

            if (ret.Faces.Any(x => x is Displacement))
            {
                ret.Faces.ForEach(x => x.IsHidden = !(x is Displacement));
            }

            ret.UpdateBoundingBox(false);

            return ret;
        }

        private static GenericStructure WriteSolid(Solid solid)
        {
            var ret = new GenericStructure("solid");
            ret["id"] = solid.ID.ToString(CultureInfo.InvariantCulture);

            foreach (var face in solid.Faces.OrderBy(x => x.ID))
            {
                ret.Children.Add(WriteFace(face));
            }

            var editor = WriteEditor(solid);
            ret.Children.Add(editor);

            if (solid.IsVisgroupHidden)
            {
                var hidden = new GenericStructure("hidden");
                hidden.Children.Add(ret);
                ret = hidden;
            }

            return ret;
        }

        private static Entity ReadEntity(GenericStructure entity, IDGenerator generator)
        {
            var ret = new Entity(GetObjectID(entity, generator))
                          {
                              ClassName = entity["classname"],
                              EntityData = ReadEntityData(entity),
                              Origin = entity.PropertyCoordinate("origin")
                          };
            var editor = entity.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
            ret.Colour = editor.PropertyColour("color", Colour.GetRandomBrushColour());
            ret.Visgroups.AddRange(editor.GetAllPropertyValues("visgroupid").Select(int.Parse).Where(x => x > 0));
            foreach (var child in entity.GetChildren("solid").Select(solid => ReadSolid(solid, generator)).Where(s => s != null))
            {
                child.SetParent(ret, false);
            }
            ret.UpdateBoundingBox(false);
            return ret;
        }

        private static GenericStructure WriteEntity(Entity ent)
        {
            var ret = new GenericStructure("entity");
            ret["id"] = ent.ID.ToString(CultureInfo.InvariantCulture);
            ret["classname"] = ent.EntityData.Name;
            WriteEntityData(ret, ent.EntityData);
            if (!ent.HasChildren) ret["origin"] = FormatCoordinate(ent.Origin);

            var editor = WriteEditor(ent);
            ret.Children.Add(editor);

            foreach (var solid in ent.GetChildren().SelectMany(x => x.FindAll()).OfType<Solid>().OrderBy(x => x.ID))
            {
                ret.Children.Add(WriteSolid(solid));
            }

            return ret;
        }

        private static Group ReadGroup(GenericStructure group, IDGenerator generator)
        {
            var g = new Group(GetObjectID(group, generator));
            var editor = group.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
            g.Colour = editor.PropertyColour("color", Colour.GetRandomBrushColour());
            g.Visgroups.AddRange(editor.GetAllPropertyValues("visgroupid").Select(int.Parse).Where(x => x > 0));
            return g;
        }

        private static GenericStructure WriteGroup(Group group)
        {
            var ret = new GenericStructure("group");
            ret["id"] = group.ID.ToString(CultureInfo.InvariantCulture);

            var editor = WriteEditor(group);
            ret.Children.Add(editor);

            return ret;
        }

        private static World ReadWorld(GenericStructure world, IDGenerator generator)
        {
            var ret = new World(GetObjectID(world, generator))
                          {
                              ClassName = "worldspawn",
                              EntityData = ReadEntityData(world)
                          };

            // Load groups
            var groups = new Dictionary<Group, long>();
            foreach (var group in world.GetChildren("group"))
            {
                var g = ReadGroup(group, generator);
                var editor = group.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
                var gid = editor.PropertyLong("groupid");
                groups.Add(g, gid);
            }

            // Build group tree
            var assignedGroups = groups.Where(x => x.Value == 0).Select(x => x.Key).ToList();
            foreach (var ag in assignedGroups)
            {
                // Add the groups with no parent
                ag.SetParent(ret, false);
                groups.Remove(ag);
            }
            while (groups.Any())
            {
                var canAssign = groups.Where(x => assignedGroups.Any(y => y.ID == x.Value)).ToList();
                if (!canAssign.Any())
                {
                    break;
                }
                foreach (var kv in canAssign)
                {
                    // Add the group to the tree and the assigned list, remove it from the groups list
                    var parent = assignedGroups.First(y => y.ID == kv.Value);
                    kv.Key.SetParent(parent, false);
                    assignedGroups.Add(kv.Key);
                    groups.Remove(kv.Key);
                }
            }

            // Load visible solids
            foreach (var read in world.GetChildren("solid").AsParallel().Select(x => new { Solid = ReadSolid(x, generator), Structure = x}))
            {
                var s = read.Solid;
                var solid = read.Structure;
                if (s == null) continue;

                var editor = solid.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
                var gid = editor.PropertyLong("groupid");
                var parent = gid > 0 ? assignedGroups.FirstOrDefault(x => x.ID == gid) ?? (MapObject) ret : ret;
                s.SetParent(parent, false);
            }

            // Load hidden solids
            foreach (var hidden in world.GetChildren("hidden"))
            {
                foreach (var read in hidden.GetChildren("solid").AsParallel().Select(x => new { Solid = ReadSolid(x, generator), Structure = x }))
                {
                    var s = read.Solid;
                    var solid = read.Structure;
                    if (s == null) continue;

                    s.IsVisgroupHidden = true;

                    var editor = solid.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
                    var gid = editor.PropertyLong("groupid");
                    var parent = gid > 0 ? assignedGroups.FirstOrDefault(x => x.ID == gid) ?? (MapObject)ret : ret;
                    s.SetParent(parent, false);
                }
            }

            assignedGroups.ForEach(x => x.UpdateBoundingBox());
            ret.UpdateBoundingBox();

            return ret;
        }

        private static GenericStructure WriteWorld(DataStructures.MapObjects.Map map, IEnumerable<Solid> solids, IEnumerable<Group> groups)
        {
            var world = map.WorldSpawn;
            var ret = new GenericStructure("world");
            ret["id"] = world.ID.ToString(CultureInfo.InvariantCulture);
            ret["classname"] = "worldspawn";
            ret["mapversion"] = map.Version.ToString(CultureInfo.InvariantCulture);
            WriteEntityData(ret, world.EntityData);

            foreach (var solid in solids.OrderBy(x => x.ID))
            {
                ret.Children.Add(WriteSolid(solid));
            }

            foreach (var group in groups.OrderBy(x => x.ID))
            {
                ret.Children.Add(WriteGroup(group));
            }

            return ret;
        }

        private static Visgroup ReadVisgroup(GenericStructure visgroup)
        {
            var v = new Visgroup
                        {
                            Name = visgroup["name"],
                            ID = visgroup.PropertyInteger("visgroupid"),
                            Colour = visgroup.PropertyColour("color", Colour.GetRandomBrushColour()),
                            Visible = true
                        };
            return v;
        }

        private static GenericStructure WriteVisgroup(Visgroup visgroup)
        {
            var ret = new GenericStructure("visgroup");
            ret["name"] = visgroup.Name;
            ret["visgroupid"] = visgroup.ID.ToString(CultureInfo.InvariantCulture);
            ret["color"] = FormatColor(visgroup.Colour);
            return ret;
        }

        public static GenericStructure CreateCopyStream(List<MapObject> objects)
        {
            var stream = new GenericStructure("clipboard");

            var entitySolids = objects.OfType<Entity>().SelectMany(x => x.Find(y => y is Solid)).ToList();
            stream.Children.AddRange(objects.OfType<Solid>().Where(x => !x.IsCodeHidden && !x.IsVisgroupHidden && !entitySolids.Contains(x)).Select(WriteSolid));
            stream.Children.AddRange(objects.OfType<Group>().Select(WriteGroup));
            stream.Children.AddRange(objects.OfType<Entity>().Select(WriteEntity));

            return stream;
        }

        public static IEnumerable<MapObject> ExtractCopyStream(GenericStructure gs, IDGenerator generator)
        {
            if (gs == null || gs.Name != "clipboard") return null;
            var dummyGen = new IDGenerator();
            var list = new List<MapObject>();
            var world = ReadWorld(gs, dummyGen);
            foreach (var entity in gs.GetChildren("entity"))
            {
                var ent = ReadEntity(entity, dummyGen);
                var groupid = entity.Children.Where(x => x.Name == "editor").Select(x => x.PropertyInteger("groupid")).FirstOrDefault();
                var entParent = groupid > 0 ? world.Find(x => x.ID == groupid && x is Group).FirstOrDefault() ?? world : world;
                ent.SetParent(entParent);
            }
            list.AddRange(world.GetChildren());
            Reindex(list, generator);
            return list;
        }

        private static void Reindex(IEnumerable<MapObject> objs, IDGenerator generator)
        {
            foreach (var o in objs)
            {
                if (o is Solid) ((Solid) o).Faces.ForEach(x => x.ID = generator.GetNextFaceID());

                // Remove the children
                var children = o.GetChildren().ToList();
                children.ForEach(x => x.SetParent(null));

                // re-index the children
                Reindex(children, generator);

                // Change the ID
                o.ID = generator.GetNextObjectID();

                // Re-add the children
                children.ForEach(x => x.SetParent(o));

                if (!o.HasChildren) o.UpdateBoundingBox();
            }
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var parent = new GenericStructure("Root");
                parent.Children.AddRange(GenericStructure.Parse(reader));
                // Sections from a Hammer map:
                // - world
                // - entity
                // - visgroups
                // - cordon
                // Not done yet
                // - versioninfo
                // - viewsettings
                // - cameras

                var map = new DataStructures.MapObjects.Map();

                var world = parent.GetChildren("world").FirstOrDefault();
                var entities = parent.GetChildren("entity");
                var visgroups = parent.GetChildren("visgroups").SelectMany(x => x.GetChildren("visgroup"));
                var cameras = parent.GetChildren("cameras").FirstOrDefault();
                var cordon = parent.GetChildren("cordon").FirstOrDefault();
                var viewsettings = parent.GetChildren("viewsettings").FirstOrDefault();

                foreach (var visgroup in visgroups)
                {
                    var vg = ReadVisgroup(visgroup);
                    if (vg.ID < 0 && vg.Name == "Auto") continue; 
                    map.Visgroups.Add(vg);
                }

                if (world != null) map.WorldSpawn = ReadWorld(world, map.IDGenerator);
                foreach (var entity in entities)
                {
                    var ent = ReadEntity(entity, map.IDGenerator);
                    var groupid = entity.Children.Where(x => x.Name == "editor").Select(x => x.PropertyInteger("groupid")).FirstOrDefault();
                    var entParent = groupid > 0 ? map.WorldSpawn.Find(x => x.ID == groupid && x is Group).FirstOrDefault() ?? map.WorldSpawn : map.WorldSpawn;
                    ent.SetParent(entParent);
                }

                var activeCamera = 0;
                if (cameras != null)
                {
                    activeCamera = cameras.PropertyInteger("activecamera");
                    foreach (var cam in cameras.GetChildren("camera"))
                    {
                        var pos = cam.PropertyCoordinate("position");
                        var look = cam.PropertyCoordinate("look");
                        if (pos != null && look != null)
                        {
                            map.Cameras.Add(new Camera {EyePosition = pos, LookPosition = look});
                        }
                    }
                }
                if (!map.Cameras.Any())
                {
                    map.Cameras.Add(new Camera { EyePosition = Coordinate.Zero, LookPosition = Coordinate.UnitY });
                }
                if (activeCamera < 0 || activeCamera >= map.Cameras.Count)
                {
                    activeCamera = 0;
                }
                map.ActiveCamera = map.Cameras[activeCamera];

                if (cordon != null)
                {
                    var start = cordon.PropertyCoordinate("mins", map.CordonBounds.Start);
                    var end = cordon.PropertyCoordinate("maxs", map.CordonBounds.End);
                    map.CordonBounds = new Box(start, end);
                    map.Cordon = cordon.PropertyBoolean("active", map.Cordon);
                }

                if (viewsettings != null)
                {
                    map.SnapToGrid = viewsettings.PropertyBoolean("bSnapToGrid", map.SnapToGrid);
                    map.Show2DGrid = viewsettings.PropertyBoolean("bShowGrid", map.Show2DGrid);
                    map.Show3DGrid = viewsettings.PropertyBoolean("bShow3DGrid", map.Show3DGrid);
                    map.GridSpacing = viewsettings.PropertyDecimal("nGridSpacing", map.GridSpacing);
                    map.IgnoreGrouping = viewsettings.PropertyBoolean("bIgnoreGrouping", map.IgnoreGrouping);
                    map.HideFaceMask = viewsettings.PropertyBoolean("bHideFaceMask", map.HideFaceMask);
                    map.HideNullTextures = viewsettings.PropertyBoolean("bHideNullTextures", map.HideNullTextures);
                    map.TextureLock = viewsettings.PropertyBoolean("bTextureLock", map.TextureLock);
                    map.TextureScalingLock = viewsettings.PropertyBoolean("bTextureScalingLock", map.TextureScalingLock);
                }

                return map;
            }
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            var groups = new List<Group>();
            var solids = new List<Solid>();
            var ents = new List<Entity>();
            FlattenTree(map.WorldSpawn, solids, ents, groups);

            var fvi = FileVersionInfo.GetVersionInfo(typeof (VmfProvider).Assembly.Location);
            var versioninfo = new GenericStructure("versioninfo");
            versioninfo.AddProperty("editorname", "Sledge");
            versioninfo.AddProperty("editorversion", fvi.ProductMajorPart.ToString(CultureInfo.InvariantCulture) + "." + fvi.ProductMinorPart.ToString(CultureInfo.InvariantCulture));
            versioninfo.AddProperty("editorbuild", fvi.ProductBuildPart.ToString(CultureInfo.InvariantCulture));
            versioninfo.AddProperty("mapversion", map.Version.ToString(CultureInfo.InvariantCulture));
            versioninfo.AddProperty("formatversion", "100");
            versioninfo.AddProperty("prefab", "0");

            var visgroups = new GenericStructure("visgroups");
            foreach (var visgroup in map.Visgroups.OrderBy(x => x.ID).Where(x => !x.IsAutomatic))
            {
                visgroups.Children.Add(WriteVisgroup(visgroup));
            }

            var viewsettings = new GenericStructure("viewsettings");

            viewsettings.AddProperty("bSnapToGrid", map.SnapToGrid ? "1" : "0");
            viewsettings.AddProperty("bShowGrid", map.Show2DGrid ? "1" : "0");
            viewsettings.AddProperty("bShow3DGrid", map.Show3DGrid ? "1" : "0");
            viewsettings.AddProperty("nGridSpacing", map.GridSpacing.ToString(CultureInfo.InvariantCulture));
            viewsettings.AddProperty("bIgnoreGrouping", map.IgnoreGrouping ? "1" : "0");
            viewsettings.AddProperty("bHideFaceMask", map.HideFaceMask ? "1" : "0");
            viewsettings.AddProperty("bHideNullTextures", map.HideNullTextures ? "1" : "0");
            viewsettings.AddProperty("bTextureLock", map.TextureLock ? "1" : "0");
            viewsettings.AddProperty("bTextureScalingLock", map.TextureScalingLock ? "1" : "0");

            var world = WriteWorld(map, solids, groups);

            var entities = ents.OrderBy(x => x.ID).Select(WriteEntity).ToList();

            var cameras = new GenericStructure("cameras");
            cameras.AddProperty("activecamera", map.Cameras.IndexOf(map.ActiveCamera).ToString(CultureInfo.InvariantCulture));
            foreach (var cam in map.Cameras)
            {
                var camera = new GenericStructure("camera");
                camera.AddProperty("position", "[" + FormatCoordinate(cam.EyePosition) + "]");
                camera.AddProperty("look", "[" + FormatCoordinate(cam.LookPosition) + "]");
                cameras.Children.Add(camera);
            }

            var cordon = new GenericStructure("cordon");
            cordon.AddProperty("mins", map.CordonBounds.Start.ToString());
            cordon.AddProperty("maxs", map.CordonBounds.End.ToString());
            cordon.AddProperty("active", map.Cordon ? "1" : "0");

            using (var sw = new StreamWriter(stream))
            {
                versioninfo.PrintToStream(sw);
                visgroups.PrintToStream(sw);
                viewsettings.PrintToStream(sw);
                world.PrintToStream(sw);
                entities.ForEach(e => e.PrintToStream(sw));
                cameras.PrintToStream(sw);
                cordon.PrintToStream(sw);
            }
        }
    }
}
