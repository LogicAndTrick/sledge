using System;
using System.Collections.Generic;
using Sledge.Gui.Bindings;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Controls
{
    public abstract class ControlBase<T> : IControl where T : IControl
    {
        internal T Control { get; private set; }

        public IContainer Parent
        {
            get { return Control.Parent; }
        }

        public IControl Implementation
        {
            get { return Control; }
        }

        protected ControlBase()
        {
            Control = CreateControl();
        }

        protected virtual T CreateControl()
        {
            return UIManager.Manager.Construct<T>();
        }

        public object BindingSource
        {
            get { return Control.BindingSource; }
            set { Control.BindingSource = value; }
        }

        public Binding Bind(string property, string sourceProperty, BindingDirection direction = BindingDirection.Auto, Dictionary<string, object> meta = null)
        {
            return Control.Bind(property, sourceProperty, direction, meta);
        }

        public void UnbindAll()
        {
            Control.UnbindAll();
        }

        public void Unbind(string property)
        {
            Control.Unbind(property);
        }

        public bool Enabled
        {
            get { return Control.Enabled; }
            set { Control.Enabled = value; }
        }

        public bool Focused
        {
            get { return Control.Focused; }
        }

        public Size ActualSize
        {
            get { return Control.ActualSize; }
        }

        public Size PreferredSize
        {
            get { return Control.PreferredSize; }
            set { Control.PreferredSize = value; }
        }

        public event EventHandler ActualSizeChanged
        {
            add { Control.ActualSizeChanged += value; }
            remove { Control.ActualSizeChanged -= value; }
        }

        public event EventHandler PreferredSizeChanged
        {
            add { Control.PreferredSizeChanged += value; }
            remove { Control.PreferredSizeChanged -= value; }
        }

        public event MouseEventHandler MouseDown
        {
            add { Control.MouseDown += value; }
            remove { Control.MouseDown -= value; }
        }

        public event MouseEventHandler MouseUp
        {
            add { Control.MouseUp += value; }
            remove { Control.MouseUp -= value; }
        }

        public event MouseEventHandler MouseWheel
        {
            add { Control.MouseWheel += value; }
            remove { Control.MouseWheel -= value; }
        }

        public event MouseEventHandler MouseMove
        {
            add { Control.MouseMove += value; }
            remove { Control.MouseMove -= value; }
        }

        public event MouseEventHandler MouseClick
        {
            add { Control.MouseClick += value; }
            remove { Control.MouseClick -= value; }
        }

        public event EventHandler MouseDoubleClick
        {
            add { Control.MouseDoubleClick += value; }
            remove { Control.MouseDoubleClick -= value; }
        }

        public event EventHandler MouseEnter
        {
            add { Control.MouseEnter += value; }
            remove { Control.MouseEnter -= value; }
        }

        public event EventHandler MouseLeave
        {
            add { Control.MouseLeave += value; }
            remove { Control.MouseLeave -= value; }
        }
    }
}
