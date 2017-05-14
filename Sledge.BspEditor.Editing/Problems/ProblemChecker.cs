using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Editing.Problems
{
    public static class ProblemChecker
    {
        private static readonly List<IProblemCheck> Checkers;

        static ProblemChecker()
        {
            Checkers = typeof (IProblemCheck).Assembly.GetTypes()
                .Where(x => typeof (IProblemCheck).IsAssignableFrom(x))
                .Where(x => !x.IsInterface)
                .Select(Activator.CreateInstance)
                .OfType<IProblemCheck>()
                .ToList();
        }

        public static void AddCheck(IProblemCheck check)
        {
            Checkers.Add(check);
        }

        public static void RemoveCheck<T>()
        {
            Checkers.RemoveAll(x => x.GetType() == typeof(T));
        }

        public static IEnumerable<Problem> Check(MapDocument map, bool visibleOnly)
        {
            return Checkers.SelectMany(x => x.Check(map, visibleOnly));
        }
    }
}