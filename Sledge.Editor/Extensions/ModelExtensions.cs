using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Providers.Model;

namespace Sledge.Editor.Extensions
{
    public static class ModelExtensions
    {
        private const string ModelMetaKey = "Model";
        private const string ModelNameMetaKey = "ModelName";
        private const string ModelBoundingBoxMetaKey = "BoundingBox";

        public static bool UpdateModels(this Map map, Document document)
        {
            if (Sledge.Settings.View.DisableModelRendering) return false;

            var cache = document.GetMemory<Dictionary<string, ModelReference>>("ModelCache");
            if (cache == null)
            {
                cache = new Dictionary<string, ModelReference>();
                document.SetMemory("ModelCache", cache);
            }

            return UpdateModels(document, map.WorldSpawn, cache);
        }

        public static bool UpdateModels(this Map map, Document document, IEnumerable<MapObject> objects)
        {
            if (Sledge.Settings.View.DisableModelRendering) return false;

            var cache = document.GetMemory<Dictionary<string, ModelReference>>("ModelCache");
            if (cache == null)
            {
                cache = new Dictionary<string, ModelReference>();
                document.SetMemory("ModelCache", cache);
            }

            var updated = false;
            foreach (var mo in objects) updated |= UpdateModels(document, mo, cache);
            return updated;
        }

        private static bool UpdateModels(Document document, MapObject mo, Dictionary<string, ModelReference> cache)
        {
            var updatedChildren = false;
            foreach (var child in mo.GetChildren()) updatedChildren |= UpdateModels(document, child, cache);

            var e = mo as Entity;
            if (e == null || !ShouldHaveModel(e))
            {
                var has = e != null && HasModel(e);
                if (has) UnsetModel(e);
                return updatedChildren || has;
            }

            var model = GetModelName(e);
            var existingModel = e.MetaData.Get<string>(ModelNameMetaKey);
            if (String.Equals(model, existingModel, StringComparison.InvariantCultureIgnoreCase)) return updatedChildren; // Already set; No need to continue

            if (cache.ContainsKey(model))
            {
                var mr = cache[model];
                if (mr == null) UnsetModel(e);
                else SetModel(e, mr);
                return true;
            }
            else
            {
                var file = document.Environment.Root.TraversePath(model);
                if (file == null || !ModelProvider.CanLoad(file))
                {
                    // Model not valid, get rid of it
                    UnsetModel(e);
                    cache.Add(model, null);
                    return true;
                }

                try
                {
                    var mr = ModelProvider.CreateModelReference(file, document.Palette);
                    SetModel(e, mr);
                    cache.Add(model, mr);
                    return true;
                }
                catch
                {
                    // Couldn't load
                    cache.Add(model, null);
                    return updatedChildren;
                }
            }
        }

        private static bool ShouldHaveModel(Entity entity)
        {
            return GetModelName(entity) != null;
        }

        private static string GetModelName(Entity entity)
        {
            if (entity.GameData == null) return null;
            var studio = entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "studio", StringComparison.InvariantCultureIgnoreCase))
                         ?? entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (studio == null) return null;

            // First see if the studio behaviour forces a model...
            if (String.Equals(studio.Name, "studio", StringComparison.InvariantCultureIgnoreCase)
                && studio.Values.Count == 1 && !String.IsNullOrWhiteSpace(studio.Values[0]))
            {
                return studio.Values[0].Trim();
            }

            // Find the first property that is a studio type, or has a name of "model"...
            var prop = entity.GameData.Properties.FirstOrDefault(x => x.VariableType == VariableType.Studio);
            if (prop == null) prop = entity.GameData.Properties.FirstOrDefault(x => String.Equals(x.Name, "model", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.GetPropertyValue(prop.Name);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private static void SetModel(Entity entity, ModelReference model)
        {
            entity.MetaData.Set(ModelMetaKey, model);
            entity.MetaData.Set(ModelNameMetaKey, GetModelName(entity));
            entity.MetaData.Set(ModelBoundingBoxMetaKey, model.Model.GetBoundingBox());
            entity.UpdateBoundingBox();
        }

        private static void UnsetModel(Entity entity)
        {
            entity.MetaData.Unset(ModelMetaKey);
            entity.MetaData.Unset(ModelNameMetaKey);
            entity.MetaData.Unset(ModelBoundingBoxMetaKey);
            entity.UpdateBoundingBox();
        }

        public static ModelReference GetModel(this Entity entity)
        {
            return entity.MetaData.Get<ModelReference>(ModelMetaKey);
        }

        public static bool HasModel(this Entity entity)
        {
            return entity.MetaData.Has<ModelReference>(ModelMetaKey);
        }

        public static int HideDistance(this Entity entity)
        {
            return HasModel(entity) ? Sledge.Settings.View.ModelRenderDistance : int.MaxValue;
        }
    }
}
