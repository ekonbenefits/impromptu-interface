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
		
		public static Func<T1, T2, TR> Arguments<T1,T2>(Func<T1,T2,TR> del)
        {
            return del;
        }
		
			
		public static Func<T1, T2, T3, TR> Arguments<T1,T2,T3>(Func<T1,T2,T3, TR> del)
        {
            return del;
        }
		
		public static Func<T1, T2, T3,T4, TR> Arguments<T1,T2,T3,T4>(Func<T1,T2,T3,T4, TR> del)
        {
            return del;
        }
		
		public static Func<T1, T2, T3,T4, T5, TR> Arguments<T1,T2,T3,T4,T5>(Func<T1,T2,T3,T4,T5, TR> del)
        {
            return del;
        }
		public static Func<T1, T2, T3,T4, T5,T6, TR> Arguments<T1,T2,T3,T4,T5,T6>(Func<T1,T2,T3,T4,T5,T6, TR> del)
        {
            return del;
        }
		
		public static Func<T1, T2, T3,T4, T5,T6,T7, TR> Arguments<T1,T2,T3,T4,T5,T6,T7>(Func<T1,T2,T3,T4,T5,T6,T7, TR> del)
        {
            return del;
        }
		
		public static Func<T1, T2, T3,T4, T5,T6,T7,T8, TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Func<T1,T2,T3,T4,T5,T6,T7,T8, TR> del)
        {
            return del;
        }
		
		public static Func<T1, T2, T3,T4, T5,T6,T7,T8,T9, TR> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Func<T1,T2,T3,T4,T5,T6,T7,T8,T9, TR> del)
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
		
		 public static Action<T1> Arguments<T1>(Action<T1> del)
        {
            return del;
        }
		
		 public static Action<T1,T2> Arguments<T1,T2>(Action<T1,T2> del)
        {
            return del;
        }
		
		 public static Action<T1,T2,T3> Arguments<T1,T2,T3>(Action<T1,T2,T3> del)
        {
            return del;
        }
		 public static Action<T1,T2,T3,T4> Arguments<T1,T2,T3,T4>(Action<T1,T2,T3,T4> del)
        {
            return del;
        }
		
		 public static Action<T1,T2,T3,T4,T5> Arguments<T1,T2,T3,T4,T5>(Action<T1,T2,T3,T4,T5> del)
        {
            return del;
        }
		
		 public static Action<T1,T2,T3,T4,T5,T6> Arguments<T1,T2,T3,T4,T5,T6>(Action<T1,T2,T3,T4,T5,T6> del)
        {
            return del;
        }
		
		 public static Action<T1,T2,T3,T4,T5,T6,T7> Arguments<T1,T2,T3,T4,T5,T6,T7>(Action<T1,T2,T3,T4,T5,T6,T7> del)
        {
            return del;
        }
		
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8> Arguments<T1,T2,T3,T4,T5,T6,T7,T8>(Action<T1,T2,T3,T4,T5,T6,T7,T8> del)
        {
            return del;
        }
		 public static Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> Arguments<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Action<T1,T2,T3,T4,T5,T6,T7,T8,T9> del)
        {
            return del;
        }
    }
}
