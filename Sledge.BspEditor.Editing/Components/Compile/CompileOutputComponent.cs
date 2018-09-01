using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Compile;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    [AutoTranslate]
    [Export(typeof(IBottomTabComponent))]
    public class CompileOutputComponent : IBottomTabComponent
    {
        private readonly RichTextBox _control;

        public string Title { get; set; } = "Compile";
        public string OrderHint => "D";
        public object Control => _control;

        public string CompileCompletedSuccessfully { get; set; }
        public string CompileFailed { get; set; }

        private BufferQueue<Output> _buffer;
        private readonly Subject<int> _queue;

        public CompileOutputComponent()
        {
            _control = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Font = new Font(FontFamily.GenericMonospace, 8),
                BackColor = Color.White,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                ReadOnly = true
            };

            _buffer = new BufferQueue<Output>();

            Oy.Subscribe<Batch>("Compile:Started", async b =>
            {
                Clear();
                await Oy.Publish("?", this);
            });

            Oy.Subscribe<string>("Compile:Output", async data => Append(Color.Black, data));
            Oy.Subscribe<string>("Compile:Error", async data => Append(Color.Red, data));
            Oy.Subscribe<string>("Compile:Information", async data => Append(Color.DodgerBlue, data));
            Oy.Subscribe<string>("Compile:Success", async data => Append(Color.Green, data));
            Oy.Subscribe<string>("Compile:Debug", async data => Append(Color.Magenta, data));

            Oy.Subscribe<Batch>("Compile:Finished", async b =>
            {
                var msg = b.Successful ? CompileCompletedSuccessfully : CompileFailed;
                var col = b.Successful ? Color.Green : Color.Red;
                var flowerbox = new string('*', msg.Length + 4);
                Append(col, $"{flowerbox}\r\n* {msg} *\r\n{flowerbox}\r\n");
            });
            
            _queue = new Subject<int>();
            _queue.Publish().RefCount()
                .Sample(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => _control.InvokeLater(UpdateText));
        }

        private void UpdateText()
        {
            _control.SuspendLayout();

            _control.Clear();
            foreach (var output in _buffer)
            {
                _control.Select(_control.TextLength, 0);
                _control.SelectionColor = output.Color;
                _control.AppendText(output.Data);
            }

            _control.Select(_control.TextLength, 0);
            _control.ScrollToCaret();

            _control.ResumeLayout();
        }

        private void Clear()
        {
            _buffer = new BufferQueue<Output>();
            _queue.OnNext(0);
        }

        private void Append(Color color, string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;
            _buffer.Push(new Output(color, data));
            _queue.OnNext(0);
        }

        public bool IsInContext(IContext context)
        {
            return true;
        }

        private class Output
        {
            public Color Color { get; set; }
            public string Data { get; set; }

            public Output(Color color, string data)
            {
                Color = color;
                Data = data;
            }
        }

        private class BufferQueue<T> : ConcurrentQueue<T>
        {
            private readonly object _lock = new object();

            public int Size { get; private set; }

            public BufferQueue(int size = 300)
            {
                Size = size;
            }

            public void Push(T obj)
            {
                Enqueue(obj);
                lock (_lock)
                {
                    while (Count > Size) TryDequeue(out T _);
                }
            }
        }
    }
}
