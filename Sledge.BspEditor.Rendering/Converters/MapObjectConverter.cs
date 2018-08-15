using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export]
    public class MapObjectConverter
    {
        [ImportMany] private IEnumerable<Lazy<IMapObjectSceneConverter>> _converters;
        [Import] private Lazy<EngineInterface> _engine;

        public async Task<SceneBuilder> Convert(MapDocument document, IEnumerable<IMapObject> objs)
        {
            var builder = new SceneBuilder(_engine.Value);
            var converters = _converters.Select(x => x.Value).OrderBy(x => (int) x.Priority).ToList();

            foreach (var obj in objs)
            {
                foreach (var converter in converters)
                {
                    if (!converter.Supports(obj)) continue;
                    await converter.Convert(builder, document, obj);
                    if (converter.ShouldStopProcessing(document, obj)) break;
                }
            }

            builder.MainBuffer.Complete();
            return builder;
        }
    }
}