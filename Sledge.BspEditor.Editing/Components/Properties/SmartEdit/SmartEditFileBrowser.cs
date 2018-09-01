using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Sledge.BspEditor.Controls.FileSystem;
using Sledge.BspEditor.Documents;
using Sledge.Common.Logging;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
    [Export(typeof(IObjectPropertyEditor))]
    [AutoTranslate]
    public class SmartEditFileBrowser : SmartEditControl
    {
        private WeakReference<IFile> _root;

        private readonly TextBox _textBox;
        private readonly Button _browseButton;
        private readonly Button _previewButton;

        public SmartEditFileBrowser()
        {
            CreateHandle();

            _textBox = new TextBox { Width = 180 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);

            _browseButton = new Button { Text = "Browse", Margin = new Padding(1), UseVisualStyleBackColor = true };
            _browseButton.Click += OpenModelBrowser;
            Controls.Add(_browseButton);

            _previewButton = new Button { Text = "Preview", Margin = new Padding(1), UseVisualStyleBackColor = true };
            _previewButton.Click += PreviewSelection;
            Controls.Add(_previewButton);

            _root = new WeakReference<IFile>(null);
        }

        public string Browse
        {
            set { this.InvokeLater(() => _browseButton.Text = value); }
        }

        public string Preview
        {
            set { this.InvokeLater(() => _previewButton.Text = value); }
        }

        public override string PriorityHint => "H";

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.Studio || type == VariableType.Sound;
        }

        private FileSystemBrowserDialog CreateDialog(IFile root)
        {
            var fs = new FileSystemBrowserDialog(
                Property.VariableType == VariableType.Sound
                    ? root.GetChild("sound")
                    : root
                );
            switch (Property.VariableType)
            {
                case VariableType.Studio:
                    fs.Filter = "*.mdl";
                    fs.FilterText = "Models (*.mdl)";
                    break;
                case VariableType.Sprite:
                    fs.Filter = "*.spr";
                    fs.FilterText = "Sprites (*.spr)";
                    break;
                case VariableType.Sound:
                    fs.Filter = "*.wav,*.mp3";
                    fs.FilterText = "Audio (*.wav, *.mp3)";
                    break;
            }
            return fs;
        }

        private void OpenModelBrowser(object sender, EventArgs e)
        {
            if (!_root.TryGetTarget(out var rt)) return;

            using (var fb = CreateDialog(rt))
            {
                if (fb.ShowDialog() == DialogResult.OK && fb.SelectedFiles.Any())
                {
                    var f = fb.SelectedFiles.First();
                    _textBox.Text = GetPath(f);
                }
            }
        }

        private void PreviewSelection(object sender, EventArgs e)
        {
            if (!_root.TryGetTarget(out var rt)) return;

            var path = _textBox.Text;
            if (Property.VariableType == VariableType.Sound) path = "sound/" + path;

            var file = rt.TraversePath(path);
            if (file == null || !file.Exists) return;

            switch (file.Extension?.ToLower())
            {
                case "mp3":
                case "wav":
                    PreviewAudio(file);
                    break;
            }
        }

        private void PreviewAudio(IFile audioFile)
        {
            // Copy the stream to memory to avoid any nasty issues
            MemoryStream ms;
            try
            {
                ms = new MemoryStream();
                using (var stream = audioFile.Open()) stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                Log.Error(nameof(SmartEditFileBrowser), "Exception opening audio file", e);
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                using (ms)
                {
                    try
                    {
                        using (var player = new SoundPlayer(ms))
                        {
                            player.PlaySync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(nameof(SmartEditFileBrowser), "Exception playing audio file", e);
                    }
                }
            });
        }

        private string GetPath(IFile file)
        {
            var path = "";
            while (file != null && !(file is RootFile))
            {
                if (Property.VariableType == VariableType.Sound && file.Name.ToLower() == "sound" && (file.Parent == null || file.Parent is RootFile)) break;
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

        protected override void OnSetProperty(MapDocument document)
        {
            _previewButton.Visible = Property.VariableType == VariableType.Sound;

            _root = new WeakReference<IFile>(document.Environment.Root);
            _textBox.Text = PropertyValue;
        }
    }
}