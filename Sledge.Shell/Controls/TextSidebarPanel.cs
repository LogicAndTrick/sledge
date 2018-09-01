using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;

namespace Sledge.Shell.Controls
{
    public abstract partial class TextSidebarPanel : UserControl, ISidebarComponent
    {
        public abstract string Title { get; }
        public abstract string Text { get; }
        public object Control => this;

        protected TextSidebarPanel()
        {
            InitializeComponent();
            UpdateText();
        }

        public abstract bool IsInContext(IContext context);

        protected void UpdateText()
        {
            var text = Text ?? "";
            HelpTextBox.ResetFont();
            var rtf = ConvertSimpleMarkdownToRtf(text);
            HelpTextBox.Rtf = rtf;
            var size = TextRenderer.MeasureText(HelpTextBox.Text, HelpTextBox.Font, HelpTextBox.Size, TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            Height = size.Height + HelpTextBox.Margin.Vertical + HelpTextBox.Lines.Length * 5;
        }

        /// <summary>
        /// Converts simple markdown into RTF.
        /// Simple markdown is a very limited subset of markdown. It supports:
        /// - Lists, delimited with -
        /// - Bold, delimited with *
        /// - Paragraphs/new lines
        /// </summary>
        /// <param name="simpleMarkdown"></param>
        private string ConvertSimpleMarkdownToRtf(string simpleMarkdown)
        {
            /*
             * {\rtf1\utf8\f0\pard
             *   This is some {\b bold} text.\par
             * }";
             */
            var escaped = simpleMarkdown
                .Replace("\\", "\\\\")
                .Replace("{", "\\{")
                .Replace("}", "\\}");

            var sb = new StringBuilder();
            foreach (var c in escaped)
            {
                if (c > 127) sb.AppendFormat(@"\u{0}?", (int) c);
                else if (c == '\\') sb.Append("\\\\");
                else if (c == '{') sb.Append("\\{");
                else if (c == '}') sb.Append("\\}");
                else sb.Append(c);
            }

            var bolded = Regex.Replace(sb.ToString(), @"\*(?:\b(?=\w)|(?=\\))(.*?)\b(?!\w)\*", @"{\b $1}");
            var bulleted = Regex.Replace(bolded, @"^\s*-\s+", @" \bullet  ", RegexOptions.Multiline);
            var paragraphs = Regex.Replace(bulleted, @"(\r?\n){2,}", "\\par\\par ");
            var lines = Regex.Replace(paragraphs, @"(\r?\n)+", "\\par ");

            return @"{\rtf1\ansi\f0\pard\sa60 " + lines + " }";
        }
    }
}
