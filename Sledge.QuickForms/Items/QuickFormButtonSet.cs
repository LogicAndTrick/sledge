using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows a number of buttons.
	/// </summary>
	public class QuickFormButtonSet : QuickFormItem
	{
	    public override object Value => null;

	    public QuickFormButtonSet(IEnumerable<(string, DialogResult, Action)> buttons)
		{
		    FlowDirection = FlowDirection.RightToLeft;

		    // Add in reverse since the direction is RTL
		    foreach (var b in buttons.Reverse())
		    {
		        var btn = new Button
		        {
		            Text = b.Item1,
		            MinimumSize = new Size(80, 0),
		            Anchor = AnchorStyles.Right,
                    DialogResult = b.Item2
		        };

		        btn.Click += (s, e) => b.Item3();
		        Controls.Add(btn);
		    }
		}
	}
}
