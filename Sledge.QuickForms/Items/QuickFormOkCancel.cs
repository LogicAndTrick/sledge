using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows OK and Cancel buttons.
	/// </summary>
	public class QuickFormOkCancel : QuickFormItem
	{
	    private readonly string _okText;
	    private readonly string _cancelText;
	    private readonly Action<QuickForm> _okevent;
	    private readonly Action<QuickForm> _cancelevent;

        public QuickFormOkCancel(Action<QuickForm> ok, Action<QuickForm> cancel) : this("OK", "Cancel", ok, cancel)
		{
		}

        public QuickFormOkCancel(string okText, string cancelText, Action<QuickForm> ok, Action<QuickForm> cancel)
		{
		    _okText = okText;
		    _cancelText = cancelText;
		    _okevent = ok;
			_cancelevent = cancel;
		}
		
		public override List<Control> GetControls(QuickForm qf)
		{
			var controls = new List<Control>();
			
			var b1 = new Button();
			if (_okevent != null) b1.Click += (sender, e) => _okevent(((Control) sender).Parent as QuickForm);
		    b1.Click += (s,e) => qf.DialogResult = DialogResult.OK;
			b1.Click += qf.Close;
			b1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			b1.Width = 80;
			b1.Text = _okText;
			b1.DialogResult = DialogResult.OK;
			Location (b1, qf, false);
			b1.Location = new Point(qf.ClientSize.Width - (QuickForm.ItemPadding + b1.Width) * 2, b1.Location.Y);
			controls.Add(b1);
			
			var b2 = new Button();
            if (_cancelevent != null) b2.Click += (sender, e) => _cancelevent(((Control) sender).Parent as QuickForm);
            b2.Click += (s, e) => qf.DialogResult = DialogResult.Cancel;
			b2.Click += qf.Close;
			b2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			b2.Width = 80;
			b2.Text = _cancelText;
			b2.DialogResult = DialogResult.Cancel;
			Location (b2, qf, false);
            b2.Location = new Point(qf.ClientSize.Width - QuickForm.ItemPadding - b2.Width, b2.Location.Y);
			controls.Add(b2);
			
			return controls;
		}
	}
}
