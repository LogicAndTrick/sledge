using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public class HotkeyForm : Form
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return Hotkeys.HotkeyDown(keyData) || base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
