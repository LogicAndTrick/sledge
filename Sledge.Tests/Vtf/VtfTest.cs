using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.FileSystem;
using Sledge.Providers.Texture.Vtf;

namespace Sledge.Tests.Vtf
{
    [TestClass]
    public class VtfTest
    {
        [TestMethod]
        public void TestLoadVtfSize()
        {
            var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\rockground001.vtf");
            var size = VtfProvider.GetSize(file);
        }

        [TestMethod]
        public void TestLoadVtfImage()
        {
            var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\rockground001.vtf");
            var image = VtfProvider.GetImage(file);
            image.Save(@"D:\Github\sledge\_Resources\VTF\_test2.png");
        }
    }
}
