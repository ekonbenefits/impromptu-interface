using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImpromptuInterface
{
    public class ImpromptuFactory:ImpromptuObject
    {

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = GetInstanceForDynamicMember(binder.Name);
            return result != null;
        }

        protected virtual object CreateType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public virtual object GetInstanceForDynamicMember(string memberName)
        {
            Type type;
            if (TryTypeForName(memberName, out type))
            {
                return CreateType(type);
            }
            return null;
        }
    }

    public class ImpromptuSingleInstancesFactory : ImpromptuFactory
    {
        protected readonly Dictionary<string, dynamic> _hashFactoryTypes= new Dictionary<string, dynamic>();
        protected readonly object _lockTable = new object();


        public override object GetInstanceForDynamicMember(string memberName)
        {
            lock (_lockTable)
            {
                if (!_hashFactoryTypes.ContainsKey(memberName))
                {
                    Type type;
                    if (TryTypeForName(memberName, out type))
                    {
                        _hashFactoryTypes.Add(memberName, CreateType(type));
                    }
                    else
                    {
                        return null;
                    }

                }

                return _hashFactoryTypes[memberName];
            }
        }
    }
}
