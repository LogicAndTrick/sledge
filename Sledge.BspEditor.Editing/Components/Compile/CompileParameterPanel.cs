using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Editing.Components.Compile.Specification;
using Sledge.Common;
using Sledge.Common.Extensions;

namespace Sledge.BspEditor.Editing.Components.Compile
{
    public partial class CompileParameterPanel : UserControl
    {
        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null && !_pauseEvents)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        private List<ParameterTogglePanel> _panels;
        private bool _pauseEvents;

        public CompileParameterPanel()
        {
            InitializeComponent();
            _panels = new List<ParameterTogglePanel>();
            DescriptionLabel.Text = "";
        }

        public void SetDescription(string val)
        {
            DescriptionLabel.Text = val;
        }

        public void ClearParameters()
        {
            _pauseEvents = true;
            _panels.Clear();
            FlowPanel.Controls.Clear();
            AdditionalParametersCheckbox.Checked = false;
            AdditionalParametersTextbox.Text = "";
            GeneratedParametersTextbox.Text = "";
            HoverTip.RemoveAll();
            _pauseEvents = false;
        }

        public void AddParameters(IEnumerable<CompileParameter> parameters)
        {
            _pauseEvents = true;
            foreach (var cp in parameters)
            {
                var fp = new ParameterTogglePanel(cp);
                fp.ValueChanged += ToggleParameter;
                FlowPanel.Controls.Add(fp);

                _panels.Add(fp);
                HoverTip.SetToolTip(fp.CheckBox, cp.Description);
            }
            _pauseEvents = false;
            UpdateGeneratedCommand();
        }

        private void UpdateGeneratedCommand()
        {
            _pauseEvents = true;
            GeneratedParametersTextbox.Text = String.Join(" ", _panels.Where(x => x.CheckBox.Checked).Select(x => x.GetValue()));
            _pauseEvents = false;
        }

        private void ToggleParameter(object sender, EventArgs e)
        {
            UpdateGeneratedCommand();
            OnValueChanged();
        }

        public string GeneratedCommands
        {
            get
            {
                return GeneratedParametersTextbox.Text;
            }
        }

        public void SetCommands(string generated, string additional)
        {
            _pauseEvents = true;
            var split = generated.SplitWithQuotes().ToList();
            _panels.ForEach(x => x.Clear());
            while (split.Any())
            {
                foreach (var panel in _panels)
                {
                    if (panel.TrySetValue(split)) break;
                }
                split.RemoveAt(0);
            }

            AdditionalParametersTextbox.Text = additional;
            AdditionalParametersCheckbox.Checked = !String.IsNullOrWhiteSpace(additional);
            _pauseEvents = false;

            UpdateGeneratedCommand();
            OnValueChanged();
        }

        public string AdditionalCommands
        {
            get { return AdditionalParametersCheckbox.Checked ? AdditionalParametersTextbox.Text : ""; }
        }

        private class ParameterTogglePanel : TableLayoutPanel
        {
            public CompileParameter Parameter { get; private set; }
            public CheckBox CheckBox { get; private set; }
            private Control _valueControl;

            public event EventHandler ValueChanged;

            protected virtual void OnValueChanged()
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }

            public ParameterTogglePanel(CompileParameter cp)
            {
                AutoSize = true;
                RowCount = 1;
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
                Margin = new Padding(0, 0, 10, 0);
                Parameter = cp;
                CheckBox = new CheckBox
                {
                    Text = cp.Name,
                    Checked = cp.Enabled,
                    Width = 135,
                    Height = 17,
                    Margin = new Padding(3, 3, 0, 3)
                };
                CheckBox.CheckedChanged += (s,e) => OnValueChanged();
                Controls.Add(CheckBox);
                AddControls();
            }

