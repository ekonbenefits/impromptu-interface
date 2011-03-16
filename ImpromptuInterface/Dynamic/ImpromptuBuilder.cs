using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;


namespace ImpromptuInterface.Dynamic
{

    /// <summary>
    /// Interface for simplistic builder options
    /// </summary>
    public interface IImpromptuBuilder
    {  
        
        /// <summary>
        /// Creates a prototype list
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        dynamic List(params dynamic[] contents);

        /// <summary>
        /// Alternative name for <see cref="List"/>
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        dynamic Array(params dynamic[] contents);

        /// <summary>
        /// Generates Object, use by calling with named arguments <code>builder.Object(Prop1:"test",Prop2:"test")</code>
        /// returns new object;
        /// </summary>
        dynamic Object { get; }

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        dynamic ObjectSetup(params dynamic[] constructorArgs);

        /// <summary>
        /// Setups up named builders 
        /// </summary>
        /// <value>The setup.</value>
        dynamic Setup { get; }
    }

    /// <summary>
    /// Builds Expando-Like Objects with an inline Syntax
    /// </summary>
    /// <typeparam name="TObjectProtoType">The type of the object proto type.</typeparam>
    public class ImpromptuBuilder<TObjectProtoType>: ImpromptuObject, IImpromptuBuilder where TObjectProtoType: new()
    {
		protected IDictionary<string,Activate> _buildType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuBuilder&lt;TObjectProtoType&gt;"/> class.
        /// </summary>
		public ImpromptuBuilder(){
            _buildType = new Dictionary<string, Activate>();
			Setup = new SetupTrampoline(this);
			Object = new BuilderTrampoline(this);
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
        public dynamic Object { get; private set; }

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        public dynamic ObjectSetup(params dynamic[] constructorArgs)
        {
            _buildType["Object"] = new Activate<TObjectProtoType>(constructorArgs);
            return this;
        }

        /// <summary>
        /// Trapoline for setting up Builders
        /// </summary>
        public dynamic Setup { get; private set; }


        ///<summary>
        /// Trampoline for builder
        ///</summary>
        public class BuilderTrampoline:DynamicObject
        {
            ImpromptuBuilder<TObjectProtoType> _buider;

            public BuilderTrampoline(ImpromptuBuilder<TObjectProtoType> builder)
            {
				_buider = builder;
			}

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                Activate tBuildType;
                if (!_buider._buildType.TryGetValue("Object", out tBuildType))
                    tBuildType = null;

                result = InvokeHelper(binder.CallInfo, args, tBuildType);
                return true;
            }
        }

        /// <summary>
        /// Trampoline for setup builder
        /// </summary>
		public class SetupTrampoline:DynamicObject
        {
			ImpromptuBuilder<TObjectProtoType> _buider;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImpromptuBuilder&lt;TObjectProtoType&gt;.SetupTrampoline"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
			public SetupTrampoline(ImpromptuBuilder<TObjectProtoType> builder){
				_buider = builder;
			}
			
            public override bool TryInvoke(InvokeBinder binder, dynamic[] args, out object result)
            {
				if (binder.CallInfo.ArgumentNames.Count != binder.CallInfo.ArgumentCount)
               		 throw new ArgumentException("Requires argument names for every argument");
                var tArgs = args.Select(it => it is Type ? new Activate(it) : (Activate) it);
                foreach (var tKeyPair in binder.CallInfo.ArgumentNames.Zip(tArgs, (n, a) => new KeyValuePair<string, Activate>(n, a)))
                {
					_buider._buildType[tKeyPair.Key]=tKeyPair.Value;
				}
				result = _buider;
				return true;
            }
        }
		
		public override bool TrySetMember(SetMemberBinder binder, dynamic value){
            if (value != null)
            {
                if (value is Type)
                {
                    _buildType[binder.Name] = new Activate(value);
                    return true;
                }

                if (value is Activate)
                {
                    _buildType[binder.Name] = value;
                    return true;
                }
            }
            else
            {
                _buildType[binder.Name] = null;
                return true;
            }
			return false;
		}

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type tType;

			Activate tBuildType;
			if(!_buildType.TryGetValue(binder.Name, out tBuildType))
				tBuildType = null;

            result = InvokeHelper(binder.CallInfo, args,tBuildType);
            if (TryTypeForName(binder.Name, out tType))
            {
                if (tType.IsInterface && result != null && !tType.IsAssignableFrom(result.GetType()))
                {
                    result = Impromptu.DynamicActLike(result, tType);
                }
            }
            return true;

        }

        private static object InvokeHelper(CallInfo callinfo, IEnumerable<object> args, Activate buildType =null)
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
				? buildType.Create() 
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
