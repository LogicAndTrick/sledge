using System;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Renderables;
using Entity = Sledge.DataStructures.MapObjects.Entity;

namespace Sledge.BspEditor.Rendering.Converters
{
    public class EntitySpriteConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return !Sledge.Settings.View.DisableSpriteRendering && obj is Entity && GetSpriteName((Entity)obj) != null;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var entity = (Entity) obj;

            var spriteName = GetSpriteName(entity);
            TextureItem tex = document.TextureCollection.TryGetTextureItem(spriteName);
            if (tex != null)
            {
                var spr = CreateSpriteData(entity, tex, spriteName);
                smo.SceneObjects.Add(entity, spr);
                smo.MetaData.Add("ContentsReplaced", "True");
            }
            else
            {
                ClearSpriteData(entity);
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        private static string GetSpriteName(Entity entity)
        {
            if (entity.GameData == null) return null;
            var spr = entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase))
                      ?? entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "iconsprite", StringComparison.InvariantCultureIgnoreCase));
            if (spr == null) return null;

            // First see if the studio behaviour forces a model...
            if (spr.Values.Count == 1 && !String.IsNullOrWhiteSpace(spr.Values[0]))
            {
                return spr.Values[0].Trim();
            }

            // Find the first property that is a studio type, or has a name of "sprite"...
            var prop = entity.GameData.Properties.FirstOrDefault(x => x.VariableType == VariableType.Sprite);
            if (prop == null) prop = entity.GameData.Properties.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.GetPropertyValue(prop.Name);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private static Sprite CreateSpriteData(Entity entity, TextureItem tex, string spriteName)
        {
            var scale = 1m;
            if (entity.GameData != null && entity.GameData.Properties.Any(x => String.Equals(x.Name, "scale", StringComparison.CurrentCultureIgnoreCase)))
            {
                var scaleStr = entity.GetEntityData().GetPropertyValue("scale");
                if (!Decimal.TryParse(scaleStr, out scale)) scale = 1;
                if (scale <= 0.1m) scale = 1;
            }
            var bb = new Coordinate(tex.Width, tex.Width, tex.Height) * scale;

            // Don't set the bounding box if the sprite comes from the iconsprite gamedata
            if (entity.GameData == null || !entity.GameData.Behaviours.Any(x => String.Equals(x.Name, "iconsprite", StringComparison.CurrentCultureIgnoreCase)))
            {
                entity.MetaData.Set("BoundingBox", new Box(-bb / 2, bb / 2));
                entity.MetaData.Set("RotateBoundingBox", false); // todo rotations
                entity.UpdateBoundingBox();
            }

            var spr = new Sprite(entity.Origin.ToVector3(), Material.Texture(spriteName, true), (float)bb.X, (float)bb.Z);
            return spr;
        }

        private static void ClearSpriteData(Entity entity)
        {
            if (!entity.MetaData.Has<Box>("BoundingBox") && !entity.MetaData.Has<bool>("RotateBoundingBox")) return;
            entity.MetaData.Unset("BoundingBox");
            entity.MetaData.Unset("RotateBoundingBox");
            entity.UpdateBoundingBox();
        }
    }
}