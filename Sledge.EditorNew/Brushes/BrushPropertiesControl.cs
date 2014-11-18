using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.EditorNew.Brushes.Controls;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Models;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.Brushes
{
    public class BrushPropertiesControl : VerticalScrollContainer
    {
        private bool _roundVertices;

        public delegate void BrushEventHandler(IBrush brush);

        public event BrushEventHandler ValuesChanged;
        public event BrushEventHandler BrushSelected;

        private void OnValuesChanged()
        {
            if (ValuesChanged != null)
            {
                ValuesChanged(CurrentBrush);
            }
        }

        private void OnBrushSelected()
        {
            if (BrushSelected != null)
            {
                BrushSelected(CurrentBrush);
            }
        }

        public IBrush CurrentBrush
        {
            get
            {
                var sel = _list.SelectedItem;
                return sel == null ? null : sel.Value as IBrush;
            }
        }

        public bool RoundVertices
        {
            get { return _roundVertices; }
            set
            {
                _roundVertices = value;
                OnValuesChanged();
            }
        }

        private readonly VerticalBox _container;
        private readonly IComboBox _list;
        private readonly ICheckbox _roundVerticesCheckbox;

        public BrushPropertiesControl()
        {
            _container = new VerticalBox{Margin = new Padding(5,5,5,5)};
            _roundVertices = true;

            _list = new ComboBox();
            foreach (var brush in BrushManager.Brushes)
            {
                _list.Items.Add(new ComboBoxItem{ Text = brush.Name, Value = brush });
            }
            if (_list.Items.Any()) _list.SelectedIndex = 0;
            BrushManager.BrushAdded += brush =>
            {
                _list.Items.Add(new ComboBoxItem {Text = brush.Name, Value = brush});
                if (_list.SelectedIndex < 0 && _list.Items.Any()) _list.SelectedIndex = 0;
            };
            _list.SelectedItemChanged += UpdateSelectedBrush;

            var listLabel = new Label{TextKey = "Tools/BrushTool/Controls/BrushType"};
            _roundVerticesCheckbox = new CheckBox {TextKey = "Tools/BrushTool/Controls/RoundVertices", Checked = RoundVertices};

            var listBox = new HorizontalBox{Tag = true};
            listBox.Add(listLabel, false);
            listBox.Add(_list, true);
            _container.Add(listBox);

            var roundBox = new HorizontalBox {Tag = true};
            roundBox.Add(_roundVerticesCheckbox, true);
            _container.Add(roundBox);

            _roundVerticesCheckbox.Toggled += (s, e) => RoundVertices = _roundVerticesCheckbox.Checked;

            this.Set(_container);
        }

        private void UpdateSelectedBrush(object sender, EventArgs eventArgs)
        {
            _container.StartUpdate();
            
            foreach (var child in _container.Children.OfType<BrushControl>().ToList())
            {
                child.ValuesChanged -= ControlValuesChanged;
                _container.Remove(child);
            }

            var cur = CurrentBrush;
            if (cur != null)
            {
                _roundVerticesCheckbox.Enabled = cur.CanRound;
                foreach (var control in cur.GetControls())
                {
                    _container.Add(control);
                    control.ValuesChanged += ControlValuesChanged;
                }
            }

            OnValuesChanged();
            _container.EndUpdate();
        }

        private void ControlValuesChanged(object sender, IBrush brush)
        {
            OnValuesChanged();
        }
    }
}