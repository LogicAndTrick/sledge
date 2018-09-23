using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.ChangeHandling;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.GameData;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    [Export(typeof(IMapDocumentChangeHandler))]
    public class EntityModelChangeHandler : IMapDocumentChangeHandler
    {
        private readonly Lazy<ResourceCollection> _resourceCollection;

        public string OrderHint => "M";

        [ImportingConstructor]
        public EntityModelChangeHandler(
            [Import] Lazy<ResourceCollection> resourceCollection
        )
        {
            _resourceCollection = resourceCollection;
        }

        public async Task Changed(Change change)
        {
            var gd = await change.Document.Environment.GetGameData();
            foreach (var entity in change.Added.Union(change.Updated).OfType<Entity>())
            {
                var modelName = GetModelName(entity, gd);
                var existingEntityModel = entity.Data.GetOne<EntityModel>();

                // If the model data is unchanged then we can skip
                if (ModelDataMatches(existingEntityModel, modelName)) continue;

                // Load the model if the name is specified
                // This doesn't cause unnecessary load as if the model is already loaded then
                // nothing will happen, and otherwise we need to load the model anyway.
                IModel model = null;
                if (!String.IsNullOrWhiteSpace(modelName))
                {
                    model = await _resourceCollection.Value.GetModel(change.Document.Environment, modelName);
                    if (model == null) modelName = null;
                }

                // If there's no model then we need to remove the entity model if it exists
                if (model == null)
                {
                    if (entity.Data.Remove(x => x is EntityModel) > 0) entity.DescendantsChanged();
                    return;
                }

                var sd = new EntityModel(modelName, model);
                entity.Data.Replace(sd);
                entity.DescendantsChanged();
            }
        }

        private bool ModelDataMatches(EntityModel model, string name)
        {
            return String.IsNullOrWhiteSpace(name)
                ? model == null
                : string.Equals(model?.Name, name, StringComparison.InvariantCultureIgnoreCase) && model?.Model != null;
        }

        private static string GetModelName(Entity entity, GameData gd)
        {
            if (entity.Hierarchy.HasChildren || String.IsNullOrWhiteSpace(entity.EntityData.Name)) return null;
            var cls = gd?.GetClass(entity.EntityData.Name);
            if (cls == null) return null;

            var studio = cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "studio", StringComparison.InvariantCultureIgnoreCase))
                         ?? cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (studio == null) return null;

            // First see if the studio behaviour forces a model...
            if (studio.Values.Count == 1 && !String.IsNullOrWhiteSpace(studio.Values[0]))
            {
                return studio.Values[0].Trim();
            }

            // Find the first property that is a studio type, or has a name of "model"...
            var prop = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Studio) ??
                       cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "model", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.Get(prop.Name, prop.DefaultValue);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }
    }
}
