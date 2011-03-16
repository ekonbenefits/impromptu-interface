using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Builds Expando-Like Objects with an inline Syntax
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImpromptuBuilder<TObjectProtoType>: ImpromptuObject where TObjectProtoType: new()
    {
		protected IDictionary<string,Type> _buildType;
		
		public ImpromptuBuilder(){
			_buildType = new Dictionary<string,Type>();
			Setup = new SetupTrampoline(this);
			Object = new BuilderTrampoline();
		}
		
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
        public readonly dynamic Object;
		
		public readonly dynamic Setup;


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
		
		public class SetupTrampoline:DynamicObject
        {
			ImpromptuBuilder<TObjectProtoType> _buider;
			public SetupTrampoline(ImpromptuBuilder<TObjectProtoType> builder){
				_buider = builder;
			}
			
            public override bool TryInvoke(InvokeBinder binder, dynamic[] args, out object result)
            {
				if (binder.CallInfo.ArgumentNames.Count != binder.CallInfo.ArgumentCount)
               		 throw new ArgumentException("Requires argument names for every argument");
	             
				foreach(var tKeyPair in binder.CallInfo.ArgumentNames.Zip(args, (n, a) => new KeyValuePair<string, Type>(n, a))){
					_buider._buildType[tKeyPair.Key]=tKeyPair.Value;
				}
				result = _buider;
				return true;
            }
        }
		
		public override bool TrySetMember(SetMemberBinder binder, dynamic value){
			if(value != null && !(value is Type)){
				return false;	
			}
			
			_buildType[binder.Name]=value;
			return true;
		}

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type tType;
			Type tBuildType;
			
			if(!_buildType.TryGetValue(binder.Name, out tBuildType))
				tBuildType = null;

            result = InvokeHelper(binder.CallInfo, args,tBuildType);
            if (TryTypeForName(binder.Name, out tType))
            {
                if (tType.IsInterface && result != null && tType.IsAssignableFrom(result.GetType()))
                {
                    result = Impromptu.DynamicActLike(result, tType);
                }
            }
            return true;

        }

        private static object InvokeHelper(CallInfo callinfo, IEnumerable<object> args, Type buildType =null)
        {
            IEnumerable<KeyValuePair<string, object>> keyValues =null;
            if (callinfo.ArgumentNames.Count == 0 && callinfo.ArgumentCount == 1)
            {
				var tArg =args.FirstOrDefault();
                keyValues = tArg as IDictionary<string, object>;
				if(keyValues ==null 
					&& tArg !=null
					&& tArg.GetType().IsNotPublic  
					&& Attribute.IsDefined(
									tArg.GetType(), 
									typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), 
									false)){
					var keyDict = new Dictionary<string,object>();
					foreach(var tProp in tArg.GetType().GetProperties()){
						keyDict[tProp.Name] = Impromptu.InvokeGet(tArg, tProp.Name);
					}
					keyValues = keyDict;
				}
            }

            if (keyValues == null && callinfo.ArgumentNames.Count != callinfo.ArgumentCount)
                throw new ArgumentException("Requires argument names for every argument");
            var result = buildType !=null 
				? Activator.CreateInstance(buildType) 
					: Activator.CreateInstance<TObjectProtoType>();
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
