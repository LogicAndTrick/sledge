using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Documents;

namespace Sledge.MinimalEditor.TextEditor
{
    public class TextDocument : IDocument
    {
        private string _name;
        private TextBox _control;

        public string FileName { get; private set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public object Control => _control;

        public TextDocument()
        {
            _name = "Untitled";
            FileName = null;
            Init();
        }

        public TextDocument(string fileName)
        {
            _name = Path.GetFileNameWithoutExtension(fileName);
            FileName = fileName;
            Init();
            _control.Text = File.ReadAllText(fileName);
        }

        private void Init()
        {
            _control = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical };

            Oy.Subscribe<IDocument>("Document:RequestClose", IfThis(RequestClose));
        }

        private Action<IDocument> IfThis(Func<Task> callback)
        {
            return d =>
            {
                if (d == this) callback();
            };
        }

        private async Task RequestClose()
        {
            if (MessageBox.Show("Are you sure", "Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await Oy.Publish("Document:Closed", this);
                // now do anything else?
                _control.Dispose();
            }
        }
    }
}
