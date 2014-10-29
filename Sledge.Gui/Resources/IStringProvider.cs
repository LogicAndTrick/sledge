using System.Linq;
using System.Text;
using Sledge.Settings;

namespace Sledge.Gui.Resources
{
    public interface IStringProvider
    {
        string Fetch(string key);
    }
}
