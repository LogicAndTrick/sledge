using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows a button.
	/// </summary>
	public class QuickFormButton : QuickFormItem
	{
	    private readonly Action _clickevent;

		public QuickFormButton(string text, Action click)
		{
			Name = text;
			_clickevent = click;
		}
		
		public override List<Control> GetControls(QuickForm qf)
		{
			var controls = new List<Control>();
			
			var b = new Button();
            b.Click += (sender, e) => _clickevent();
			b.Width = 120;
			b.Text = Name;
			Location (b, qf, true);
			controls.Add(b);
			
			return controls;
		}
	}
}
