using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Sledge.Common.Mediator;
using Sledge.Editor.UI.DockPanels;

namespace Sledge.Editor.Compiling
{
    public static class BatchCompiler
    {
        public static void Compile(Batch batch)
        {
            /*
            var script = new StringBuilder();
            script.AppendLine(batch.BeforeExecute);
            foreach (var step in batch.Steps)
            {
                script.AppendLine(batch.BeforeExecuteStep);
                script.AppendLine(step.BeforeExecute);
                var cq = step.SystemCommand ? "" : "\"";
                script.Append(cq).Append(step.Operation).Append(cq).Append(' ').AppendLine(step.Flags);
                script.AppendLine(step.AfterExecute);
                script.AppendLine(batch.AfterExecuteStep);
            }
            script.AppendLine(batch.AfterExecute);
            var batchFile = Path.ChangeExtension(batch.TargetFile, "bat");
            File.WriteAllText(batchFile, script.ToString());
            Process.Start(batchFile);*/

            Task.Factory.StartNew(() => DoCompile(batch));
        }

        private static void DoCompile(Batch batch)
        {
            foreach (var step in batch.Steps)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(step.SystemCommand ? "cmd.exe" : step.Operation, step.SystemCommand ? step.Operation + " " + step.Flags : step.Flags)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WorkingDirectory = Path.GetDirectoryName(batch.TargetFile)
                    }
                };
                var logger = new CompileLogTracer();
                process.OutputDataReceived += (sender, args) => logger.AddLine(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }
    }

    public class CompileLogTracer
    {
        private static Regex _settingsStart = new Regex(@"Current hl(csg|bsp|vis|rad) settings", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _settingsLine = new Regex(@"(.*?)(\[)([^]]+)(\]\s*\[)([^]]+)(\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private ConsoleColor _current;
        private LogState _state;
        private List<OutputWord> _words;

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

        private void Emit(string text, ConsoleColor colour = ConsoleColor.Black)
        {
            _words.Add(new OutputWord(text, colour));
        }

        private void StateLineStart(string line)
        {
            switch (_state)
            {
                case LogState.None:
                    if (_settingsStart.IsMatch(line)) _state = LogState.Settings;
                    break;
                case LogState.Startup:
                    break;
                case LogState.Settings:
                    if (!_settingsLine.IsMatch(line)
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
            var spl = _settingsLine.Split(line);
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
