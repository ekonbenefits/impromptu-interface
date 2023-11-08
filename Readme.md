net4.0/netstd2.0 framework to allow you to wrap any object (static or dynamic) with a static interface even though it didn't inherit from it. It does this by emitting cached dynamic binding code inside a proxy.

ImpromptuInterface is available Nuget [![NuGet](https://img.shields.io/nuget/dt/ImpromptuInterface.svg)](https://www.nuget.org/packages/ImpromptuInterface/)


Platform | Status
-------- | ------
NET4 (Win)    | [![Build status](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/dotnet48.yml/badge.svg)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/dotnet48.yml?query=branch%3Amaster)
NETSTD (Win/Mac/Linux)  | [![Build status](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ekonbenefits/impromptu-interface/actions/workflows/dotnet.yml?query=branch%3Amaster)
 

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
