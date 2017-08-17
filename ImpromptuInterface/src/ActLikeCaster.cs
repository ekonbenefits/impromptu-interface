using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ImpromptuInterface
{

    public class ActLikeCaster: DynamicObject
    {
        public object Target { get; }
        private List<Type> _interfaceTypes;

        public override bool TryConvert(System.Dynamic.ConvertBinder binder, out object result)
        {
            result = null;

            if (binder.Type.IsInterface)
            {
                _interfaceTypes.Insert(0, binder.Type);
                result = Impromptu.DynamicActLike(Target, _interfaceTypes.ToArray());
                return true;
            }

            if(binder.Type.IsInstanceOfType(Target))
            {
                result = Target;
            }

            return false;
        }


        public ActLikeCaster(object target, IEnumerable<Type> types)
        {
            Target = target;
            _interfaceTypes = types.ToList();
        }

    }
}
