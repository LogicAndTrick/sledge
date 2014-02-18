using System;
using Sledge.Extensions;

namespace Sledge.Common.Easings
{
    public class Easing
    {
        private Func<decimal, decimal> Function { get; set; }
        private EasingDirection Direction { get; set; }

        public Easing(Func<decimal, decimal> function, EasingDirection direction)
        {
            Function = function;
            Direction = direction;
        }

        public decimal Evaluate(decimal input)
        {
            switch (Direction)
            {
                case EasingDirection.In:
                    return Function(input);
                case EasingDirection.Out:
                    return 1 - Function(1 - input);
                case EasingDirection.InOut:
                    return input < 0.5m
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

        private static Func<decimal, decimal> FunctionFromType(EasingType easing)
        {
            switch (easing)
            {
                case EasingType.Constant:
                    return x => 1;
                case EasingType.Linear:
                    return x => x;
                case EasingType.Quadratic:
                    return x => DMath.Pow(x, 2);
                case EasingType.Cubic:
                    return x => DMath.Pow(x, 3);
                case EasingType.Quartic:
                    return x => DMath.Pow(x, 4);
                case EasingType.Quintic:
                    return x => DMath.Pow(x, 5);
                case EasingType.Sinusoidal:
                    return x => 1 - DMath.Cos(x * DMath.PI / 2); // Wait... That's not Sine!
                case EasingType.Exponential:
                    return x => DMath.Pow(x, 5);
                case EasingType.Circular:
                    return x => 1 - DMath.Sqrt(1 - x * x);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}