            private void AddControls()
            {
                var controls = new List<Control>();
                switch (Parameter.Type)
                {
                    case CompileParameterType.Checkbox:
                        break;
                    case CompileParameterType.String:
                        var text = new TextBox
                        {
                            Width = 100,
                            Text = Parameter.Value,
                            Margin = new Padding(0)
                        };
                        text.TextChanged += (s, e) => OnValueChanged();
                        controls.Add(text);
                        break;
                    case CompileParameterType.Decimal:
                        var nud = new NumericUpDown
                        {
                            Width = 60,
                            Minimum = Parameter.Min,
                            Maximum = Parameter.Max,
                            Value = Parameter.DecimalValue,
                            DecimalPlaces = Parameter.Precision,
                            Margin = new Padding(0),
                            Increment = (decimal) Math.Pow(10, -Parameter.Precision)
                        };
                        nud.ValueChanged += (s, e) => OnValueChanged();
                        controls.Add(nud);
                        break;
                    case CompileParameterType.Choice:
                        var combo = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            Margin = new Padding(0),
                            Width = 100
                        };
                        foreach (var option in Parameter.Options) combo.Items.Add(option);
                        combo.SelectedItem = Parameter.ChoiceValue;
                        combo.SelectedIndexChanged += (s, e) => OnValueChanged();
                        controls.Add(combo);
                        break;
                    case CompileParameterType.File:
                        var file = new TextBox
                        {
                            Text = Parameter.Value,
                            Tag = "File",
                            Margin = new Padding(0),
                            Width = 120
                        };
                        file.TextChanged += (s, e) => OnValueChanged();
                        controls.Add(file);
                        var fileButton = new Button
                        {
                            Text = "...",
                            Width = 25,
                            Height = 20,
                            TextAlign = ContentAlignment.TopCenter,
                            Margin = new Padding(0)
                        };
                        fileButton.Click += (s, e) =>
                        {
                            using (var fo = new OpenFileDialog{Filter = Parameter.Filter})
                            {
                                if (fo.ShowDialog() == DialogResult.OK)
                                {
                                    file.Text = fo.FileName;
                                }
                            }
                        };
                        controls.Add(fileButton);
                        break;
                    case CompileParameterType.Folder:
                        var folder = new TextBox
                        {
                            Text = Parameter.Value,
                            Tag = "Folder",
                            Margin = new Padding(0),
                            Width = 120
                        };
                        folder.TextChanged += (s, e) => OnValueChanged();
                        controls.Add(folder);
                        var folderButton = new Button
                        {
                            Text = "...",
                            Width = 25,
                            Height = 20,
                            TextAlign = ContentAlignment.TopCenter,
                            Margin = new Padding(0)
                        };
                        folderButton.Click += (s, e) =>
                        {
                            using (var fo = new FolderBrowserDialog())
                            {
                                if (fo.ShowDialog() == DialogResult.OK)
                                {
                                    folder.Text = fo.SelectedPath;
                                }
                            }
                        };
                        controls.Add(folderButton);
                        break;
                    case CompileParameterType.Colour:
                    case CompileParameterType.ColourFloat:
                        var cpanel = new Panel
                        {
                            Width = 40,
                            Height = 20,
                            Margin = new Padding(0),
                            BackColor = Parameter.ColourValue,
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        cpanel.Click += (s, e) =>
                        {
                            using (var cp = new ColorDialog{Color = cpanel.BackColor})
                            {
                                if (cp.ShowDialog() == DialogResult.OK)
                                {
                                    cpanel.BackColor = cp.Color;
                                    OnValueChanged();
                                }
                            }
                        };
                        controls.Add(cpanel);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                foreach (var control in controls)
                {
                    ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                    Controls.Add(control);
                }
            }

            private string ExtractValue()
            {
                switch (Parameter.Type)
                {
                    case CompileParameterType.String:
                        var text = Controls.OfType<TextBox>().FirstOrDefault();
                        return text == null ? "" : text.Text;
                    case CompileParameterType.Decimal:
                        var nud = Controls.OfType<NumericUpDown>().FirstOrDefault();
                        return nud == null ? "" : nud.Value.ToString(CultureInfo.InvariantCulture);
                    case CompileParameterType.Choice:
                        var combo = Controls.OfType<ComboBox>().FirstOrDefault();
                        if (combo != null && combo.SelectedIndex >= 0)
                        {
                            var idx = combo.SelectedIndex;
                            var list = Parameter.Options.Count == Parameter.OptionValues.Count ? Parameter.OptionValues : Parameter.Options;
                            if (idx >= 0 && idx < list.Count)
                            {
                                return list[idx];
                            }
                        }
                        return "";
                    case CompileParameterType.File:
                        var file = Controls.OfType<TextBox>().FirstOrDefault(x => String.Equals(x.Tag as string, "File"));
                        return '"' + (file == null ? "" : file.Text) + '"';
                    case CompileParameterType.Folder:
                        var folder = Controls.OfType<TextBox>().FirstOrDefault(x => String.Equals(x.Tag as string, "Folder"));
                        return '"' + (folder == null ? "" : folder.Text) + '"';
                    case CompileParameterType.Colour:
                    case CompileParameterType.ColourFloat:
                        var c = Color.Black;
                        var colour = Controls.OfType<Panel>().FirstOrDefault();
                        if (colour != null) c = colour.BackColor;
                        return String.Join(" ",
                            new[] {c.R, c.G, c.B}.Select(x => Parameter.Type == CompileParameterType.ColourFloat
                                ? (x / 255f).ToString("0.##")
                                : x.ToString(CultureInfo.InvariantCulture)));
                    default:
                        return "";
                }
            }

            private void SetValue(string val)
            {
                switch (Parameter.Type)
                {
                    case CompileParameterType.String:
                        var text = Controls.OfType<TextBox>().FirstOrDefault();
                        if (text != null) text.Text = val;
                        break;
                    case CompileParameterType.Decimal:
                        var nud = Controls.OfType<NumericUpDown>().FirstOrDefault();
                        if (nud == null) break;
                        decimal d;
                        nud.Value = Decimal.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0;
                        break;
                    case CompileParameterType.Choice:
                        var combo = Controls.OfType<ComboBox>().FirstOrDefault();
                        var list = Parameter.Options.Count == Parameter.OptionValues.Count ? Parameter.OptionValues : Parameter.Options;
                        if (combo != null) combo.SelectedIndex = Math.Max(0, list.FindIndex(x => String.Equals(x, val, StringComparison.InvariantCultureIgnoreCase)));
                        break;
                    case CompileParameterType.File:
                        var file = Controls.OfType<TextBox>().FirstOrDefault(x => String.Equals(x.Tag as string, "File"));
                        if (file != null) file.Text = val;
                        break;
                    case CompileParameterType.Folder:
                        var folder = Controls.OfType<TextBox>().FirstOrDefault(x => String.Equals(x.Tag as string, "Folder"));
                        if (folder != null) folder.Text = val;
                        break;
                    case CompileParameterType.Colour:
                    case CompileParameterType.ColourFloat:
                        var colour = Controls.OfType<Panel>().FirstOrDefault();
                        if (colour == null) break;
                        var spl = (val ?? "").Split(' ');
                        float r, g, b;
                        var c = Color.Black;
                        if (spl.Length == 3 && float.TryParse(spl[0], out r) && float.TryParse(spl[1], out g) && float.TryParse(spl[2], out b))
                        {
                            if (Parameter.Type == CompileParameterType.ColourFloat)
                            {
                                r *= 255;
                                g *= 255;
                                b *= 255;
                            }
                            c = Color.FromArgb((int) r, (int) g, (int) b);
                        }
                        colour.BackColor = c;
                        break;
                }
            }

            public string GetValue()
            {
                var ex = ExtractValue();
                return Parameter.Flag + (String.IsNullOrWhiteSpace(ex) ? "" : " " + ex);
            }

            public bool TrySetValue(List<string> data)
            {
                if (!data.Any()) return false;
                if (data[0] != Parameter.Flag) return false;

                CheckBox.Checked = true;
                switch (Parameter.Type)
                {
                    case CompileParameterType.String:
                    case CompileParameterType.Decimal:
                    case CompileParameterType.Choice:
                    case CompileParameterType.File:
                    case CompileParameterType.Folder:
                        if (data.Count > 1) SetValue(data[1]);
                        break;
                    case CompileParameterType.Colour:
                    case CompileParameterType.ColourFloat:
                        if (data.Count > 3) SetValue(data[1] + ' ' + data[2] + ' ' + data[3]);
                        break;
                }
                return true;
            }

            public void Clear()
            {
                CheckBox.Checked = false;
                SetValue(Parameter.Value);
            }
        }

        private void AdditionalParametersChanged(object sender, EventArgs e)
        {
            AdditionalParametersTextbox.Enabled = AdditionalParametersCheckbox.Checked;
        }

        private void AdditionalParametersTextboxChanged(object sender, EventArgs e)
        {
            if (AdditionalParametersCheckbox.Enabled)
            {
                OnValueChanged();
            }
        }
    }
}
