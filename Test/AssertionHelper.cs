// 
//  Copyright 2011  Ekon Benefits
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
using System.Dynamic;
using System.Linq.Expressions;
namespace Test
{
	public class AssertionHelper
	{
		public AssertionHelper ()
		{
			Assert = new ExpandoObject();
			Action<dynamic,dynamic> tFunc = (x,y) => {
				
				if(x == y)
					Console.WriteLine("Success");
				else
					Console.WriteLine("Fail");
				
				
			};
			Assert.AreEqual = tFunc;
		}
		
		public dynamic Assert{get;set;}
		
		
	}
	
	static public class TestPrint{
		public static  void RunTest<T>(this T obj,Expression<Action<T>> func){
			Console.WriteLine("Start {0}:", func);
			try{
				func.Compile()(obj);
			}catch{
					Console.WriteLine("Fail Exception");
			}

		}	
	}
}

