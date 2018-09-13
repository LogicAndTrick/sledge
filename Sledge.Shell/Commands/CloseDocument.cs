using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// Internal: Close a document
    /// </summary>
    [Export(typeof(ICommand))]
    [CommandID("Internal:CloseDocument")]
    [InternalCommand]
    public class CloseDocument : ICommand
    {
        public string Name { get; set; } = "Close";
        public string Details { get; set; } = "Close";

        public bool IsInContext(IContext context)
        {
            return true;
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            var doc = parameters.Get<IDocument>("Document");
            if (doc != null)
            {
                await Oy.Publish("Document:CloseAndPrompt", doc);
            }
        }
    }
}