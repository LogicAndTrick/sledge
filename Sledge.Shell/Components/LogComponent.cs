using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.Shell.Components
{
    [AutoTranslate]
    [Export(typeof(IBottomTabComponent))]
    public class LogComponent : IBottomTabComponent
    {
        private TextBox _control;
        private List<LogMessage> _logs;

        public string Title { get; set; } = "Log";
        public string OrderHint => "B";
        public object Control => _control;

        public LogComponent()
        {
            _logs = new List<LogMessage>();
            _control = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Font = new Font(FontFamily.GenericMonospace, 10),
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                ReadOnly = true
            };
            Oy.Subscribe<LogMessage>("Log", AppendLog);
        }

        private async Task AppendLog(LogMessage log)
        {
            _logs.Insert(0, log);
            if (_logs.Count > 100) _logs = _logs.Take(100).ToList();
            var sb = new StringBuilder();
            foreach (var lm in _logs)
            {
                sb.AppendFormat("{0} [{1}]: {2}\r\n", lm.Type, lm.Source, lm.Message);
            }
            if (!_control.Created) return;

            _control.Invoke((MethodInvoker) delegate
            {
                _control.Text = sb.ToString();
                _control.SelectionStart = 0;
                _control.ScrollToCaret();
            });
        }

        public bool IsInContext(IContext context)
        {
            return true;
        }
    }
}
