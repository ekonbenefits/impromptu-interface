using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestQuackInteraface
{


    public interface SimpeleClassProps
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

    public interface SimpeleClassMeth
    {
        void Action1();
        void Action2(bool value);
        string Action3();
    }

  
}
