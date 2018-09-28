using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class InvalidSolid : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Uri Url => null;
        public bool CanFix => true;

        public Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            var solids = document.Map.Root.FindAll()
                .Where(x => filter(x))
                .OfType<Solid>()
                .Where(x => !x.IsValid())
                .Select(x => new Problem().Add(x))
                .ToList();

            return Task.FromResult(solids);
        }

        public Task Fix(MapDocument document, Problem problem)
        {
            var delete = new Transaction();
            foreach (var g in problem.Objects.GroupBy(x => x.Hierarchy.Parent.ID))
            {
                delete.Add(new Detatch(g.Key, g));
            }
            return MapDocumentOperation.Perform(document, delete);
        }
    }
}