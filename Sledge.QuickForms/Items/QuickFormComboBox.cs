using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a ComboBox control.
    /// </summary>
    public class QuickFormComboBox : QuickFormItem
    {
        private readonly IEnumerable<object> _items; 

        public QuickFormComboBox(string name, IEnumerable<object> items)
        {
            Name = name;
            _items = items;
        }

        public override List<Control> GetControls(QuickForm qf)
        {
            var controls = new List<Control>();

            var l = new Label { Text = Name };
            Location(l, qf, true);
            Size(l, qf.LabelWidth);
            TextAlign(l);
            controls.Add(l);

            var cb = new ComboBox
                         {
                             Name = Name,
                             Anchor = AnchorStyles.Top | AnchorStyles.Right,
                             DropDownStyle = ComboBoxStyle.DropDownList
                         };
            cb.Items.AddRange(_items.ToArray());
            cb.SelectedIndex = 0;

            Location(cb, qf, false);
            Size(cb, qf, qf.LabelWidth);
            controls.Add(cb);

            return controls;
        }
    }
}