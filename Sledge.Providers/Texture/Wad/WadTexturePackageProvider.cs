using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Wad
{
    [Export("Wad3", typeof(ITexturePackageProvider))]
    public class WadTexturePackageProvider : ITexturePackageProvider
    {
        public IEnumerable<TexturePackageReference> GetPackagesInFile(IFile file)
        {
            if (!file.Exists || !file.IsContainer) return new TexturePackageReference[0];
            return file.GetFilesWithExtension("wad").Select(x => new TexturePackageReference(x.Name, x));
        }

        public async Task<TexturePackage> GetTexturePackage(TexturePackageReference reference)
        {
            return await Task.Factory.StartNew(() =>
            {
                if (!reference.File.Exists || !string.Equals(reference.File.Extension, "wad", StringComparison.InvariantCultureIgnoreCase)) return null;
                using (var stream = reference.File.Open())
                {
                    return new WadTexturePackage(reference);
                }
            });
        }

        public async Task<IEnumerable<TexturePackage>> GetTexturePackages(IEnumerable<TexturePackageReference> references)
        {
            return await Task.Factory.StartNew(() =>
            {
                return references.AsParallel().Select(reference =>
                {
                    if (!reference.File.Exists || !string.Equals(reference.File.Extension, "wad", StringComparison.InvariantCultureIgnoreCase)) return null;
                    return new WadTexturePackage(reference);
                }).Where(x => x != null);
            });
        }
    }
}