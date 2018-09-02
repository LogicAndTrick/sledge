using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class PointEntityWithChildren : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Uri Url => null;
        public bool CanFix => true;

        public async Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            var gamedata = await document.Environment.GetGameData();
            return document.Map.Root.FindAll()
                .OfType<Entity>()
                .Where(x => x.Hierarchy.HasChildren)
                .Where(x => filter(x))
                .Select(x => new { Object = x, x.EntityData })
                .Where(x => x.EntityData != null)
                .Select(x => new { x.Object, x.EntityData, GameData = gamedata.GetClass(x.EntityData.Name) })
                .Where(x => x.GameData != null && x.GameData.ClassType != ClassType.Solid)
                .Select(x => new Problem().Add(x.Object))
                .ToList();
        }

        public Task Fix(MapDocument document, Problem problem)
        {
            var transaction = new Transaction();

            foreach (var obj in problem.Objects)
            {
                transaction.Add(new Detatch(obj.ID, obj.Hierarchy));
                transaction.Add(new Attach(document.Map.Root.ID, obj.Hierarchy));
            }

            return MapDocumentOperation.Perform(document, transaction);
        }
    }
}