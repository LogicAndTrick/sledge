using System;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.FileSystem;
using Sledge.Providers.Model;
using Model = Sledge.Rendering.Scenes.Renderables.Model;

namespace Sledge.Editor.Rendering.Converters
{
    public class EntityModelConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.DefaultLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return !Sledge.Settings.View.DisableModelRendering && obj is Entity && GetModelName((Entity)obj) != null;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var entity = (Entity) obj;

            var model = GetModelName(entity);
            var file = document.Environment.Root.TraversePath(model);
            if (document.ModelCollection.CanLoad(file))
            {
                var modelReference = document.ModelCollection.GetModel(file);
                if (modelReference != null) 
                {
                    var mdl = CreateModelData(entity, modelReference, file.FullPathName);
                    smo.SceneObjects.Add(entity, mdl);
                    smo.MetaData.Add("ContentsReplaced", "True");
                    return true;
                }
            }

            ClearModelData(entity);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return false;
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

        private static Model CreateModelData(Entity entity, ModelReference model, string modelName)
        {
            entity.MetaData.Set("ModelName", modelName);
            entity.MetaData.Set("BoundingBox", model.Model.GetBoundingBox());
            entity.UpdateBoundingBox();

            var angles = entity.EntityData.GetPropertyCoordinate("angles", Coordinate.Zero).ToVector3();
            angles = new Vector3(MathHelper.DegreesToRadians(angles.Z), -MathHelper.DegreesToRadians(angles.X), MathHelper.DegreesToRadians(angles.Y));

            var renderable = new Model(modelName);
            renderable.Position = entity.Origin.ToVector3();
            renderable.Rotation = angles;
            return renderable;
        }

        private static void ClearModelData(Entity entity)
        {
            if (!entity.MetaData.Has<string>("ModelName") && !entity.MetaData.Has<Box>("BoundingBox")) return;
            entity.MetaData.Unset("ModelName");
            entity.MetaData.Unset("BoundingBox");
            entity.UpdateBoundingBox();
        }
    }
}