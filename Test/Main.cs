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

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			
			
			var tTest2 = new Test();
			
			tTest2.RunTest(it=>it.ExpandoMethodsTest());

			tTest2.RunTest(it=>it.ExpandoPropertyTest());

			
			tTest2.RunTest(it=>it.AnonPropertyTest());
			
			

			tTest2.RunTest(it=>it.StringMethodTest());

			tTest2.RunTest(it=>it.StringPropertyTest());
			
			tTest2.RunTest(it=>it.CacheTest());
			
			//Fails!
			tTest2.RunTest(it=>it.TestGeneric());
			
			var tTest =new PrivateTest();
			
			tTest.RunTest(it=>it.Test());
			
			

			
			
			
		}
	}
}

