using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Providers.Model;
using Sledge.Providers.Texture;

namespace Sledge.Editor.Extensions
{
    public static class SpriteExtensions
    {
        private const string SpriteMetaKey = "Model";
        private const string SpriteBoundingBoxMetaKey = "BoundingBox";

        public static bool UpdateSprites(this Map map, Document document)
        {
            if (Sledge.Settings.View.DisableSpriteRendering) return false;
            return UpdateSprites(document, map.WorldSpawn);
        }

        public static bool UpdateSprites(this Map map, Document document, IEnumerable<MapObject> objects)
        {
            if (Sledge.Settings.View.DisableSpriteRendering) return false;

            var updated = false;
            foreach (var mo in objects) updated |= UpdateSprites(document, mo);
            return updated;
        }

        private static bool UpdateSprites(Document document, MapObject mo)
        {
            var updatedChildren = false;
            foreach (var child in mo.GetChildren()) updatedChildren |= UpdateSprites(document, child);

            var e = mo as Entity;
            if (e == null || !ShouldHaveSprite(e))
            {
                var has = e != null && HasSprite(e);
                if (has) UnsetSprite(e);
                return updatedChildren || has;
            }

            var sprite = GetSpriteName(e);
            var existingSprite = e.MetaData.Get<string>(SpriteMetaKey);
            if (String.Equals(sprite, existingSprite, StringComparison.InvariantCultureIgnoreCase)) return updatedChildren; // Already set; No need to continue

            var tex = document.TextureCollection.GetItem(sprite);
            if (tex == null)
            {
                UnsetSprite(e);
                return true;
            }
            SetSprite(e, tex);
            return true;
        }

        private static bool ShouldHaveSprite(Entity entity)
        {
            return GetSpriteName(entity) != null;
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

        private static void SetSprite(Entity entity, TextureItem tex)
        {
            entity.MetaData.Set(SpriteMetaKey, tex.Name);
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
                entity.MetaData.Set(SpriteBoundingBoxMetaKey, new Box(-bb / 2, bb / 2));
                entity.MetaData.Set("RotateBoundingBox", false); // todo rotations
                entity.UpdateBoundingBox();
            }
        }

        private static void UnsetSprite(Entity entity)
        {
            entity.MetaData.Unset(SpriteMetaKey);
            entity.MetaData.Unset(SpriteBoundingBoxMetaKey);
            entity.MetaData.Unset("RotateBoundingBox");
            entity.UpdateBoundingBox();
        }

        public static string GetSprite(this Entity entity)
        {
            return entity.MetaData.Get<string>(SpriteMetaKey);
        }

        public static bool HasSprite(this Entity entity)
        {
            return entity.MetaData.Has<string>(SpriteMetaKey);
        }
    }
}
