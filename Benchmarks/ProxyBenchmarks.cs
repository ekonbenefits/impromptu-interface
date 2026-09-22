using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ImpromptuInterface;

namespace Benchmarks
{
    /// <summary>
    /// What a proxy costs once its type exists: creating one, and calling through it.
    /// The baseline in each group is the same operation on a type that declares the interface,
    /// so a number reads as "how much the proxy adds", not as a bare duration.
    /// </summary>
    [MemoryDiagnoser]
    [CategoriesColumn]
    // One baseline per category, not per class.
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class ProxyBenchmarks
    {
        private readonly Poco _poco = new Poco();
        private readonly Direct _direct = new Direct();
        private IPoco _proxy;
        private IPoco _declared;
        private PropertyInfo _prop1;
        private MethodInfo _func;
        private dynamic _dynamic;

        [GlobalSetup]
        public void Setup()
        {
            _proxy = _poco.ActLike<IPoco>();
            _declared = _direct;
            _prop1 = typeof(Poco).GetProperty(nameof(Poco.Prop1));
            _func = typeof(Poco).GetMethod(nameof(Poco.Func));
            _dynamic = _poco;
        }

        // --- making one -----------------------------------------------------------------------

        /// <summary>ActLike with the proxy type already built - the cost every call after the first.</summary>
        [Benchmark(Description = "ActLike<IPoco>() (type cached)"), BenchmarkCategory("create")]
        public IPoco ActLike_Cached() => _poco.ActLike<IPoco>();

        [Benchmark(Baseline = true, Description = "new Direct() (no proxy)"), BenchmarkCategory("create")]
        public IPoco Create_Declared() => new Direct();

        // --- reading a property ---------------------------------------------------------------

        [Benchmark(Description = "proxy.Prop1"), BenchmarkCategory("get")]
        public string Get_Proxy() => _proxy.Prop1;

        [Benchmark(Baseline = true, Description = "declared.Prop1"), BenchmarkCategory("get")]
        public string Get_Declared() => _declared.Prop1;

        [Benchmark(Description = "dynamic .Prop1"), BenchmarkCategory("get")]
        public string Get_Dynamic() => _dynamic.Prop1;

        [Benchmark(Description = "PropertyInfo.GetValue"), BenchmarkCategory("get")]
        public object Get_Reflection() => _prop1.GetValue(_poco, null);

        // --- writing a property ---------------------------------------------------------------

        [Benchmark(Description = "proxy.Prop1 = v"), BenchmarkCategory("set")]
        public void Set_Proxy() => _proxy.Prop1 = "x";

        [Benchmark(Baseline = true, Description = "declared.Prop1 = v"), BenchmarkCategory("set")]
        public void Set_Declared() => _declared.Prop1 = "x";

        [Benchmark(Description = "PropertyInfo.SetValue"), BenchmarkCategory("set")]
        public void Set_Reflection() => _prop1.SetValue(_poco, "x", null);

        // --- calling a method -----------------------------------------------------------------

        [Benchmark(Description = "proxy.Func(arg)"), BenchmarkCategory("call")]
        public string Call_Proxy() => _proxy.Func("a");

        [Benchmark(Baseline = true, Description = "declared.Func(arg)"), BenchmarkCategory("call")]
        public string Call_Declared() => _declared.Func("a");

        [Benchmark(Description = "dynamic .Func(arg)"), BenchmarkCategory("call")]
        public string Call_Dynamic() => _dynamic.Func("a");

        [Benchmark(Description = "MethodInfo.Invoke"), BenchmarkCategory("call")]
        public object Call_Reflection() => _func.Invoke(_poco, new object[] { "a" });
    }
}
