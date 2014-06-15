using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Brushes;
using Sledge.Editor.Tools.SelectTool.TransformationTools;
using Sledge.Settings;

namespace Sledge.Editor.Tools.TextureTool
{
    public partial class TextureToolSidebarPanel : UserControl
    {
        public delegate void RandomiseXShiftValuesEventHandler(object sender, int min, int max);
        public delegate void RandomiseYShiftValuesEventHandler(object sender, int min, int max);
        public delegate void TileFitEventHandler(object sender, int tileX, int tileY);

        public event RandomiseXShiftValuesEventHandler RandomiseXShiftValues;
        public event RandomiseYShiftValuesEventHandler RandomiseYShiftValues;
        public event TileFitEventHandler TileFit;

        protected virtual void OnRandomiseXShiftValues(int min, int max)
        {
            if (RandomiseXShiftValues != null)
            {
                RandomiseXShiftValues(this, min, max);
            }
        }

        protected virtual void OnRandomiseYShiftValues(int min, int max)
        {
            if (RandomiseYShiftValues != null)
            {
                RandomiseYShiftValues(this, min, max);
            }
        }

        protected virtual void OnTileFit(int tileX, int tileY)
        {
            if (TileFit != null)
            {
                TileFit(this, tileX, tileY);
            }
        }

        public TextureToolSidebarPanel()
        {
            InitializeComponent();
        }

        private void RandomShiftXButtonClicked(object sender, EventArgs e)
        {
            OnRandomiseXShiftValues((int) RandomShiftMin.Value, (int) RandomShiftMax.Value);
        }

        private void RandomShiftYButtonClicked(object sender, EventArgs e)
        {
            OnRandomiseYShiftValues((int)RandomShiftMin.Value, (int)RandomShiftMax.Value);
        }

        private void TileFitButtonClicked(object sender, EventArgs e)
        {
            OnTileFit((int) TileFitX.Value, (int) TileFitY.Value);
        }
    }
}
