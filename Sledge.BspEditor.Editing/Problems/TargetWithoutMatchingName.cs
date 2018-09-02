using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class TargetWithoutMatchingName : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Uri Url => null;
        public bool CanFix => true;

        public Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            // Unfiltered list of entities in the map
            var entities = document.Map.Root.FindAll()
                .OfType<Entity>()
                .Select(x => new { Object = x, x.EntityData })
                .Where(x => x.EntityData != null)
                .ToList();

            // Unfiltered list of targetnames in the map
            var targetnames = entities
                .Select(x => x.EntityData.Get("targetname", ""))
                .Where(x => x.Length > 0)
                .ToHashSet();

            // Filtered list of entities with targets without matching targetnames
            var targets = entities
                .Where(x => filter(x.Object))
                .Select(x => new { x.Object, Target = x.EntityData.Get("target", "") })
                .Where(x => x.Target.Length > 0 && !targetnames.Contains(x.Target))
                .Select(x => new Problem { Text = x.Target }.Add(x.Object))
                .ToList();

            return Task.FromResult(targets);
        }

        public Task Fix(MapDocument document, Problem problem)
        {
            var transaction = new Transaction();

            foreach (var obj in problem.Objects)
            {
                var data = obj.Data.GetOne<EntityData>();
                if (data == null) continue;

                var vals = new Dictionary<string, string> {["target"] = null};
                transaction.Add(new EditEntityDataProperties(obj.ID, vals));
            }

            return MapDocumentOperation.Perform(document, transaction);
        }
    }
}