namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Reprepsents an object that has been hidden from the renderer but is still visible to the user.
    /// This is used when a tool is handling the rendering of an object itself and doesn't want the renderer to convert it.
    /// </summary>
    public interface IRenderVisibility
    {
        bool IsRenderHidden { get; }
    }
}