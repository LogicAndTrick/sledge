using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Sledge.Gui.Gtk.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Structures;
using Binding = Sledge.Gui.Bindings.Binding;
using Padding = Sledge.Gui.Structures.Padding;

namespace Sledge.Gui.Gtk.Containers
{
    public abstract class GtkContainer : GtkControl, IContainer
    {
        protected Container Container;
        private Alignment _alignment;
        protected List<GtkControl> Children { get; private set; }
        protected Dictionary<GtkControl, ContainerMetadata> Metadata { get; private set; }

        public int NumChildren
        {
            get { return Children.Count; }
        }

        IEnumerable<IControl> IContainer.Children
        {
            get { return Children; }
        }

        public Padding Margin
        {
            get
            {
                return new Padding((int) _alignment.TopPadding, (int) _alignment.LeftPadding, (int) _alignment.BottomPadding, (int) _alignment.RightPadding);
                //Container.
                return new Padding();
                // return Control.Padding.ToPadding();
            }
            set
            {
                // Control.Padding = new System.Windows.Forms.Padding(value.Left, value.Top, value.Right, value.Bottom);
                _alignment.SetPadding((uint) value.Top, (uint) value.Bottom, (uint) value.Left, (uint) value.Right);
            }
        }

        protected GtkContainer() : this(new VBox(false, 0))
        {
        }

        protected GtkContainer(Container container) : base(new Alignment(0, 0, 1, 1))
        {
            _alignment = (Alignment) Control;
            _alignment.Add(container);
            container.Show();
            Container = container;
            Children = new List<GtkControl>();
            Metadata = new Dictionary<GtkControl, ContainerMetadata>();
            container.Added += ChildrenChanged;
            container.Removed += ChildrenChanged;
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
            var c = (GtkControl) child.Implementation;
            c.Parent = this;
            Metadata.Add(c, metadata);
            BindChildEvents(child);
            AppendChild(index, c);
            Children.Insert(index, c);
            CalculateLayout();
        }

        public void Remove(IControl child)
        {
            var c = (GtkControl) child.Implementation;
            Metadata.Remove(c);
            Children.Remove(c);
            RemoveChild(c);
            UnbindChildEvents(child);
            c.Parent = null;
            CalculateLayout();
        }

        protected virtual void AppendChild(int index, GtkControl child)
        {
            Container.Add(child.Control);
            child.Control.ShowAll();
        }

        protected virtual void RemoveChild(GtkControl child)
        {
            Container.Remove(child.Control);
        }

        protected virtual void BindChildEvents(IControl child)
        {
            child.PreferredSizeChanged += ChildPreferredSizeChanged;
            child.ActualSizeChanged += ChildActualSizeChanged;
        }

        protected virtual void UnbindChildEvents(IControl child)
        {
            child.PreferredSizeChanged -= ChildPreferredSizeChanged;
            child.ActualSizeChanged -= ChildActualSizeChanged;
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