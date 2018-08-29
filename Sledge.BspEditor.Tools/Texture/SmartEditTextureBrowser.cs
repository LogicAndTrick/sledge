using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components.Properties;
using Sledge.BspEditor.Editing.Components.Properties.SmartEdit;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    [Export(typeof(IObjectPropertyEditor))]
    [AutoTranslate]
    public class SmartEditTextureBrowser : SmartEditControl
    {
        private WeakReference<MapDocument> _document;
        [Import] private Lazy<ITranslationStringProvider> _translation;

        private readonly TextBox _textBox;
        private readonly Button _browseButton;

        public SmartEditTextureBrowser()
        {
            CreateHandle();

            _textBox = new TextBox { Width = 180 };
            _textBox.TextChanged += (sender, e) => OnValueChanged();
            Controls.Add(_textBox);

            _browseButton = new Button { Text = "Browse...", Margin = new Padding(1), UseVisualStyleBackColor = true };
            _browseButton.Click += OpenTextureBrowser;
            Controls.Add(_browseButton);

            _document = new WeakReference<MapDocument>(null);
        }

        public string Browse
        {
            set { this.InvokeLater(() => _browseButton.Text = value); }
        }

        public override string PriorityHint => "H";

        public override bool SupportsType(VariableType type)
        {
            return type == VariableType.Decal || type == VariableType.Material || type == VariableType.Sprite;
        }

        private async void OpenTextureBrowser(object sender, EventArgs e)
        {
            await OpenTextureBrowser();
        }

        private async Task OpenTextureBrowser()
        {
            if (!_document.TryGetTarget(out var doc)) return;

            using (var tb = new TextureBrowser(doc))
            {
                await tb.Initialise(_translation.Value);
                tb.SetTextureList(await GetTextureList(doc));
                tb.SetSelectedTextures(GetSelectedTextures());
                tb.ShowDialog();
                if (tb.SelectedTexture != null)
                {
                    _textBox.Text = tb.SelectedTexture;
                }
            }
        }

        private IEnumerable<string> GetSelectedTextures()
        {
            yield return _textBox.Text;
        }

        private async Task<IEnumerable<string>> GetTextureList(MapDocument doc)
        {
            var tc = await doc.Environment.GetTextureCollection();
            switch (Property.VariableType)
            {
                case VariableType.Decal:
                    return tc.GetDecalTextures();
                case VariableType.Sprite:
                    return tc.GetSpriteTextures();
                default:
                    return tc.GetAllTextures();
            }
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
            _document = new WeakReference<MapDocument>(document);
            _textBox.Text = PropertyValue;
        }
    }
}