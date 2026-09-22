using System;
using System.Runtime.CompilerServices;

namespace Benchmarks
{
    /// <summary>What a proxy is made over: an ordinary object with no interface of its own.</summary>
    public class Poco
    {
        public string Prop1 { get; set; } = "one";
        public long Prop2 { get; set; } = 2;
        public string Func(object arg) => "f:" + arg;
    }

    /// <summary>
    /// The same shape, declared — for the cost of not going through a proxy at all.
    /// The accessors are <c>NoInlining</c> so the comparison stays a real call: inlined, the
    /// JIT elides them entirely and the baseline measures 0 ns, which makes every ratio
    /// meaningless.
    /// </summary>
    public class Direct : IPoco
    {
        private string _prop1 = "one";
        private long _prop2 = 2;

        public string Prop1
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get => _prop1;
            [MethodImpl(MethodImplOptions.NoInlining)] set => _prop1 = value;
        }

        public long Prop2
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get => _prop2;
            [MethodImpl(MethodImplOptions.NoInlining)] set => _prop2 = value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string Func(object arg) => "f:" + arg;
    }

    public interface IPoco
    {
        string Prop1 { get; set; }
        long Prop2 { get; set; }
        string Func(object arg);
    }
}
