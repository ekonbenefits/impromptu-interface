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
using System.Linq;

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
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
        public static Func<TR> Arguments(Func<TR> del)
        {
            return del;
        }


        /// <summary>
        /// This and arguments.
        /// </summary>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR> ThisAndArguments(ThisFunc<TR> del)
        {
            return del;
        }

		
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,TR> Arguments<T1>(Func<T1,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1> ThisAndArguments<T1>(ThisFunc<TR,T1> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,TR> Arguments<T1,T2>(Func<T1,T2,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2> ThisAndArguments<T1,T2>(ThisFunc<TR,T1,T2> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,TR> Arguments<T1,T2,T3>(Func<T1,T2,T3,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3> ThisAndArguments<T1,T2,T3>(ThisFunc<TR,T1,T2,T3> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,TR> Arguments<T1,T2,T3,T4>(Func<T1,T2,T3,T4,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4> ThisAndArguments<T1,T2,T3,T4>(ThisFunc<TR,T1,T2,T3,T4> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,TR> Arguments<T1,T2,T3,T4,T5>(Func<T1,T2,T3,T4,T5,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5> ThisAndArguments<T1,T2,T3,T4,T5>(ThisFunc<TR,T1,T2,T3,T4,T5> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,TR> Arguments<T1,T2,T3,T4,T5,T6>(Func<T1,T2,T3,T4,T5,T6,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6> ThisAndArguments<T1,T2,T3,T4,T5,T6>(ThisFunc<TR,T1,T2,T3,T4,T5,T6> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,TR> Arguments<T1,T2,T3,T4,T5,T6,T7>(Func<T1,T2,T3,T4,T5,T6,T7,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Func<T1,T2,T3,T4,T5,T6,T7,T8,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> del)
        {
            return del;
        }

     
		

        /// <summary>
        /// Arguments 
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
		/// <typeparam name="T16">The type of the Argument 16.</typeparam>
        /// <param name="del">The lambdas.</param>
        /// <returns></returns>
        public static Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,TR> del)
        {
            return del;
        }

        /// <summary>
        /// this and Arguments.
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
		/// <typeparam name="T16">The type of the Argument 16.</typeparam>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(ThisFunc<TR,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> del)
        {
            return del;
        }

     	}


	/// <summary>
    /// Fluent class for writing inline lambdas that return void
    /// </summary>
	public static class ReturnVoid
    { 
	    /// <summary>
        /// Arguments
        /// </summary>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
        public static Action Arguments(Action del)
        {
            return del;
        }


        /// <summary>
        /// This and arguments.
        /// </summary>
        /// <param name="del">The del.</param>
        /// <returns></returns>
        public static ThisAction ThisAndArguments(ThisAction del)
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
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1> ThisAndArguments<T1>(ThisAction<T1> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2> Arguments<T1,T2>(Action<T1,T2> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2> ThisAndArguments<T1,T2>(ThisAction<T1,T2> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3> Arguments<T1,T2,T3>(Action<T1,T2,T3> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3> ThisAndArguments<T1,T2,T3>(ThisAction<T1,T2,T3> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4> Arguments<T1,T2,T3,T4>(Action<T1,T2,T3,T4> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4> ThisAndArguments<T1,T2,T3,T4>(ThisAction<T1,T2,T3,T4> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5> Arguments<T1,T2,T3,T4,T5>(Action<T1,T2,T3,T4,T5> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5> ThisAndArguments<T1,T2,T3,T4,T5>(ThisAction<T1,T2,T3,T4,T5> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6> Arguments<T1,T2,T3,T4,T5,T6>(Action<T1,T2,T3,T4,T5,T6> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6> ThisAndArguments<T1,T2,T3,T4,T5,T6>(ThisAction<T1,T2,T3,T4,T5,T6> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7> Arguments<T1,T2,T3,T4,T5,T6,T7>(Action<T1,T2,T3,T4,T5,T6,T7> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7>(ThisAction<T1,T2,T3,T4,T5,T6,T7> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Action<T1,T2,T3,T4,T5,T6,T7,T8> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> del)
         {
             return del;
         }

		 
		
        /// <summary>
        /// Arguments
        /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
		/// <typeparam name="T16">The type of the Argument 16.</typeparam>
        /// <param name="del">The lambda.</param>
        /// <returns>The lambda.</returns>
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> del)
        {
            return del;
        }


         /// <summary>
         /// This and Arguments.
         /// </summary>
		/// <typeparam name="T1">The type of the Argument 1.</typeparam>
		/// <typeparam name="T2">The type of the Argument 2.</typeparam>
		/// <typeparam name="T3">The type of the Argument 3.</typeparam>
		/// <typeparam name="T4">The type of the Argument 4.</typeparam>
		/// <typeparam name="T5">The type of the Argument 5.</typeparam>
		/// <typeparam name="T6">The type of the Argument 6.</typeparam>
		/// <typeparam name="T7">The type of the Argument 7.</typeparam>
		/// <typeparam name="T8">The type of the Argument 8.</typeparam>
		/// <typeparam name="T9">The type of the Argument 9.</typeparam>
		/// <typeparam name="T10">The type of the Argument 10.</typeparam>
		/// <typeparam name="T11">The type of the Argument 11.</typeparam>
		/// <typeparam name="T12">The type of the Argument 12.</typeparam>
		/// <typeparam name="T13">The type of the Argument 13.</typeparam>
		/// <typeparam name="T14">The type of the Argument 14.</typeparam>
		/// <typeparam name="T15">The type of the Argument 15.</typeparam>
		/// <typeparam name="T16">The type of the Argument 16.</typeparam>
         /// <param name="del">The del.</param>
         /// <returns></returns>
         public static ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> ThisAndArguments<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(ThisAction<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> del)
         {
             return del;
         }

		      }
}

