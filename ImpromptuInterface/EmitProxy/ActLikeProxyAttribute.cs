using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    /// <summary>
    /// Meta info describing proxy usage. Can be used to preload proxy.
    /// </summary>
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
