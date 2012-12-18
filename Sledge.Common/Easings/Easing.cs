using System;
using Sledge.Extensions;

namespace Sledge.Common.Easings
{
    public class Easing
    {
        private Func<decimal, decimal> Function { get; set; }

        public Easing(Func<decimal, decimal> function)
        {
            Function = function;
        }

        public decimal Evaluate(decimal input)
        {
            return Function(input);
        }

        public static Easing FromType(EasingType type)
        {
            return new Easing(FunctionFromType(type));
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