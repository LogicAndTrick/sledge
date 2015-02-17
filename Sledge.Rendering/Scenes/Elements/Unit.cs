namespace Sledge.Rendering.Scenes.Elements
{
    public class Unit
    {
        public static Unit Pixels(float pixels)
        {
            return new Unit(ValueType.Pixel, pixels);
        }

        public static Unit Units(float units)
        {
            return new Unit(ValueType.Unit, units);
        }

        public ValueType Type { get; set; }
        public float Value { get; set; }

        public Unit(ValueType type, float value)
        {
            Type = type;
            Value = value;
        }
    }
}