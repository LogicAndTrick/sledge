using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.BspEditor.Environment.Goldsource
{
    public partial class GoldsourceEnvironmentEditor : UserControl, IEnvironmentEditor
    {
        public event EventHandler EnvironmentChanged;
        public Control Control => this;

        public IEnvironment Environment
        {
            get => GetEnvironment();
            set => SetEnvironment(value as GoldsourceEnvironment);
        }

        public GoldsourceEnvironmentEditor()
        {
            InitializeComponent();

            txtGameDir.TextChanged += OnEnvironmentChanged;
            cmbBaseGame.SelectedIndexChanged += OnEnvironmentChanged;
            cmbGameMod.SelectedIndexChanged += OnEnvironmentChanged;
            cmbGameExe.SelectedIndexChanged += OnEnvironmentChanged;
            chkLoadHdModels.CheckedChanged += OnEnvironmentChanged;

            cmbDefaultPointEntity.SelectedIndexChanged += OnEnvironmentChanged;
            cmbDefaultBrushEntity.SelectedIndexChanged += OnEnvironmentChanged;
            chkOverrideMapSize.CheckedChanged += OnEnvironmentChanged;
            cmbMapSizeOverrideLow.SelectedIndexChanged += OnEnvironmentChanged;
            cmbMapSizeOverrideHigh.SelectedIndexChanged += OnEnvironmentChanged;
            chkIncludeFgdDirectories.CheckedChanged += OnEnvironmentChanged;

            txtBuildToolsDirectory.TextChanged += OnEnvironmentChanged;
            chkIncludeToolsDirectory.CheckedChanged += OnEnvironmentChanged;
            cmbBspExe.SelectedIndexChanged += OnEnvironmentChanged;
            cmbCsgExe.SelectedIndexChanged += OnEnvironmentChanged;
            cmbVisExe.SelectedIndexChanged += OnEnvironmentChanged;
            cmbRadExe.SelectedIndexChanged += OnEnvironmentChanged;

            nudDefaultTextureScale.ValueChanged += OnEnvironmentChanged;
        }

        private void OnEnvironmentChanged(object sender, EventArgs e)
        {
            EnvironmentChanged?.Invoke(this, e);
        }

        public void SetEnvironment(GoldsourceEnvironment env)
        {
            if (env == null) env = new GoldsourceEnvironment();

            txtGameDir.Text = env.BaseDirectory;
            cmbBaseGame.SelectedItem = env.GameDirectory;
            cmbGameMod.SelectedItem = env.ModDirectory;
            cmbGameExe.SelectedItem = env.GameExe;
            chkLoadHdModels.Checked = env.LoadHdModels;

            lstFgds.Items.Clear();
            foreach (var fileName in env.FgdFiles)
            {
                lstFgds.Items.Add(new ListViewItem(new[]
                    {
                        Path.GetFileName(fileName),
                        fileName
                    })
                    {ToolTipText = fileName});
                UpdateFgdList();
            }
            cmbDefaultPointEntity.SelectedItem = env.DefaultPointEntity;
            cmbDefaultBrushEntity.SelectedItem = env.DefaultBrushEntity;
            chkOverrideMapSize.Checked = env.OverrideMapSize;
            cmbMapSizeOverrideLow.SelectedItem = Convert.ToString(env.MapSizeLow, CultureInfo.InvariantCulture);
            cmbMapSizeOverrideHigh.SelectedItem = Convert.ToString(env.MapSizeHigh, CultureInfo.InvariantCulture);
            chkIncludeFgdDirectories.Checked = env.IncludeFgdDirectoriesInEnvironment;

            txtBuildToolsDirectory.Text = env.ToolsDirectory;
            chkIncludeToolsDirectory.Checked = env.IncludeToolsDirectoryInEnvironment;
            cmbBspExe.SelectedItem = env.BspExe;
            cmbCsgExe.SelectedItem = env.CsgExe;
            cmbVisExe.SelectedItem = env.VisExe;
            cmbRadExe.SelectedItem = env.RadExe;

            nudDefaultTextureScale.Value = env.DefaultTextureScale;
        }

        public GoldsourceEnvironment GetEnvironment()
        {
            return new GoldsourceEnvironment
            {
                BaseDirectory = txtGameDir.Text,
                GameDirectory = Convert.ToString(cmbBaseGame.SelectedItem, CultureInfo.InvariantCulture),
                ModDirectory = Convert.ToString(cmbGameMod.SelectedItem, CultureInfo.InvariantCulture),
                GameExe = Convert.ToString(cmbGameExe.SelectedItem, CultureInfo.InvariantCulture),
                LoadHdModels = chkLoadHdModels.Checked,

                FgdFiles = lstFgds.Items.OfType<ListViewItem>().Select(x => x.SubItems[1].Text).Where(File.Exists).ToList(),
                DefaultPointEntity = Convert.ToString(cmbDefaultPointEntity.SelectedItem, CultureInfo.InvariantCulture),
                DefaultBrushEntity = Convert.ToString(cmbDefaultBrushEntity.SelectedItem, CultureInfo.InvariantCulture),
                OverrideMapSize = chkOverrideMapSize.Checked,
                MapSizeLow = decimal.TryParse(Convert.ToString(cmbMapSizeOverrideLow.SelectedItem, CultureInfo.InvariantCulture), out decimal l) ? l : 0,
                MapSizeHigh = decimal.TryParse(Convert.ToString(cmbMapSizeOverrideHigh.SelectedItem, CultureInfo.InvariantCulture), out decimal h) ? h : 0,
                IncludeFgdDirectoriesInEnvironment = chkIncludeFgdDirectories.Checked,

                ToolsDirectory = txtBuildToolsDirectory.Text,
                IncludeToolsDirectoryInEnvironment = chkIncludeToolsDirectory.Checked,
                BspExe = Convert.ToString(cmbBspExe.SelectedItem, CultureInfo.InvariantCulture),
                CsgExe = Convert.ToString(cmbCsgExe.SelectedItem, CultureInfo.InvariantCulture),
                VisExe = Convert.ToString(cmbVisExe.SelectedItem, CultureInfo.InvariantCulture),
                RadExe = Convert.ToString(cmbRadExe.SelectedItem, CultureInfo.InvariantCulture),

                DefaultTextureScale = nudDefaultTextureScale.Value
            };
        }

        // Directories

        private void BrowseGameDirectory(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(txtGameDir.Text)) fbd.SelectedPath = txtGameDir.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtGameDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void GameDirectoryTextChanged(object sender, EventArgs e)
        {
            UpdateGameDirectory();
        }

        private void UpdateGameDirectory()
        {
            var dir = txtGameDir.Text;
            if (!Directory.Exists(dir)) return;

            // Set game/mod directories
            var mod = cmbGameMod.SelectedItem ?? "";
            var bse = cmbBaseGame.SelectedItem ?? "";
            
            cmbGameMod.Items.Clear();
            cmbBaseGame.Items.Clear();

            var mods = Directory.GetDirectories(dir).Select(Path.GetFileName);
            var ignored = new[] { "gldrv", "logos", "logs", "errorlogs", "platform", "config" };

            var range = mods.Where(x => !ignored.Contains(x.ToLowerInvariant())).OfType<object>().ToArray();
            cmbGameMod.Items.AddRange(range);
            cmbBaseGame.Items.AddRange(range);

            if (cmbGameMod.Items.Contains(mod)) cmbGameMod.SelectedItem = mod;
            else if (cmbGameMod.Items.Count > 0) cmbGameMod.SelectedIndex = 0;

            if (cmbBaseGame.Items.Contains(bse)) cmbBaseGame.SelectedItem = bse;
            else if (cmbBaseGame.Items.Count > 0) cmbBaseGame.SelectedIndex = 0;

            // Set game executable

            var exe = cmbGameExe.SelectedItem ?? "";

            cmbGameExe.Items.Clear();

            var exes = Directory.GetFiles(dir, "*.exe").Select(Path.GetFileName);
            ignored = new[] { "sxuninst.exe", "utdel32.exe", "upd.exe", "hlds.exe", "hltv.exe" };

            range = exes.Where(x => !ignored.Contains(x.ToLowerInvariant())).OfType<object>().ToArray();
            cmbGameExe.Items.AddRange(range);

            if (cmbGameExe.Items.Contains(exe)) cmbGameExe.SelectedItem = exe;
            else if (cmbGameExe.Items.Count > 0) cmbGameExe.SelectedIndex = 0;
        }

        // Game data files

        public string FgdFilesLabel { get; set; } = "Forge Game Data files";

        private void BrowseFgd(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = FgdFilesLabel + @" (*.fgd)|*.fgd", Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in ofd.FileNames)
                    {
                        lstFgds.Items.Add(new ListViewItem(new[]
                        {
                            Path.GetFileName(fileName),
                            fileName
                        }) {ToolTipText = fileName});
                    }
                    UpdateFgdList();
                    OnEnvironmentChanged(this, EventArgs.Empty);
                }
            }
        }
        
        private void RemoveFgd(object sender, EventArgs e)
        {
            if (lstFgds.SelectedItems.Count > 0)
            {
                foreach (var i in lstFgds.SelectedItems.OfType<ListViewItem>().ToList())
                {
                    lstFgds.Items.Remove(i);
                }
                UpdateFgdList();
                OnEnvironmentChanged(this, EventArgs.Empty);
            }
        }

        private void UpdateFgdList()
        {
            lstFgds.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // todo load fgds
        }

        // Build tools

        private void BrowseBuildToolsDirectory(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(txtBuildToolsDirectory.Text)) fbd.SelectedPath = txtBuildToolsDirectory.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtBuildToolsDirectory.Text = fbd.SelectedPath;
                }
            }
        }

        private void BuildToolsDirectoryTextChanged(object sender, EventArgs e)
        {
            UpdateBuildToolsDirectory();
        }

        private void UpdateBuildToolsDirectory()
        {
            var dir = txtBuildToolsDirectory.Text;
            if (!Directory.Exists(dir)) return;

            var selBsp = cmbBspExe.SelectedItem ?? "";
            var selCsg = cmbCsgExe.SelectedItem ?? "";
            var selVis = cmbVisExe.SelectedItem ?? "";
            var selRad = cmbRadExe.SelectedItem ?? "";

            cmbBspExe.Items.Clear();
            cmbCsgExe.Items.Clear();
            cmbVisExe.Items.Clear();
            cmbRadExe.Items.Clear();

            var range = Directory.GetFiles(dir, "*.exe").Select(Path.GetFileName).ToList();
            var rangeArr = range.OfType<object>().ToArray();

            cmbBspExe.Items.AddRange(rangeArr);
            cmbCsgExe.Items.AddRange(rangeArr);
            cmbVisExe.Items.AddRange(rangeArr);
            cmbRadExe.Items.AddRange(rangeArr);

            cmbBspExe.SelectedIndex = -1;
            cmbCsgExe.SelectedIndex = -1;
            cmbVisExe.SelectedIndex = -1;
            cmbRadExe.SelectedIndex = -1;

            if (cmbBspExe.Items.Contains(selBsp)) cmbBspExe.SelectedItem = selBsp;
            else if (cmbBspExe.Items.Count > 0) cmbBspExe.SelectedIndex = Math.Max(0, range.FindIndex(x => x.ToLower().Contains("bsp")));

            if (cmbCsgExe.Items.Contains(selCsg)) cmbCsgExe.SelectedItem = selCsg;
            else if (cmbCsgExe.Items.Count > 0) cmbCsgExe.SelectedIndex = Math.Max(0, range.FindIndex(x => x.ToLower().Contains("csg")));

            if (cmbVisExe.Items.Contains(selVis)) cmbVisExe.SelectedItem = selVis;
            else if (cmbVisExe.Items.Count > 0) cmbVisExe.SelectedIndex = Math.Max(0, range.FindIndex(x => x.ToLower().Contains("vis")));

            if (cmbRadExe.Items.Contains(selRad)) cmbRadExe.SelectedItem = selRad;
            else if (cmbRadExe.Items.Count > 0) cmbRadExe.SelectedIndex = Math.Max(0, range.FindIndex(x => x.ToLower().Contains("rad")));
        }
    }
}
