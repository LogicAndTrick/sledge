using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Vertex.Controls
{
    [AutoTranslate]
    [Export]
    public partial class VertexEditFaceControl : UserControl
    {
        #region Translations
        
        public string WithSelectedFaces { set => this.InvokeLater(() => WithSelectedFacesLabel.Text = value); }
        public string Units { set => this.InvokeLater(() => { UnitsLabel1.Text = value; UnitsLabel2.Text = value; }); }
        public string PokeBy { set => this.InvokeLater(() => PokeByLabel.Text = value); }
        public string BevelBy { set => this.InvokeLater(() => BevelByLabel.Text = value); }
        public string Poke { set => this.InvokeLater(() => PokeFaceButton.Text = value); }
        public string Bevel { set => this.InvokeLater(() => BevelButton.Text = value); }

        #endregion

        public VertexEditFaceControl()
        {
            InitializeComponent();
            CreateHandle();
        }

        private void BevelButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("VertexEditFaceTool:Bevel", (int) BevelValue.Value);
        }

        private void PokeFaceButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("VertexEditFaceTool:Poke", (int) PokeFaceCount.Value);
        }
    }
}
