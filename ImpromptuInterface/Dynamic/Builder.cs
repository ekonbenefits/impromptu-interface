// 
//  Copyright 2011 Ekon Benefits
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
using System.Linq;

namespace ImpromptuInterface.Dynamic
{


    /// <summary>
    /// Builds Objects with a Fluent Syntax
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// New Builder
        /// </summary>
        /// <returns></returns>
        public static IImpromptuBuilder New()
        {
            return new ImpromptuBuilder<ImpromptuChainableDictionary>();
        }

        /// <summary>
        /// New Builder
        /// </summary>
        /// <typeparam name="TObjectPrototype">The type of the object prototype.</typeparam>
        /// <returns></returns>
        public static IImpromptuBuilder New<TObjectPrototype>() where TObjectPrototype : new()
        {
            return new ImpromptuBuilder<TObjectPrototype>();
        }
    }

    /// <summary>
    /// Encapsulates an Activator
    /// </summary>
    public class Activate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activate"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        public Activate(Type type, params object[] args)
        {
            Type = type;

            var tArg = args.OfType<Func<object[]>>().SingleOrDefault();
            if (tArg != null)
                Arguments = tArg;
            else
                Arguments = () => args;
            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Activate"/> class. With Factory Function
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        public Activate(Type type, Func<object[]> args)
        {
            Type = type;
            Arguments = args;
        }
        /// <summary>
        /// Gets or sets the constructor type.
        /// </summary>
        /// <value>The type.</value>
        public virtual Type Type { get; private set; }

        /// <summary>
        /// Gets or sets the constructor arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public virtual Func<object[]> Arguments
        {
            get; private set;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual dynamic Create()
        {
            var tArgs = Arguments();
            return Activator.CreateInstance(Type, tArgs);
        }
    }

    /// <summary>
    /// Encapsulates an Activator
    /// </summary>
    /// <typeparam name="TObjectPrototype">The type of the object prototype.</typeparam>
    public class Activate<TObjectPrototype> : Activate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activate&lt;TObjectPrototype&gt;"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        public Activate(params object[] args) : base(typeof(TObjectPrototype), args)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activate&lt;TObjectPrototype&gt;"/> class. With Factory Function
        /// </summary>
        /// <param name="args">The args.</param>
        public Activate(Func<object[]> args)
            : base(typeof(TObjectPrototype), args)
        {
        }

        public override dynamic Create()
        {
            var tArgs = Arguments();

            return tArgs.Any() 
                ? base.Create() 
                : Activator.CreateInstance<TObjectPrototype>();
        }
    }


}
