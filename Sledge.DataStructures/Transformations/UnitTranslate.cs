using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    [Serializable]
    public class UnitTranslate : IUnitTransformation
    {
        public Coordinate Translation { get; set; }

        public UnitTranslate(Coordinate translation)
        {
            Translation = translation;
        }

        protected UnitTranslate(SerializationInfo info, StreamingContext context)
        {
            Translation = (Coordinate) info.GetValue("Translation", typeof (Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Translation", Translation);
        }

        public Coordinate Transform(Coordinate c)
        {
            return c + Translation;
        }
    }
}
