using System.Windows.Forms;

namespace Sledge.Editor.UI
{
    public class HotkeyForm : Form
    {
        public bool PreventSimpleHotkeyPassthrough { get; set; }

        public HotkeyForm()
        {
            PreventSimpleHotkeyPassthrough = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var source = FromHandle(msg.HWnd);
            if (source != null && source.Tag == Hotkeys.SuppressHotkeysTag) return base.ProcessCmdKey(ref msg, keyData);

            if (PreventSimpleHotkeyPassthrough)
            {
                var str = KeyboardState.KeysToString(keyData).ToLowerInvariant();
                if (!str.Contains("ctrl") && !str.Contains("alt") && !str.Contains("shift"))
                {
                    return base.ProcessCmdKey(ref msg, keyData);
                }
            }
            // Prevent short curcuiting so the base method always runs
            return Hotkeys.HotkeyDown(keyData) | base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
