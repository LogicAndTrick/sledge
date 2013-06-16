using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common;

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

        public Map()
        {
            Version = 1;
            Visgroups = new List<Visgroup>();
            Cameras = new List<Camera>();
            ActiveCamera = null;
            IDGenerator = new IDGenerator();
            WorldSpawn = new World(IDGenerator.GetNextObjectID());

            Show2DGrid = SnapToGrid = true;
        }

        public IEnumerable<string> GetAllTextures()
        {
            return GetAllTexturesRecursive(WorldSpawn).Distinct();
        }

        private static IEnumerable<string> GetAllTexturesRecursive(MapObject obj)
        {
            if (obj is Entity && obj.Children.Count == 0)
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

            return obj.Children.SelectMany(GetAllTexturesRecursive);
        }

        /// <summary>
        /// Should be called when a map is loaded. Sets up visgroups, object ids, gamedata, and textures.
        /// </summary>
        public void PostLoadProcess(GameData.GameData gameData, Func<string, ITexture> textureAccessor)
        {
            PartialPostLoadProcess(x => true, gameData, textureAccessor);

            var all = WorldSpawn.FindAll();

            // Set maximum ids
            var maxObjectId = all.Max(x => x.ID);
            var faces = all.OfType<Solid>().SelectMany(x => x.Faces).ToList();
            var maxFaceId = faces.Any() ? faces.Max(x => x.ID) : 0;
            IDGenerator.Reset(maxObjectId, maxFaceId);

            // todo visgroups
            // WorldSpawn.ForEach(x => x.IsVisgroupHidden, x => x.IsVisgroupHidden = true, true);

            // Purge empty groups
            foreach (var emptyGroup in WorldSpawn.Find(x => x is Group && !x.Children.Any()))
            {
                emptyGroup.Parent.Children.Remove(emptyGroup);
            }
        }

        public void PartialPostLoadProcess(GameData.GameData gameData, Func<string, ITexture> textureAccessor)
        {
            PartialPostLoadProcess(x => (x is Entity && (((Entity)x ).GameData == null || ((Entity)x).Decal != null))
                || (x is Solid && ((Solid) x).Faces.Any(y => y.Texture.Texture == null)), gameData, textureAccessor);
        }

        public void PartialPostLoadProcess(Predicate<MapObject> matcher, GameData.GameData gameData, Func<string, ITexture> textureAccessor)
        {
            var objects = WorldSpawn.Find(matcher);
            foreach (var obj in objects)
            {
                if (obj is Entity)
                {
                    var ent = (Entity) obj;
                    var gd = gameData.Classes.FirstOrDefault(x => x.Name == ent.EntityData.Name);
                    ent.GameData = gd;
                    if (gd != null)
                    {
                        var beh = gd.Behaviours.FirstOrDefault(x => x.Name == "iconsprite");
                        if (beh != null && beh.Values.Count == 1) ent.Sprite = textureAccessor(beh.Values[0]);
                    }
                    if (ent.EntityData.Name == "infodecal")
                    {
                        var tex = ent.EntityData.Properties.FirstOrDefault(x => x.Key == "texture");
                        if (tex != null) ent.Decal = textureAccessor(tex.Value);
                        ent.CalculateDecalGeometry();
                    }
                    ent.UpdateBoundingBox();
                }
                else if (obj is Solid)
                {
                    ((Solid)obj).Faces.ForEach(f =>
                    {
                        f.Texture.Texture = textureAccessor(f.Texture.Name);
                        f.CalculateTextureCoordinates();
                    });
                }
            }
        }
    }
}
