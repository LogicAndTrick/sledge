using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class RightClickMenuBuilder
    {
        public ViewportEvent Event { get; }
        public MapViewport Viewport { get; }
        public bool Intercepted { get; set; }
        private List<ToolStripItem> Items { get; }
        public bool IsEmpty => Items.Count == 0;

        public RightClickMenuBuilder(MapViewport viewport, ViewportEvent viewportEvent)
        {
            Event = viewportEvent;
            Viewport = viewport;
            Items = new List<ToolStripItem>
            {
                new CommandItem("BspEditor:Edit:Paste"),
                new CommandItem("BspEditor:Edit:PasteSpecial"),
                new ToolStripSeparator(),
                new CommandItem("BspEditor:Edit:Undo"),
                new CommandItem("BspEditor:Edit:Redo")
            };
        }

        public ToolStripMenuItem CreateCommandItem(string commandId, object parameters = null)
        {
            return new CommandItem(commandId, parameters);
        }

        public ToolStripMenuItem AddCommand(string commandId, object parameters = null)
        {
            var mi = CreateCommandItem(commandId, parameters);
            Items.Add(mi);
            return mi;
        }

        public ToolStripMenuItem AddCallback(string description, Action callback)
        {
            var mi = new ToolStripMenuItem(description);
            mi.Click += (s, e) => callback();
            Items.Add(mi);
            return mi;
        }

        public ToolStripSeparator AddSeparator()
        {
            var mi = new ToolStripSeparator();
            Items.Add(mi);
            return mi;
        }

        public ToolStripMenuItem AddGroup(string description)
        {
            var g = new ToolStripMenuItem(description);
            Items.Add(g);
            return g;
        }

        public void Add(params ToolStripItem[] items)
        {
            Items.AddRange(items);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public void Populate(ContextMenuStrip menu)
        {
            menu.Items.Clear();
            foreach (var command in Items)
            {
                menu.Items.Add(command);
            }
        }

        private class CommandItem : ToolStripMenuItem
        {
            private readonly string _commandID;
            private readonly object _parameters;

            public CommandItem(string commandID, object parameters = null)
            {
                _commandID = commandID;
                _parameters = parameters;
                Click += RunCommand;

                var register = Common.Container.Get<Shell.Registers.CommandRegister>();
                var cmd = register.Get(_commandID);
                Text = cmd == null ? _commandID : cmd.Name;
            }

            private void RunCommand(object sender, EventArgs e)
            {
                Oy.Publish("Command:Run", new CommandMessage(_commandID, _parameters));
            }
        }
    }
}