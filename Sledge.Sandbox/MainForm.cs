using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Easings;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Sandbox
{
    public partial class MainForm : Form
    {
        private Viewport3D _viewport;

        public MainForm()
        {
            InitializeComponent();
        }
    }
}
