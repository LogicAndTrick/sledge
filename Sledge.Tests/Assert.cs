using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sledge.Tests
{
    public static class AssertExtensions
    {
        public static void DecimalEquals(this Assert assert, decimal expected, decimal actual, decimal delta = 0.0005m)
        {
            var abs = Math.Abs(expected - actual);
            if (abs > delta)
            {
                throw new AssertFailedException($"Assert.DecimalEquals failed. Expected: <{expected}>. Actual: <{actual}>.");
            }
        }
    }
}
