using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    public class UnitTranslate : IUnitTransformation
    {
        public Coordinate Translation { get; set; }

        public UnitTranslate(Coordinate translation)
        {
            Translation = translation;
        }

        public Coordinate Transform(Coordinate c)
        {
            return c + Translation;
        }
    }
}
