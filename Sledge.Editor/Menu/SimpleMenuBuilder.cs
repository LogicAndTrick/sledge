using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sledge.Editor.Menu
{
    public class SimpleMenuBuilder : IMenuBuilder
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public object Parameter { get; set; }
        public Func<string> Text { get; set; }
        public Func<bool> IsVisible { get; set; }
        public Func<bool> IsActive { get; set; }
        public Func<bool> IsChecked { get; set; }
        public Image Image { get; set; }
        public bool ShowInMenu { get; set; }
        public bool ShowInToolStrip { get; set; }

        public SimpleMenuBuilder(string name, string message, object parameter = null)
        {
            Name = name;
            Message = message;
            IsVisible = IsActive = null;
            Parameter = parameter;
            ShowInMenu = true;
        }

        public SimpleMenuBuilder(string name, Enum message, object parameter = null)
        {
            Name = name;
            Message = message.ToString();
            IsVisible = IsActive = null;
            Parameter = parameter;
            ShowInMenu = true;
        }

        public IEnumerable<ToolStripItem> Build()
        {
            //if (IsVisible != null && !IsVisible()) yield break;
            yield return new UpdatingToolStripMenuItem(Name, Image, CombineActions(IsVisible, IsActive), IsChecked, Text, Message, Parameter);
        }

        public IEnumerable<ToolStripItem> BuildToolStrip()
        {
            //if (IsVisible != null && !IsVisible()) yield break;
            yield return new UpdatingToolStripButton(Name, Image, CombineActions(IsVisible, IsActive), IsChecked, Text, Message, Parameter);
        }

        private Func<bool> CombineActions(Func<bool> one, Func<bool> two)
        {
            return () => (one == null && two == null) || ((one == null || one()) && (two == null || two()));
        }
    }
}