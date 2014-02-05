using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;

namespace Sledge.Editor.UI.DockPanels
{
    public partial class OutputMessagePanel : UserControl, IMediatorListener
    {
        private readonly Dictionary<string, List<OutputWord>> _words;
        private string _currentType;

        public OutputMessagePanel()
        {
            _words = new Dictionary<string, List<OutputWord>>();

            InitializeComponent();

            OutputType.SelectedIndex = OutputType.Items.Count - 1;

            Mediator.Subscribe(EditorMediator.OutputMessage, this);
            Mediator.Subscribe(EditorMediator.CompileStarted, this);
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

        private void CompileStarted()
        {
            if (_words.ContainsKey("Compile")) _words["Compile"].Clear();
            if (_currentType == "Compile") OutputBox.Clear();
        }

        private void OutputMessage(string type, string text)
        {
            AddOutput(type, new OutputWord(text));
        }

        private void OutputMessage(string type, OutputWord word)
        {
            AddOutput(type, word);
        }

        private void OutputMessage(string type, List<OutputWord> words)
        {
            if (!_words.ContainsKey(type)) _words.Add(type, new List<OutputWord>());
            _words[type].AddRange(words);
            if (type == _currentType)
            {
                OutputBox.Select(OutputBox.TextLength, 0);
                foreach (var word in words)
                {
                    OutputBox.SelectionColor = word.GetColour();
                    OutputBox.AppendText(word.Text);
                }
                OutputBox.ScrollToCaret();
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

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        private void ClearButtonClicked(object sender, EventArgs e)
        {
            if (_words.ContainsKey(_currentType)) _words[_currentType].Clear();
            OutputBox.Clear();
        }
    }
}
