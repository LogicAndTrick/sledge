using System;
using System.Linq;

namespace Sledge.Common.Shell.Components
{
    public class OrderHintAttribute : Attribute
    {
        public string OrderHint { get; set; }

        public OrderHintAttribute(string orderHint)
        {
            OrderHint = orderHint;
        }

        public static string GetOrderHint(Type ty)
        {
            return ty.GetCustomAttributes(typeof(OrderHintAttribute), false).OfType<OrderHintAttribute>().FirstOrDefault()?.OrderHint;
        }
    }
}