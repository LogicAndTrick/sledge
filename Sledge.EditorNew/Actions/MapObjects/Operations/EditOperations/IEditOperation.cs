using Sledge.DataStructures.MapObjects;

namespace Sledge.EditorNew.Actions.MapObjects.Operations.EditOperations
{
    public interface IEditOperation
    {
        void PerformOperation(MapObject mo);
    }
}
