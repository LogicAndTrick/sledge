using System;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;

namespace Sledge.Editor.Actions.MapObjects.Operations.EditOperations
{
    public class AlignObjectsEditOperation : IEditOperation
    {
        public enum AlignAxis
        {
            X,
            Y,
            Z
        }

        public enum AlignDirection
        {
            Min,
            Max
        }

        private readonly Box _alignBox;
        private readonly TransformFlags _transformFlags;
        private readonly AlignAxis _axis;
        private readonly AlignDirection _direction;

        public AlignObjectsEditOperation(Box alignBox, AlignAxis axis, AlignDirection direction, TransformFlags transformFlags)
        {
            _alignBox = alignBox;
            _axis = axis;
            _direction = direction;
            _transformFlags = transformFlags;
        }

        public void PerformOperation(MapObject mo)
        {
            var current = Extractor(mo.BoundingBox);
            var target = Extractor(_alignBox);
            var value = target - current;
            var translate = Creator(value);
            var transform = new UnitTranslate(translate);
            mo.Transform(transform, _transformFlags);
        }

        private decimal Extractor(Box box)
        {
            Coordinate coord;
            switch (_direction)
            {
                case AlignDirection.Min:
                    coord = box.Start;
                    break;
                case AlignDirection.Max:
                    coord = box.End;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (_axis)
            {
                case AlignAxis.X:
                    return coord.X;
                case AlignAxis.Y:
                    return coord.Y;
                case AlignAxis.Z:
                    return coord.Z;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Coordinate Creator(decimal value)
        {
            switch (_axis)
            {
                case AlignAxis.X:
                    return new Coordinate(value, 0, 0);
                case AlignAxis.Y:
                    return new Coordinate(0, value, 0);
                case AlignAxis.Z:
                    return new Coordinate(0, 0, value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}