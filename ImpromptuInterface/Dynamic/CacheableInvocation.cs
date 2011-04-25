using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// <param name="convertExplict">if set to <c>true</c> [convert explict].</param>
        /// <returns></returns>
        public static CacheableInvocation CreateConvert(Type convertType, bool convertExplict=false)
        {
            return new CacheableInvocation(InvocationKind.Convert, convertType: convertType, convertExplict: convertExplict);
        }


        private readonly int _argCount;
        private readonly string[] _argNames;
        private readonly bool _staticContext;
        private Type _context;
        private CallSite _callSite;
        private CallSite _callSite2;
        private CallSite _callSite3;
        private CallSite _callSite4;
        private bool _convertExplict;
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
        /// <param name="convertExplict">if set to <c>true</c> [convert explict].</param>
        /// <param name="storedArgs">The stored args.</param>
        public CacheableInvocation(InvocationKind kind,
                                   String_OR_InvokeMemberName name=null,
                                   int argCount =0,
                                   string[] argNames =null,
                                   object context = null,
                                   Type convertType = null,
                                   bool convertExplict = false, 
                                   object[] storedArgs = null)
            : base(kind, name, storedArgs)
        {

            _convertType = convertType;
            _convertExplict = convertExplict;

            _argNames = argNames ?? new string[] {};

            if (storedArgs != null)
            {
                _argCount = storedArgs.Length;
                string[] tArgNames;
                Args = Impromptu.GetArgsAndNames(storedArgs, out tArgNames);
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
            
            if(_argCount > 0)//setup argname array
            {
                var tBlank = new string[_argCount];
                if(_argNames.Length !=0)
                    Array.Copy(_argNames,0, tBlank, tBlank.Length - _argNames.Length, tBlank.Length);
                _argNames = tBlank;
            }

            if (context != null)
            {
                var tDummy = context.GetTargetContext(out _context, out _staticContext);
            }
            else
            {
                _context = typeof (object);
            }


        }

        public override object Invoke(object target, params object[] args)
        {

            if (args == null)
            {
                args = new object[]{null};
            }

            if (args.Length != _argCount)
            {
                throw new ArgumentException("args",string.Format("Incorrect number of Arguments for CachedInvocation, Expected:{0}", _argCount));
            }

            switch (Kind)
            {
                case InvocationKind.Constructor:
                    var tTarget = (Type) target;
                    return InvokeHelper.InvokeConstructorCallSite(tTarget, tTarget.IsValueType, args, _argNames, _context,
                                                                  ref _callSite);
                case InvocationKind.Convert:
                    return InvokeHelper.InvokeConvertCallSite(target, _convertExplict, _convertType, _context,
                                                              ref _callSite);
                case InvocationKind.Get:
                    return InvokeHelper.InvokeGetCallSite(target, Name.Name, _context, _staticContext, ref _callSite);
                case InvocationKind.Set:
                    InvokeHelper.InvokeSetCallSite(target, Name.Name, args.First(), _context, _staticContext, ref _callSite);
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