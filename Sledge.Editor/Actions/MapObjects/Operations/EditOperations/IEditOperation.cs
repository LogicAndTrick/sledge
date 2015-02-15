using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Actions.MapObjects.Operations.EditOperations
{
    public interface IEditOperation
    {
        void PerformOperation(MapObject mo);
    }
}
