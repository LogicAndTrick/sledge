using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Shaders;
using Veldrid;
using Texture = Sledge.Rendering.Renderables.Texture;

namespace Sledge.Rendering.Engine
{
    public class ResourceLoader
    {
        private readonly RenderContext _context;

        public ResourceLayout ProjectionLayout { get; }
        public ResourceLayout TextureLayout { get; }
        public Sampler TextureSampler { get; }

        public VertexLayoutDescription VertexStandard4LayoutDescription { get; }

        public ResourceLoader(RenderContext context)
        {
            _context = context;
            
            ProjectionLayout = context.Device.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                )
            );
            TextureLayout = context.Device.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );

            VertexStandard4LayoutDescription = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                new VertexElementDescription("Colour", VertexElementSemantic.Color, VertexElementFormat.Float4),
                new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
            );
            TextureSampler = context.Device.ResourceFactory.CreateSampler(SamplerDescription.Aniso4x);
        }
        
        private readonly ConcurrentDictionary<string, Texture> _textures = new ConcurrentDictionary<string, Texture>();

        public TextureBinding CreateTextureBinding(string name, ITextureDataSource source)
        {
            var tex = _textures.GetOrAdd(name, n => new Texture(Engine.Instance.Context, source));
            return new TextureBinding(tex);
        }

        internal void DeleteUnreferencedTextures()
        {
            foreach (var tkv in _textures.Where(x => x.Value.RefCount == 0).ToList())
            {
                _textures.TryRemove(tkv.Key, out _);
                tkv.Value.Dispose();
            }
        }

        public (Shader, Shader) LoadShaders(string name)
        {
            return (
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main")),
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main"))
            );
        }

        private static readonly Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
        private static byte[] GetEmbeddedShader(string name)
        {
            using (var s = ResourceAssembly.GetManifestResourceStream(typeof(Scope), name))
            using (var ms = new MemoryStream())
            {
                if (s == null) throw new FileNotFoundException($"The `{name}` shader could not be found.", name);
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}