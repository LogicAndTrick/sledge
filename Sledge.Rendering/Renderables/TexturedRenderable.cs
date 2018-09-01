using System.Collections.Generic;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public class TexturedRenderable : SimpleRenderable
    {
        private readonly List<IndirectDrawIndexedArguments> _textureIndices = new List<IndirectDrawIndexedArguments>();
        
        public TexturedRenderable(Buffer buffer, string pipeline, int indexOffset, int indexCount) : base(buffer, pipeline, indexOffset, indexCount)
        {
        }

        public TexturedRenderable(Buffer buffer, IEnumerable<string> pipelines, int indexOffset, int indexCount) : base(buffer, pipelines, indexOffset, indexCount)
        {
        }

        public void ClearTextureIndices()
        {
            _textureIndices.Clear();
        }

        public void AddTextureIndex(TextureBinding texture, int offset, int count)
        {
            _textureIndices.Add(new TextureIndex {Texture = texture, Offset = offset, Count = count});
        }

        public void CommitTextureIndices()
        {

        }

        protected override void Draw(IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            foreach (var ti in _textureIndices)
            {
                ti.Texture.BindTo(cl, 1);
                cl.DrawIndexed((uint) ti.Count, 1, (uint) ti.Offset, 0, 0);
            }
        }
    }
}