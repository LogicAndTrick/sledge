using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
    [OrderHint("B")]
    public class LogComponent : IBottomTabComponent
    {
        private readonly TextBox _control;
        private readonly LogBuffer _logs;

        public string Title { get; set; } = "Log";
        public object Control => _control;

        public LogComponent()
        {
            _logs = new LogBuffer(100);
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

        private Task AppendLog(LogMessage log)
        {
            _logs.Add(log);
            var sb = new StringBuilder();
            foreach (var lm in _logs.ToList())
            {
                sb.AppendFormat("{0} [{1}]: {2}\r\n", lm.Type, lm.Source, lm.Message);
            }

            if (_control.Created)
            {
                _control.InvokeLater(() =>
                {
                    _control.Text = sb.ToString();
                    _control.SelectionStart = 0;
                    _control.ScrollToCaret();
                });
            }

            return Task.CompletedTask;
        }

        public bool IsInContext(IContext context)
        {
            return true;
        }

        // Based off https://codereview.stackexchange.com/a/134147
        private class LogBuffer
        {
            private readonly ConcurrentQueue<LogMessage> _data;
            private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
            private readonly int _size;

            public LogBuffer(int size)
            {
                _data = new ConcurrentQueue<LogMessage>();
                _size = size;
            }

            public List<LogMessage> ToList()
            {
                var list = _data.ToList();
                list.Reverse();
                return list;
            }

            public void Add(LogMessage t)
            {
                _lock.EnterWriteLock();
                try
                {
                    if (_data.Count >= _size) _data.TryDequeue(out _);
                    _data.Enqueue(t);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}
