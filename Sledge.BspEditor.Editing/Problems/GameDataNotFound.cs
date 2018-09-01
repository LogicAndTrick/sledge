using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class GameDataNotFound : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Uri Url => null;
        public bool CanFix => false;

        public async Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            var gd = await document.Environment.GetGameData();
            var missing = document.Map.Root.FindAll()
                .Where(x => filter(x))
                .SelectMany(x => x.Data.OfType<EntityData>()
                    .Where(ed => gd.GetClass(ed.Name) == null)
                    .Select(ed => new {Object = x, Data = ed}))
                .Select(x => new Problem {Text = x.Data.Name}.Add(x.Object).Add(x.Data))
                .ToList();
            return missing;
        }

        public Task Fix(MapDocument document, Problem problem)
        {
            throw new NotImplementedException();
        }
    }
}