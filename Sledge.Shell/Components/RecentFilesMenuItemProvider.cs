using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Components
{
    [Export(typeof(IMenuItemProvider))]
    [Export(typeof(ISettingsContainer))]
    public class RecentFilesMenuItemProvider : IMenuItemProvider, ISettingsContainer
    {
        private readonly List<RecentFile> _recentFiles;

        public RecentFilesMenuItemProvider()
        {
            _recentFiles = new List<RecentFile>();
            Oy.Subscribe<IDocument>("Document:Opened", OpenDocument);
        }

        private async Task OpenDocument(IDocument doc)
        {
            if (doc?.FileName != null && File.Exists(doc.FileName))
            {
                _recentFiles.RemoveAll(x => String.Equals(doc.FileName, x.Location, StringComparison.InvariantCultureIgnoreCase));
                _recentFiles.Add(new RecentFile {Location = doc.FileName});
                while (_recentFiles.Count > 10) _recentFiles.RemoveAt(0);
            }
        }

        public event EventHandler MenuItemsChanged;

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            for (var i = 0; i < _recentFiles.Count; i++)
            {
                yield return new RecentFilesMenuItem(_recentFiles.Count - i - 1, _recentFiles[i]);
            }
        }

        public string Name => "Sledge.Shell.RecentFilesMenuItemProvider";
        public IEnumerable<SettingKey> GetKeys()
        {
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            var list = store.Get("Files", new List<RecentFile>());
            _recentFiles.Clear();
            _recentFiles.AddRange(list);
            MenuItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Files", _recentFiles);
        }

        private class RecentFile
        {
            public string Location { get; set; }
        }

        private class RecentFilesMenuItem : IMenuItem
        {
            private static readonly Dictionary<int, Image> Icons = new Dictionary<int, Image>();

            private static Image GetIcon(int num)
            {
                if (!Icons.ContainsKey(num))
                {
                    var ico = new Bitmap(16, 16);
                    using (var g = Graphics.FromImage(ico))
                    {
                        var str = Convert.ToString(num, CultureInfo.InvariantCulture);
                        var sz = g.MeasureString(str, SystemFonts.DefaultFont);

                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.FillRectangle(Brushes.Transparent, 0, 0, 16, 16);
                        g.DrawString(str, SystemFonts.DefaultFont, SystemBrushes.MenuText, (16 - sz.Width) / 2, (16 - sz.Height) / 2);
                    }
                    Icons[num] = ico;
                }
                return Icons[num];
            }


            private readonly int _index;
            private readonly RecentFile _file;

            public string ID => $"Sledge.Shell.RecentFile[{_index}]";
            public string Name => System.IO.Path.GetFileName(_file.Location);
            public string Description => _file.Location;
            public Image Icon => GetIcon(_index + 1);
            public bool AllowedInToolbar => false;
            public string Section => "File";
            public string Path => "";
            public string Group => "Recent";
            public string OrderHint => Convert.ToString((char) (_index + 'a'));
            public string ShortcutText => "";
            public bool IsToggle => false;

            public RecentFilesMenuItem(int index, RecentFile file)
            {
                _index = index;
                _file = file;
            }

            public bool IsInContext(IContext context)
            {
                return true;
            }

            public async Task Invoke(IContext context)
            {
                if (!File.Exists(_file.Location)) return;

                await Oy.Publish("Command:Run", new CommandMessage("Internal:OpenDocument", new
                {
                    Path = _file.Location
                }));
            }

            public bool GetToggleState(IContext context)
            {
                return false;
            }
        }
    }
}