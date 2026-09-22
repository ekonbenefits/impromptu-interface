# Benchmarks

What a proxy costs, measured with [BenchmarkDotNet](https://benchmarkdotnet.org/).

```
dotnet run --project Benchmarks -c Release -- --filter '*'
dotnet run --project Benchmarks -c Release -- --filter '*' --job short   # quick, noisier
```

Each category's baseline is the same operation on a type that *declares* the interface, so a
ratio reads as "what the proxy adds", not as a bare duration. Those accessors are
`MethodImpl(NoInlining)`: inlined, the JIT elides them and the baseline measures 0 ns, which
makes every ratio meaningless.

Indicative numbers (`--job short`, Apple M-series, net8.0) — run them yourself before quoting:

| | vs. a declared type |
| --- | --- |
| `ActLike<IPoco>()`, proxy type already built | ~130× (≈560 ns) |
| read a property through the proxy | ~6× (≈5 ns) |
| write a property through the proxy | ~1.7× (≈4 ns) |
| call a method through the proxy | ~1.7× (≈13 ns) |

For contrast, in the same run `MethodInfo.Invoke` is ~4.4× the declared call and
`PropertyInfo.GetValue` ~10×, so a proxy call is cheaper than reflection; creating one is not,
which is the thing to hold onto rather than the absolute numbers.

## What used to be here

`Tests/UnitTestImpromptuInterface/SpeedTest.cs` held 28 `[Test]` methods that timed 500,000
iterations and asserted `Assert.Less(impromptuTime, reflectionTime)`. They failed on a loaded
machine and proved nothing on a fast one, and the CI workflow filtered them out with a category
that did not match them. They also measured **Dynamitey** — `Dynamic.InvokeSet`,
`CacheableInvocation`, `FastDynamicInvoke` — rather than anything in this library; not one of
them touched `ActLike`. They were left over from the 2013 split, and are gone.
