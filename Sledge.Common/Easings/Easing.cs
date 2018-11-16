using System;

namespace Sledge.Common.Easings
{
    /// <summary>
    /// Easing functions specify the rate of change of a parameter over time.
    /// This class represents a number of commonly used easings.
    /// </summary>
    /// <remarks>https://easings.net/</remarks>
    public class Easing
    {
        private Func<double, double> Function { get; set; }
        private EasingDirection Direction { get; set; }

        public Easing(Func<double, double> function, EasingDirection direction)
        {
            Function = function;
            Direction = direction;
        }

        public double Evaluate(double input)
        {
            switch (Direction)
            {
                case EasingDirection.In:
                    return Function(input);
                case EasingDirection.Out:
                    return 1 - Function(1 - input);
                case EasingDirection.InOut:
                    return input < 0.5d
                        ? Function(input * 2) / 2
                        : 1 - Function(input * -2 + 2) / 2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Easing FromType(EasingType type, EasingDirection direction)
        {
            return new Easing(FunctionFromType(type), direction);
        }

        private static Func<double, double> FunctionFromType(EasingType easing)
        {
            switch (easing)
            {
                case EasingType.Constant:
                    return x => 1;
                case EasingType.Linear:
                    return x => x;
                case EasingType.Quadratic:
                    return x => Math.Pow(x, 2);
                case EasingType.Cubic:
                    return x => Math.Pow(x, 3);
                case EasingType.Quartic:
                    return x => Math.Pow(x, 4);
                case EasingType.Quintic:
                    return x => Math.Pow(x, 5);
                case EasingType.Sinusoidal:
                    return x => 1 - Math.Cos(x * Math.PI / 2);
                case EasingType.Exponential:
                    return x => Math.Pow(2, 10*(x-1));
                case EasingType.Circular:
                    return x => 1 - Math.Sqrt(1 - x * x);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}