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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    [Serializable]
    public enum InvocationKind
    {
        /// <summary>
        /// NotSet
        /// </summary>
        NotSet=0,
        /// <summary>
        /// Convert, Implicit or Explict depending on arguments
        /// </summary>
        Convert,
        /// <summary>
        /// Get Property
        /// </summary>
        Get,
        /// <summary>
        /// Set Property
        /// </summary>
        Set,
        /// <summary>
        /// Get Indexer
        /// </summary>
        GetIndex,
        /// <summary>
        /// Set Indexer
        /// </summary>
        SetIndex,
        /// <summary>
        /// Invoke Method the has return value
        /// </summary>
        InvokeMember,
        /// <summary>
        /// Invoke Method that returns void
        /// </summary>
        InvokeMemberAction,
        /// <summary>
        /// Invoke Method that could return a value or void
        /// </summary>
        InvokeMemberUnknown,
    }

    [Serializable]
    public class Invocation
    {

        /// <summary>
        /// Defacto Binder Name for Convert Op
        /// </summary>
        public static readonly string ConvertBinderName = "(Convert)";

        /// <summary>
        /// Defacto Binder Name for Indexer
        /// </summary>
        public static readonly string IndexBinderName = "Item";
        public static readonly string ConstructorBinderName = "new()";

        /// <summary>
        /// Gets or sets the kind.
        /// </summary>
        /// <value>The kind.</value>
        public InvocationKind Kind { get; protected set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        /// <value>The args.</value>
        public object[] Args { get; protected set; }


        /// <summary>
        /// Creates the invocation.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static Invocation Create(InvocationKind kind,string name, params object[] args)
        {
            return new Invocation(kind,name,args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invocation"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        public Invocation(InvocationKind kind, string name, params object[] args)
        {
            Kind = kind;
            Name = name;
            Args = args;
        }

        /// <summary>
        /// Invokes the invocation on specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public object Invoke(object target)
        {
            switch (Kind)
            {
                case InvocationKind.Convert:
                    bool tExplict =false;
                    if(Args.Length == 2)
                        tExplict = (bool) Args[1];
                    return Impromptu.InvokeConvert(target, (Type) Args[0], tExplict);
                case InvocationKind.Get:
                    return Impromptu.InvokeGet(target, Name);
                case InvocationKind.Set:
                    Impromptu.InvokeSet(target, Name, Args.FirstOrDefault());
                    return null;
                case InvocationKind.GetIndex:
                    return Impromptu.InvokeGetIndex(target, Name, Args);
                case InvocationKind.SetIndex:
                    Impromptu.InvokeSetIndex(target, Name, Args);
                    return null;
                case InvocationKind.InvokeMember:
                    return Impromptu.InvokeMember(target, Name, Args);
                case InvocationKind.InvokeMemberAction:
                    Impromptu.InvokeMemberAction(target, Name, Args);
                    return null;
                case InvocationKind.InvokeMemberUnknown:
                    {
                        try
                        {
                             return Impromptu.InvokeMember(target, Name, Args);
                        }
                        catch (RuntimeBinderException)
                        {
                            
                            Impromptu.InvokeMemberAction(target, Name, Args);
                            return null;
                        }
                    }
                default:
                    throw new InvalidOperationException("Unknown Invocation Kind: "+ Kind);
            }
        }
    }
}
