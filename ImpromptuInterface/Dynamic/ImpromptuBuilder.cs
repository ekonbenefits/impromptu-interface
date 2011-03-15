using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Builds Expando-Like Objects with an inline Syntax
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImpromptuBuilder<T>: ImpromptuObject where T: new()
    {

        /// <summary>
        /// Creates a prototype list
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public dynamic List(params dynamic[] contents)
        {
            return new ImpromptuList(contents);
        }


        /// <summary>
        /// Alternative name for <see cref="List"/>
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public dynamic Array(params dynamic[] contents)
        {
            return List(contents);
        }

        /// <summary>
        /// Creates a Prototype object.
        /// </summary>
        /// <value>The object.</value>
        public readonly dynamic Object = new BuilderTrampoline();

        ///<summary>
        /// Trampoline for pulder
        ///</summary>
        public class BuilderTrampoline:DynamicObject
        {

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                result =InvokeHelper(binder.CallInfo, args);
                return true;
            }
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type tType;
            result = InvokeHelper(binder.CallInfo, args);
            if (TryTypeForName(binder.Name, out tType))
            {
                if (tType.IsInterface && result != null && tType.IsAssignableFrom(result.GetType()))
                {
                    result = Impromptu.DynamicActLike(result, tType);
                }
            }
            return true;

        }

        private static object InvokeHelper(CallInfo callinfo, IEnumerable<object> args)
        {
            IEnumerable<KeyValuePair<string, object>> keyValues =null;
            if (callinfo.ArgumentNames.Count == 0 && callinfo.ArgumentCount == 1)
            {
                keyValues = args.FirstOrDefault() as IDictionary<string, object>;
            }

            if (keyValues == null && callinfo.ArgumentNames.Count != callinfo.ArgumentCount)
                throw new ArgumentException("Requires argument names for every argument");
            var result = Activator.CreateInstance<T>();
            var tDict = result as IDictionary<string, object>;
            keyValues = keyValues ?? callinfo.ArgumentNames.Zip(args, (n, a) => new KeyValuePair<string, object>(n, a));
            foreach (var tArgs in keyValues)
            {
                if (tDict != null)
                {
                    tDict[tArgs.Key] = tArgs.Value;
                }
                else
                {
                    Impromptu.InvokeSet(result, tArgs.Key, tArgs.Value);
                }
            }
            return result;
        }
    }
}
