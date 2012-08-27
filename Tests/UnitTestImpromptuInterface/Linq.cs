using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ImpromptuInterface;

#if !SELFRUNNER
using IronPython.Hosting;
using Microsoft.Scripting;
using NUnit.Framework;
#endif


#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
    [TestFixture]
    public class Linq : Helper
    {
        [Test]
        public void SimpleLinq()
        {

            var expected = Enumerable.Range(1, 10).Where(i => i > 5).Skip(1).Take(2).Max();
            var actual = Impromptu.Linq(Enumerable.Range(1, 10)).Where(i => i > 5).Skip(1).Take(2).Max();

            Assert.AreEqual(expected,actual);
        }
            [Test]
        public void MoreGenericsLinq()
        {
            var expected = Enumerable.Range(1, 10).Select(i=> Tuple.Create(1,i)).Aggregate(0,(accum,each)=> each.Item2);
            var actual = Impromptu.Linq(Enumerable.Range(1, 10)).Select(i => Tuple.Create(1, i)).Aggregate(0, (accum, each) => each.Item2);

            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void SimpleLinqDynamicLinq()
        {

            var expected = Enumerable.Range(1, 10).Where(i => i > 5).Skip(1).Take(2).Max();
            var actual = Impromptu.DynamicLinq(Enumerable.Range(1, 10)).Where(new Func<int,bool>(i => i > 5)).Skip(1).Take(2).Max();

            Assert.AreEqual(expected, actual);
        }
            [Test]
        public void MoreGenericsDynamicLinq()
        {
            var expected = Enumerable.Range(1, 10).Select(i => Tuple.Create(1, i)).Aggregate(0, (accum, each) => each.Item2);
            var actual = Impromptu.DynamicLinq(Enumerable.Range(1, 10))
                .Select(new Func<int,Tuple<int,int>>(i => Tuple.Create(1, i)))
                .Aggregate(0, new Func<int,Tuple<int,int>,int>((accum, each) => each.Item2));

            Assert.AreEqual(expected, actual);

        }

          [Test]
        public void PythonLinq()
          {

              var expected = Enumerable.Range(1, 10).Where(x=> x < 5).OrderBy(x => 10 - x).First();


            var tEngine = Python.CreateEngine();
            var tScope = tEngine.CreateScope();

            tScope.SetVariable("linq", Impromptu.Linq(Enumerable.Range(1, 10)));

            var tSource = tEngine.CreateScriptSourceFromString(@"
import System
result = linq.Where.Overloads[System.Func[int, bool]](lambda x: x < 5).OrderBy(lambda x: 10-x).First()

".Trim(), SourceCodeKind.Statements);
            var tCompiled = tSource.Compile();

            tCompiled.Execute(tScope);
              var actual = tScope.GetVariable("result");

              Assert.AreEqual( expected,actual);
        }

          [Test]
          public void PythonDynamicLinq()
          {
              var expected = Enumerable.Range(1, 10).Where(x => x < 5).OrderBy(x => 10 - x).First();

              var tEngine = Python.CreateEngine();
              var tScope = tEngine.CreateScope();

              tScope.SetVariable("linq", Impromptu.DynamicLinq(Enumerable.Range(1, 10)));

              var tSource = tEngine.CreateScriptSourceFromString(@"
import System
result = linq.Where.Overloads[System.Func[int, bool]](lambda x: x < 5).OrderBy(lambda x: 10-x).First()

".Trim(), SourceCodeKind.Statements);
              var tCompiled = tSource.Compile();

              tCompiled.Execute(tScope);
              var actual = tScope.GetVariable("result");

              Assert.AreEqual(expected, actual);
          }


           [Test]
        public void PrintOutInterface()
           {
               var tList =
                   typeof (Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).OrderBy(it => it.Name).
                       ToList();

            Console.WriteLine("public interface ILinq<TSource>:IEnumerable<TSource>");
            Console.WriteLine("{");
            foreach (var line in tList
            .Where(it => it.GetParameters().Any()
            && it.GetParameters().First().ParameterType.IsGenericType
            && it.GetParameters().First().ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            && HelperSignleGenericArgMatch(it.GetParameters().First().ParameterType.GetGenericArguments().Single())
            )
            .Select(HelperMakeName))
            {
                Console.WriteLine("\t"+line);
            }
            Console.WriteLine("}");
            Console.WriteLine();

            Console.WriteLine("public interface IOrderedLinq<TSource> : ILinq<TSource>, IOrderedEnumerable<TSource>");
            Console.WriteLine("{");
            foreach (var line in tList
            .Where(it => it.GetParameters().Any()
            && it.GetParameters().First().ParameterType.IsGenericType
            && it.GetParameters().First().ParameterType.GetGenericTypeDefinition() == typeof(IOrderedEnumerable<>)
            && HelperSignleGenericArgMatch(it.GetParameters().First().ParameterType.GetGenericArguments().Single())
            )
            .Select(HelperMakeName))
            {
                       Console.WriteLine("\t"+line);
            }
            Console.WriteLine("}");
            Console.WriteLine();

            Console.WriteLine("//Skipped Methods");
            foreach (var line in tList
            .Where(it => it.GetParameters().Any()
            && !(it.GetParameters().First().ParameterType.IsGenericType
            && it.GetParameters().First().ParameterType.GetGenericTypeDefinition() != typeof(IOrderedEnumerable<>)
            && HelperSignleGenericArgMatch(it.GetParameters().First().ParameterType.GetGenericArguments().Single()))
            && !(it.GetParameters().First().ParameterType.IsGenericType
            && it.GetParameters().First().ParameterType.GetGenericTypeDefinition() == typeof(IOrderedEnumerable<>)
            && HelperSignleGenericArgMatch(it.GetParameters().First().ParameterType.GetGenericArguments().Single()))
            ).Select(HelperMakeNameDebug))
            {
                Console.WriteLine("//" + line);
            }


        }

        bool HelperSignleGenericArgMatch(Type info)
        {
            foreach (var name in new[] { "TSource", "TFirst", "TOuter" })
            {
                if (info.Name == name)
                {
                    return true;
                }
            }

            return false;
        }


        // Define other methods and classes here
        string HelperFormatType(Type it)
        {
            if (HelperSignleGenericArgMatch(it))
            {
                return "TSource";
            }

            if (it.IsGenericType)
            {
                return String.Format("{0}<{1}>", it.Name.Substring(0, it.Name.IndexOf("`")), String.Join(",", it.GetGenericArguments().Select(a => HelperFormatType(a))));
            }
            else
            {
                return it.Name;
            }
        }

        string HelperGenericParams(Type[] it)
        {
            var tArgs = it.Where(t => !HelperSignleGenericArgMatch(t)).Select(t => HelperFormatType(t));
            if (!tArgs.Any())
            {
                return "";
            }
            return "<" + String.Join(",", tArgs) + ">";
        }
        string HelperReturnTypeSub(Type it)
        {
            if (it.IsGenericType && (it.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return String.Format("ILinq<{0}>", HelperFormatType(it.GetGenericArguments().Single()));
            }
            if (it.IsGenericType && (it.GetGenericTypeDefinition() == typeof(IOrderedEnumerable<>)))
            {
                return String.Format("IOrderedLinq<{0}>", HelperFormatType(it.GetGenericArguments().Single()));
            }
            return HelperFormatType(it);

        }

        string HelperGetParams(ParameterInfo[] it)
        {
            var parms = it.Skip(1);
            return String.Join(",", parms.Select(p => HelperFormatType(p.ParameterType) + " " + p.Name));

        }

        string HelperGetParamsDebug(ParameterInfo[] it)
        {
            var parms = it;
            return String.Join(",", parms.Select(p => HelperFormatType(p.ParameterType) + " " + p.Name));

        }

        string HelperMakeName(MethodInfo it)
        {
            return String.Format("{0} {1}{2}({3});", HelperReturnTypeSub(it.ReturnType), it.Name, HelperGenericParams(it.GetGenericArguments()), HelperGetParams(it.GetParameters()));
        }
        string HelperMakeNameDebug(MethodInfo it)
        {
            return String.Format("{0} {1}{2}({3});", HelperReturnTypeSub(it.ReturnType), it.Name, HelperGenericParams(it.GetGenericArguments()), HelperGetParamsDebug(it.GetParameters()));
        }
    }
}
