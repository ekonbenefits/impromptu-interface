using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestSupportLibrary
{
    public static class ExampleOp
    {
        public static dynamic a = 1;
        public static dynamic b = 2;

        public static void Plus()
        {
           dynamic c = a + b;
        }
        public static void Multiply()
        {
            dynamic c = a * b;
        }

        public static void Subract()
        {
            dynamic c = a - b;
        }

        public static void Divide()
        {
            dynamic c = a / b;
        }

        public static void Lessthan()
        {
            dynamic c = a < b;
        }

        public static void LessthanEqual()
        {
            dynamic c = a <= b;
        }

        public static void MultipleAssign()
        {
            a *= b;
        }

        public static void AddAssign()
        {
            a += b;
        }
    }
}
