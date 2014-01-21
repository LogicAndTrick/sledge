using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI.DockPanels
{
    public partial class OutputMessagePanel : UserControl
    {
        private readonly Dictionary<string, List<OutputWord>> _words;
        private string _currentType;

        public OutputMessagePanel()
        {
            _words = new Dictionary<string, List<OutputWord>>();

            InitializeComponent();

            OutputType.SelectedIndex = OutputType.Items.Count - 1;

            AddOutput("Debug", new OutputWord { Colour = ConsoleColor.Black, Text = "Test1 " });
            AddOutput("Debug", new OutputWord { Colour = ConsoleColor.Red, Text = "Test2\n" });
            AddOutput("Debug", new OutputWord { Colour = ConsoleColor.DarkGreen, Text = "Test3 - " });
            AddOutput("Debug", new OutputWord { Colour = ConsoleColor.Magenta, Text = "Test4\n\n" });
            AddOutput("Debug", new OutputWord { Colour = ConsoleColor.Cyan, Text = "Test5" });
        }

        private void OutputTypeChanged(object sender, EventArgs e)
        {
            var type = (string) OutputType.SelectedItem;
            if (type != _currentType)
            {
                OutputBox.Clear();
                _currentType = type;
                if (_words.ContainsKey(_currentType))
                {
                    foreach (var word in _words[_currentType])
                    {
                        OutputBox.SelectionColor = word.GetColour();
                        OutputBox.AppendText(word.Text);
                    }
                }
            }
        }

        public void AddOutput(string type, OutputWord word)
        {
            if (!_words.ContainsKey(type)) _words.Add(type, new List<OutputWord>());
            _words[type].Add(word);
            if (type == _currentType)
            {
                OutputBox.Select(OutputBox.TextLength, 0);
                OutputBox.SelectionColor = word.GetColour();
                OutputBox.AppendText(word.Text);
                OutputBox.ScrollToCaret();
            }
        }
    }
}
