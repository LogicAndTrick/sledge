using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;
using Sledge.Providers.Texture;
using Sledge.Settings.Models;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [SmartEdit(VariableType.Decal)]
    [SmartEdit(VariableType.Material)]
    [SmartEdit(VariableType.Sprite)]
    internal class SmartEditTextureBrowser : SmartEditControl
    {
        private readonly TextBox _textBox;
        public SmartEditTextureBrowser()
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
            using (var tb = new TextureBrowser(Document))
            {
                tb.SetTextureList(GetTextureList());
                tb.SetSelectedTextures(GetSelectedTextures());
                tb.SetFilterText(GetFilterText());
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

        private string GetFilterText()
        {
            switch (Property.VariableType)
            {
                case VariableType.Sprite:
                    return "sprites/";
                case VariableType.Decal:
                    // TODO goldsource/source
                    if (Document.Game.Engine == Engine.Goldsource) return "{";
                    else return "decals/";
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

        protected override void OnSetProperty()
        {
            _textBox.Text = PropertyValue;
        }
    }
}