using System;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Shell.Registers;
using Message = System.Windows.Forms.Message;

namespace Sledge.Shell.Forms
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            KeyPreview = true;
        }

        internal static HotkeyRegister HotkeyRegister { get; set; }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var source = FromHandle(msg.HWnd);
            if (source != null && CheckIgnoreHotkey(source, keyData))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            
            // Intentional non-short-circuiting | rather than ||
            return (HotkeyRegister != null && HotkeyRegister.Fire(keyData)) | base.ProcessCmdKey(ref msg, keyData);
        }

        private bool CheckIgnoreHotkey(Control source, Keys keyData)
        {
            if (source is TextBox)
            {
                return IsTextBoxKey(keyData);
            }
            return false;
        }

        private bool IsTextBoxKey(Keys keyData)
        {
            var ctrl = keyData.HasFlag(Keys.Control);
            var shift = keyData.HasFlag(Keys.Shift);
            var alt = keyData.HasFlag(Keys.Alt);

            // All no-mod and shift combinations should be passed through
            if (!ctrl && !alt)
            {
                return true;
            }

            // Common commands in text boxes
            if (ctrl && !shift && !alt)
            {
                return keyData.HasFlag(Keys.A) // Select all
                       || keyData.HasFlag(Keys.X) // Cut
                       || keyData.HasFlag(Keys.C) // Copy
                       || keyData.HasFlag(Keys.V) // Paste
                       || keyData.HasFlag(Keys.Z) // Undo
                       || keyData.HasFlag(Keys.Y); // Redo
            }

            return false;
        }
    }
}
