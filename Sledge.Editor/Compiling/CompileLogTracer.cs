using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sledge.Common.Mediator;
using Sledge.Editor.UI.DockPanels;

namespace Sledge.Editor.Compiling
{
    public class CompileLogTracer
    {
        private static readonly Regex SettingsStart = new Regex(@"Current hl(csg|bsp|vis|rad) settings", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SettingsLine = new Regex(@"(.*?)(\[)([^]]+)(\]\s*\[)([^]]+)(\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private LogState _state;
        private readonly List<OutputWord> _words;

        private enum LogState
        {
            None,
            Startup,
            Settings
        }

        public CompileLogTracer()
        {
            _state = LogState.Startup;
            _words = new List<OutputWord>();
        }

        public void AddLine(string line)
        {
            _words.Clear();
            line += "\r\n";
            StateLineStart(line);
            switch (_state)
            {
                case LogState.None:
                    LineNone(line);
                    break;
                case LogState.Startup:
                    LineStartup(line);
                    break;
                case LogState.Settings:
                    LineSettings(line);
                    break;
            }
            StateLineEnd(line);
            Mediator.Publish(EditorMediator.OutputMessage, "Compile", _words);
        }

        public void AddErrorLine(string line)
        {
            Mediator.Publish(EditorMediator.OutputMessage, "Compile", new OutputWord(line, ConsoleColor.Red));
        }

        private void Emit(string text, ConsoleColor colour = ConsoleColor.Black)
        {
            _words.Add(new OutputWord(text, colour));
        }

        private void StateLineStart(string line)
        {
            switch (_state)
            {
                case LogState.None:
                    if (SettingsStart.IsMatch(line)) _state = LogState.Settings;
                    break;
                case LogState.Startup:
                    break;
                case LogState.Settings:
                    if (!SettingsLine.IsMatch(line)
                        && !line.StartsWith("Current")
                        && !line.StartsWith("Name")
                        && !line.StartsWith("-----")
                        && !String.IsNullOrWhiteSpace(line)
                        && !line.StartsWith("custom shadows with bounce light"))
                    {
                        _state = LogState.None;
                    }
                    break;
            }
        }

        private void StateLineEnd(string line)
        {
            switch (_state)
            {
                case LogState.None:

                    break;
                case LogState.Startup:
                    break;
                case LogState.Settings:
                    break;
            }
        }

        private void LineNone(string line)
        {
            Emit(line, ConsoleColor.Black);
        }

        private void LineSettings(string line)
        {
            var spl = SettingsLine.Split(line);
            if (spl.Length == 8)
            {
                Emit(spl[1], ConsoleColor.Black);
                Emit(spl[2], ConsoleColor.DarkGray);
                Emit(spl[3], ConsoleColor.Blue);
                Emit(spl[4], ConsoleColor.DarkGray);
                Emit(spl[5], ConsoleColor.Blue);
                Emit(spl[6] + spl[7], ConsoleColor.DarkGray);
            }
            else if (line.StartsWith("Name"))
            {
                spl = line.Split('|');
                for (int i = 0; i < spl.Length; i++)
                {
                    if (i > 0) Emit("|", ConsoleColor.DarkGray);
                    Emit(spl[i], ConsoleColor.Black);
                }
            }
            else
            {
                Emit(line, line.StartsWith("---") ? ConsoleColor.DarkGray : ConsoleColor.Black);
            }
        }

        private void LineStartup(string line)
        {
            if (line.StartsWith("----- "))
            {
                Emit(line, ConsoleColor.Gray);
                _state = LogState.None;
            }
            else
            {
                Emit(line, ConsoleColor.Green);
            }
        }

        private static ConsoleColor DetectColour(string text)
        {
            if (text.StartsWith("----- ")) return ConsoleColor.Gray;
            if (text.StartsWith("Warning")) return ConsoleColor.DarkYellow;
            if (text.StartsWith("Error")) return ConsoleColor.Red;
            if (text.StartsWith("=== ")) return ConsoleColor.Blue;
            if (text.StartsWith(">> ")) return ConsoleColor.Magenta;
            return ConsoleColor.Black;
        }
    }
}