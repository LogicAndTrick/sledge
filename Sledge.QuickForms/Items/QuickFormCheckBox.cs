using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows a checkbox.
	/// </summary>
	public class QuickFormCheckBox : QuickFormItem
	{
	    public override object Value => _checkBox.Checked;

        private readonly CheckBox _checkBox;

	    public QuickFormCheckBox(string text, bool isChecked)
		{
	        _checkBox = new CheckBox
	        {
	            Text = text,
	            Checked = isChecked
	        };

		    var margin = _checkBox.Margin;
		    margin.Left += LabelWidth + 6;
		    _checkBox.Margin = margin;

            Controls.Add(_checkBox);
		}
	}
}
