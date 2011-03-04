impromptu-interface http://code.google.com/p/impromptu-interface/

C# 4.0 framework to allow you to wrap any object with a static Duck Typing Interface, emits cached dynamic binding code inside a proxy.

Copyright 2010 Ekon Benefits
Apache Licensed: http://www.apache.org/licenses/LICENSE-2.0

Author:
Jay Tuley jay+code@tuley.name

Usage:
   public interface ISimpeleClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }
    var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

    ISimpeleClassProps tActsLike = tAnon.ActLike<ISimpeleClassProps>();
Or
    dynamic tNew = new ExpandoObject();
    tNew.Prop1 = "Test";
    tNew.Prop2 = 42L;
    tNew.Prop3 = Guid.NewGuid();

    ISimpeleClassProps tActsLike = Impromptu.ActLike<ISimpeleClassProps>(tNew);
Also Contains some primitive base classes:

ImpromptuObject --Similar to DynamicObject but the expected static return type from the wrapped interface can be queried.
ImpromptuFactory -- Functional base class, used to create fluent factories with less boilerplate
ImpromptuDictionary -- Similar to ExpandoObject but returns default of the static return type if the property has never been set.