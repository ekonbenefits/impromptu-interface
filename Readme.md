.net 4.0 framework to allow you to wrap any object (static or dynamic) with a static interface even though it didn't inherit from it. It does this by emitting cached dynamic binding code inside a proxy.

ImpromptuInterface is available Nuget [![NuGet](https://img.shields.io/nuget/dt/ImpromptuInterface.svg)](https://www.nuget.org/packages/ImpromptuInterface/)

You can find the latest bleed edge on MyGet [![MyGet Pre Release](https://img.shields.io/myget/dynamitey-ci/vpre/ImpromptuInterface.svg)](https://www.myget.org/feed/dynamitey-ci/package/nuget/ImpromptuInterface)

Platofrm | Status
-------- | ------
Windows | [![Build status](https://ci.appveyor.com/api/projects/status/36mhw90u9d7gmohb?svg=true)](https://ci.appveyor.com/project/jbtule/impromptu-interface)
Mac     | [![Build Status](https://travis-matrix-badges.herokuapp.com/repos/ekonbenefits/impromptu-interface/branches/master/2)](https://travis-ci.org/ekonbenefits/impromptu-interface)
Linux   | [![Build Status](https://travis-matrix-badges.herokuapp.com/repos/ekonbenefits/impromptu-interface/branches/master/1)](https://travis-ci.org/ekonbenefits/impromptu-interface)
 

Some of the features of `ImpromptuInterface` have been moved into another library called [Dynamitey](https://github.com/ekonbenefits/dynamitey), `ImpromptuInterface` depends on `Dynamitey`.

`ImpromptuInterface.FSharp` has been spun off into [FSharp.Interop.Dynamic](https://github.com/fsprojects/FSharp.Interop.Dynamic) and also depends on `Dynamitey`.

`ImpromptuInterface.MVVM` only exists for `ImpromptuInterface 6.X` and earlier.

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
    }

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
