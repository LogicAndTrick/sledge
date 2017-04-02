namespace Sledge.BspEditor.Components
{
    public interface IMapDocumentControlFactory
    {
        string Type { get; }
        IMapDocumentControl Create();
    }
}