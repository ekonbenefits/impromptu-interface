using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Cacheable representation of an invocation without the target or arguments  also by default only does public methods to make it easier to cache.
    ///  /// </summary>
    [Serializable]
    public class CacheableInvocation:Invocation
    {
        /// <summary>
        /// Creates the cacheable convert call.
        /// </summary>
        /// <param name="convertType">Type of the convert.</param>
        /// <param name="convertExplicit">if set to <c>true</c> [convert explict].</param>
        /// <returns></returns>
        public static CacheableInvocation CreateConvert(Type convertType, bool convertExplicit=false)
        {
            return new CacheableInvocation(InvocationKind.Convert, convertType: convertType, convertExplicit: convertExplicit);
        }

        /// <summary>
        /// Creates the cacheable method or indexer or property call.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="callInfo">The callInfo.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static CacheableInvocation CreateCall(InvocationKind kind, String_OR_InvokeMemberName name = null, CallInfo callInfo = null,object context = null)
        {
            var tArgCount = callInfo != null ? callInfo.ArgumentCount : 0;
            var tArgNames = callInfo != null ? callInfo.ArgumentNames.ToArray() : null;

            return new CacheableInvocation(kind, name, tArgCount, tArgNames, context);
        }

        private readonly int _argCount;
        private readonly string[] _argNames;
        private readonly bool _staticContext;
        private Type _context;

        [NonSerialized]
        private CallSite _callSite;
        [NonSerialized]
        private CallSite _callSite2;
        [NonSerialized]
        private CallSite _callSite3;
        [NonSerialized]
        private CallSite _callSite4;

        private bool _convertExplicit;
        private Type _convertType;

     

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheableInvocation"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="argCount">The arg count.</param>
        /// <param name="argNames">The arg names.</param>
        /// <param name="context">The context.</param>
        /// <param name="convertType">Type of the convert.</param>
        /// <param name="convertExplicit">if set to <c>true</c> [convert explict].</param>
        /// <param name="storedArgs">The stored args.</param>
        public CacheableInvocation(InvocationKind kind,
                                   String_OR_InvokeMemberName name=null,
                                   int argCount =0,
                                   string[] argNames =null,
                                   object context = null,
                                   Type convertType = null,
                                   bool convertExplicit = false, 
                                   object[] storedArgs = null)
            : base(kind, name, storedArgs)
        {

            _convertType = convertType;
            _convertExplicit = convertExplicit;

            _argNames = argNames ?? new string[] {};

            if (storedArgs != null)
            {
                _argCount = storedArgs.Length;
                string[] tArgNames;
                Args = Util.GetArgsAndNames(storedArgs, out tArgNames);
                if (_argNames.Length < tArgNames.Length)
                {
                    _argNames = tArgNames;
                }
            }

            switch (kind) //Set required argcount values
            {
                case InvocationKind.GetIndex:
                    if (argCount < 1)
                    {
                        throw new ArgumentException("Arg Count must be at least 1 for a GetIndex", "argCount");
                    }
                    _argCount = argCount;
                    break;
                case InvocationKind.SetIndex:
                    if (argCount < 2)
                    {
                        throw new ArgumentException("Arg Count Must be at least 2 for a SetIndex", "argCount");
                    }
                    _argCount = argCount;
                    break;
                case InvocationKind.Convert:
                    _argCount = 0;
                    if(convertType==null)
                        throw new ArgumentNullException("convertType"," Convert Requires Convert Type ");
                    break;
                case InvocationKind.SubtractAssign:
                case InvocationKind.AddAssign:
                case InvocationKind.Set:
                    _argCount = 1;
                    break;
                case InvocationKind.Get:
                case InvocationKind.IsEvent:
                    _argCount = 0;
                    break;
                default:
                    _argCount = Math.Max(argCount, _argNames.Length);
                    break;
            }

            if (_argCount > 0)//setup argname array
            {
                var tBlank = new string[_argCount];
                if (_argNames.Length != 0)
                    Array.Copy(_argNames, 0, tBlank, tBlank.Length - _argNames.Length, tBlank.Length);
                else
                    tBlank = null;
                _argNames = tBlank;
            }


            if (context != null)
            {
#pragma warning disable 168
                var tDummy = context.GetTargetContext(out _context, out _staticContext);
#pragma warning restore 168
            }
            else
            {
                _context = typeof (object);
            }


        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(CacheableInvocation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) 
                && other._argCount == _argCount 
                && Equals(other._argNames, _argNames) 
                && other._staticContext.Equals(_staticContext)
                && Equals(other._context, _context) 
                && other._convertExplicit.Equals(_convertExplicit)
                && Equals(other._convertType, _convertType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as CacheableInvocation);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ _argCount;
                result = (result*397) ^ (_argNames != null ? _argNames.GetHashCode() : 0);
                result = (result*397) ^ _staticContext.GetHashCode();
                result = (result*397) ^ (_context != null ? _context.GetHashCode() : 0);
                result = (result*397) ^ _convertExplicit.GetHashCode();
                result = (result*397) ^ (_convertType != null ? _convertType.GetHashCode() : 0);
                return result;
            }
        }
      

        public override object Invoke(object target, params object[] args)
        {
            var tIContext = target as InvokeContext;
            if (tIContext !=null)
            {
                target = tIContext.Target;
            }

            if (args == null)
            {
                args = new object[]{null};
            }
           

            if (args.Length != _argCount)
            {
                switch (Kind)
                {
                    case InvocationKind.Convert:
                        if (args.Length > 0)
                        {
                            if (!Equals(args[0], _convertType))
                                throw new ArgumentException("CacheableInvocation can't change conversion type on invoke.", "args");
                        }
                        if (args.Length > 1)
                        {
                            if(!Equals(args[1], _convertExplicit))
                                throw new ArgumentException("CacheableInvocation can't change explict/implict conversion on invoke.", "args");
                        }

                        if(args.Length > 2)
                            goto default;
                        break;
                    default:
                        throw new ArgumentException("args", string.Format("Incorrect number of Arguments for CachedInvocation, Expected:{0}", _argCount));
                }
            }

            switch (Kind)
            {
                case InvocationKind.Constructor:
                    var tTarget = (Type) target;
                    return InvokeHelper.InvokeConstructorCallSite(tTarget, tTarget.IsValueType, args, _argNames,
                                                                  ref _callSite);
                case InvocationKind.Convert:
                    return InvokeHelper.InvokeConvertCallSite(target, _convertExplicit, _convertType, _context,
                                                              ref _callSite);
                case InvocationKind.Get:
                    return InvokeHelper.InvokeGetCallSite(target, Name.Name, _context, _staticContext, ref _callSite);
                case InvocationKind.Set:
                    InvokeHelper.InvokeSetCallSite(target, Name.Name, args[0], _context, _staticContext, ref _callSite);
                    return null;
                case InvocationKind.GetIndex:
                    return InvokeHelper.InvokeGetIndexCallSite(target, args, _argNames, _context, _staticContext, ref _callSite);
                case InvocationKind.SetIndex:
                    Impromptu.InvokeSetIndex(target, args);
                    return null;
                case InvocationKind.InvokeMember:
                    return InvokeHelper.InvokeMemberCallSite(target, Name, args, _argNames, _context, _staticContext, ref _callSite);
                case InvocationKind.InvokeMemberAction:
                    InvokeHelper.InvokeMemberActionCallSite(target, Name, args, _argNames, _context, _staticContext, ref _callSite);
                    return null;
                case InvocationKind.InvokeMemberUnknown:
                    {
                       
                            try
                            {
                                var tObj = InvokeHelper.InvokeMemberCallSite(target, Name, args, _argNames, _context, _staticContext, ref _callSite);
                                return tObj;
                            }
                            catch (RuntimeBinderException)
                            {
                               InvokeHelper.InvokeMemberActionCallSite(target, Name, args, _argNames, _context, _staticContext, ref _callSite2);
                            return null;

                            }
                          
                    }
                case InvocationKind.Invoke:
                    return InvokeHelper.InvokeDirectCallSite(target, args, _argNames, _context, _staticContext, ref _callSite);
                case InvocationKind.InvokeAction:
                    InvokeHelper.InvokeDirectActionCallSite(target, args, _argNames, _context, _staticContext, ref _callSite);
                    return null;
                case InvocationKind.InvokeUnknown:
                    {

                        try
                        {
                            var tObj = InvokeHelper.InvokeDirectCallSite(target, args, _argNames, _context, _staticContext, ref _callSite);
                            return tObj;
                        }
                        catch (RuntimeBinderException)
                        {
                            InvokeHelper.InvokeDirectActionCallSite(target, args, _argNames, _context, _staticContext, ref _callSite2);
                            return null;

                        }
                    }
                case InvocationKind.AddAssign:
                    InvokeHelper.InvokeAddAssignCallSite(target, Name.Name, args, _argNames, _context, _staticContext,ref _callSite,ref  _callSite2,ref _callSite3, ref _callSite4);
                    return null;
                case InvocationKind.SubtractAssign:
                    InvokeHelper.InvokeSubtractAssignCallSite(target, Name.Name, args, _argNames, _context, _staticContext, ref _callSite, ref _callSite2, ref _callSite3, ref _callSite4);
                    return null;
                case InvocationKind.IsEvent:
                    return InvokeHelper.InvokeIsEventCallSite(target, Name.Name, _context, ref _callSite);
                default:
                    throw new InvalidOperationException("Unknown Invocation Kind: " + Kind);
            }
        }

       
    }
}