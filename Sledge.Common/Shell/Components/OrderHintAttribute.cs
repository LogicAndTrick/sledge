using System;
using System.Linq;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// An attribute to attach to various objects to embue them with an optional order hint
    /// </summary>
    public class OrderHintAttribute : Attribute
    {
        public string OrderHint { get; set; }

        public OrderHintAttribute(string orderHint)
        {
            OrderHint = orderHint;
        }

        /// <summary>
        /// Get the order hint from a given type, if any.
        /// </summary>
        /// <param name="ty">The type to get the hint from</param>
        /// <returns>The order hint, or null if none are specified</returns>
        public static string GetOrderHint(Type ty)
        {
            return ty.GetCustomAttributes(typeof(OrderHintAttribute), false).OfType<OrderHintAttribute>().FirstOrDefault()?.OrderHint;
        }
    }
}