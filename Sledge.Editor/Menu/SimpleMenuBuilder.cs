using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Settings;

namespace Sledge.Editor.Menu
{
    public class SimpleMenuBuilder : IMenuBuilder
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public Func<bool> IsVisible { get; set; }
        public Func<bool> IsActive { get; set; }
        public Func<bool> IsChecked { get; set; }

        public SimpleMenuBuilder(string name, string message)
        {
            Name = name;
            Message = message;
            IsVisible = IsActive = null;
        }

        public SimpleMenuBuilder(string name, Enum message)
        {
            Name = name;
            Message = message.ToString();
            IsVisible = IsActive = null;
        }

        public IEnumerable<ToolStripItem> Build()
        {
            if (IsVisible != null && !IsVisible()) yield break;
            var mi = new ToolStripMenuItem(Name);
            mi.Click += (sender, e) => Mediator.Publish(Message);
            if (IsActive != null) mi.Enabled = IsActive();
            if (IsChecked != null) mi.Checked = IsChecked();
            var hk = Hotkeys.GetHotkeyForMessage(Message);
            if (hk != null) mi.ShortcutKeyDisplayString = hk.DefaultHotkey;
            yield return mi;
        }
    }
}