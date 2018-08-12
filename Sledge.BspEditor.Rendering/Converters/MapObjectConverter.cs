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
                await converter.Convert(smo, document, obj);
                if (converter.ShouldStopProcessing(smo, document, obj)) break;
            }
            return smo;
        }

        /// <summary>
        /// Updates a <see cref="SceneMapObject"/>. Returns true if the renderable list doesn't need to change.
        /// </summary>
        /// <param name="smo">The scene map object to update</param>
        /// <param name="document">The current document</param>
        /// <param name="obj">The current object</param>
        /// <returns>True if the renderable list was unmodified, false otherwise</returns>
        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var res = true;
            foreach (var converter in _converters.Select(x => x.Value).OrderBy(x => (int)x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!await converter.Update(smo, document, obj)) res = false;
                if (converter.ShouldStopProcessing(smo, document, obj)) break;
            }
            return res;
        }
    }
}