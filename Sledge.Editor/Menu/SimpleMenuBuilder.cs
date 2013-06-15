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
        public object Parameter { get; set; }
        public Func<string> Text { get; set; }
        public Func<bool> IsVisible { get; set; }
        public Func<bool> IsActive { get; set; }
        public Func<bool> IsChecked { get; set; }

        public SimpleMenuBuilder(string name, string message, object parameter = null)
        {
            Name = name;
            Message = message;
            IsVisible = IsActive = null;
            Parameter = parameter;
        }

        public SimpleMenuBuilder(string name, Enum message, object parameter = null)
        {
            Name = name;
            Message = message.ToString();
            IsVisible = IsActive = null;
            Parameter = parameter;
        }

        public IEnumerable<ToolStripItem> Build()
        {
            if (IsVisible != null && !IsVisible()) yield break;
            var mi = new UpdatingToolStripMenuItem(Name, IsActive, Text);
            mi.Click += (sender, e) => Mediator.Publish(Message, Parameter);
            if (IsActive != null) mi.Enabled = IsActive();
            if (IsChecked != null) mi.Checked = IsChecked();
            var hk = Hotkeys.GetHotkeyForMessage(Message);
            if (hk != null) mi.ShortcutKeyDisplayString = hk.DefaultHotkey;
            yield return mi;
        }
    }

    class UpdatingToolStripMenuItem : ToolStripMenuItem, IMediatorListener
    {
        private readonly Func<string> _text;
        private readonly Func<bool> _isActive;

        public UpdatingToolStripMenuItem(string text, Func<bool> isActive, Func<string> textAction) : base(text)
        {
            _isActive = isActive;
            _text = textAction;
            if (_isActive != null || _text != null) Mediator.Subscribe(EditorMediator.UpdateMenu, this);
        }

        public void Notify(string message, object data)
        {
            if (_isActive != null) Enabled = _isActive();
            if (_text != null) Text = _text();
        }
    }
}