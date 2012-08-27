using System.Dynamic;
using System;
using System.Reflection;
using ImpromptuInterface.Internal.Support;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Class for TDD, used for mocking any dynamic object
    /// </summary>
    public class Mimic : DynamicObject,ICustomTypeProvider
    {
        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="arg"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Impromptu.InvokeConstructor(binder.ReturnType);
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = new Mimic();
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return true;
        }

        /// <summary>
        /// Override on DynamicObject
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            result = new Mimic();
            return true;
        }


#if SILVERLIGHT5

        /// <summary>
        /// Gets the custom Type.
        /// </summary>
        /// <returns></returns>
        public Type GetCustomType()
        {
            return this.GetDynamicCustomType();
        }
#endif
    }
}