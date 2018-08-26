using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    [Export(typeof(IMapDocumentChangeHandler))]
    public class EntitySpriteChangeHandler : IMapDocumentChangeHandler
    {
        public async Task Changed(Change change)
        {
            var gd = await change.Document.Environment.GetGameData();
            foreach (var entity in change.Added.Union(change.Updated).OfType<Entity>())
            {
                var sn = GetSpriteName(entity, gd);
                if (sn == null) entity.Data.Remove(x => x is EntitySprite);
                else entity.Data.Replace(CreateSpriteData(entity, gd, sn));
            }
        }

        private static EntitySprite CreateSpriteData(Entity entity, GameData gd, string name)
        {
            var cls = gd?.GetClass(entity.EntityData.Name);
            var scale = 1f;
            var color = Color.Black;

            if (cls != null)
            {
                if (cls.Properties.Any(x => String.Equals(x.Name, "scale", StringComparison.CurrentCultureIgnoreCase)))
                {
                    scale = entity.EntityData.Get<float>("scale", 1);
                    if (scale <= 0.1f) scale = 1;
                }

                var colProp = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Color255 || x.VariableType == VariableType.Color1);
                if (colProp != null)
                {
                    var col = entity.EntityData.GetVector3(colProp.Name);
                    if (colProp.VariableType == VariableType.Color1) col *= 255f;
                    if (col.HasValue) color = col.Value.ToColor();
                }
            }

            return new EntitySprite(name, scale, color);
        }

        private static string GetSpriteName(Entity entity, GameData gd)
        {
            if (entity.Hierarchy.HasChildren || String.IsNullOrWhiteSpace(entity.EntityData.Name)) return null;
            var cls = gd?.GetClass(entity.EntityData.Name);
            if (cls == null) return null;

            var spr = cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase))
                      ?? cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "iconsprite", StringComparison.InvariantCultureIgnoreCase));
            if (spr == null) return null;

            // First see if the studio behaviour forces a model...
            if (spr.Values.Count == 1 && !String.IsNullOrWhiteSpace(spr.Values[0]))
            {
                return spr.Values[0].Trim();
            }

            // Find the first property that is a studio type, or has a name of "sprite"...
            var prop = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Sprite) ??
                       cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.Get<string>(prop.Name);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }
    }
}
