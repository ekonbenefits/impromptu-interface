using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Dynamic
{
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
    public static class Void
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
