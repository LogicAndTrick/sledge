using System;
using Sledge.Common.Mediator;
using Sledge.Editor.UI.DockPanels;

namespace Sledge.Editor.Compiling
{
    public static class CompileLogTracer
    {
        public static void Add(string text)
        {
            Mediator.Publish(EditorMediator.OutputMessage, "Compile", text + "\r\n");
        }

        public static void AddLine(string line)
        {
            if (line == null) return;
            Mediator.Publish(EditorMediator.OutputMessage, "Compile", line + "\r\n");
        }

        public static void AddErrorLine(string line)
        {
            Mediator.Publish(EditorMediator.OutputMessage, "Compile", new OutputWord(line, ConsoleColor.Red));
        }
    }
}