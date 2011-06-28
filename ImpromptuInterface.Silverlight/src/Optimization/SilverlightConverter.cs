using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ImpromptuInterface.Optimization
{
  
    /// <summary>
    /// Dummy Framework Element to access Properties
    /// </summary>
    public class SilverConverterFE:FrameworkElement
    {


        private static IDictionary<Type, DependencyProperty> _dependencyProperties =
            new Dictionary<Type, DependencyProperty>();

            

            public static DependencyProperty GetProperty(Type type)
            {
                DependencyProperty tProp;
                if (!_dependencyProperties.TryGetValue(type, out tProp))
                {

                    var tType2 = type.IsValueType &&
                                 (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof (Nullable<>))
                                     ? typeof (Nullable<>).MakeGenericType(type)
                                     : type;

                    tProp = DependencyProperty.Register(string.Format("SilverConvert_{0}", type.GetHashCode()), tType2,
                                                        typeof (SilverConverterFE),
                                                         new PropertyMetadata(null));
                    _dependencyProperties[type] = tProp;
                }

                return tProp;
            }
    }
     public class SilverConvertertBV
     {
       
     }
    public class SilverConvertertDC
    {
        public SilverConvertertDC(string value)
        {
            StringValue = value;
        }
        public string StringValue { get; set; }
    }

   
}
