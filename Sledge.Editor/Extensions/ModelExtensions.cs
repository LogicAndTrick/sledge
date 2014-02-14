using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Providers.Model;

namespace Sledge.Editor.Extensions
{
    public static class ModelExtensions
    {
        private const string ModelMetaKey = "Model";
        private const string ModelBoundingBoxMetaKey = "ModelBoundingBox";

        public static void UpdateModels(this Map map, Document document)
        {
            if (Sledge.Settings.View.DisableModelRendering) return;
            UpdateModels(document, map.WorldSpawn);
        }

        private static void UpdateModels(Document document, MapObject mo)
        {
            mo.Children.ForEach(x => UpdateModels(document, x));
            var e = mo as Entity;
            if (e == null || !ShouldHaveModel(e)) return;
            var model = GetModelName(e);
            var file = document.Environment.Root.TraversePath(model);
            if (file == null || !ModelProvider.CanLoad(file)) return;
            try
            {
                SetModel(e, ModelProvider.CreateModelReference(file));
            }
            catch
            {
                // Couldn't load
            }
        }

        public static bool ShouldHaveModel(this Entity entity)
        {
            return GetModelName(entity) != null;
        }

        public static string GetModelName(this Entity entity)
        {
            if (entity.GameData == null) return null;
            var studio = entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "studio", StringComparison.InvariantCultureIgnoreCase));
            if (studio == null) return null;

            // First see if the studio behaviour forces a model...
            if (studio.Values.Count == 1 && !String.IsNullOrWhiteSpace(studio.Values[0]))
            {
                return studio.Values[0].Trim();
            }

            // Find the first property that is a studio type...
            var prop = entity.GameData.Properties.FirstOrDefault(x => x.VariableType == VariableType.Studio);
            if (prop == null) prop = entity.GameData.Properties.FirstOrDefault(x => String.Equals(x.Name, "model", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.GetPropertyValue(prop.Name);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        public static void SetModel(this Entity entity, ModelReference model)
        {
            entity.MetaData.Set(ModelMetaKey, model);
            entity.MetaData.Set(ModelBoundingBoxMetaKey, (Box) null); //todo.
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
