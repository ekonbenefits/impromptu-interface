C# 4.0 (.net & silverlight) framework to allow you to wrap any object (static or dynamic) with a static interface even though it didn't inherit from it. It does this by emitting cached dynamic binding code inside a proxy. By expanding on the DLR plumbing used to implement this library, it has grown to support more general dynamic implementations.

See [Wiki](http://github.com/ekonbenefits/impromptu-interface/wiki) for full examples of all features. Other features include Really Late Binding, Inline Syntaxes, Currying, some dynamic extensions for FSharp, and unsealed Dynamic Objects including a fully dynamic MVVM ViewModel

###Roadmap: 
 - 6.5 
   - obsolete duplicate functionality with  [Dynamitey](https://github.com/ekonbenefits/dynamitey) 
   - Remove ImpromptuInterface.FSharp replaced by [FSharp.Dynamic](https://github.com/ekonbenefits/FSharp.Dynamic)
 - 7.0
   - Strip out Obsolete Code
   - Full PCL port of Interface Wrapping
   - Tool to pregenerate proxies for WinRT

###Quick Usage:

```csharp
    using ImpromptuInterface;
    using ImpromptuInterface.Dynamic;

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

##OR##

```csharp
   //Dynamic Expando object
    dynamic expando = Build<ExpandoObject>.NewObject(
             Prop1: "Test",
             Prop2: 42L,
             Prop3: Guid.NewGuid(),
             Meth1: Return<bool>.Arguments<int>(it => it > 5)
    );

    IMyInterface myInterface = Impromptu.ActLike(expando);
```

##ALSO##

```fsharp
    //In F#
    let expando:obj = !?Build<ExpandoObject>.NewObject (
    			dynArg("Test")         ? Prop1,
				dynArg(42L)            ? Prop2,
				dynArg(Guid.NewGuid()) ? Prop3,
				dynArg(Return<bool>.Arguments<int>(fun x -> x > 5))
                                                       ? Meth1
		        )

    let myInterface = expando.ActLike<IMyInterface>()
```

###Get The Code###
Project can be checked out from git repository, works with .net 4.0, Silverlight 4.0 & 5.0  and mono 2.10

###Get The Binaries###
use [NuGet](http://nuget.org ) Visual Studio Extension "Add Libary Package Refrence... from Visual Studio
or download the zip file.  Source Code for debugging the binaries can be provided automatically from [SymbolSource.org](http://www.symbolsource.org/Public/Home/VisualStudio)

###Get The Samples###
There is a Sample mercurial repository for code samples. Currently it has an ImpromptuInterface.MVVM based calculator implemented in WPF or Silverlight or F#. [sample repository](https://code.google.com/p/impromptu-interface/source/checkout?repo=sample).
