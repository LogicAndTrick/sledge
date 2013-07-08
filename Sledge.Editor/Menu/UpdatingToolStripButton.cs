using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Common.Mediator;

namespace Sledge.Editor.Menu
{
    class UpdatingToolStripButton : ToolStripButton, IMediatorListener
    {
        private readonly Func<string> _text;
        private readonly Func<bool> _isChecked;
        private readonly Func<bool> _isActive;
        private readonly string _message;
        private readonly object _parameter;

        public UpdatingToolStripButton(string text, Image image, Func<bool> isActive, Func<bool> isChecked, Func<string> textAction, string message, object parameter)
            : base(text, image)
        {
            DisplayStyle = image == null ? ToolStripItemDisplayStyle.Text : ToolStripItemDisplayStyle.Image;
            _isActive = isActive;
            _isChecked = isChecked;
            _text = textAction;
            _message = message;
            _parameter = parameter;
            if (_isActive != null || _text != null || _isChecked != null)
            {
                Mediator.Subscribe(EditorMediator.UpdateMenu, this);
                Mediator.Subscribe(EditorMediator.UpdateToolstrip, this);
                Mediator.Subscribe(EditorMediator.SelectionChanged, this);
                Mediator.Subscribe(EditorMediator.HistoryChanged, this);
                Mediator.Subscribe(EditorMediator.ClipboardChanged, this);
            }
            Notify(null, null);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Mediator.Publish(_message, _parameter);
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void Notify(string message, object data)
        {
            if (_isActive != null) Enabled = _isActive();
            if (_isChecked != null) Checked = _isChecked();
            if (_text != null) Text = _text();
            ToolTipText = Text;
        }
    }
}