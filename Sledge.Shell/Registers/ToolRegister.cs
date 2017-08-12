using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The tool register controls tools
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class ToolRegister : IStartupHook
    {
        // The tool register needs direct access to the shell
        [Import] private Forms.Shell _shell;
        [ImportMany] private IEnumerable<Lazy<ITool>> _tools;

        public async Task OnStartup()
        {
            // Register the exported tools
            foreach (var export in _tools.OrderBy(x => OrderHintAttribute.GetOrderHint(x.Value.GetType())))
            {
                Log.Debug(nameof(ToolRegister), "Loaded: " + export.Value.GetType().FullName);
                _components.Add(export.Value);
            }

            // Subscribe to context changes
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);

            Oy.Subscribe<string>("ActivateTool", async x =>
            {
                await ActivateTool(_components.FirstOrDefault(t => t.Name == x));
            });
            Oy.Subscribe<ITool>("ActivateTool", ActivateTool);
        }

        private readonly List<ITool> _components;

        public ToolRegister()
        {
            _components = new List<ITool>();
        }

        private async Task ContextChanged(IContext context)
        {
            var activeTool = context.Get<ITool>("ActiveTool");
            var toolsInContext = _components.Where(x => x.IsInContext(context)).ToList();

            // If there are any tools available, or the active tool exists
            // And if the active tool isn't in context
            // Then activate the first available tool
            if ((toolsInContext.Any() || activeTool != null) && !toolsInContext.Contains(activeTool))
            {
                // This will change the context anyway so return immediately
                activeTool = toolsInContext.FirstOrDefault();
                await ActivateTool(activeTool);
                return;
            }

            _shell.Invoke((MethodInvoker)delegate
            {
                _shell.ToolsContainer.SuspendLayout();
                _shell.ToolsContainer.Items.Clear();
                foreach (var tl in toolsInContext)
                {
                    var toolButton = new ToolStripButton("", tl.Icon, async (s, ea) => await ActivateTool(tl), tl.Name)
                    {
                        Checked = tl == activeTool,
                        ToolTipText = tl.Name,
                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                        ImageScaling = ToolStripItemImageScaling.None,
                        AutoSize = false,
                        Width = 36,
                        Height = 36
                    };
                    _shell.ToolsContainer.Items.Add(toolButton);
                }
                _shell.ToolsContainer.ResumeLayout();
            });
        }

        private async Task ActivateTool(ITool tool)
        {
            await Task.WhenAll(
                Oy.Publish("Tool:Activated", tool),
                Oy.Publish(tool == null ? "Context:Remove" : "Context:Add", new ContextInfo("ActiveTool", tool))
            );
        }
    }
}
