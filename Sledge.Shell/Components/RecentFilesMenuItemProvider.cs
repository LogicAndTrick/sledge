using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
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
            if (doc != null && doc.FileName != null && File.Exists(doc.FileName))
            {
                _recentFiles.Add(new RecentFile {Location = doc.FileName});
            }
        }

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            for (var i = 0; i < _recentFiles.Count; i++)
            {
                yield return new RecentFilesMenuItem(i, _recentFiles[i]);
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
            // todo !menu need a way to trigger a menu item update
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
            private readonly int _index;
            private readonly RecentFile _file;

            public string ID => $"Sledge.Shell.RecentFile[{_index}]";
            public string Name => System.IO.Path.GetFileName(_file.Location);
            public string Description => System.IO.Path.GetFileName(_file.Location);
            public Image Icon => null;
            public string Section => "File";
            public string Path => "";
            public string Group => "Recent";
            public string OrderHint => Convert.ToString((char) (_index + 'a'));

            public RecentFilesMenuItem(int index, RecentFile file)
            {
                _index = index;
                _file = file;
            }

            public bool IsInContext(IContext context)
            {
                return true;
            }

            public Task Invoke(IContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}