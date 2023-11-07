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

using System.IO;

namespace ImpromptuInterface.Build
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;


#if NET40
    internal static class CompatHelper
    {
        public static Type GetTypeInfo(this Type type) => type;

        public static Type CreateTypeInfo(this TypeBuilder builder) => builder.CreateType();

        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access) => AppDomain.CurrentDomain.DefineDynamicAssembly(name, access);
    }

#else
    internal static class CompatHelper
    {
        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access) => AssemblyBuilder.DefineDynamicAssembly(name, access);

    }

#endif

    ///<summary>
    /// Does most of the work buiding and caching proxies
    ///</summary>
    public static class BuildProxy
    {
        public static readonly ActLikeMaker DefaultMaker = new ActLikeMaker();

        public static ActLikeMaker CollectableProxyMaker() => new ActLikeMaker(AssemblyBuilderAccess.RunAndCollect);

#if NET40
        public static SaveableActLikeMaker SaveableProxyMaker(string assemblyName = null) => new SaveableActLikeMaker(AssemblyBuilderAccess.RunAndSave, assemblyName);
#endif
    }


}
