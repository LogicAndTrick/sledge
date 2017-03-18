using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using Sledge.Common.Mediator;
using Sledge.Providers.Texture;

namespace Sledge.Editor.Documents
{
    public static class TextureCache
    {
        private static readonly MultiDictionary<string, TexturePackage> Packages;
        private static readonly List<TextureCollection> Collections;
        private static readonly object Lock = new object();

        static TextureCache()
        {
            Packages = new MultiDictionary<string, TexturePackage>();
            Collections = new List<TextureCollection>();
        }

        public async static Task<TextureCollection> CreateCollection(IEnumerable<string> paths)
        {
            var tcs = new TaskCompletionSource<TextureCollection>();

            var pathsToLoad = new HashSet<string>(paths.Select(x => x.ToLowerInvariant()));
            ConcurrentBag<TexturePackage> loadedPackages;
            lock (Lock)
            {
                var foundPaths = new HashSet<string>(pathsToLoad.Where(x => Packages.ContainsKey(x)));
                pathsToLoad.ExceptWith(foundPaths);

                var existing = foundPaths.SelectMany(x => Packages[x]);
                loadedPackages = new ConcurrentBag<TexturePackage>(existing);
            }

            return await pathsToLoad
                .Select(x => Gimme.Fetch<TexturePackage>(x, null))
                .Merge()
                .ToList()
                .ToTask()
                .ContinueWith(x => new TextureCollection(x.Result.ToList()))
                .ContinueWith(AddCollection);
        }

        private static TextureCollection AddCollection(Task<TextureCollection> task)
        {
            // This shouldn't run very often
            lock (Lock)
            {
                Collections.Add(task.Result);
                foreach (var tp in task.Result.Packages)
                {
                    Packages.AddValue(tp.Location.ToLowerInvariant(), tp);
                }
            }
            return task.Result;
        }

        public static void DestroyCollection(TextureCollection collection)
        {
            lock (Lock)
            {
                Collections.Remove(collection);
                var all = new HashSet<TexturePackage>(Packages.SelectMany(x => x.Value));
                var used = new HashSet<TexturePackage>(Collections.SelectMany(x => x.Packages));
                all.ExceptWith(used);
                foreach (var tp in all)
                {
                    Packages.RemoveValue(tp.Location.ToLowerInvariant(), tp);
                }
            }
        }
    }
}