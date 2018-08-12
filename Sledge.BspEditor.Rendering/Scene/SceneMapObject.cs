using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Renderables;
using Buffer = Sledge.Rendering.Renderables.Buffer;

namespace Sledge.BspEditor.Rendering.Scene
{
    /// <summary>
    /// A collection of <see cref="Buffer"/>s and <see cref="IRenderable"/>s for an <see cref="IMapObject"/>.
    /// </summary>
    /// <seealso cref="Converters.IMapObjectSceneConverter">The interface that handles this class's buffers and renderables</seealso>
    /// <seealso cref="ConvertedScene">The class that manages instances of this class</seealso>
    public class SceneMapObject : IDisposable
    {
        public IMapObject MapObject { get; }

        public Dictionary<object, Buffer> Buffers { get; }
        public Dictionary<object, IRenderable> Renderables { get; }
        public Dictionary<string, object> MetaData { get; }

        public SceneMapObject(IMapObject mapObject)
        {
            MapObject = mapObject;
            Buffers = new Dictionary<object, Buffer>();
            Renderables = new Dictionary<object, IRenderable>();
            MetaData = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            foreach (var r in Renderables.Values) r.Dispose();
            foreach (var b in Buffers.Values) b.Dispose();
            foreach (var m in MetaData.Values.OfType<IDisposable>()) m.Dispose();

            Renderables.Clear();
            Buffers.Clear();
            MetaData.Clear();
        }
    }
}