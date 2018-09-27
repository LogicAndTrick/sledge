using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.ChangeHandling;
using Sledge.BspEditor.Primitives.MapObjectData;
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
                var modelDetails = GetModelDetails(entity, gd);
                var modelName = modelDetails?.Name;
                var existingEntityModel = entity.Data.GetOne<EntityModel>();

                // If the model data is unchanged then we can skip
                if (ModelDataMatches(existingEntityModel, modelDetails))
                {
                    if (existingEntityModel != null)
                    {
                        UpdateSequence(existingEntityModel, modelDetails);
                        entity.DescendantsChanged();
                    }
                    continue;
                }

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
                    continue;
                }

                var renderable = _resourceCollection.Value.CreateModelRenderable(change.Document.Environment, model);
                var sd = new EntityModel(modelName, renderable);
                UpdateSequence(sd, modelDetails);

                entity.Data.Replace(sd);
                entity.DescendantsChanged();
            }

            // Ensure removed entity models are disposed properly
            foreach (var rem in change.Removed)
            {
                var em = rem.Data.GetOne<EntityModel>();
                if (em?.Renderable == null) continue;

                _resourceCollection.Value.DestroyModelRenderable(change.Document.Environment, em.Renderable);
                rem.Data.Remove(em);
            }
        }

        private void UpdateSequence(EntityModel entityModel, ModelDetails modelDetails)
        {
            if (modelDetails == null || entityModel.Renderable == null) return;

            var sequences = entityModel.Renderable.Model.GetSequences();
            var seq = modelDetails.Sequence;
            if (seq >= sequences.Count) seq = -1;

            // Find the default sequence if one isn't set
            if (seq < 0) seq = sequences.IndexOf("idle");
            if (seq < 0) seq = sequences.FindIndex(x => x.StartsWith("idle", StringComparison.InvariantCultureIgnoreCase));
            if (seq < 0) seq = 0;
            
            entityModel.Renderable.Sequence = seq;

            entityModel.Renderable.Origin = modelDetails.Origin;
            entityModel.Renderable.Angles = modelDetails.Angles;
        }

        private bool ModelDataMatches(EntityModel model, ModelDetails details)
        {
            var name = details?.Name;
            return String.IsNullOrWhiteSpace(name)
                ? model == null
                : string.Equals(model?.Name, name, StringComparison.InvariantCultureIgnoreCase) && model?.Renderable != null;
        }

        private static ModelDetails GetModelDetails(Entity entity, GameData gd)
        {
            if (entity.Hierarchy.HasChildren || String.IsNullOrWhiteSpace(entity.EntityData.Name)) return null;
            var cls = gd?.GetClass(entity.EntityData.Name);
            if (cls == null) return null;

            var studio = cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "studio", StringComparison.InvariantCultureIgnoreCase))
                         ?? cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (studio == null) return null;

            var details = new ModelDetails();

            // First see if the studio behaviour forces a model...
            if (studio.Values.Count == 1 && !String.IsNullOrWhiteSpace(studio.Values[0]))
            {
                details.Name = studio.Values[0].Trim();
            }
            else
            {
                // Find the first property that is a studio type, or has a name of "model"...
                var prop = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Studio) ??
                           cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "model", StringComparison.InvariantCultureIgnoreCase));
                if (prop != null)
                {
                    var val = entity.EntityData.Get(prop.Name, prop.DefaultValue);
                    if (!String.IsNullOrWhiteSpace(val)) details.Name = val;
                }
            }

            if (details.Name != null)
            {
                var seqProp = cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "sequence", StringComparison.InvariantCultureIgnoreCase));
                var seqVal = entity.EntityData.Get("sequence", seqProp?.DefaultValue);
                if (!string.IsNullOrWhiteSpace(seqVal) && int.TryParse(seqVal, out var v)) details.Sequence = v;

                details.Origin = entity.Data.GetOne<Origin>()?.Location ?? entity.BoundingBox.Center;

                var ang = entity.EntityData.GetVector3("angles");
                if (ang.HasValue) details.Angles = ang.Value * (float) Math.PI / 180f;

                return details;
            }

            return null;
        }

        private class ModelDetails
        {
            public string Name { get; set; }
            public int Sequence { get; set; } = -1;
            public Vector3 Origin { get; set; }
            public Vector3 Angles { get; set; }
        }
    }
}
