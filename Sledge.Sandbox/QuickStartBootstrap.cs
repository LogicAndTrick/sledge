using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Sledge.Editor.Documents;
using Sledge.Editor.UI.FileSystem;
using Sledge.FileSystem;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Providers.Texture;
using Sledge.Settings.Models;
using Sledge.Editor.Tools;

namespace Sledge.Sandbox
{
    public class QuickStartBootstrap
    {
        public static string MapFile { get; set; }
        public static Game Game { get; set; }

        public static void Start()
        {
            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            //TextureProvider.Register(new WadProvider());

           // var editor = new Editor.Editor();
           // editor.Load += (sender, e) => PostStart(sender as Editor.Editor);
           // Application.Run(editor);
           // var settings = Context.DBContext.GetAllSettings().ToDictionary(x => x.Key, x => x.Value);
           // Serialise.DeserialiseSettings(settings);
           // var settingsform = new Editor.Settings.SettingsForm();
           // Application.Run(settingsform);

           // var map = MapProvider.GetMapFromFile(MapFile);
           // Document.Game = Game;
           // Document.Map = map;
           // Document.GameData = GameDataProvider.GetGameDataFromFiles(Game.Fgds.Select(f => f.Path));
           // var entity = new EntityEditor();
           // entity.Objects.AddRange(map.WorldSpawn.Children.OfType<Entity>().Take(1));
           // Application.Run(entity);

            /*
            var nat = new NativeFile(new DirectoryInfo(@"F:\Steam\steamapps\common\Half-Life"));
            var gcf1 = new GcfFile(@"F:\Steam\steamapps\half-life.gcf");
            var gcf2 = new GcfFile(@"F:\Steam\steamapps\half-life engine.gcf");
            //var gcf3 = new GcfFile(@"F:\Steam\steamapps\half-life base content.gcf");
            var gcf4 = new GcfFile(@"F:\Steam\steamapps\platform.gcf");
            var com = new CompositeFile(null, new IFile[] { nat, gcf1, gcf2, gcf4 });
            */
            var nat = new NativeFile(new DirectoryInfo(@"F:\Half-Life WON"));
            var com = new CompositeFile(null, new[]
            {
                new NativeFile(new DirectoryInfo(@"F:\Half-Life WON\valve")),
                new NativeFile(new DirectoryInfo(@"F:\Half-Life WON\tfc")),
            });
            //var pak = new PakFile(@"F:\Half-Life WON\valve\pak0.pak");
           // var vir = new VirtualFile(null, "valve", new[] {pak});
            //var com = new CompositeFile(null, new IFile[] { nat, vir });
            var fsb = new FileSystemBrowserControl {Dock = DockStyle.Fill, File = com};//, FilterText = "WAD Files", Filter = "*.wad"};
            var form = new Form {Controls = {fsb}, Width = 500, Height = 500};
            Application.Run(form);
        }

        public static void PostStart(Editor.Editor editor)
        {
            if (File.Exists(MapFile) && Game != null)
            {
                var map = MapProvider.GetMapFromFile(MapFile);
                DocumentManager.AddAndSwitch(new Document(MapFile, map, Game));
            }
            // Texture panel
            //using (var tlp = new TextureListPanel())
            //{
            //    tlp.ImageSize = 128;
            //    tlp.Dock = DockStyle.Fill;
            //    tlp.AddTextures(TexturePackage.GetLoadedItems());
            //    using (var dialog = new Form())
            //    {
            //        dialog.Controls.Add(tlp);
            //        dialog.ShowDialog();
            //    }
            //}
            // Texture tool
            editor.SelectTool(ToolManager.Tools[0]);
        }
    }
}
