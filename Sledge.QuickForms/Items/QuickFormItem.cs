using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// Abstract base class for all form items.
	/// </summary>
	public abstract class QuickFormItem
	{
		protected string Name;
		public abstract List<Control> GetControls(QuickForm qf);
		
		protected void Anchor(Control c)
		{
			c.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
		}
		
		protected void TextAlign(Label l)
		{
			l.TextAlign = ContentAlignment.MiddleLeft;
		}
		
		protected void Location(Control c, QuickForm qf, bool isLabel)
		{
            var x = QuickForm.ItemPadding;
            if (!isLabel) x += qf.LabelWidth + QuickForm.ItemPadding;
			var y = qf.CurrentOffset;
			c.Location = new Point(x, y);
		}
		
		protected void Size(Control c, int width)
		{
			var w = width;
            var h = QuickForm.ItemHeight;
			c.Size = new Size(w, h);
		}
		
		protected void Size(Control c, QuickForm qf, int offset)
		{
            var w = qf.ClientSize.Width - (QuickForm.ItemPadding * 2);
			if (offset > 0)
            {
				w -= offset;
                w -= QuickForm.ItemPadding;
			}
			var h = QuickForm.ItemHeight;
			c.Size = new Size(w, h);
		}
	}
}
