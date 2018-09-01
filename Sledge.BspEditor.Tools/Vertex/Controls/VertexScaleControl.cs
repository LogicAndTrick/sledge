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
    public partial class VertexScaleControl : UserControl
    {
        #region Translations
        
        public string ScaleDistance { set => this.InvokeLater(() => ScaleDistanceLabel.Text = value); }
        public string Reset { set => this.InvokeLater(() => ResetDistanceButton.Text = value); }
        public string ResetOrigin { set => this.InvokeLater(() => ResetOriginButton.Text = value); }

        #endregion

        private bool _freeze;

        public VertexScaleControl()
        {
            _freeze = true;
            InitializeComponent();
            _freeze = false;

            CreateHandle();
        }

        public void ResetValue()
        {
            _freeze = true;
            DistanceValue.Value = 100;
            Oy.Publish("VertexScaleTool:ValueReset", DistanceValue.Value);
            _freeze = false;
        }

        private void DistanceValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            Oy.Publish("VertexScaleTool:ValueChanged", DistanceValue.Value);
        }

        private void ResetDistanceClicked(object sender, EventArgs e)
        {
            ResetValue();
        }

        private void ResetOriginClicked(object sender, EventArgs e)
        {
            Oy.Publish("VertexScaleTool:ResetOrigin");
        }
    }
}
