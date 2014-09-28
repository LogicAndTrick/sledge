using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sledge.Gui.Controls;
using Padding = Sledge.Gui.Controls.Padding;

namespace Sledge.Gui.WinForms.Controls
{
    public abstract class WinFormsContainer : WinFormsControl, IContainer
    {
        public List<WinFormsControl> Children;
        public Dictionary<WinFormsControl, ContainerMetadata> Metadata;

        public int NumChildren
        {
            get { return Children.Count; }
        }

        IEnumerable<IControl> IContainer.Children
        {
            get { return Children; }
        }

        public override Size PreferredSize
        {
            get
            {
                var width = 0;
                var height = 0;
                foreach (var child in Children)
                {
                    var ps = child.PreferredSize;
                    width = Math.Max(width, ps.Width);
                    height = Math.Max(height, ps.Height);
                }
                width += Margin.Left + Margin.Right;
                height += Margin.Top + Margin.Bottom;
                return new Size(width, height);
            }
        }

        public Padding Margin
        {
            get { return Control.Padding.ToPadding(); }
            set { Control.Padding = new System.Windows.Forms.Padding(value.Left, value.Top, value.Right, value.Bottom); }
        }

        protected WinFormsContainer() : this(new Panel())
        {
        }

        protected WinFormsContainer(Control container) : base(container)
        {
            Children = new List<WinFormsControl>();
            Metadata = new Dictionary<WinFormsControl, ContainerMetadata>();
            container.Resize += Resize;
            container.ControlAdded += Resize;
            container.ControlRemoved += Resize;
        }

        private void Resize(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        public void Insert(int index, IControl child)
        {
            Insert(index, child, GetDefaultMetadata(child));
        }

        protected virtual ContainerMetadata GetDefaultMetadata(IControl child)
        {
            return new ContainerMetadata();
        }

        public void Insert(int index, IControl child, ContainerMetadata metadata)
        {
            BindChildEvents(child);
            AppendChild(index, (WinFormsControl) child);
            Children.Insert(index, (WinFormsControl) child);
            Metadata.Add((WinFormsControl) child, metadata);
            CalculateLayout();
        }

        protected virtual void AppendChild(int index, WinFormsControl child)
        {
            Control.Controls.Add(child.Control);
        }

        protected virtual void BindChildEvents(IControl child)
        {
            child.PreferredSizeChanged += ChildPreferredSizeChanged;
            child.ActualSizeChanged += ChildActualSizeChanged;
        }

        protected virtual void ChildPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        protected virtual void ChildActualSizeChanged(object sender, EventArgs e)
        {

        }

        // protected virtual void BuildContainer()
        // {
        //     ClearControls();
        //     CalculateLayout();
        //     AddControls();
        // }

        /*
        protected virtual void ClearControls()
        {
            Control.Controls.Clear();
        }

        protected virtual void AddControls()
        {
            foreach (var wfc in Children)
            {
                Control.Controls.Add(wfc.Control);
            }
        }
         */

        protected abstract void CalculateLayout();
    }
}