using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Scene;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export]
    public class MapObjectConverter
    {
        private readonly IEnumerable<Lazy<IMapObjectSceneConverter>> _converters;
        private readonly IEnumerable<Lazy<IMapObjectGroupSceneConverter>> _groupConverters;

        [ImportingConstructor]
        public MapObjectConverter(
            [ImportMany] IEnumerable<Lazy<IMapObjectSceneConverter>> converters,
            [ImportMany] IEnumerable<Lazy<IMapObjectGroupSceneConverter>> groupConverters
        )
        {
            _converters = converters;
            _groupConverters = groupConverters;
        }

        public async Task Convert(MapDocument document, SceneBuilder builder, IEnumerable<IMapObject> affected, ResourceCollector resourceCollector)
        {
            var objs = document.Map.Root.FindAll();
            if (affected != null)
            {
                var groups = affected.Select(x => x.ID / 200).ToHashSet();
                foreach (var g in groups)
                {
                    resourceCollector.RemoveRenderables(builder.GetRenderablesForGroup(g));
                    builder.DeleteGroup(g);
                }
                objs = objs.Where(x => groups.Contains(x.ID / 200)).ToList();
            }

            var converters = _converters.Select(x => x.Value).OrderBy(x => (int) x.Priority).ToList();
            var groupConverters = _groupConverters.Select(x => x.Value).OrderBy(x => (int) x.Priority).ToList();

            foreach (var g in objs.GroupBy(x => x.ID / 200))
            {
                builder.EnsureGroupExists(g.Key);
                var buffer = builder.GetBufferForGroup(g.Key);
                var collector = new ResourceCollector();

                foreach (var gc in groupConverters)
                {
                    gc.Convert(buffer, document, g, collector);
                }

                foreach (var obj in g)
                {
                    foreach (var converter in converters)
                    {
                        if (!converter.Supports(obj)) continue;
                        await converter.Convert(buffer, document, obj, collector);
                        if (converter.ShouldStopProcessing(document, obj)) break;
                    }
                }

                builder.RemoveRenderablesFromGroup(g.Key, collector.GetRenderablesToRemove());
                builder.AddRenderablesToGroup(g.Key, collector.GetRenderablesToAdd());

                resourceCollector.Merge(collector);
            }

            builder.Complete();
        }
    }
}