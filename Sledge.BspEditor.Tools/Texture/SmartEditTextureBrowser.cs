using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

            _browseButton = new Button { Text = "Browse...", Margin = new Padding(1), Height = 24 };
            _browseButton.Click += OpenModelBrowser;
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

        private void OpenModelBrowser(object sender, EventArgs e)
        {
            if (!_document.TryGetTarget(out var doc)) return;

            using (var tb = new TextureBrowser(doc))
            {
                tb.Initialise(_translation.Value).Wait();
                //tb.SetTextureList(GetTextureList(doc));
                tb.SetSelectedTextures(GetSelectedTextures());
                tb.SetFilterText(GetFilterText(doc));
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

        private IEnumerable<string> GetTextureList()
        {
            // todo texture list
            throw new NotImplementedException();
            //switch (Property.VariableType)
            //{
            //    case VariableType.Decal:
            //        // TODO goldsource/source
            //        if (Document.Game.Engine == Engine.Goldsource) return Document.TextureCollection.Packages.Where(x => x.PackageRelativePath.Contains("decal")).SelectMany(x => x.Items.Values);
            //        else return Document.TextureCollection.GetAllBrowsableItems();
            //    case VariableType.Sprite:
            //        // TODO goldsource/source
            //        if (Document.Game.Engine == Engine.Goldsource) return Document.TextureCollection.Packages.Where(x => !x.IsBrowsable).SelectMany(x => x.Items.Values);
            //        else return Document.TextureCollection.GetAllItems();
            //    default:
            //        return Document.TextureCollection.GetAllBrowsableItems();
            //}
        }

        private string GetFilterText(MapDocument doc)
        {
            switch (Property.VariableType)
            {
                case VariableType.Sprite:
                    return "sprites/";
                case VariableType.Decal:
                    // TODO goldsource/source
                    //if (Document.Game.Engine == Engine.Goldsource) return "{";
                    //else return "decals/";
                    return ""; // todo environment
                default:
                    return null;
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