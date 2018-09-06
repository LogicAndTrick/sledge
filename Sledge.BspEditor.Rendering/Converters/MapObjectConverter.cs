using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export]
    public class MapObjectConverter
    {
        [ImportMany] private IEnumerable<Lazy<IMapObjectSceneConverter>> _converters;
        [ImportMany] private IEnumerable<Lazy<IMapObjectGroupSceneConverter>> _groupConverters;
        [Import] private Lazy<EngineInterface> _engine;

        public async Task Convert(MapDocument document, SceneBuilder builder, IEnumerable<IMapObject> affected, ResourceCollector resourceCollector)
        {
            var objs = document.Map.Root.FindAll();
            if (affected != null)
            {
                var groups = affected.Select(x => x.ID / 200).ToHashSet();
                foreach (var g in groups) builder.DeleteGroup(g);
                objs = objs.Where(x => groups.Contains(x.ID / 200)).ToList();
            }

            var converters = _converters.Select(x => x.Value).OrderBy(x => (int) x.Priority).ToList();
            var groupConverters = _groupConverters.Select(x => x.Value).OrderBy(x => (int) x.Priority).ToList();

            foreach (var g in objs.GroupBy(x => x.ID / 200))
            {
                builder.SetCurrentGroup(g.Key);

                foreach (var gc in groupConverters)
                {
                    gc.Convert(builder.MainBuffer, document, g, resourceCollector);
                }

                foreach (var obj in g)
                {
                    foreach (var converter in converters)
                    {
                        if (!converter.Supports(obj)) continue;
                        await converter.Convert(builder.MainBuffer, document, obj, resourceCollector);
                        if (converter.ShouldStopProcessing(document, obj)) break;
                    }
                }
            }

            builder.Complete();
        }
    }
}