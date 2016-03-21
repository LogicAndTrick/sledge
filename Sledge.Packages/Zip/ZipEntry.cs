using System;
using System.IO;
using System.IO.Compression;
using System.Drawing;

namespace Sledge.Packages.Zip
{
    public class ZipEntry : IPackageEntry
    {
        public ZipPackage Package { get; private set; }

        public string Path { get; private set; }
        public string Name { get; }
        public string FullName { get { return Path; } }
        public string ParentPath { get { return GetParent(); } }
        public long Length { get; private set; }
        public ContentTypes ContentType { get; set; }
        public int Width { get; }
        public int Height { get; }

        public MemoryStream StreamCopy
        {
            get
            {
                _streamCopy.Position = 0;
                MemoryStream temp = new MemoryStream();
                _streamCopy.CopyTo(temp);
                return temp;
            }
        }

        private readonly MemoryStream _streamCopy;

        public ZipEntry(ZipPackage package, string path, Stream entry)
        {
            Package = package;
            Length = entry.Length;

            if (path.ToLowerInvariant().StartsWith("textures/"))
			{
				Path = System.IO.Path.ChangeExtension(path.ToLowerInvariant(), null);
				Name = System.IO.Path.ChangeExtension(path.ToLowerInvariant(), null);
				//Name = System.IO.Path.ChangeExtension(path.Substring(9).ToLowerInvariant(), null);
			}
            else
            {
                return;
            }

            if (path.EndsWith(".png"))
            {
				ContentType = ContentTypes.Png;
			}
			else if (path.EndsWith(".jpg"))
			{
				ContentType = ContentTypes.Jpg;
			}
			else if (path.EndsWith(".tga"))
			{
				ContentType = ContentTypes.Tga;
			}

			_streamCopy = new MemoryStream();
	        entry.Position = 0;
            entry.CopyTo(_streamCopy);
            using (Image img = Image.FromStream(_streamCopy))
            {
                Width = img.Width;
                Height = img.Height;
            }
        }

        public Stream Open()
        {
            return Package.OpenStream(this);
        }

        private string GetParent()
        {
            var idx = Path.LastIndexOf('/');
            if (idx < 0) return "";
            return Path.Substring(0, idx);
        }

        public Stream GetStream()
        {
            return StreamCopy;
		}

		public enum ContentTypes
		{
			None,
			Jpg,
			Png,
			Tga,
			Md3
		}
	}
}
