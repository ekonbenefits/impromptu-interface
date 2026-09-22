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

## Against Castle DynamicProxy

The other run-time proxy generator, doing the same job — duck-typing an object that does not
implement the interface:

| | `ActLike` | Castle |
| --- | --- | --- |
| create the proxy (type already built) | ≈576 ns | ≈588 ns |
| call a method through it | ≈13 ns | ≈84 ns |
| read a property through it | ≈5 ns | ≈68 ns |

Creating one costs the same; going through one does not. The difference is what the emitted
proxy body does: `ActLike` bakes a DLR call site per member into the proxy itself, while Castle
emits a proxy that hands every call to an interceptor as an `IInvocation`, and the interceptor
then has to find the target member and invoke it.

Read that as a comparison of *this capability*, not of the two libraries. Castle does not bind
to a target that lacks the interface, so the forwarding is the caller's to write —
`DuckInterceptor` here, with the resolved `MethodInfo` cached per interface method, which is the
fast honest version of it. A Castle proxy over a target that *does* implement the interface is a
different and much cheaper thing, and not what `ActLike` is for.

## What used to be here

`Tests/UnitTestImpromptuInterface/SpeedTest.cs` held 28 `[Test]` methods that timed 500,000
iterations and asserted `Assert.Less(impromptuTime, reflectionTime)`. They failed on a loaded
machine and proved nothing on a fast one, and the CI workflow filtered them out with a category
that did not match them. They also measured **Dynamitey** — `Dynamic.InvokeSet`,
`CacheableInvocation`, `FastDynamicInvoke` — rather than anything in this library; not one of
them touched `ActLike`. They were left over from the 2013 split, and are gone.
