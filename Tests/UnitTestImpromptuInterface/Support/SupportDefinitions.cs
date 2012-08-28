// 
//  Copyright 2010  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Diagnostics;
using ImpromptuInterface.Dynamic;

#if !SELFRUNNER
using System.Windows.Media;
#endif

namespace UnitTestImpromptuInterface
{

    public static class TimeIt
    {
        public static Stopwatch Go(Action action, int interation = 1000000)
        {
            var tStopwatch = new Stopwatch();
            tStopwatch.Start();
            for (int i = 0; i < interation; i++)
            {
                action();
            }
            tStopwatch.Stop();
            return tStopwatch;
        }
    }

    public class TestForwarder:ImpromptuForwarder
    {
        public TestForwarder(object target) : base(target)
        {
        }
    }

    public interface IDynamicArg
    {
        dynamic ReturnIt(dynamic arg);

        bool Params(params dynamic [] args);
    }

    public class PocoNonDynamicArg
    {
        public int ReturnIt(int i)
        {
            return i;
        }


        public List<string> ReturnIt(List<string> i)
        {
            return i;
        }

        public bool Params(object fallback)
        {
            return false;
        }

        public bool Params(params int [] args)
        {
            return true;
        }
    }

    public static class StaticType
    {
        public static TReturn Create<TReturn>(int type)
        {
            return default(TReturn);
        }

        public static bool Test
        {
            get { return true; }
        }

        public static int TestSet { get; set; }
    }

