using System.Collections.Generic;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a text box.
    /// </summary>
    public class QuickFormTextBox : QuickFormItem
    {
        public QuickFormTextBox(string tbname)
        {
            Name = tbname;
        }

        public override List<Control> GetControls(QuickForm qf)
        {
            var controls = new List<Control>();
            var l = new Label { Text = Name };
            Location(l, qf, true);
            Size(l, qf.LabelWidth);
            TextAlign(l);
            controls.Add(l);
            var t = new TextBox { Name = Name };
            Anchor(t);
            Location(t, qf, false);
            Size(t, qf, qf.LabelWidth);
            controls.Add(t);
            return controls;
        }
    }
}
