using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Settings;
using Sledge.Editor.UI;
using Sledge.FileSystem;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Database;
using Sledge.Editor.Tools;
using Sledge.Providers.Model;
using Sledge.Providers.Texture;
using Sledge.Graphics;
using Sledge.UI;
using System.Diagnostics;

namespace Sledge.Editor
{
    public partial class Editor : Form
    {
        public static Editor Instance { get; private set; }

        public Editor()
        {
            InitializeComponent();
            tsbNew.Click += (sender, e) => NewFile();
            tsbOpen.Click += (sender, e) => OpenFile();
            tsbSave.Click += (sender, e) => SaveFile();
            Instance = this;
        }

        public void SelectTool(BaseTool t)
        {
            ToolManager.Activate(t);
            foreach (var tsb in from object item in tspTools.Items select ((ToolStripButton) item))
            {
                tsb.Checked = (tsb.Name == ToolManager.ActiveTool.GetName());
            }
        }

        public static void NewFile()
        {
            using (var gsd = new GameSelectionForm())
            {
                gsd.ShowDialog();
                if (gsd.SelectedGameID < 0) return;
                var game = Context.DBContext.GetAllGames().Single(g => g.ID == gsd.SelectedGameID);
                Document.New(game);
            }
        }

        private static void OpenFile()
        {
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                using (var gsd = new GameSelectionForm())
                {
                    gsd.ShowDialog();
                    if (gsd.SelectedGameID < 0) return;
                    var game = Context.DBContext.GetAllGames().Single(g => g.ID == gsd.SelectedGameID);
                    var filename = ofd.FileName;
                    //try
                    {
                        Document.Open(filename, game);
                    }
                    //catch (Exception e)
                    {
                        //Error.Warning("The map file could not be opened:\n" + e.Message);
                        //throw;
                    }
                }
            }
        }

        private static void SaveFile()
        {
            if (!Document.MapOpen) return;
            var filename = Document.CurrentMapFile;
            if (filename == null)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"RMF Files (*.rmf)|*.rmf";
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    filename = sfd.FileName;
                }
            }
            Document.Save(filename);
        }

        private void EditorLoad(object sender, EventArgs e)
        {
            ViewportManager.Init(tblQuadView);
            ToolManager.Init();

            foreach (var tool in ToolManager.Tools)
            {
                var tl = tool;
                tspTools.Items.Add(new ToolStripButton(
                    "",
                    tl.GetIcon(),
                    (s, ea) => SelectTool(tl),
                    tl.GetName())
                        {
                            Checked = (tl == ToolManager.ActiveTool)
                        }
                    );
            }

            MapProvider.Register(new RmfProvider());
            MapProvider.Register(new VmfProvider());
            GameDataProvider.Register(new FgdProvider());
            TextureProvider.Register(new WadProvider());

            //try
            //{
            //    var mdl = new MdlProvider();
            //    var filePath = @"D:\Sledge\_Resources\MDL\HL2_44\seagull.mdl";
            //    //var filePath = @"D:\Sledge\_Resources\MDL\TF2_48\heavy.mdl";
            //    //var filePath = @"D:\Sledge\_Resources\MDL\HL1_10\barney.mdl";
            //    var file = FileSystemFactory.Create(filePath);
            //    var rdr = mdl.LoadMDL(file, ModelLoadItems.AllStatic);
            //    var modelRender = new ModelRenderable(rdr);
            //    ViewportManager.AddContext3D(modelRender);
            //    ViewportManager.AddContext2D(modelRender);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Suppress presses of the alt key if required
            if (Document.CaptureAltPresses && (keyData & Keys.Alt) == Keys.Alt)
            {
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void OpenSettingsDialog(object sender, EventArgs e)
        {
            using (var sf = new SettingsForm())
            {
                sf.ShowDialog();
            }
        }
    }
}
