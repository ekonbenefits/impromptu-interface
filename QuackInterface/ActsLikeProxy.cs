using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuackInterface
{
    public interface IActsLikeProxy
    {
        dynamic Original { get; }
    }


    public class ActsLikeProxy : IActsLikeProxy
    {
        public dynamic Original{ get; private set;}

        public ActsLikeProxy(dynamic original)
        {
            Original = original;
        }
    }
}
