using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActLikeProxyAttribute:Attribute
    {
        public ActLikeProxyAttribute(Type[] interfaces, Type context)
        {
            Interfaces = interfaces;
            Context = context;
        }

        public Type[] Interfaces { get; set; }
        public Type Context { get; set; }
    }
}
