using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Components
{
    public interface IMapDocumentControl : IDisposable
    {
        string Type { get; }
        Control Control { get; }
        string GetSerialisedSettings();
        void SetSerialisedSettings(string settings);
    }
}