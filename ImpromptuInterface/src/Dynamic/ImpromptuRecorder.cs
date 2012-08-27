// 
//  Copyright 2011  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.


using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Proxy that Records Dynamic Invocations on an object
    /// </summary>
    [Serializable]
    public class ImpromptuRecorder:ImpromptuForwarder
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRecorder"/> class.
        /// </summary>
        public ImpromptuRecorder():base(new ImpromptuDummy())
        {
            Recording = new List<Invocation>();
        }

        /// <summary>
        /// Gets or sets the recording.
        /// </summary>
        /// <value>The recording.</value>
        public IList<Invocation> Recording { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRecorder"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ImpromptuRecorder(object target) : base(target)
        {
            Recording = new List<Invocation>();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuRecorder"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuRecorder(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {


            Recording = info.GetValue<IList<Invocation>>("Recording");
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
            info.AddValue("Recording", Recording);
        }
#endif

        /// <summary>
        /// Replays the recording on target.
        /// </summary>
        /// <param name="target">The target.</param>
        public T ReplayOn<T>(T target)
        {
            foreach (var tInvocation in Recording)
            {
                tInvocation.InvokeWithStoredArgs(target);
            }

            return target;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result))
            {
                Recording.Add(new Invocation(InvocationKind.Get,binder.Name));
                return true;
            }
            return false;
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            if (base.TrySetMember(binder, value))
            {
                Recording.Add(new Invocation(InvocationKind.Set,binder.Name,value));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the invoke member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="args">The args.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result))
            {
                Recording.Add(new Invocation(InvocationKind.InvokeMemberUnknown, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo, args)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the get.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, object[] indexes, out object result)
        {
            if (base.TryGetIndex(binder, indexes, out result))
            {
                Recording.Add(new Invocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, indexes)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the set.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetIndex(System.Dynamic.SetIndexBinder binder, object[] indexes, object value)
        {
            if (base.TrySetIndex(binder, indexes, value))
            {
                var tCombinedArgs = indexes.Concat(new[] { value }).ToArray();
                Recording.Add(new Invocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, tCombinedArgs)));
                return true;
            }
            return false;
        }

    }
}
