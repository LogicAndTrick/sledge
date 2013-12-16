using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common;
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
            if (file == null) return;
            SetModel(e, ModelProvider.CreateModelReference(file));
        }

        public static bool ShouldHaveModel(this Entity entity)
        {
            return GetModelName(entity) != null;
        }

        public static string GetModelName(this Entity entity)
        {
            if (entity.GameData == null) return null;
            var studio = entity.GameData.Behaviours.FirstOrDefault(x => x.Name == "studio");
            if (studio == null || studio.Values.Count != 1) return null;
            var model = studio.Values[0].Trim();
            return String.IsNullOrWhiteSpace(model) ? null : model;
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
