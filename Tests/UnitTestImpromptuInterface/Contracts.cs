using System;
using System.Collections.Generic;
using System.Linq;
using AnyUnit.Run;
using AnyUnit.Style.Nunit;
using AnyUnit.Constraints;
using ImpromptuInterface;
using ImpromptuInterface.Build;
using ImpromptuInterface.Optimization;

namespace UnitTestImpromptuInterface
{
    /// <summary>
    /// The contracts a proxy keeps with the object it wraps, and the key the proxy-type cache is
    /// built on. Neither is exercised by using a proxy, so neither shows up in the rest of the
    /// suite — and both are what a change to the emit would quietly break.
    /// </summary>
    [TestFixture]
    public class Contracts : Helper
    {
        // --- a proxy stands in for what it wraps ------------------------------------------

        [Test]
        public void Proxy_equals_the_object_it_wraps()
        {
            var poco = new PropPoco { Prop1 = "one" };
            var proxy = poco.ActLike<ISimpeleClassProps>();

            Assert.IsTrue(proxy.Equals(poco));               // the proxy answers for its target
            Assert.AreEqual(poco.GetHashCode(), proxy.GetHashCode());
        }

        [Test]
        public void Two_proxies_over_one_object_are_equal()
        {
            var poco = new PropPoco { Prop1 = "one" };
            var first = poco.ActLike<ISimpeleClassProps>();
            var second = poco.ActLike<ISimpeleClassProps>();

            Assert.IsFalse(ReferenceEquals(first, second));  // distinct proxies
            Assert.IsTrue(first.Equals(second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Test]
        public void Proxies_over_different_objects_are_not_equal()
        {
            var first = new PropPoco { Prop1 = "one" }.ActLike<ISimpeleClassProps>();
            var second = new PropPoco { Prop1 = "two" }.ActLike<ISimpeleClassProps>();

            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(first.Equals(null));
            Assert.IsFalse(first.Equals("not a proxy"));
        }

        [Test]
        public void Proxy_equality_is_one_directional()
        {
            // The proxy answers for its target, but the target knows nothing of the proxy, so
            // equality is asymmetric - and a hash lookup asks the *stored* key, which means a
            // proxy does not find its target in a dictionary. See #75.
            var poco = new PropPoco { Prop1 = "one" };
            var proxy = poco.ActLike<ISimpeleClassProps>();

            Assert.IsTrue(proxy.Equals(poco));
            Assert.IsFalse(poco.Equals(proxy));

            var keyedByTarget = new Dictionary<object, string> { { poco, "found" } };
            Assert.IsFalse(keyedByTarget.ContainsKey(proxy));

            var keyedByProxy = new Dictionary<object, string> { { proxy, "found" } };
            Assert.IsTrue(keyedByProxy.ContainsKey(poco));   // the other way round does work
        }

        [Test]
        public void ToString_forwards_to_the_target()
        {
            var poco = new PropPoco { Prop1 = "one" };
            var proxy = poco.ActLike<ISimpeleClassProps>();

            Assert.AreEqual(poco.ToString(), proxy.ToString());
        }

        // --- a proxy with no target -------------------------------------------------------

        [Test]
        public void A_proxy_built_without_a_target_says_so()
        {
            // #37: Activator.CreateInstance on a proxy *type* gives a proxy with nothing to
            // forward to. It used to fail on first use with "Cannot perform runtime binding on
            // a null reference", which names neither the cause nor the cure.
            var proxyType = new PropPoco { Prop1 = "one" }.ActLike<ISimpeleClassProps>().GetType();
            var orphan = (ISimpeleClassProps)Activator.CreateInstance(proxyType);

            var error = Assert.Throws<InvalidOperationException>(() => { var ignored = orphan.Prop1; });

            Assert.IsTrue(error.Message.Contains("no target"), "says what is wrong");
            Assert.IsTrue(error.Message.Contains("ActLike"), "says what to do instead");
        }

        [Test]
        public void An_orphan_proxy_says_so_for_every_member_not_only_the_forwarded_ones()
        {
            // Equals, GetHashCode and ToString read the target directly rather than through
            // IActLikeProxy.Original, and printing one in a log or putting it in a dictionary
            // is at least as likely as calling a member. They explain themselves too.
            var proxyType = new PropPoco { Prop1 = "one" }.ActLike<ISimpeleClassProps>().GetType();
            var orphan = (ISimpeleClassProps)Activator.CreateInstance(proxyType);

            Assert.Throws<InvalidOperationException>(() => orphan.ToString());
            Assert.Throws<InvalidOperationException>(() => orphan.GetHashCode());
            Assert.Throws<InvalidOperationException>(() => orphan.Equals(new PropPoco()));
            Assert.Throws<InvalidOperationException>(() => Impromptu.UndoActLike(orphan));
        }

        // --- getting back out -------------------------------------------------------------

        [Test]
        public void UndoActLike_returns_the_wrapped_object()
        {
            var poco = new PropPoco { Prop1 = "one" };
            var proxy = poco.ActLike<ISimpeleClassProps>();

            object original = Impromptu.UndoActLike(proxy);
            Assert.IsTrue(ReferenceEquals(poco, original));
        }

        [Test]
        public void UndoActLike_on_something_that_is_not_a_proxy_returns_it_unchanged()
        {
            var poco = new PropPoco { Prop1 = "one" };
            object same = Impromptu.UndoActLike(poco);
            Assert.IsTrue(ReferenceEquals(poco, same));
        }

        [Test]
        public void AllActLike_proxies_a_sequence()
        {
            var pocos = new[]
            {
                new PropPoco { Prop1 = "one" },
                new PropPoco { Prop1 = "two" },
            };

            var proxies = pocos.AllActLike<ISimpeleClassProps>().ToList();

            Assert.AreEqual(2, proxies.Count);
            Assert.AreEqual("one", proxies[0].Prop1);
            Assert.AreEqual("two", proxies[1].Prop1);
            Assert.IsTrue(proxies[0].Equals(pocos[0]));
        }

        // --- the proxy-type cache's key ---------------------------------------------------

        [Test]
        public void TypeHash_ignores_the_order_of_the_types()
        {
            var one = TypeHash.Create(new[] { typeof(ISimpeleClassProps), typeof(IDisposable) });
            var other = TypeHash.Create(new[] { typeof(IDisposable), typeof(ISimpeleClassProps) });

            Assert.AreEqual(one, other);                     // so the cache does not build the type twice
            Assert.AreEqual(one.GetHashCode(), other.GetHashCode());
            Assert.IsTrue(one == other);
            Assert.IsFalse(one != other);
        }

        [Test]
        public void TypeHash_distinguishes_different_interface_sets()
        {
            var one = TypeHash.Create(new[] { typeof(ISimpeleClassProps) });
            var more = TypeHash.Create(new[] { typeof(ISimpeleClassProps), typeof(IDisposable) });
            var other = TypeHash.Create(new[] { typeof(IDisposable) });

            Assert.AreNotEqual(one, more);                   // a subset is not the same key
            Assert.AreNotEqual(one, other);
            Assert.IsTrue(one != more);
            Assert.IsFalse(one.Equals(null));
            Assert.IsFalse(one.Equals("not a TypeHash"));
        }

        [Test]
        public void TypeHash_of_an_informal_interface_depends_on_it()
        {
            var one = TypeHash.Create(typeof(ISimpeleClassProps),
                new Dictionary<string, Type> { { "Prop1", typeof(string) } });
            var same = TypeHash.Create(typeof(ISimpeleClassProps),
                new Dictionary<string, Type> { { "Prop1", typeof(string) } });
            var different = TypeHash.Create(typeof(ISimpeleClassProps),
                new Dictionary<string, Type> { { "Prop1", typeof(int) } });
            var without = TypeHash.Create(new[] { typeof(ISimpeleClassProps) });

            Assert.AreEqual(one, same);
            Assert.AreNotEqual(one, different);
            Assert.AreNotEqual(one, without);                // an informal interface is part of the key
        }

        // --- constructing the target ------------------------------------------------------

        [Test]
        public void Create_constructs_a_struct_target()
        {
            // A dynamic invocation cannot see a value type's parameterless constructor, so that
            // case goes through Activator instead. Both paths land in the same place.
            // Through the params overload deliberately: Create<TTarget, TInterface>() with no
            // arguments binds to the `where TTarget : new()` overload, which is `new TTarget()`
            // and never reaches DynamicConstructor at all.
            var withoutArgs = Impromptu.Create<TallyStruct, ITally>(new object[0]);
            Assert.AreEqual(0, withoutArgs.Count);

            var withArgs = Impromptu.Create<TallyStruct, ITally>(7);
            Assert.AreEqual(7, withArgs.Count);
        }

        [Test]
        public void Create_chooses_a_constructor_by_the_arguments_runtime_types()
        {
            // Not Activator.CreateInstance: the overload is picked from what the arguments
            // actually are, as C#'s `new T(dynamicArg)` does.
            var fromInt = Impromptu.Create<TallyClass, ITally>(3);
            var fromString = Impromptu.Create<TallyClass, ITally>("12");

            Assert.AreEqual(3, fromInt.Count);
            Assert.AreEqual(12, fromString.Count);   // the string overload parsed it
        }

        // --- anonymous types --------------------------------------------------------------

        [Test]
        public void IsAnonymousType_recognises_one()
        {
            Assert.IsTrue(Util.IsAnonymousType(new { Anonymous = true }));
            Assert.IsFalse(Util.IsAnonymousType(new PropPoco()));
            Assert.IsFalse(Util.IsAnonymousType("a string"));
            Assert.IsFalse(Util.IsAnonymousType(null));
        }

        // --- casting between interfaces ---------------------------------------------------

        [Test]
        public void A_caster_currently_discards_a_non_interface_conversion()   // pinned, see #80
        {
            // TryConvert's second branch assigns `result = Target` when the target already is
            // the requested type, and then returns false regardless - so the conversion is
            // reported as failed and the assignment is discarded. See #80.
            var poco = new PropPoco { Prop1 = "one" };
            dynamic caster = Impromptu.ActLike(poco, typeof(ISimpeleClassProps));

            Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
            {
                PropPoco asTarget = caster;
            });
        }

        [Test]
        public void A_caster_will_not_convert_to_an_unrelated_type()
        {
            var poco = new PropPoco { Prop1 = "one" };
            dynamic caster = Impromptu.ActLike(poco, typeof(ISimpeleClassProps));

            Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() =>
            {
                Guid unrelated = caster;
            });
        }

        [Test]
        public void One_caster_converts_to_each_interface_it_was_given()
        {
            var poco = new PropPoco { Prop1 = "one" };

            // ActLike(object, params Type[]) hands back an ActLikeCaster, which builds the proxy
            // at the point of conversion - so one caster serves every interface asked of it.
            dynamic caster = Impromptu.ActLike(poco, typeof(ISimpeleClassProps), typeof(IPropPocoGet));

            ISimpeleClassProps asProps = caster;
            IPropPocoGet asGet = caster;

            Assert.AreEqual("one", asProps.Prop1);
            Assert.AreEqual("one", asGet.Prop1);
        }
    }
}
