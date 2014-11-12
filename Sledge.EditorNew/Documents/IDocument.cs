using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Providers;
using Sledge.Providers.GameData;
using Sledge.Settings;

namespace Sledge.EditorNew.Documents
{
    public interface IDocument
    {
        void Activate();
        void Deactivate();
        void Close();
        string Text { get; }
    }
}
