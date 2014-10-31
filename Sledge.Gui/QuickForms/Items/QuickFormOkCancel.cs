using System;
using System.Collections.Generic;
using System.Drawing;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.QuickForms.Items
{
	/// <summary>
	/// A control that shows OK and Cancel buttons.
	/// </summary>
	public class QuickFormOkCancel : IQuickFormItem
	{
	    public string Name { get; set; }

	    private readonly Action<QuickForm> _okevent;
	    private readonly Action<QuickForm> _cancelevent;

        public QuickFormOkCancel(Action<QuickForm> ok, Action<QuickForm> cancel)
		{
			_okevent = ok;
			_cancelevent = cancel;
		}

        public IControl GetControl(QuickForm qf)
		{
		    var hbox = new HorizontalBox();

		    var blank = new Label {Text = ""};

            var ok = new Button {Text = "OK"};
            ok.Clicked += (sender, args) => { qf.Success = true; qf.Close(); };

            var cancel = new Button {Text = "Cancel"};
            cancel.Clicked += (sender, args) => { qf.Success = false; qf.Close(); };

            hbox.Add(blank, true);
            hbox.Add(cancel, false);
            hbox.Add(ok, false);
		    return hbox;
		}
	}
}
