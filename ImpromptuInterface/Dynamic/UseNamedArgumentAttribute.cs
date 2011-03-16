using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Attribute for Methods and Parameters on Custom Interfaces designed to be used with a dynamic implementation
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Method |
                       System.AttributeTargets.Parameter)]
    public class UseNamedArgumentAttribute : Attribute
    {

    }
}
