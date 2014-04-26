using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Map
    {
        public decimal Version { get; set; }
        public List<Visgroup> Visgroups { get; private set; }
        public List<Camera> Cameras { get; private set; }
        public Camera ActiveCamera { get; set; }
        public World WorldSpawn { get; set; }
        public IDGenerator IDGenerator { get; private set; }

        public bool Show2DGrid { get; set; }
        public bool Show3DGrid { get; set; }
        public bool SnapToGrid { get; set; }
        public decimal GridSpacing { get; set; }
        public bool HideFaceMask { get; set; }
        public bool HideDisplacementSolids { get; set; }
        public bool HideNullTextures { get; set; }
        public bool IgnoreGrouping { get; set; }
        public bool TextureLock { get; set; }
        public bool TextureScalingLock { get; set; }
        public bool Cordon { get; set; }
        public Box CordonBounds { get; set; }

        public Map()
        {
            Version = 1;
            Visgroups = new List<Visgroup>();
            Cameras = new List<Camera>();
            ActiveCamera = null;
            IDGenerator = new IDGenerator();
            WorldSpawn = new World(IDGenerator.GetNextObjectID());

            Show2DGrid = SnapToGrid = true;
            TextureLock = true;
            HideDisplacementSolids = true;
            CordonBounds = new Box(Coordinate.One * -1024, Coordinate.One * 1024);
        }

        public IEnumerable<MapFeature> GetUsedFeatures()
        {
            var all = WorldSpawn.FindAll();

            // Too generic: this should be assumed
            // yield return MapFeature.Worldspawn;

            if (all.Any(x => x is Solid))
                yield return MapFeature.Solids;

            if (all.Any(x => x is Entity))
                yield return MapFeature.Entities;

            if (all.Any(x => x is Group))
                yield return MapFeature.Groups;

            if (all.OfType<Solid>().Any(x => x.Faces.Any(y => y is Displacement)))
                yield return MapFeature.Displacements;
            
            // Not implemented yet
            // yield return MapFeature.Instances;

            if (Visgroups.Any())
                yield return MapFeature.SingleVisgroups;

            if (all.Any(x => x.Visgroups.Count > 1))
                yield return MapFeature.MultipleVisgroups;

            // If we have more than one camera, we care about losing them
            if (Cameras.Count > 1)
                yield return MapFeature.Cameras;

            // Not important enough to care about:
            // yield return MapFeature.Colours;
            // yield return MapFeature.CordonBounds;
            // yield return MapFeature.ViewSettings;
        }

        public TransformFlags GetTransformFlags()
        {
            var flags = TransformFlags.None;
            if (TextureLock) flags |= TransformFlags.TextureLock;
            if (TextureScalingLock) flags |= TransformFlags.TextureScalingLock;
            return flags;
        }

        public IEnumerable<string> GetAllTextures()
        {
            return GetAllTexturesRecursive(WorldSpawn).Distinct();
        }

        private static IEnumerable<string> GetAllTexturesRecursive(MapObject obj)
        {
            if (obj is Entity && obj.ChildCount == 0)
            {
                var ent = (Entity) obj;
                if (ent.EntityData.Name == "infodecal")
                {
                    var tex = ent.EntityData.Properties.FirstOrDefault(x => x.Key == "texture");
                    if (tex != null) return new[] {tex.Value};
                }
            }
            else if (obj is Solid)
            {
                return ((Solid) obj).Faces.Select(f => f.Texture.Name);
            }

            return obj.GetChildren().SelectMany(GetAllTexturesRecursive);
        }

        /// <summary>
        /// Should be called when a map is loaded. Sets up visgroups, object ids, gamedata, and textures.
        /// </summary>
        public void PostLoadProcess(GameData.GameData gameData, Func<string, ITexture> textureAccessor, Func<string, float> textureOpacity)
        {
            PartialPostLoadProcess(gameData, textureAccessor, textureOpacity);

            var all = WorldSpawn.FindAll();

            // Set maximum ids
            var maxObjectId = all.Max(x => x.ID);
            var faces = all.OfType<Solid>().SelectMany(x => x.Faces).ToList();
            var maxFaceId = faces.Any() ? faces.Max(x => x.ID) : 0;
            IDGenerator.Reset(maxObjectId, maxFaceId);

            // todo visgroups
            // WorldSpawn.ForEach(x => x.IsVisgroupHidden, x => x.IsVisgroupHidden = true, true);

            // Auto visgroup
            var auto = AutoVisgroup.GetDefaultAutoVisgroup();
            var quickHide = new AutoVisgroup {ID = int.MinValue, IsHidden = true, Name = "Autohide", Parent = auto, Visible = false};
            auto.Children.Add(quickHide);
            Visgroups.Add(auto);
            UpdateAutoVisgroups(all, false);

            // Purge empty groups
            foreach (var emptyGroup in WorldSpawn.Find(x => x is Group && !x.HasChildren))
            {
                emptyGroup.SetParent(null);
            }
        }

        public void UpdateAutoVisgroups(MapObject node, bool recursive)
        {
            var nodes = recursive ? node.FindAll() : new List<MapObject> { node };
            UpdateAutoVisgroups(nodes, false);
        }

        public void UpdateAutoVisgroups(IEnumerable<MapObject> nodes, bool recursive)
        {
            var autos = GetAllVisgroups().OfType<AutoVisgroup>().Where(x => x.Filter != null).ToList();
            var list = recursive ? nodes.SelectMany(x => x.FindAll()) : nodes;
            foreach (var o in list)
            {
                var obj = o;
                obj.Visgroups.RemoveAll(x => o.AutoVisgroups.Contains(x));
                obj.AutoVisgroups.Clear();
                foreach (var vg in autos.Where(x => x.Filter(obj)))
                {
                    // Add this visgroup and all parents
                    Visgroup visgroup = vg;
                    while (visgroup != null)
                    {
                        if (o.AutoVisgroups.Contains(visgroup.ID)) break; // Break out of infinite loop (just in case)
                        o.AutoVisgroups.Add(visgroup.ID);
                        visgroup = visgroup.Parent;
                    }
                }
                o.Visgroups.AddRange(o.AutoVisgroups);
            }
        }

        public IEnumerable<Visgroup> GetAllVisgroups()
        {
            return GetAllVisgroups(Visgroups);
        }

        private IEnumerable<Visgroup> GetAllVisgroups(IEnumerable<Visgroup> groups)
        {
            var g = groups.ToList();
            return g.SelectMany(x => GetAllVisgroups(x.Children)).Union(g);
        }

        public void PartialPostLoadProcess(GameData.GameData gameData, Func<string, ITexture> textureAccessor, Func<string, float> textureOpacity)
        {
            var objects = WorldSpawn.FindAll();
            foreach (var obj in objects)
            {
                if (obj is Entity)
                {
                    var ent = (Entity) obj;
                    if (ent.GameData == null || !String.Equals(ent.GameData.Name, ent.EntityData.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var gd = gameData.Classes.FirstOrDefault(x => x.Name == ent.EntityData.Name);
                        ent.GameData = gd;
                        ent.UpdateBoundingBox();
                    }
                }
                else if (obj is Solid)
                {
                    var s = ((Solid)obj);
                    var disp = HideDisplacementSolids && s.Faces.Any(x => x is Displacement);
                    s.Faces.ForEach(f =>
                    {
                        if (f.Texture.Texture == null)
                        {
                            f.Texture.Texture = textureAccessor(f.Texture.Name.ToLowerInvariant());
                            f.CalculateTextureCoordinates(true);
                        }
                        if (disp && !(f is Displacement))
                        {
                            f.Opacity = 0;
                        }
                        else if (f.Texture.Texture != null)
                        {
                            f.Opacity = textureOpacity(f.Texture.Name.ToLowerInvariant());
                            if (!HideNullTextures && f.Opacity < 0.1) f.Opacity = 1;
                        }
                    });
                }
            }
        }

        public Camera GetActiveCamera()
        {
            if (!Cameras.Any() || ActiveCamera == null) return null;
            return ActiveCamera;
        }
    }
}
