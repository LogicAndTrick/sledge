using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public interface IProblemCheck
    {
        string Name { get; }
        string Details { get; }
        Uri Url { get; }
        bool CanFix { get; }

        Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter);
        Task Fix(MapDocument document, Problem problem);
    }
}
