using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public virtual bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public abstract string Name { get; }
        public abstract string Details { get; }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument doc))
            {
                await Invoke(doc, parameters);
            }
        }

        protected abstract Task Invoke(MapDocument document, CommandParameters parameters);
    }
}