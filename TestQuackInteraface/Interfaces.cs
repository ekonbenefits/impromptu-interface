using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestQuackInteraface
{


    public interface SimpeleAnnoymousClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }

    public interface SimpleStringProperty
    {
        int Length { get; }

    }

    public interface SimpleStringMethod
    {
        bool StartsWith(string value);

    }

    public interface SimpleTest0
    {
        string Test0Prop1 { get; set; }

        bool Test0Meth1();
    }

    public interface ComplexTest1
    {
        string Test1Prop1 { get; set; }

        void Test1VoidMeth();

        void Test1VoidMeth(string arg1);

        Guid Test1GuidMeth();

        Guid Test1GuidLong();

        TR Test1Generic<TR,T>();
    }

    public interface ComplexTest2<T>
    {
        T Test2Prop1 { get; set; }

        TR Test2Generic<TR>(T test);
    }
}
