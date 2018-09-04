namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Represents an object that has been hidden from the user.
    /// Effectively means that the object is not interactive in any way.
    /// </summary>
    public interface IObjectVisibility
    {
        bool IsHidden { get; }
    }
}