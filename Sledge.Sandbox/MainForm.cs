using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Gimme;
using Sledge.Providers.Texture;
using Sledge.Providers.Texture.Wad;

namespace Sledge.Sandbox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Gimme.Register(new WadTexturePackageProvider());
            //Gimme.Register(new VmtTexturePackageProvider());

            Load += (sender, args) =>
            {
                Gimme.Fetch<TexturePackage>(
                    @"D:\Github\sledge\_Resources\WAD",
                    null,
                    package =>
                    {
                        // 
                    });
                //var items = new List<TexturePackage>();
                //Gimme.Fetch<TexturePackage>(
                //    @"F:\Steam\SteamApps\common\Team Fortress 2\tf",
                //    null,
                //    package =>
                //    {
                //        items.Add(package);
                //        //Console.WriteLine(package.Root + "/" + package.RelativePath + ": " + package.Items.Count + " items");
                //    }).ContinueWith(x =>
                //{
                //    Console.WriteLine(items.Count + " packages loaded.");
                //    foreach (var item in items.Take(10))
                //    {
                //        Console.WriteLine(item.Items.First());
                //    }
                //});
            };
        }
    }
}
