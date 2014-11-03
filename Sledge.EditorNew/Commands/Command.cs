using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.EditorNew.Language;

namespace Sledge.EditorNew.Commands
{
    public interface ICommand
    {
        string Group { get; }
        string Identifier { get; }
        string TextKey { get; }
        string Context { get; }
        void Fire();
    }
}
