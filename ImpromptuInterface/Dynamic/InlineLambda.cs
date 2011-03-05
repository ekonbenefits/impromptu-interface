using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Dynamic
{
    public static class Return<TR>
    {
        public static Func<TR> Arguments(Func<TR> del)
        {
            return del;
        }

        public static Func<T1, TR> Arguments<T1>(Func<T1, TR> del)
        {
            return del;
        }

    }

    public static class Void
    {
        public static Action Arguments(Action del)
        {
            return del;
        }
    }
}
