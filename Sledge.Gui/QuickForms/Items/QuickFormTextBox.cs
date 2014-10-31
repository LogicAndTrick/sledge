using System.Collections.Generic;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.QuickForms.Items
{
    /// <summary>
    /// A control that shows a text box.
    /// </summary>
    public class QuickFormTextBox : IQuickFormItem
    {
        public string Name { get; set; }
        private readonly string _defaultValue;

        public QuickFormTextBox(string tbname, string value)
        {
            Name = tbname;
            _defaultValue = value;
        }

        public IControl GetControl(QuickForm qf)
        {
            var hbox = new HorizontalBox();
            hbox.Add(new Label{ Text = Name }, false);
            hbox.Add(new TextBox{BindingSource = Name, Text = _defaultValue}, true);
            return hbox;
        }
    }
}