    public interface ISimpeleClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }

    public interface IInheritProp:ISimpeleClassProps
    {
        PropPoco ReturnProp { get; set; }
    }


    public interface IPropPocoProp
    {
        PropPoco ReturnProp { get; set; }
    }

    public interface IEventCollisions
    {
        int Event { get; set; }
    }


    public interface IEvent
    {
        event EventHandler<EventArgs> Event;
        void OnEvent(object obj, EventArgs args);
    }

    public class PocoEvent
    {
        public event EventHandler<EventArgs> Event;

        public void OnEvent(object obj, EventArgs args)
        {
            if (Event != null)
                Event(obj, args);
        }
    }


    public class PocoOptConstructor
    {
        public string One { get; set; }
        public string Two { get; set; }
        public string Three { get; set; }

        public PocoOptConstructor(string one="-1", string two="-2", string three="-3")
        {
            One = one;
            Two = two;
            Three = three;
        }
    }

    public enum TestEnum
    {
       None,
       One,
       Two
    }

    public interface IDynamicDict
    {
        int Test1 { get; }

        long Test2 { get; }

        TestEnum Test3 { get; }

        TestEnum Test4 { get; }

        dynamic TestD { get; }
    }

    public interface INonDynamicDict
    {
        int Test1 { get; }

        long Test2 { get; }

        TestEnum Test3 { get; }  
        
        TestEnum Test4 { get; }

        IDictionary<string,object> TestD { get; }
    }

    public interface ISimpleStringProperty
    {
        int Length { get; }

    }

    public interface IRobot
    {
        string Name { get; }
    }
     public class Robot
     {
         public string Name { get; set; } 
     }

    public interface ISimpleStringMethod
    {
        bool StartsWith(string value);

    }

    public interface ISimpleStringMethodCollision
    {
        int StartsWith(string value);

    }

    public interface ISimpeleClassMeth
    {
        void Action1();
        void Action2(bool value);
        string Action3();
    }

    public interface ISimpeleClassMeth2 : ISimpeleClassMeth
    {
       
        string Action4(int arg);
    }

    public interface IGenericMeth
    {
        string Action<T>(T arg);

        T Action2<T>(T arg);
    }

    public interface IStringIntIndexer
    {
        string this[int index] { get; set; }
    }

    public interface IObjectStringIndexer
    {
        object this[string index] { get; set; }
    }

	 public interface IGenericMethWithConstraints
    {
        string Action<T>(T arg) where T:class;
        string Action2<T>(T arg) where T : IComparable;
    }
	
	 public interface IGenericType<T>
    {
		string Funct(T arg);
		
     
    }

     public interface IGenericTypeConstraints<T> where T:class
     {
         string Funct(T arg);

     }


    public interface IOverloadingMethod
    {
       string Func(int arg);

       string Func(object arg);
    }
	
    [Serializable]
	public class PropPoco{
	 	public string Prop1 { get; set; }

        public long Prop2 { get; set; }

        public Guid Prop3 { get;set; }

        public int Event { get; set; }
	}

    public struct PropStruct
    {
        public string Prop1 { get; set; }

        public long Prop2 { get; set; }

        public Guid Prop3 { get; set; }

        public int Event { get; set; }
    }


    public interface IVoidMethod
    {
        void Action();
    }

    public class VoidMethodPoco
    {
        public void Action()
        {
           Console.WriteLine("VoidFunc");
        }
    }

    public class OverloadingMethPoco
    {
        public string Func(int arg)
        {
            return "int";
        }

        public string Func(object arg)
        {
            return "object";
        }
        public string Func(object arg, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return "object 6";
        }

        public string Func(object one = null, object two = null, object three = null)
        {
            return "object named";
        }
    }

    /// <summary>
    /// Dynamic Delegates need to return object or void, first parameter should be a CallSite, second object, followed by the expected arguments
    /// </summary>
    public delegate object DynamicTryString(CallSite callsite, object target, out string result);

    public class MethOutPoco
    {
        public bool Func(out string result)
        {
            result = "success";
            return true;
        }
    }


    public class Thing { }

    public interface IGenericTest
    {
        List<T> GetThings<T>(Guid test) where T : Thing;
    }
    public class OtherThing 
    {


        List<T> GetThings<T>(Guid test) where T : Thing
        {
            return new List<T>();
        }

    }


    public class GenericMethOutPoco
    {
        public bool Func<T>(out T result)
        {
            result = default(T);
            return true;
        }
    }

    public interface IGenericMethodOut
    {
        bool Func<T>(out T result);
    }

    public interface IMethodOut2
    {
        bool Func(out int result);
    }


    public interface IMethodOut
    {
        bool Func(out string result);
    }

    public class MethRefPoco
    {
        public bool Func(ref int result)
        {
            result = result + 2;
            return true;
        }

    }

    public interface IMethodRef
    {
        bool Func(ref int result);
    }
	
	public interface IBuilder{
		 INest Nester(object props);
		 INested Nester2(object props);

         [UseNamedArgument]
         INest Nester(string NameLevel1, INested Nested);

         INested Nester2([UseNamedArgument]string NameLevel2);	
	}

    [NonRecursiveInterface]
    public interface INonNest
    {
        String NameLevel1 { get; set; }
        INested Nested { get; set; }
        INested Nested2();
    }

    public interface INonPartialNest
    {
        String NameLevel1 { get; set; }
        INested Nested { get; set; }

        [NonRecursiveInterface]
        INested Nested2();
    }

	public interface INest
    {
		String NameLevel1 {get;set;}
		INested Nested {get;set;}
	}

    public interface INestMeth
    {
        String NameLevel1 { get; set; }
        INested Nested(object one, object two);
    }

    public interface INested
    {
		string NameLevel2 {get;}
    }

    public class PocoAdder
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }

    [Serializable]
    public class PocoDoubleProp:IInheritProp, IPropPocoProp,IEnumerable
    {
        public string Prop1
        {
            get { throw new NotImplementedException(); }
        }

        public long Prop2
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Prop3
        {
            get { throw new NotImplementedException(); }
        }

        public PropPoco ReturnProp
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        PropPoco IPropPocoProp.ReturnProp
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class PocoCollection:IList
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }
    }

#if !SELFRUNNER
    public interface IColor
    {
        Color ColorViaString { get; }
    }
#endif
}
