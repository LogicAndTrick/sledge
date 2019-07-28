using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Precision.Plane;
using Polygon = Sledge.DataStructures.Geometric.Precision.Polygon;
using Polyhedron = Sledge.DataStructures.Geometric.Precision.Polyhedron;
using PVector3 = Sledge.DataStructures.Geometric.Precision.Vector3;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class VmfBspSourceProvider : IBspSourceProvider
    {
        [Import] private SerialisedObjectFormatter _formatter;
        [Import] private MapElementFactory _factory;
        [Import] private SquareGridFactory _squareGridFactory;

        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            // VMF supports everything because it's an extensible format
            typeof(IMapObject),
            typeof(IMapObjectData),
            typeof(IMapData),
        };

        public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

        [ImportingConstructor]
        public VmfBspSourceProvider([Import] Lazy<SerialisedObjectFormatter> formatter, [Import] Lazy<MapElementFactory> factory, [Import] Lazy<SquareGridFactory> squareGridFactory)
        {
            _formatter = formatter.Value;
            _factory = factory.Value;
            _squareGridFactory = squareGridFactory.Value;
        }

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; } = new[]
        {
            new FileExtensionInfo("Valve map format", ".vmf", ".vmx"), 
        };

        public async Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment)
        {
            var task = await Task.Factory.StartNew(async () =>
            {
                var result = new BspFileLoadResult();

                var map = new Map();
                var so = _formatter.Deserialize(stream).ToList();

                foreach (var s in so) s.Name = s.Name?.ToLower();

                // If the sledge_native node is found, we should use it to maximise compatibility
                var native = so.FirstOrDefault(x => x.Name == "sledge_native");
                if (native != null)
                {
                    foreach (var o in native.Children)
                    {
                        if (o.Name == nameof(Root))
                        {
                            map.Root.Unclone((Root) _factory.Deserialise(o));
                        }
                        else
                        {
                            map.Data.Add((IMapData) _factory.Deserialise(o));
                        }
                    }
                }
                else
                {
                    // Load a VHE4 VMF format
                    LoadVisgroups(map, so.FirstOrDefault(x => x.Name == "visgroups"));
                    LoadWorld(map, so);
                    LoadCameras(map, so.FirstOrDefault(x => x.Name == "cameras"));
                    LoadCordon(map, so.FirstOrDefault(x => x.Name == "cordon"));
                    await LoadViewSettings(map, so.FirstOrDefault(x => x.Name == "viewsettings"), environment);

                    await Task.FromResult(0);
                }

                map.Root.DescendantsChanged();

                result.Map = map;
                return result;
            });

            return await task;
        }

        #region Reading

        private void LoadVisgroups(Map map, SerialisedObject visgroups)
        {
            if (visgroups == null) return;
            foreach (var vg in visgroups.Children.Where(x => x.Name?.ToLower() == "visgroup"))
            {
                var v = new Visgroup
                {
                    Name = vg.Get("name", ""),
                    ID = vg.Get("visgroupid", -1),
                    Colour = vg.GetColor("color"),
                    Visible = true
                };
                if (v.Colour.GetBrightness() < 0.001f) v.Colour = Colour.GetRandomBrushColour();
                if (v.ID < 0) continue;
                map.Data.Add(v);
            }
        }

        private void LoadWorld(Map map, List<SerialisedObject> objects)
        {
            var vos = objects.Select(VmfObject.Deserialise).Where(vo => vo != null).ToList();

            var world = vos.OfType<VmfWorld>().FirstOrDefault();
            if (world == null) return;

            // A map of ids from the map -> ids from the vmf
            var mapToSource = new Dictionary<long, long>();
            
            world.Editor.Apply(map.Root);
            mapToSource.Add(map.Root.ID, world.ID);

            var tree = new List<VmfObject>();

            foreach (var vo in vos)
            {
                // Set the parent id to the world id
                if (vo.ID != world.ID && vo.Editor.ParentID == 0) vo.Editor.ParentID = world.ID;

                // Flatten the tree (nested hiddens -> no more hiddens)
                // (Flat tree includes self as well)
                var flat = vo.Flatten().ToList();

                // Set the default parent id for all the child objects
                foreach (var child in flat)
                {
                    if (child.Editor.ParentID == 0) child.Editor.ParentID = vo.ID;
                }

                // Add the objects to the tree
                tree.AddRange(flat);
            }

            tree.Remove(world);

            // All objects should have proper ids by now, get rid of anything with parentid 0 just in case
            var grouped = tree.GroupBy(x => x.Editor.ParentID).Where(x => x.Key > 0).ToDictionary(x => x.Key, x => x.ToList());
            
            // Step through each level of the tree and add them to their parent branches
            var leaves = new List<IMapObject> {map.Root};

            // Use a iteration limit of 1000. If the tree's that deep, I don't want to load your map anyway...
            for (var i = 0; i < 1000 && leaves.Any(); i++) // i.e. while (leaves.Any())
            {
                var newLeaves = new List<IMapObject>();
                foreach (var leaf in leaves)
                {
                    var sourceId = mapToSource[leaf.ID];
                    if (!grouped.ContainsKey(sourceId)) continue;

                    var items = grouped[sourceId];

                    // Create objects from items
                    foreach (var item in items)
                    {
                        var mapObject = item.ToMapObject(map.NumberGenerator);
                        mapToSource.Add(mapObject.ID, item.ID);
                        mapObject.Hierarchy.Parent = leaf;
                        newLeaves.Add(mapObject);
                    }
                }
                leaves = newLeaves;
            }

            // Now we should have a nice neat hierarchy of objects
        }

        private void LoadCameras(Map map, SerialisedObject cameras)
        {
            if (cameras == null) return;
            var activeCam = cameras.Get("activecamera", 0);

            var cams = cameras.Children.Where(x => x.Name?.ToLower() == "camera").ToList();
            for (var i = 0; i < cams.Count; i++)
            {
                var cm = cams[i];
                map.Data.Add(new Camera
                {
                    EyePosition = cm.Get("position", Vector3.Zero),
                    LookPosition = cm.Get("look", Vector3.UnitX),
                    IsActive = activeCam == i
                });
            }
        }

        private void LoadCordon(Map map, SerialisedObject cordon)
        {
            if (cordon == null) return;

            var start = cordon.Get("mins", Vector3.One * -1024);
            var end = cordon.Get("maxs", Vector3.One * 1024);
            map.Data.Add(new CordonBounds
            {
                Box = new Box(start, end),
                Enabled = cordon.Get("active", 0) > 0
            });
        }

        private async Task LoadViewSettings(Map map, SerialisedObject viewsettings, IEnvironment environment)
        {
            if (viewsettings == null) return;

            var snapToGrid = viewsettings.Get("bSnapToGrid", 1) == 1;
            var show2DGrid = viewsettings.Get("bShowGrid", 1) == 1;
            var show3DGrid = viewsettings.Get("bShow3DGrid", 0) == 1; // todo, I guess
            var gridSpacing = viewsettings.Get("nGridSpacing", 64);

            var grid = show2DGrid ? await _squareGridFactory.Create(environment) : new NoGrid();
            if (grid is SquareGrid sg) sg.Step = gridSpacing;

            map.Data.Add(new GridData(grid)
            {
                SnapToGrid = snapToGrid
            });

            var ignoreGrouping = viewsettings.Get("bIgnoreGrouping", 0) == 1;
            map.Data.Add(new SelectionOptions
            {
                IgnoreGrouping = ignoreGrouping
            });

            var hideFaceMask = viewsettings.Get("bHideFaceMask", 0) == 1;
            map.Data.Add(new HideFaceMask
            {
                Hidden = hideFaceMask
            });

            var hideNullTextures = viewsettings.Get("bHideNullTextures", 0) == 1;
            map.Data.Add(new DisplayFlags
            {
                HideNullTextures = hideNullTextures,
                HideDisplacementSolids = false
            });

            var textureLock = viewsettings.Get("bTextureLock", 1) == 1;
            var textureScalingLock = viewsettings.Get("bTextureScalingLock", 0) == 1;
            map.Data.Add(new TransformationFlags
            {
                TextureLock = textureLock,
                TextureScaleLock = textureScalingLock
            });
        }

        #endregion

        public Task Save(Stream stream, Map map)
        {
            return Task.Factory.StartNew(() =>
            {
                var list = new List<SerialisedObject>();

                SaveVisgroups(map, list);
                SaveWorld(map, list);
                SaveCameras(map, list);
                SaveCordon(map, list);
                SaveViewSettings(map, list);

                var native = new SerialisedObject("sledge_native");
                native.Children.Add(_factory.Serialise(map.Root));
                native.Children.AddRange(map.Data.Select(_factory.Serialise).Where(x => x != null));
                list.Add(native);

                _formatter.Serialize(stream, list);
            });
        }

        #region Saving

        private void SaveVisgroups(Map map, List<SerialisedObject> list)
        {
            var so = new SerialisedObject("visgroups");
            foreach (var visgroup in map.Data.OfType<Visgroup>())
            {
                var vgo = new SerialisedObject("visgroup");
                vgo.Set("name", visgroup.Name);
                vgo.Set("visgroupid", visgroup.ID);
                vgo.SetColor("color", visgroup.Colour);
                so.Children.Add(vgo);
            }
            list.Add(so);
        }

        private void SaveWorld(Map map, List<SerialisedObject> list)
        {
            // call the avengers

            // World is a normal entity, except it should also include groups
            var world = map.Root;
            var sw = SerialiseEntity(world);
            foreach (var group in world.FindAll().OfType<Group>())
            {
                var sg = VmfObject.Serialise(group);
                if (sg != null) sw.Children.Add(sg.ToSerialisedObject());
            }
            if (sw != null) list.Add(sw);

            // Entities are separate from the world
            var entities = map.Root.FindAll().OfType<Entity>().Select(SerialiseEntity).ToList();
            list.AddRange(entities);
        }

        private SerialisedObject SerialiseEntity(IMapObject obj)
        {
            var self = VmfObject.Serialise(obj);
            if (self == null) return null;
            
            var so = self.ToSerialisedObject();

            foreach (var solid in obj.FindAll().OfType<Solid>())
            {
                var s = VmfObject.Serialise(solid);
                if (s != null) so.Children.Add(s.ToSerialisedObject());
            }

            return so;
        }

        private void SaveCameras(Map map, List<SerialisedObject> list)
        {
            var cams = map.Data.OfType<Camera>().ToList();
            if (!cams.Any()) return;

            var so = new SerialisedObject("cameras");
            for (var i = 0; i < cams.Count; i++)
            {
                var camera = cams[i];
                if (camera.IsActive) so.Set("activecamera", i);

                var vgo = new SerialisedObject("camera");
                vgo.Set("position", $"[{FormatVector3(camera.EyePosition)}]");
                vgo.Set("look", $"[{FormatVector3(camera.LookPosition)}]");
                so.Children.Add(vgo);
            }

            list.Add(so);
        }

        private void SaveCordon(Map map, List<SerialisedObject> list)
        {
            var cordon = map.Data.GetOne<CordonBounds>();
            if (cordon == null) return;

            var so = new SerialisedObject("cordon");

            so.Set("mins", $"({FormatVector3(cordon.Box.Start)})");
            so.Set("maxs", $"({FormatVector3(cordon.Box.End)})");
            so.Set("active", cordon.Enabled ? 1 : 0);

            list.Add(so);
        }

        private void SaveViewSettings(Map map, List<SerialisedObject> list)
        {
            var so = new SerialisedObject("viewsettings");

            var grid = map.Data.GetOne<GridData>();
            var sel = map.Data.GetOne<SelectionOptions>();
            var face = map.Data.GetOne<HideFaceMask>();
            var dis = map.Data.GetOne<DisplayFlags>();
            var tf = map.Data.GetOne<TransformationFlags>();

            if (grid != null)
            {
                so.Set("bSnapToGrid", grid.SnapToGrid ? 1 : 0);
                so.Set("bShowGrid", grid.Grid is NoGrid ? 0 : 1);
                so.Set("bShow3DGrid", 0);
                so.Set("nGridSpacing", grid.Grid is SquareGrid s ? s.Step : 64);
            }

            if (sel != null)
            {
                so.Set("bIgnoreGrouping", sel.IgnoreGrouping ? 1 : 0);
            }

            if (face != null)
            {
                so.Set("bHideFaceMask", face.Hidden ? 1 : 0);
            }

            if (dis != null)
            {
                so.Set("bHideNullTextures", dis.HideNullTextures ? 1 : 0);
            }

            if (tf != null)
            {
                so.Set("bTextureLock", tf.TextureLock ? 1 : 0);
                so.Set("bTextureScalingLock", tf.TextureScaleLock ? 1 : 0);
            }

            list.Add(so);
        }
        
        #endregion
        
        private static string FormatVector3(Vector3 c)
        {
            return $"{FormatDecimal(c.X)} {FormatDecimal(c.Y)} {FormatDecimal(c.Z)}";
        }

        private static string FormatDecimal(float d)
        {
            return d.ToString("0.00####", CultureInfo.InvariantCulture);
        }

        private static bool ParseFloatArray(string input, char[] splitChars, int expected, out float[] array)
        {
            var spl = input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length == expected)
            {
                var parsed = spl.Select(x => float.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out var o) ? (float?) o : null).ToList();
                if (parsed.All(x => x.HasValue))
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    array = parsed.Select(x => x.Value).ToArray();
                    return true;
                }
            }
            array = new float[expected];
            return false;
        }

        private static bool ParseDoubleArray(string input, char[] splitChars, int expected, out double[] array)
        {
            var spl = input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length == expected)
            {
                var parsed = spl.Select(x => double.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out var o) ? (double?) o : null).ToList();
                if (parsed.All(x => x.HasValue))
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    array = parsed.Select(x => x.Value).ToArray();
                    return true;
                }
            }
            array = new double[expected];
            return false;
        }

        private abstract class VmfObject
        {
            public long ID { get; set; }
            public VmfEditor Editor { get; set; }

            protected VmfObject(SerialisedObject obj)
            {
                ID = obj.Get("id", 0L);
                Editor = new VmfEditor(obj.Children.FirstOrDefault(x => x.Name == "editor"));
            }

            protected VmfObject(IMapObject obj)
            {
                ID = obj.ID;
                Editor = new VmfEditor(obj);
            }

            public abstract IEnumerable<VmfObject> Flatten();
            public abstract IMapObject ToMapObject(UniqueNumberGenerator generator);
            public abstract SerialisedObject ToSerialisedObject();

            public static VmfObject Deserialise(SerialisedObject obj)
            {
                switch (obj.Name)
                {
                    case "world":
                        return new VmfWorld(obj);
                    case "entity":
                        return new VmfEntity(obj);
                    case "group":
                        return new VmfGroup(obj);
                    case "solid":
                        return new VmfSolid(obj);
                    case "hidden":
                        return new VmfHidden(obj);
                }
                return null;
            }

            public static VmfObject Serialise(IMapObject obj)
            {
                if (obj is Root r) return new VmfWorld(r);
                if (obj is Entity e) return new VmfEntity(e);
                if (obj is Group g) return new VmfGroup(g);
                if (obj is Solid s) return new VmfSolid(s);
                return null;
            }
        }

        private class VmfEntity : VmfObject
        {
            public List<VmfObject> Objects { get; set; }
            public EntityData EntityData { get; set; }
            public Vector3? Origin { get; set; }

            private static readonly string[] ExcludedKeys = { "id", "spawnflags", "classname", "origin", "wad", "mapversion" };

            public VmfEntity(SerialisedObject obj) : base(obj)
            {
                Objects = new List<VmfObject>();
                foreach (var so in obj.Children)
                {
                    var o = Deserialise(so);
                    if (o != null) Objects.Add(o);
                }

                var ed = new EntityData();
                foreach (var kv in obj.Properties)
                {
                    if (kv.Key == null || ExcludedKeys.Contains(kv.Key.ToLower())) continue;
                    ed.Set(kv.Key, kv.Value);
                }
                ed.Name = obj.Get("classname", "");
                ed.Flags = obj.Get("spawnflags", 0);
                EntityData = ed;

                if (obj.Properties.Any(x => x.Key == "origin"))
                {
                    Origin = obj.Get("origin", Vector3.Zero);
                }
            }

            public VmfEntity(Entity ent) : this((IMapObject) ent)
            {
                
            }

            protected VmfEntity(IMapObject obj) : base(obj)
            {
                EntityData = obj.Data.GetOne<EntityData>() ?? new EntityData();
                Origin = obj.Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
            }

            public override IEnumerable<VmfObject> Flatten()
            {
                return Objects.SelectMany(x => x.Flatten()).Union(new[] {this});
            }

            public override IMapObject ToMapObject(UniqueNumberGenerator generator)
            {
                var ent = new Entity(generator.Next("MapObject"));

                ent.Data.Add(EntityData);
                if (Origin != null) ent.Data.Add(new Origin(Origin.Value));

                Editor.Apply(ent);

                return ent;
            }

            protected virtual string SerialisedObjectName => "entity";

            public override SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject(SerialisedObjectName);
                so.Set("id", ID);
                so.Set("classname", EntityData.Name);
                so.Set("spawnflags", EntityData.Flags);
                if (Origin != null)
                {
                    so.Set("origin", Origin);
                }
                foreach (var prop in EntityData.Properties)
                {
                    so.Properties.Add(new KeyValuePair<string, string>(prop.Key, prop.Value));
                }

                so.Children.Add(Editor.ToSerialisedObject());

                return so;
            }
        }

        private class VmfWorld : VmfEntity
        {
            public VmfWorld(SerialisedObject obj) : base(obj)
            {
            }

            public VmfWorld(Root root) : base(root)
            {
            }

            public override IMapObject ToMapObject(UniqueNumberGenerator generator)
            {
                throw new NotSupportedException();
            }

            protected override string SerialisedObjectName => "world";
        }

        private class VmfGroup : VmfObject
        {
            public VmfGroup(SerialisedObject obj) : base(obj)
            {
            }

            public VmfGroup(Group grp) : base(grp)
            {
            }

            public override IEnumerable<VmfObject> Flatten()
            {
                yield return this;
            }

            public override IMapObject ToMapObject(UniqueNumberGenerator generator)
            {
                var grp = new Group(generator.Next("MapObject"));
                Editor.Apply(grp);
                return grp;
            }

            public override SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject("group");
                so.Set("id", ID);

                so.Children.Add(Editor.ToSerialisedObject());

                return so;
            }
        }

        private class VmfSolid : VmfObject
        {
            public List<VmfSide> Sides { get; set; }

            public VmfSolid(SerialisedObject obj) : base(obj)
            {
                Sides = new List<VmfSide>();
                foreach (var so in obj.Children.Where(x => x.Name == "side"))
                {
                    Sides.Add(new VmfSide(so));
                }
            }

            public VmfSolid(Solid sol) : base(sol)
            {
                Sides = sol.Faces.Select(x => new VmfSide(x)).ToList();
            }

            public override IEnumerable<VmfObject> Flatten()
            {
                yield return this;
            }

            public override IMapObject ToMapObject(UniqueNumberGenerator generator)
            {
                var sol = new Solid(generator.Next("MapObject"));
                Editor.Apply(sol);
                CreateFaces(sol, Sides, generator);
                return sol;
            }

            public override SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject("solid");
                so.Set("id", ID);
                so.Children.AddRange(Sides.Select(x => x.ToSerialisedObject()));
                so.Children.Add(Editor.ToSerialisedObject());
                return so;
            }

            private void CreateFaces(Solid solid, List<VmfSide> sides, UniqueNumberGenerator generator)
            {
                // If all the sides don't have enough vertices, calculate them
                if (!sides.All(x => x.Vertices.Count >= 3))
                {
                    // We need to create the solid from intersecting planes
                    var poly = new Polyhedron(sides.Select(x => x.Plane));
                    foreach (var side in sides)
                    {
                        side.Vertices.Clear();
                        var pg = poly.Polygons.FirstOrDefault(x => x.Plane.Normal.EquivalentTo(side.Plane.Normal));
                        if (pg == null)
                        {
                            continue;
                        }
                        side.Vertices.AddRange(pg.Vertices.Select(x => x.ToStandardVector3()));
                    }
                }

                foreach (var emptySide in sides.Where(x => !x.Vertices.Any()))
                {
                    Console.WriteLine(emptySide.ID);
                }

                // We know the vertices, now create the faces
                foreach (var side in sides)
                {
                    var face = new Face(generator.Next("Face"))
                    {
                        Plane = side.Plane.ToStandardPlane(),
                        Texture = side.Texture
                    };
                    face.Vertices.AddRange(side.Vertices);
                    if (face.Vertices.Any()) solid.Data.Add(face);
                }

                solid.DescendantsChanged();
            }
        }

        private class VmfHidden : VmfObject
        {
            public List<VmfObject> Objects { get; set; }

            public VmfHidden(SerialisedObject obj) : base(obj)
            {
                Objects = new List<VmfObject>();
                foreach (var so in obj.Children)
                {
                    var o = VmfObject.Deserialise(so);
                    if (o != null) Objects.Add(o);
                }
            }

            public override IEnumerable<VmfObject> Flatten()
            {
                return Objects.SelectMany(x => x.Flatten());
            }

            public override IMapObject ToMapObject(UniqueNumberGenerator generator)
            {
                throw new NotSupportedException();
            }

            public override SerialisedObject ToSerialisedObject()
            {
                throw new NotImplementedException();
            }
        }

        private class VmfSide
        {
            public long ID { get; set; }
            public Plane Plane { get; set; }
            public Texture Texture { get; set; }
            public float LightmapScale { get; set; }
            public string SmoothingGroups { get; set; } // ?
            public List<Vector3> Vertices { get; set; }

            public VmfSide(SerialisedObject obj)
            {
                ID = obj.Get("ID", 0L);
                LightmapScale = obj.Get("lightmapscale", 0);
                SmoothingGroups = obj.Get("smoothing_groups", "");

                if (ParseDoubleArray(obj.Get("plane", ""), new[] {' ', '(', ')'}, 9, out double[] pl))
                {
                    Plane = new Plane(
                        new PVector3(pl[0], pl[1], pl[2]).Round(),
                        new PVector3(pl[3], pl[4], pl[5]).Round(),
                        new PVector3(pl[6], pl[7], pl[8]).Round()
                    );
                }
                else
                {
                    Plane = new Plane(PVector3.UnitZ, 0);
                }

                Texture = new Texture
                {
                    Name = obj.Get("material", ""),
                    Rotation = obj.Get("rotation", 0f)
                };
                if (ParseFloatArray(obj.Get("uaxis", ""), new[] {' ', '[', ']'}, 5, out float[] ua))
                {
                    Texture.UAxis = new Vector3(ua[0], ua[1], ua[2]);
                    Texture.XShift = ua[3];
                    Texture.XScale = ua[4];
                }
                if (ParseFloatArray(obj.Get("vaxis", ""), new[] {' ', '[', ']'}, 5, out float[] va))
                {
                    Texture.VAxis = new Vector3(va[0], va[1], va[2]);
                    Texture.YShift = va[3];
                    Texture.YScale = va[4];
                }

                // Older versions of sledge save vertices, this is entirely optional but why not.
                Vertices = new List<Vector3>();
                var verts = obj.Children.FirstOrDefault(x => x.Name == "vertex");
                if (verts == null) return;

                var count = obj.Get("count", 0);
                for (var i = 0; i < count; i++)
                {
                    var pt = obj.Get<Vector3>("vertex" + i);
                    if (pt == null)
                    {
                        Vertices.Clear();
                        break;
                    }
                    Vertices.Add(pt);
                }
            }

            public VmfSide(Face face)
            {
                ID = face.ID;
                Plane = face.Plane.ToPrecisionPlane();
                Texture = face.Texture;
                Vertices = face.Vertices.ToList();
            }

            public SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject("side");
                so.Set("id", ID);
                so.Set("plane", $"({FormatVector3(Vertices[0])}) ({FormatVector3(Vertices[1])}) ({FormatVector3(Vertices[2])})");
                so.Set("material", Texture.Name);
                so.Set("uaxis", $"[{FormatVector3(Texture.UAxis)} {FormatDecimal(Texture.XShift)}] {FormatDecimal(Texture.XScale)}");
                so.Set("vaxis", $"[{FormatVector3(Texture.VAxis)} {FormatDecimal(Texture.YShift)}] {FormatDecimal(Texture.YScale)}");
                so.Set("rotation", Texture.Rotation);
                so.Set("lightmapscale", LightmapScale);
                so.Set("smoothing_groups", SmoothingGroups);

                var verts = new SerialisedObject("vertex");
                verts.Set("count", Vertices.Count);
                for (var i = 0; i < Vertices.Count; i++)
                {
                    verts.Set("vertex" + i, FormatVector3(Vertices[i]));
                }
                so.Children.Add(verts);

                return so;
            }
        }

        private class VmfEditor
        {
            public Color Color { get; set; }
            public bool VisgroupShown { get; set; }
            public bool VisgroupAutoShown { get; set; }
            public List<long> VisgroupIDs { get; set; }
            private long GroupID { get; set; }
            public long ParentID { get; set; }

            public VmfEditor(SerialisedObject obj)
            {
                if (obj == null) obj = new SerialisedObject("editor");

                Color = obj.GetColor("color");
                GroupID = obj.Get("groupid", 0);
                ParentID = GroupID > 0 ? GroupID : obj.Get("parentid", 0);
                VisgroupShown = obj.Get("visgroupshown", "1") == "1";
                VisgroupAutoShown = obj.Get("visgroupautoshown", "1") == "1";
                // logicalpos?

                VisgroupIDs = new List<long>();
                foreach (var vid in obj.Properties.Where(x => x.Key == "visgroupid"))
                {
                    if (long.TryParse(vid.Value, out long id)) VisgroupIDs.Add(id);
                }
            }

            public VmfEditor(IMapObject obj)
            {
                Color = obj.Data.GetOne<ObjectColor>()?.Color ?? Color.Red;
                VisgroupShown = VisgroupAutoShown = true;
                VisgroupIDs = obj.Data.Get<VisgroupID>().Select(x => x.ID).ToList();
                GroupID = obj.Hierarchy.Parent is Group ? obj.Hierarchy.Parent.ID : 0;
                ParentID = obj.Hierarchy.Parent?.ID ?? 0;
            }

            public void Apply(IMapObject obj)
            {
                var c = Color.GetBrightness() > 0 ? Color : Colour.GetRandomBrushColour();
                obj.Data.Replace(new ObjectColor(c));
                foreach (var id in VisgroupIDs)
                {
                    obj.Data.Add(new VisgroupID(id));
                }
            }

            public SerialisedObject ToSerialisedObject()
            {
                var so = new SerialisedObject("editor");
                so.SetColor("color", Color);
                so.Set("parentid", ParentID);
                so.Set("groupid", GroupID);
                so.Set("visgroupshown", VisgroupShown ? "1" : "0");
                so.Set("visgroupautoshown", VisgroupAutoShown ? "1" : "0");
                foreach (var id in VisgroupIDs.Distinct())
                {
                    so.Properties.Add(new KeyValuePair<string, string>("visgroupid", Convert.ToString(id, CultureInfo.InvariantCulture)));
                }
                return so;
            }
        }
    }
}