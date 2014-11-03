using System.Drawing;
using Sledge.Settings.Models;

namespace Sledge.EditorNew.UI.Menus
{
    public class RecentFileMenuItem : IMenuItem
    {
        private readonly RecentFile _recent;
        public string TextKey { get { return null; } }
        public string Text { get { return _recent == null ? "No recent files" : System.IO.Path.GetFileName(_recent.Location); } }
        public Image Image { get { return null; } }
        public bool IsActive { get { return _recent != null; } }
        public bool ShowInMenu { get { return true; } }
        public bool ShowInToolstrip { get { return false; } }

        public RecentFileMenuItem(RecentFile recent)
        {
            _recent = recent;
        }

        public void Execute()
        {
            // todo blah blah
        }
    }
}