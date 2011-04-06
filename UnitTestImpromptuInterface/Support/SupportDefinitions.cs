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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Diagnostics;
using ImpromptuInterface.Dynamic;

namespace UnitTestImpromptuInterface
{


    public interface ISimpeleClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }

    public interface IDynamicDict
    {
        int Test1 { get; }

        long Test2 { get; }

        dynamic TestD { get; }
    }

    public interface INonDynamicDict
    {
        int Test1 { get; }

        long Test2 { get; }

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

    public interface ISimpeleClassMeth
    {
        void Action1();
        void Action2(bool value);
        string Action3();
    }

    public interface IGenericMeth
    {
        string Action<T>(T arg);

        T Action2<T>(T arg);
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
	
	public class PropPoco{
	 	public string Prop1 { get; set; }

        public long Prop2 { get; set; }

        public Guid Prop3 { get;set; }

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
	
	public interface INest
    {
		String NameLevel1 {get;set;}
		INested Nested {get;set;}
	}
	
    public interface INested
    {
		string NameLevel2 {get;}
    }
}
