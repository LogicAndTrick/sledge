using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Components
{
    public interface IMapDocumentControl : IDisposable
    {
        Control Control { get; }
        Task<string> GetSerialisedSettings();
        Task SetSerialisedSettings(string settings);
    }
}