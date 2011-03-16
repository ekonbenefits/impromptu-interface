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


    /// <summary>
    /// Fluent Class for writing inline lambdass
    /// </summary>
    /// <typeparam name="TR">The type of the R.</typeparam>
    public static class Return<TR>
    {
        /// <summary>
        /// Arguments 
        /// </summary>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<TR> Arguments(Func<TR> del)
        {
            return del;
        }

        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
        public static Func<T1, TR> Arguments<T1>(Func<T1, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, TR> Arguments<T1,T2>(Func<T1,T2,TR> del)
        {
            return del;
        }

        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3, TR> Arguments<T1,T2,T3>(Func<T1,T2,T3, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, TR> Arguments<T1,T2,T3,T4>(Func<T1,T2,T3,T4, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, T5, TR> Arguments<T1,T2,T3,T4,T5>(Func<T1,T2,T3,T4,T5, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, T5,T6, TR> Arguments<T1,T2,T3,T4,T5,T6>(Func<T1,T2,T3,T4,T5,T6, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, T5,T6,T7, TR> Arguments<T1,T2,T3,T4,T5,T6,T7>(Func<T1,T2,T3,T4,T5,T6,T7, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, T5,T6,T7,T8, TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Func<T1,T2,T3,T4,T5,T6,T7,T8, TR> del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="T6">The type of the 6.</typeparam>
        /// <typeparam name="T7">The type of the 7.</typeparam>
        /// <typeparam name="T8">The type of the 8.</typeparam>
        /// <typeparam name="T9">The type of the 9.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		public static Func<T1, T2, T3,T4, T5,T6,T7,T8,T9, TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9, TR> del)
        {
            return del;
        }

    }

    /// <summary>
    /// Fluent class for writing inline lambdas that return void
    /// </summary>
    public static class ReturnVoid
    {   /// <summary>
        /// Arguments
        /// </summary>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
        public static Action Arguments(Action del)
        {
            return del;
        }
        /// <summary>
        /// Arguments
        /// </summary>
        /// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1> Arguments<T1>(Action<T1> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2> Arguments<T1,T2>(Action<T1,T2> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3> Arguments<T1,T2,T3>(Action<T1,T2,T3> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4> Arguments<T1,T2,T3,T4>(Action<T1,T2,T3,T4> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <typeparam name="T5">The type of the 5.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5> Arguments<T1,T2,T3,T4,T5>(Action<T1,T2,T3,T4,T5> del)
        {
            return del;
        }

         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <typeparam name="T5">The type of the 5.</typeparam>
         /// <typeparam name="T6">The type of the 6.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6> Arguments<T1,T2,T3,T4,T5,T6>(Action<T1,T2,T3,T4,T5,T6> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <typeparam name="T5">The type of the 5.</typeparam>
         /// <typeparam name="T6">The type of the 6.</typeparam>
         /// <typeparam name="T7">The type of the 7.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7> Arguments<T1,T2,T3,T4,T5,T6,T7>(Action<T1,T2,T3,T4,T5,T6,T7> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <typeparam name="T5">The type of the 5.</typeparam>
         /// <typeparam name="T6">The type of the 6.</typeparam>
         /// <typeparam name="T7">The type of the 7.</typeparam>
         /// <typeparam name="T8">The type of the 8.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Action<T1,T2,T3,T4,T5,T6,T7,T8> del)
        {
            return del;
        }
         /// <summary>
         /// Arguments
         /// </summary>
         /// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <typeparam name="T2">The type of the 2.</typeparam>
         /// <typeparam name="T3">The type of the 3.</typeparam>
         /// <typeparam name="T4">The type of the 4.</typeparam>
         /// <typeparam name="T5">The type of the 5.</typeparam>
         /// <typeparam name="T6">The type of the 6.</typeparam>
         /// <typeparam name="T7">The type of the 7.</typeparam>
         /// <typeparam name="T8">The type of the 8.</typeparam>
         /// <typeparam name="T9">The type of the 9.</typeparam>
         /// <param name="del">The lambda.</param>
         /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> del)
        {
            return del;
        }
    }
}
