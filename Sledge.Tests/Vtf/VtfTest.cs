using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.FileSystem;
using Sledge.Packages.Vpk;
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
            var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\mudground001_height-ssbump.vtf");
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\dirtroad001a.vtf");
            //file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\rockground001.vtf");
            var image = VtfProvider.GetImage(file);
            //image.Save(@"D:\Github\sledge\_Resources\VTF\_test2.png");
        }

        [TestMethod]
        public void BulkExtractEverything()
        {
            var fs = new InlinePackageFile(@"F:\Steam\SteamApps\common\Team Fortress 2\tf\tf2_textures_dir.vpk");
            var files = fs.GetFiles("\\.vtf$", true).OrderBy(x => x.Name).Take(200).ToList();
            //return;
            /*for (var i = 0; i < files.Count; i++)
            {
                var vtf = VtfProvider.GetImage(files[i]);
                //vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + files[i].Name + ".png");
                //if (i > 1) break;
            }*/
            Parallel.ForEach(files, x =>
            {
                var vtf = VtfProvider.GetImage(x);
                //vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + x.Name + ".png");
            });
        }
    }
}
