using System.Threading.Tasks;

namespace Sledge.Shell.Commands
{
    internal interface IActivator
    {
        string Group { get; }
        string Name { get; }
        string Description { get; }
        Task Activate();
    }
}
