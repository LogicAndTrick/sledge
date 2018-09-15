using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// An item that just displays text.
	/// </summary>
	public class QuickFormLabel : QuickFormItem
	{
	    public override object Value => null;

        private readonly Label _label;

	    public QuickFormLabel(string text)
		{
		    _label = new Label
		    {
		        Text = text,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
		    };
            Controls.Add(_label);
		}
	}
}
