using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Structures;
using Sledge.Gui.WinForms.Controls;
using Binding = Sledge.Gui.Bindings.Binding;
using Padding = Sledge.Gui.Structures.Padding;

namespace Sledge.Gui.WinForms.Containers
{
    public abstract class WinFormsContainer : WinFormsControl, IContainer
    {
        protected List<WinFormsControl> Children { get; private set; }
        protected Dictionary<WinFormsControl, ContainerMetadata> Metadata { get; private set; }

        public int NumChildren
        {
            get { return Children.Count; }
        }

        IEnumerable<IControl> IContainer.Children
        {
            get { return Children; }
        }

        protected override Size DefaultPreferredSize
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
            container.ControlAdded += ChildrenChanged;
            container.ControlRemoved += ChildrenChanged;
        }

        private void ChildrenChanged(object sender, EventArgs e)
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

        public virtual void Insert(int index, IControl child, ContainerMetadata metadata)
        {
            var c = (WinFormsControl) child.Implementation;
            c.Parent = this;
            BindChildEvents(child);
            AppendChild(index, c);
            Children.Insert(index, c);
            Metadata.Add(c, metadata);
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

        protected override void OnActualSizeChanged()
        {
            CalculateLayout();
            base.OnActualSizeChanged();
        }

        protected virtual void ChildActualSizeChanged(object sender, EventArgs e)
        {

        }

        internal override void OnBindingSourceChanged()
        {
            Children.ForEach(x => x.OnBindingSourceChanged());
            base.OnBindingSourceChanged();
        }

        protected override void ApplyBinding(Binding binding)
        {
            switch (binding.TargetProperty)
            {
                case "Children":
                    ApplyListBinding(binding, GetInheritedBindingSource(), AddBoundControl, RemoveBoundControl);
                    return;
            }
            base.ApplyBinding(binding);
        }

        private void AddBoundControl(Binding binding, IList list, int index, object item)
        {
            if (ReferenceEquals(list, Children))
            {
                if (!(item is IControl))
                {
                    var bindingSource = item;
                    var type = binding.ContainsKey("Control") ? binding["Control"] : null;
                    if (type is IControl) item = type;
                    else if (type is Type) item = Activator.CreateInstance((Type) type);
                    else if (type is Func<object>) item = ((Func<object>) type).Invoke();
                    ((IControl) item).BindingSource = bindingSource;
                }
                this.Insert(index, (IControl) item);
            }
            else
            {
                list.Insert(index, item);
            }
        }

        private void RemoveBoundControl(Binding binding, IList list, object item)
        {
            if (ReferenceEquals(list, Children))
            {
                if (!(item is IControl)) item = Children.FirstOrDefault(x => x.BindingSource == item);
                // todo remove bound control
            }
            else
            {
                list.Remove(item);
            }
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