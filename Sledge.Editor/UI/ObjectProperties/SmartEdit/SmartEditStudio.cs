using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.Studio)]
    [SmartEdit(VariableType.Sprite)]
    internal class SmartEditStudio : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditStudio()
        {
            _textBox = new TextBox { Width = 180 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);

            var btn = new Button { Text = "Browse...", Margin = new Padding(1), Height = 24 };
            btn.Click += OpenModelBrowser;
            Controls.Add(btn);
        }

        private void OpenModelBrowser(object sender, EventArgs e)
        {
            var rt = Document.Environment.Root;
            using (var fb = new FileSystem.FileSystemBrowserDialog(rt) { Filter =  "*.mdl,*.spr", FilterText = "Models/Sprites (*.mdl, *.spr)"})
            {
                if (fb.ShowDialog() == DialogResult.OK && fb.SelectedFiles.Any())
                {
                    var f = fb.SelectedFiles.First();
                    _textBox.Text = GetPath(f);
                }
            }
        }

        private string GetPath(IFile file)
        {
            var path = "";
            while (file != null && !(file is RootFile))
            {
                path = "/" + file.Name + path;
                file = file.Parent;
            }
            return path.TrimStart('/');
        }

        protected override string GetName()
        {
            return OriginalName;
        }

        protected override string GetValue()
        {
            return _textBox.Text;
        }

        protected override void OnSetProperty()
        {
            _textBox.Text = PropertyValue;
        }
    }
}