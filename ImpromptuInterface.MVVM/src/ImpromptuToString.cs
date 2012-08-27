using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ImpromptuInterface.Dynamic;
using System.Linq;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Extension methods to create ImpromptuToString proxies;
    /// </summary>
    public static class ImpromptuToStringExtensions
    {

  

        /// <summary>
        /// Proxies everything to replace the ToString Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        /// <returns></returns>
        public static ImpromptuToString<T> ProxyToString<T>(this T target, Func<T,string> toStringDelegate)
        {
            return new ImpromptuToString<T>(target, toStringDelegate);
        }

        /// <summary>
        /// Proxies all items to replace the ToString Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets">The targets.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        /// <returns></returns>
        public static IEnumerable<ImpromptuToString<T>> ProxyAllToString<T>(this IEnumerable<T> targets, Func<T,string> toStringDelegate)
        {
            return targets.Select(it=> new ImpromptuToString<T>(it, toStringDelegate));
        }
    }

  
    /// <summary>
    /// Proxies Result to String
    /// </summary>
    
   
    public class ImpromptuResultToString: ImpromptuForwarder, IEnumerable
    {
     
        private IDictionary<Type, Func<object, string>> _dictionary = new Dictionary<Type, Func<object, string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuResultToString"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        public ImpromptuResultToString(object target)
            : base(target)
        {
        
        }

        public bool ContainsKey(Type type)
        {
            return _dictionary.ContainsKey(type);
        }

        public void Add<T>(Func<T, string> value)
        {
            Add(typeof(T), it => value((T)it));
        }

        public void Add(Type key, Func<object, string> value)
        {
            _dictionary.Add(key, value);
        }

        private IList<Type> BaseTypes(Type type, IList<Type> baseTypes = null)
        {
            if (baseTypes == null)
            {
                baseTypes = new List<Type>() { typeof(object) };
                foreach (var tType in type.GetInterfaces())
                {
                    baseTypes.Add(tType);
                }
            }
            baseTypes = baseTypes ?? new List<Type>();
            if (type.BaseType != null)
            {
                baseTypes = BaseTypes(type.BaseType, baseTypes);
                baseTypes.Add(type);
            }

            return baseTypes;
        }

        protected ImpromptuToString<T> GetProxy<T>(T value)
        {
            Func<object, string> tDelegate;

            if (!_dictionary.TryGetValue(typeof(T), out tDelegate))
            {
                var tList = _dictionary.Keys.Where(it => it.IsAssignableFrom(typeof (T)));
                if (tList.Any())
                {
                    var tListOfBaseTypes = BaseTypes(typeof (T));
                    tList =tList.OrderByDescending(tListOfBaseTypes.IndexOf);
                    tDelegate =_dictionary[tList.First()];
                }
                else
                {
                    tDelegate = it => it.ToString();
                }
            }


            return new ImpromptuToString<T>(value, it => tDelegate(it));
        }
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var tReturn = base.TryGetMember(binder, out result);
            if (!tReturn)
                return false;
            result = GetProxy((dynamic)result);
            return true;
        }

        public IEnumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }

    /// <summary>
    /// Proxy to override to string
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public class ImpromptuToString<TTarget>: ImpromptuForwarder
    {

        /// <summary>
        /// Null value representative object.
        /// </summary>
        /// <returns></returns>
        public static ImpromptuToString<TTarget>  NullValue(string toStringValue)
        {
            return NullValue(it => toStringValue);
        }

        /// <summary>
        /// Null  value representative object.
        /// </summary>
        /// <returns></returns>
        public static ImpromptuToString<TTarget> NullValue(Func<TTarget, string> toStringDelegate)
        {
            return new ImpromptuToString<TTarget>(default(TTarget), toStringDelegate);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ImpromptuInterface.MVVM.ImpromptuToString&lt;TTarget&gt;"/> to <see cref="TTarget"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TTarget(ImpromptuToString<TTarget> proxy)
        {
            return (TTarget)proxy.Target;
        }

        private readonly Func<TTarget, string> _toStringDelegate;



        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuToString&lt;TTarget&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="toStringDelegate">To string delegate.</param>
        public ImpromptuToString(TTarget target, Func<TTarget,string> toStringDelegate) : base(target)
        {
            _toStringDelegate = toStringDelegate;
        }

        public override string ToString()
        {
            return _toStringDelegate((TTarget)Target);
        }
    }
}
