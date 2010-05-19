using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestQuackInteraface
{


    public interface ISimpeleClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }

    public interface ISimpleStringProperty
    {
        int Length { get; }

    }

    public interface ISimpleStringMethod
    {
        bool StartsWith(string value);

    }

    public interface ISimpeleClassMeth
    {
        void Action1();
        void Action2(bool value);
        string Action3();
    }

    public interface IGenericMeth
    {
        string Action<T>(T arg);
    }

  
}
