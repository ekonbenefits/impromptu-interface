// Candidate pin tests for ekonbenefits/impromptu-interface.
// Naming is intentionally generic (no calling-application terms) so these
// can live directly in Tests/UnitTestImpromptuInterface alongside Basic.cs / Generics.cs.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using ImpromptuInterface;

#if !SELFRUNNER
using AnyUnit.Run;
using AnyUnit.Style.Nunit;
using AnyUnit.Constraints;
using AnyUnit.Constraints.Pieces;
#endif

namespace UnitTestImpromptuInterface
{
    // ---- Support types (would go in Support/SupportDefinitions.cs) ----

    public interface ITaggedProps
    {
        string Name { get; set; }
    }

    // Empty marker/tag interface: no members of its own.
    public interface IMarkerTag
    {
    }

    /// <summary>
    /// Minimal stand-in for Dynamitey.DynamicObjects.Lazy: forwards every dynamic
    /// operation to a target that is only materialized on first access, and only
    /// implements the dynamic-dispatch protocol (TryGetMember/TryInvokeMember/etc.)
    /// -- it does NOT itself implement IList/IEnumerable/generic collection interfaces.
    /// </summary>
    public class LazyForwarder : DynamicObject
    {
        private readonly Func<object> _factory;
        private object _target;
        public int FactoryCallCount { get; private set; }

        public LazyForwarder(Func<object> factory)
        {
            _factory = factory;
        }

        private object Target
        {
            get
            {
                if (_target == null)
                {
                    _target = _factory();
                    FactoryCallCount++;
                }
                return _target;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var tTarget = Target;
            result = tTarget.GetType().InvokeMember(binder.Name, BindingFlags.GetProperty, null, tTarget, null);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var tTarget = Target;
            result = tTarget.GetType().InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, tTarget, args);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var tTarget = Target;
            result = tTarget.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, tTarget, indexes);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var tTarget = Target;
            var tArgs = indexes.Concat(new[] { value }).ToArray();
            tTarget.GetType().InvokeMember("Item", BindingFlags.SetProperty, null, tTarget, tArgs);
            return true;
        }
    }

    [TestFixture]
    public class PinTests : Helper
    {
        // 1. Impromptu.DynamicActLike(obj, Type) -- the runtime-Type overload has
        //    no existing coverage anywhere in the suite.
        [Test]
        public void DynamicActLikeWithRuntimeTypeTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Name = "original";

            Type tRuntimeType = typeof(ITaggedProps); // only known at run time in real usage

            dynamic tActsLike = Impromptu.DynamicActLike(tNew, tRuntimeType);

            Assert.AreEqual("original", tActsLike.Name);

            tActsLike.Name = "updated";
            Assert.AreEqual("updated", tNew.Name);

            Assert.IsInstanceOf<ITaggedProps>(tActsLike);
        }

        // Same overload, exercised twice with the same runtime type to make sure
        // repeated calls keep behaving (proxy-type reuse/caching should be transparent).
        [Test]
        public void DynamicActLikeWithRuntimeTypeRepeatedTest()
        {
            dynamic tNewA = new ExpandoObject();
            tNewA.Name = "a";
            dynamic tNewB = new ExpandoObject();
            tNewB.Name = "b";

            Type tRuntimeType = typeof(ITaggedProps);

            dynamic tActsLikeA = Impromptu.DynamicActLike(tNewA, tRuntimeType);
            dynamic tActsLikeB = Impromptu.DynamicActLike(tNewB, tRuntimeType);

            Assert.AreEqual("a", tActsLikeA.Name);
            Assert.AreEqual("b", tActsLikeB.Name);
        }

        // 2. ActLike<IList<T>> proxied over a lazily-populated dynamic object that
        //    itself implements none of IList<T>/ICollection<T>/IEnumerable<T> --
        //    every member (Count, indexer, enumeration) must be synthesized by the proxy.
        [Test]
        public void GenericListInterfaceOverLazyDynamicTest()
        {
            var tBacking = new List<int> { 1, 2, 3 };
            var tForwarder = new LazyForwarder(() => tBacking);

            IList<int> tActsLike = Impromptu.ActLike<IList<int>>(tForwarder);

            // Not yet materialized.
            Assert.AreEqual(0, tForwarder.FactoryCallCount);

            Assert.AreEqual(3, tActsLike.Count);
            Assert.AreEqual(2, tActsLike[1]);
            Assert.IsTrue(tActsLike.Contains(3));

            tActsLike.Add(4);
            Assert.AreEqual(4, tBacking.Count);
            Assert.AreEqual(4, tBacking[3]);

            var tCollected = tActsLike.ToList(); // exercises GetEnumerator()
            Assert.AreEqual(new List<int> { 1, 2, 3, 4 }, tCollected);

            // Only materialized once despite multiple member calls above.
            Assert.AreEqual(1, tForwarder.FactoryCallCount);
        }

        // 3. Combining a real proxy interface with an empty marker/tag interface via
        //    the typeof(...) overload -- DoubleInterfacetest only covers combining two
        //    interfaces that both declare members.
        [Test]
        public void CombineWithEmptyMarkerInterfaceTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Name = "tagged";

            ITaggedProps tActsLike = Impromptu.ActLike<ITaggedProps>(tNew, typeof(IMarkerTag));

            Assert.AreEqual("tagged", tActsLike.Name);
            Assert.IsInstanceOf<IMarkerTag>(tActsLike);
        }
    }
}
