using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Providers.Processors
{
    // todo! visgroups
    [Export(typeof(IBspSourceProcessor))]
    public class HandleVisgroups : IBspSourceProcessor
    {
        public string OrderHint => "C";

        public Task AfterLoad(MapDocument document)
        {
            // hide objects in hidden visgroups
            // set up auto visgroups
            return Task.FromResult(0);
        }

        public Task BeforeSave(MapDocument document)
        {
            return Task.FromResult(0);
        }
    }
}