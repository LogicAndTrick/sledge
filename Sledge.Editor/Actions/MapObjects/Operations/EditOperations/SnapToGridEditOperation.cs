using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;

namespace Sledge.Editor.Actions.MapObjects.Operations.EditOperations
{
    public class SnapToGridEditOperation : IEditOperation
    {
        private readonly decimal _gridSpacing;
        private readonly TransformFlags _transformFlags;

        public SnapToGridEditOperation(decimal gridSpacing, TransformFlags transformFlags)
        {
            _gridSpacing = gridSpacing;
            _transformFlags = transformFlags;
        }

        public void PerformOperation(MapObject mo)
        {
            var box = mo.BoundingBox;
            var offset = box.Start.Snap(_gridSpacing) - box.Start;
            var transform = new UnitTranslate(offset);
            mo.Transform(transform, _transformFlags);
        }
    }
}