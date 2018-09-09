using System.Collections.Generic;

namespace Sledge.BspEditor.Components
{
    public interface IMapDocumentControlFactory
    {
        string Type { get; }
        IMapDocumentControl Create();
        bool IsType(IMapDocumentControl control);

        Dictionary<string, string> GetStyles();
        bool IsStyle(IMapDocumentControl control, string style);
    }
}