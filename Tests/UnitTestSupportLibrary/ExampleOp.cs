using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestSupportLibrary
{

    //Just Checking which ExpressionType the compilier uses.
    public static class ExampleOp
    {
        public static dynamic a = 1;
        public static dynamic b = 2;
        public static dynamic x = 3;
        public static dynamic y = 4;
        public static dynamic c;

        public static void Plus()
        {
            c = a + b;
        }

        public static void Multiply()
        {
            c = a*b;
        }

        public static void Subract()
        {
            c = a - b;
        }

        public static void Divide()
        {
            c = a/b;
        }

        public static void Lessthan()
        {
            c = a < b;
        }

        public static void LessthanEqual()
        {
            c = a <= b;
        }

        public static void MultipleAssign()
        {
            a *= b;
        }

        public static void AddAssign()
        {
            a += b;
        }

        public static void And()
        {
            c = a & b;
        }

        public static void AndAlso()
        {
            c = a == x && b == y;
        }

        public static void OrAlso()
        {
            c = a == b || x == y;
        }

        public static void NotEqula()
        {
            c = a != b;
        }

        public static void Xor()
        {
            c = a ^ b;
        }

        public static void PreIncrement()
        {
           b= ++a;
        }

        public static void PostIncrement()
        {
            b= a++;
        }

        public static void IsTrue()
        {
            b = a && true;
        }

        public static void Negate()
        {
            b = -a;
        }
        public static void Not()
        {
            b = !a;
        }
    }
}
