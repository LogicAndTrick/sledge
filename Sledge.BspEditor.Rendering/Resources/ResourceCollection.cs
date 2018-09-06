using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Resources
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ResourceCollection
    {
        private readonly Dictionary<string, HashSet<string>> _textures;
        private readonly Dictionary<string, List<IResource>> _resources;

        [ImportingConstructor]
        public ResourceCollection()
        {
            _textures = new Dictionary<string, HashSet<string>>();
            _resources = new Dictionary<string, List<IResource>>();
        }

        public async Task Upload(IEnvironment environment, ResourceCollector collector, EngineInterface engine)
        {
            EnsureEnvironment(environment);

            var rlist = _resources[environment.ID];
            var tlist = _textures[environment.ID];

            var textures = collector.Textures.Except(tlist).ToHashSet();

            if (textures.Any())
            {
                var tc = await environment.GetTextureCollection();
                var items = await tc.GetTextureItems(textures);
                using (var ss = tc.GetStreamSource())
                {
                    var tasks = items.Select(x => Task.Run(() => UploadTexture(engine, environment, x, ss))).ToList();
                    await Task.WhenAll(tasks);
                    rlist.AddRange(tasks.Select(x => x.Result));
                    tlist.UnionWith(textures);
                }
            }
        }

        private void EnsureEnvironment(IEnvironment environment)
        {
            if (!_textures.ContainsKey(environment.ID)) _textures.Add(environment.ID, new HashSet<string>());
            if (!_resources.ContainsKey(environment.ID)) _resources.Add(environment.ID, new List<IResource>());
        }

        private async Task<IResource> UploadTexture(EngineInterface engine, IEnvironment environment, TextureItem item, ITextureStreamSource source)
        {
            using (var bitmap = await source.GetImage(item.Name, 512, 512))
            {
                var lb = bitmap.LockBits(new Rectangle(0, 0, item.Width, item.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var data = new byte[lb.Stride * lb.Height];
                Marshal.Copy(lb.Scan0, data, 0, data.Length);
                bitmap.UnlockBits(lb);
                
                return engine.UploadTexture($"{environment.ID}::{item.Name}", bitmap.Width, bitmap.Height, data, TextureSampleType.Standard);
            }
        }

        public void DisposeOtherEnvironments(HashSet<IEnvironment> usedEnvironments, EngineInterface engine)
        {
            foreach (var dt in _textures.Keys.Except(usedEnvironments.Select(x => x.ID)).ToList())
            {
                _textures.Remove(dt);
            }
            foreach (var dr in _resources.Keys.Except(usedEnvironments.Select(x => x.ID)).ToList())
            {
                var list = _resources[dr];
                _resources.Remove(dr);
                foreach (var res in list) engine.DestroyResource(res);
            }
        }
    }
}