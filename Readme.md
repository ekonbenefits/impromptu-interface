net4.0/netstd2.0 framework to allow you to wrap any object (static or dynamic) with a static interface even though it didn't inherit from it. It does this by emitting cached dynamic binding code inside a proxy.

[![build](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/build.yml?query=branch%3Amaster)
[![Tests](https://img.shields.io/badge/tests-217%20passed-brightgreen.svg?style=flat)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/build.yml?query=branch%3Amaster)
[![Line coverage](https://img.shields.io/badge/line%20coverage-91%25-brightgreen.svg?style=flat)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/build.yml?query=branch%3Amaster)
[![Branch coverage](https://img.shields.io/badge/branch%20coverage-82%25-green.svg?style=flat)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/build.yml?query=branch%3Amaster)
[![NuGet](https://img.shields.io/nuget/dt/ImpromptuInterface.svg)](https://www.nuget.org/packages/ImpromptuInterface/)

Available on [NuGet](https://www.nuget.org/packages/ImpromptuInterface/). One workflow covers
`net40`, `netstandard2.0` and `net10.0` on Windows, macOS and Linux, plus a browser-wasm leg.
From the latest master build: [test results](https://ekonbenefits.github.io/impromptu-interface/tests/)
and [coverage](https://ekonbenefits.github.io/impromptu-interface/coverage/). Every run also
carries its own per-platform table in the run summary.


Some of the features of `ImpromptuInterface` have been moved into another library called [Dynamitey](https://github.com/ekonbenefits/dynamitey), `ImpromptuInterface` depends on `Dynamitey`.

`ImpromptuInterface.FSharp` has been spun off into [FSharp.Interop.Dynamic](https://github.com/fsprojects/FSharp.Interop.Dynamic) and also depends on `Dynamitey`.

`ImpromptuInterface.MVVM` has been spun off into it's own repo [ImpromptuInterface.MVVM](https://github.com/ekonbenefits/impromptu-interface.mvvm) and only receives maitenance updates.

### Quick Usage:

```csharp
    using ImpromptuInterface;
    using Dynamitey;

    public interface IMyInterface{

       string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }

        bool Meth1(int x);
   }

```

```csharp
   //Anonymous Class
    var anon = new {
             Prop1 = "Test",
             Prop2 = 42L,
             Prop3 = Guid.NewGuid(),
             Meth1 = Return<bool>.Arguments<int>(it => it > 5)
    };

    var myInterface = anon.ActLike<IMyInterface>();
```

## OR

```csharp
   //Dynamic Expando object
    dynamic expando = new ExpandoObject();
    expando.Prop1 ="Test";
    expando.Prop2 = 42L;
    expando.Prop3 = Guid.NewGuid();
    expando.Meth1 = Return<bool>.Arguments<int>(it => it > 5);
    IMyInterface myInterface = Impromptu.ActLike(expando);
```
