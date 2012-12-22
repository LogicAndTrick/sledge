using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Database;
using Sledge.Database.Models;
using System.IO;
using Sledge.Editor;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.UI;
using Sledge.Editor.Tools;

namespace Sledge.Sandbox
{
    public class QuickStartBootstrap
    {
        public static string MapFile { get; set; }
        public static Game Game { get; set; }

        public static void Start()
        {
           // var editor = new Editor.Editor();
           // editor.Load += (sender, e) => PostStart(sender as Editor.Editor);
           // Application.Run(editor);
            var settings = Context.DBContext.GetAllSettings().ToDictionary(x => x.Key, x => x.Value);
            Serialise.DeserialiseSettings(settings);
            var settingsform = new Editor.Settings.SettingsForm();
            Application.Run(settingsform);
        }

        public static void PostStart(Editor.Editor editor)
        {
            if (File.Exists(MapFile) && Game != null)
            {
                Document.Open(MapFile, Game);
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
