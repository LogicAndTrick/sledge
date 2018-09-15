using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Resources;
using Sledge.Shell;

namespace Sledge.BspEditor.Rendering.Scene
{
#if DEBUG_EXTRA
    [Export(typeof(ISidebarComponent))]
    [Export(typeof(IInitialiseHook))]
#endif
    [OrderHint("T")]
    public partial class SceneBufferAllocationSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
    {
        private readonly Lazy<SceneManager> _sceneManager;
        private readonly Timer _timer;

        public string Title { get; set; } = "Buffer Allocations";
        public object Control => this;

        [ImportingConstructor]
        public SceneBufferAllocationSidebarPanel([Import] Lazy<SceneManager> sceneManager)
        {
            _sceneManager = sceneManager;
            InitializeComponent();
            DoubleBuffered = true;

            _timer = new Timer {Interval = 1000};
            _timer.Tick += (s,e) => UpdateList();
        }

        protected override void OnLoad(EventArgs e)
        {
            _timer.Start();
            base.OnLoad(e);
        }

        private void ChangeAutoRefresh(object sender, EventArgs e)
        {
            if (chkAutoRefresh.Checked) _timer.Start();
            else _timer.Stop();
        }

        private void UpdateList(object sender, EventArgs e)
        {
            UpdateList();
        }

        public Task OnInitialise()
        {
            return Task.CompletedTask;
        }

        private void UpdateList()
        {
            BufferList.BeginUpdate();
            BufferList.Items.Clear();

            var allocations = _sceneManager.Value?.GetCurrentAllocationInformation();
            if (allocations != null)
            {
                for (var i = 0; i < allocations.Count; i++)
                {
                    var alloc = allocations[i];
                    for (var j = 0; j < alloc.Count; j++)
                    {
                        var ba = new BufferAllocation(i, j, alloc[j]);
                        BufferList.Items.Add(ba);
                    }
                }
            }

            BufferList.EndUpdate();
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        private void DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= BufferList.Items.Count) return;

            var ba = (BufferAllocation) BufferList.Items[e.Index];

            // Blank out the back
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            // Draw text
            var text = $"Builder #{ba.Builder}\nBuffer #{ba.Buffer}";
            var s = e.Graphics.MeasureString(text, Font);
            e.Graphics.DrawString(text, Font, Brushes.Black, 2, e.Bounds.Y + (e.Bounds.Height - s.Height) / 2);

            var xStart = 70;
            var xWidth = e.Bounds.Width - xStart - 3;
            var lineHeight = s.Height / 2;
            var l1Start = e.Bounds.Y + (e.Bounds.Height / 2) - 1 - lineHeight;
            var l2Start = e.Bounds.Y + (e.Bounds.Height / 2) + 1;

            // Ensure white background
            e.Graphics.FillRectangle(Brushes.White, xStart, l1Start, xWidth, lineHeight);
            e.Graphics.FillRectangle(Brushes.White, xStart, l2Start, xWidth, lineHeight);

            // Draw the bar outline
            e.Graphics.DrawRectangle(Pens.DodgerBlue, xStart, l1Start, xWidth, lineHeight);
            e.Graphics.DrawRectangle(Pens.DarkOrange, xStart, l2Start, xWidth, lineHeight);

            // Draw the vertex buffer bar
            var vbPercent = (float) (ba.Allocation.VertexBufferAllocatedSize / (double) ba.Allocation.VertexBufferTotalSize);
            e.Graphics.FillRectangle(Brushes.DodgerBlue, xStart, l1Start, vbPercent * xWidth, lineHeight);

            // Vertex buffer text
            var vpStr = vbPercent.ToString("P");
            if (vbPercent < 0.5)
            {
                e.Graphics.DrawString(vpStr, Font, Brushes.Black, xStart + vbPercent * xWidth, l1Start + 1);
            }
            else
            {
                var ms = e.Graphics.MeasureString(vpStr, Font);
                e.Graphics.DrawString(vpStr, Font, Brushes.White, xStart + vbPercent * xWidth - ms.Width, l1Start + 1);
            }

            // Draw the index buffer bar
            var ibPercent = (float) (ba.Allocation.IndexBufferAllocatedSize / (double) ba.Allocation.IndexBufferTotalSize);
            e.Graphics.FillRectangle(Brushes.DarkOrange, xStart, l2Start, ibPercent * xWidth, lineHeight);
            
            // Index buffer text
            var ipStr = ibPercent.ToString("P");
            if (ibPercent < 0.5)
            {
                e.Graphics.DrawString(ipStr, Font, Brushes.Black, xStart + ibPercent * xWidth, l2Start + 1);
            }
            else
            {
                var ms = e.Graphics.MeasureString(ipStr, Font);
                e.Graphics.DrawString(ipStr, Font, Brushes.White, xStart + ibPercent * xWidth - ms.Width, l2Start + 1);
            }
        }

        private class BufferAllocation
        {
            public int Builder { get; set; }
            public int Buffer { get; set; }
            public BufferBuilder.BufferAllocation Allocation { get; set; }

            public BufferAllocation(int builder, int buffer, BufferBuilder.BufferAllocation allocation)
            {
                Builder = builder;
                Buffer = buffer;
                Allocation = allocation;
            }
        }
    }
}
