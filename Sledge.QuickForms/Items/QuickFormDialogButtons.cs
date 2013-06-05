using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows any number of buttons.
    /// </summary>
    public class QuickFormDialogButtons : QuickFormItem
    {
        private readonly List<Action<QuickForm>> _actions;
        private readonly List<string> _labels;
        private readonly List<DialogResult> _results;

        public QuickFormDialogButtons()
        {
            _actions = new List<Action<QuickForm>>();
            _labels = new List<string>();
            _results = new List<DialogResult>();
        }

        public QuickFormDialogButtons Button(string label, DialogResult result, Action<QuickForm> action = null)
        {
            _labels.Add(label);
            _actions.Add(action);
            _results.Add(result);
            return this;
        }

        public override List<Control> GetControls(QuickForm qf)
        {
            var controls = new List<Control>();

            var length = _actions.Count;

            for (var i = 0; i < _labels.Count; i++)
            {
                var action = _actions[i];
                var label = _labels[i];
                var result = _results[i];

                var button = new Button();
                if (action != null) button.Click += (sender, e) => action(((Control) sender).Parent as QuickForm);
                button.Click += (s, e) => qf.DialogResult = result;
                button.Click += qf.Close;
                button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                button.Width = 80;
                button.Text = label;
                button.DialogResult = result;
                Location(button, qf, false);
                button.Location = new Point(qf.ClientSize.Width - (QuickForm.ItemPadding + button.Width) * (length - i), button.Location.Y);
                controls.Add(button);
            }

            return controls;
        }
    }
}