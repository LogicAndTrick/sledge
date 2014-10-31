using System.Collections.Generic;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.QuickForms.Items
{
	/// <summary>
	/// An item that just displays text.
	/// </summary>
	public class QuickFormLabel : IQuickFormItem
    {
        public string Name { get; set; }

		public QuickFormLabel(string text)
		{
			Name = text;
		}

        public IControl GetControl(QuickForm qf)
	    {
	        return new Label {Text = Name};
		}
	}
}
