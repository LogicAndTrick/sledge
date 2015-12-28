using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.FileSystem;
using Sledge.Packages.Vpk;
using Sledge.Providers;
using Sledge.Providers.Map;
using Sledge.Providers.Texture;
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
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\mudground001_height-ssbump.vtf");
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\dirtroad001a.vtf");
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\rockground001.vtf");
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\class_demo_dudv.vtf");
            var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\cubemap_gold001.hdr.vtf");
            //var file = new NativeFile(@"D:\Github\sledge\_Resources\VTF\800corner.vtf");
            var image = VtfProvider.GetImage(file);
            image.Save(@"D:\Github\sledge\_Resources\VTF\_test2.png");
        }

        [TestMethod]
        public void BulkExtractEverything()
        {
            //var fs = new InlinePackageFile(@"F:\Steam\SteamApps\common\Team Fortress 2\tf\tf2_textures_dir.vpk");
            var fs = new InlinePackageFile(@"F:\Steam\SteamApps\common\dota 2 beta\dota\pak01_dir.vpk");
            var files = fs.GetFiles("\\.vtf$", true).OrderBy(x => x.Name).Take(20000).GroupBy(x => x.Name).Select(x => x.First()).ToList();
            //return;
            /*foreach (var f in files)
            {
                var vtf = VtfProvider.GetImage(f);
                //Console.WriteLine(f.Name + " " + vtf.Height);
                //vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + f.Name + ".png");
                //if (i > 1) break;
            }*/
            Parallel.ForEach(files, x =>
            {
                try
                {
                    var vtf = VtfProvider.GetImage(x);
                    //Console.WriteLine(x.Name);
                    //vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + x.Name + ".png");
                }
                catch (Exception ex)
                {
                    throw new Exception(x.Name + " " + ex.Message);
                }
            });
        }

        [TestMethod]
        public void BulkExtractEverything2()
        {
            var files = Directory.GetFiles(@"D:\Github\sledge\_Resources\VTF", "*.vtf");
            //return;
            /*foreach (var file in files)
            {
                var vtf = VtfProvider.GetImage(new NativeFile(file));
                //vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + files[i].Name + ".png");
                //if (i > 1) break;
            }*/
            Parallel.ForEach(files, x =>
            {
                var vtf = VtfProvider.GetImage(new NativeFile(x));
                //Console.WriteLine(vtf.Height);
                vtf.Save(@"D:\Github\sledge\_Resources\VTF\extract\" + Path.GetFileName(x) + ".png");
            });
        }

        [TestMethod]
        public void VpkVtfCollectionTest()
        {
            TextureProvider.Register(new VmtProvider());
            var collection = TextureProvider.CreateCollection(new[]
            {
                @"F:\Steam\SteamApps\common\Team Fortress 2\tf"
            }, null, null, null, null);
        }

        [TestMethod]
        public void VmtStatsCollectorTest()
        {
            var exclude = new[]
            {
                	"ambulance"             ,
	                "backpack"              ,
	                "cable"                 ,
	                "console"               ,
	                "cp_bloodstained"       ,
	                "customcubemaps"        ,
	                "detail"                ,
	                "debug"                 ,
	                "effects"               ,
	                "engine"                ,
	                "environment maps"      ,
	                "halflife"              ,
	                "matsys_regressiontest" ,
	                "hlmv"                  ,
	                "hud"                   ,
	                "logo"                  ,
	                "maps"                  ,
	                "models"                ,
	                "overviews"             ,
	                "particle"              ,
	                "particles"             ,
	                "perftest"              ,
	                "pl_halfacre"           ,
	                "pl_hoodoo"             ,
	                "scripted"              ,
	                "shadertest"            ,
	                "sprites"               ,
	                "sun"                   ,
	                "vgui"                  ,
	                "voice"                 ,

            };

            var stats = new Dictionary<string, int>();
            using (var fs = new VpkDirectory(new FileInfo(@"F:\Steam\SteamApps\common\Team Fortress 2\tf\tf2_misc_dir.vpk")))
            {
                using (var ss = fs.GetStreamSource())
                {
                    //var vmts = fs.SearchFiles("materials", ".vmt$", true);
                    var subs = fs.GetDirectories("materials").Where(x => !exclude.Contains(x.Split('/')[1]));
                    var vmts = subs.SelectMany(x => fs.SearchFiles(x, ".vmt$", true));
                    foreach (var vmt in vmts)
                    {
                        using (var sr = new StreamReader(ss.OpenFile(vmt)))
                        {
                            var parsed = GenericStructure.Parse(sr).First();
                            var type = parsed.Name.ToLowerInvariant();
                            if (!stats.ContainsKey(type)) stats.Add(type, 0);
                            stats[type]++;
                            if (type == "refract" || type == "replacements" || type == "modulate") Console.WriteLine(type + " " + vmt);
                        }
                    }
                }
            }
            foreach (var kv in stats.OrderByDescending(x => x.Value))
            {
                Console.WriteLine(kv.Key + " - " + kv.Value);
            }
        }
    }
}
