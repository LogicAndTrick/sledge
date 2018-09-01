using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Spr
{
    [Export("Spr", typeof(ITexturePackageProvider))]
    public class SprTexturePackageProvider : ITexturePackageProvider
    {
        public IEnumerable<TexturePackageReference> GetPackagesInFile(IFile file)
        {
            yield return new TexturePackageReference("sprites", file);
        }

        public async Task<TexturePackage> GetTexturePackage(TexturePackageReference reference)
        {
            return await Task.Factory.StartNew(() => new SprTexturePackage(reference));
        }

        public async Task<IEnumerable<TexturePackage>> GetTexturePackages(IEnumerable<TexturePackageReference> references)
        {
            return await Task.Factory.StartNew(() =>
            {
                return references.AsParallel().Select(reference => new SprTexturePackage(reference));
            });
        }
    }
}