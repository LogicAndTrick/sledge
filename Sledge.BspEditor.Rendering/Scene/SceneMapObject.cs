using System.Collections.Generic;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Buffer = Sledge.Rendering.Resources.Buffer;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneBuilder
    {
        public BufferBuilder MainBuffer { get; }
        public IRenderable MainRenderable { get; }

        public List<Buffer> Buffers { get; set; }
        public List<IRenderable> Renderables { get; set; }

        public SceneBuilder(EngineInterface engine)
        {
            MainBuffer = engine.CreateBufferBuilder();
            MainRenderable = new BufferBuilderRenderable(MainBuffer);
            Buffers = new List<Buffer>();
            Renderables = new List<IRenderable>();
        }
    }
}