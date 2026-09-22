using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyUnit;
using AnyUnit.Run;
using AnyUnit.Style.Nunit;
using AnyUnit.Constraints;
using AnyUnit.Constraints.Pieces;

namespace UnitTestImpromptuInterface
{
  

   
    public class WriteLineContext
    {
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format,args);
        }
    }

    /// <summary>
    /// The classic asserts as ordinary instance methods, forwarding to AnyUnit's.
    ///
    /// AnyUnit supplies them as extension methods on <see cref="IAssert"/>, and C# refuses to
    /// dispatch an extension method when an argument is `dynamic` (CS1973) - which is most of
    /// this suite, since what it is testing is dynamic. An instance method on a statically
    /// typed receiver has no such problem: the call is bound at run time, exactly as it was
    /// against NUnit's static Assert. So the tests keep their `Assert.AreEqual(a, b)` and this
    /// sits in between.
    ///
    /// The exact-arity overloads (`Fail()`, `IsFalse(bool)`) also restore method-group
    /// conversion - `new Action(Assert.Fail)` - which AnyUnit's `params object[] args` tail
    /// prevents.
    /// </summary>
    public class ClassicAssert
    {
        private readonly IAssert _assert;

        public ClassicAssert(IAssert assert)
        {
            _assert = assert;
        }

        public int AssertCount => _assert.AssertCount;

        public void Okay() => _assert.Okay();
        public void Pass() => _assert.Okay();
        public void Fail() => _assert.Fail();
        public void Fail(string message, params object[] args) => _assert.Fail(Format(message, args));
        public void Ignore() => _assert.Ignore();
        public void Ignore(string message, params object[] args) => _assert.Ignore(Format(message, args));

        public void AreEqual(object expected, object actual) => _assert.AreEqual(expected, actual);
        public void AreEqual(object expected, object actual, string message, params object[] args) => _assert.AreEqual(expected, actual, Format(message, args));
        public void AreNotEqual(object expected, object actual) => _assert.AreNotEqual(expected, actual);
        public void AreNotEqual(object expected, object actual, string message, params object[] args) => _assert.AreNotEqual(expected, actual, Format(message, args));
        public void AreSame(object expected, object actual) => _assert.AreSame(expected, actual);
        public void AreNotSame(object expected, object actual) => _assert.AreNotSame(expected, actual);

        public void IsTrue(bool condition) => _assert.IsTrue(condition);
        public void IsTrue(bool condition, string message, params object[] args) => _assert.IsTrue(condition, Format(message, args));
        public void True(bool condition) => _assert.IsTrue(condition);
        public void IsFalse(bool condition) => _assert.IsFalse(condition);
        public void IsFalse(bool condition, string message, params object[] args) => _assert.IsFalse(condition, Format(message, args));
        public void False(bool condition) => _assert.IsFalse(condition);

        public void IsNull(object actual) => _assert.IsNull(actual);
        public void IsNull(object actual, string message, params object[] args) => _assert.IsNull(actual, Format(message, args));
        public void Null(object actual) => _assert.IsNull(actual);
        public void IsNotNull(object actual) => _assert.IsNotNull(actual);
        public void IsNotNull(object actual, string message, params object[] args) => _assert.IsNotNull(actual, Format(message, args));
        public void NotNull(object actual) => _assert.IsNotNull(actual);
        public void NotNull(object actual, string message, params object[] args) => _assert.IsNotNull(actual, Format(message, args));

        public void Greater(object arg1, object arg2) => _assert.Greater(arg1, arg2);
        public void GreaterOrEqual(object arg1, object arg2) => _assert.GreaterOrEqual(arg1, arg2);
        public void Less(object arg1, object arg2) => _assert.Less(arg1, arg2);
        public void Less(object arg1, object arg2, string message, params object[] args) => _assert.Less(arg1, arg2, Format(message, args));
        public void LessOrEqual(object arg1, object arg2) => _assert.LessOrEqual(arg1, arg2);

        public void IsInstanceOf<T>(object actual) => _assert.IsInstanceOf<T>(actual);
        public void IsNotInstanceOf<T>(object actual) => _assert.IsNotInstanceOf<T>(actual);
        public void IsEmpty(IEnumerable collection) => _assert.IsEmpty(collection);
        public void IsNotEmpty(IEnumerable collection) => _assert.IsNotEmpty(collection);
        public void Contains(object expected, IEnumerable actual) => _assert.Contains(expected, actual);

        public T Throws<T>(TestDelegate code) where T : Exception => _assert.Throws<T>(code);
        public Exception Throws(Type expectedType, TestDelegate code) => _assert.Throws(expectedType, code);
        public T Catch<T>(TestDelegate code) where T : Exception => _assert.Catch<T>(code);
        public void DoesNotThrow(TestDelegate code) => _assert.DoesNotThrow(code);

        public void That(object actual, IResolveConstraint constraint) => _assert.That(actual, constraint);
        public void That(bool condition) => _assert.That(condition);

        private static string Format(string message, object[] args)
            => args == null || args.Length == 0 ? message : string.Format(message, args);
    }

    public class Helper : AnyUnit.Run.AssertionHelper
    {
        private ClassicAssert _classic;

        /// <summary>
        /// Shadows the injected <see cref="IAssert"/> with instance methods (see
        /// <see cref="ClassicAssert"/>). The interface property the engine assigns through is the
        /// base's, so the runner is unaffected.
        /// </summary>
        public new ClassicAssert Assert => _classic ?? (_classic = new ClassicAssert(base.Assert));

        public WriteLineContext TestContext
        {
            get { return new WriteLineContext(); }
        }

        public void AssertException<T>(TestDelegate action) where T : Exception
        {
            Assert.Throws<T>(action);
        }
    }
}
