using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Veldrid;

namespace Sledge.Rendering.Resources
{
    public class Texture : IResource
    {
        private readonly Veldrid.Texture _texture;
        private readonly TextureView _view;
        private readonly ResourceSet _set;

        private bool _mipsGenerated;

        public Texture(RenderContext context, int width, int height, byte[] data, TextureSampleType sampleType)
        {
            uint w = (uint) width, h = (uint) height;

            uint numMips = 4;
            if (w < 16 || h < 16)
            {
                numMips = 1;
            }
            var device = context.Device;
            _texture = device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                w, h, numMips, 1,
                PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled | TextureUsage.GenerateMipmaps
            ));

            try
            {
                device.UpdateTexture(_texture, data, 0, 0, 0, w, h, _texture.Depth, 0, 0);
            }
            catch
            {
                // Error updating texture, the texture may have been disposed
            }
            _mipsGenerated = false;

            var sampler = context.ResourceLoader.TextureSampler;
            if (sampleType == TextureSampleType.Point) sampler = context.ResourceLoader.OverlaySampler;
            
            _view = device.ResourceFactory.CreateTextureView(_texture);
            _set = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                context.ResourceLoader.TextureLayout, _view, sampler
            ));
        }

        public void CreateResources(EngineInterface engine, RenderContext context)
        {
            // The resources are created in the constructor
        }

        public void BindTo(CommandList cl, uint slot)
        {
            if (!_mipsGenerated)
            {
                cl.GenerateMipmaps(_texture);
                _mipsGenerated = true;
            }

            cl.SetGraphicsResourceSet(slot, _set);
        }

        public void DestroyResources()
        {
            // For consistency to match the constructor, the resources are destroyed in the dispose function
        }

        public void Dispose()
        {
            _set.Dispose();
            _view.Dispose();
            _texture.Dispose();
        }
    }
}