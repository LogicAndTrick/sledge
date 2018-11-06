using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Sledge.Common.Logging;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Shaders;
using Veldrid;
using Texture = Sledge.Rendering.Resources.Texture;

namespace Sledge.Rendering.Engine
{
    public class ResourceLoader
    {
        private readonly RenderContext _context;

        public ResourceLayout ProjectionLayout { get; }
        public ResourceLayout TextureLayout { get; }
        public Sampler TextureSampler { get; }
        public Sampler OverlaySampler { get; }

        public VertexLayoutDescription VertexStandardLayoutDescription { get; }
        public VertexLayoutDescription VertexModel3LayoutDescription { get; }

        private Lazy<Texture> MissingTexture { get; }

        public ResourceLoader(RenderContext context)
        {
            _context = context;
            
            ProjectionLayout = context.Device.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment | ShaderStages.Geometry)
                )
            );
            TextureLayout = context.Device.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            );

            VertexStandardLayoutDescription = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                new VertexElementDescription("Colour", VertexElementSemantic.Color, VertexElementFormat.Float4),
                new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Tint", VertexElementSemantic.Color, VertexElementFormat.Float4),
                new VertexElementDescription("Flags", VertexElementSemantic.Position, VertexElementFormat.UInt1)
            );

            VertexModel3LayoutDescription = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                new VertexElementDescription("Texture", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Bone", VertexElementSemantic.Position, VertexElementFormat.UInt1)
            );

            TextureSampler = context.Device.Aniso4xSampler;
            OverlaySampler = context.Device.PointSampler;

            MissingTexture = new Lazy<Texture>(() => UploadTexture("", 1, 1, new byte[] { 255, 255, 255, 255 }, TextureSampleType.Standard));
        }

        // Textures
        private readonly ConcurrentDictionary<string, Texture> _textures = new ConcurrentDictionary<string, Texture>(StringComparer.InvariantCultureIgnoreCase);

        internal Texture UploadTexture(string name, int width, int height, byte[] data, TextureSampleType sampleType)
        {
            return _textures.GetOrAdd(name, n => new Texture(_context, width, height, data, sampleType));
        }

        internal void DestroyTexture(Texture texture)
        {
            var keys = _textures.Where(x => x.Value == texture).ToList();
            if (keys.Any()) _textures.TryRemove(keys[0].Key, out _);
            texture.Dispose();
        }

        internal Texture GetTexture(string name)
        {
            return _textures.TryGetValue(name, out var tex) ? tex : MissingTexture.Value;
        }

        // Shaders
        public (Shader, Shader) LoadShaders(string name)
        {
            return (
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main")),
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main"))
            );
        }

        public (Shader, Shader, Shader) LoadShadersGeometry(string name)
        {
            return (
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShader(name + ".vert.hlsl"), "main")),
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Geometry, GetEmbeddedShader(name + ".geom.hlsl"), "main")),
                _context.Device.ResourceFactory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShader(name + ".frag.hlsl"), "main"))
            );
        }

        private static readonly Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
        internal static byte[] GetEmbeddedShader(string name)
        {
            var names = new[] {name + ".bytes", name};
#if DEBUG
            // Compiling shaders manually is a pain!
            if (!Features.DirectX11OrHigher) Log.Debug("ResourceLoader", "If you're debugging on DX10 you'll need to manually compile shaders.");
            else names = new[] {name};
#endif
            foreach (var n in names)
            {
                using (var s = ResourceAssembly.GetManifestResourceStream(typeof(Scope), n))
                {
                    if (s == null) continue;
                    using (var ms = new MemoryStream())
                    {
                        s.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            throw new FileNotFoundException($"The `{name}` shader could not be found.", name);
        }
    }
}