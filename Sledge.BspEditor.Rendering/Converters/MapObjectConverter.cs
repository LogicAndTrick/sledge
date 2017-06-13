using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export]
    public class MapObjectConverter
    {
        [ImportMany]
        private IEnumerable<Lazy<IMapObjectSceneConverter>> _converters;

        public async Task<SceneMapObject> Convert(MapDocument document, IMapObject obj)
        {
            var smo = new SceneMapObject(obj);
            foreach (var converter in _converters.Select(x => x.Value).OrderBy(x => (int) x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!await converter.Convert(smo, document, obj)) return null;
                if (converter.ShouldStopProcessing(smo, document, obj)) break;
            }
            return smo;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            foreach (var converter in _converters.Select(x => x.Value).OrderBy(x => (int)x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!await converter.Update(smo, document, obj)) return false;
                if (converter.ShouldStopProcessing(smo, document, obj)) break;
            }
            return true;
        }
    }
}