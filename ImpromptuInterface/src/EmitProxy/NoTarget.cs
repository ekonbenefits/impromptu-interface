// 
//  Copyright 2011 Ekon Benefits
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

namespace ImpromptuInterface.Build
{
    /// <summary>
    /// What a proxy forwards to before it has been given a target: every operation explains
    /// that there is nothing to forward to, rather than the proxy binding against null and
    /// reporting "Cannot perform runtime binding on a null reference".
    /// </summary>
    /// <remarks>
    /// A stand-in rather than a check on the way in: the emitted members all read the target
    /// through <see cref="IActLikeProxy.Original"/> and would be covered by a check there, but
    /// <c>Equals</c>, <c>GetHashCode</c> and <c>ToString</c> read the field directly, and a
    /// branch on that property costs about a tenth of a forwarded property set. One shared
    /// instance, so an un-initialized proxy allocates nothing extra.
    /// </remarks>
    internal sealed class NoTarget : DynamicObject
    {
        internal static readonly NoTarget Instance = new NoTarget();

        private NoTarget() { }

        /// <summary>Throws if <paramref name="target"/> is the stand-in, for a reader that cannot go through it.</summary>
        internal static void ThrowIfAbsent(object target)
        {
            if (ReferenceEquals(target, Instance))
                throw Absent();
        }

        private static InvalidOperationException Absent() =>
            new InvalidOperationException(
                "This proxy has no target. A proxy wraps an object and forwards to it, so it cannot be " +
                "constructed on its own - Activator.CreateInstance on a proxy type produces one with " +
                "nothing to forward to. Use ActLike on the object you want wrapped; for an object with " +
                "storage of its own, ActLike over an ExpandoObject.");

        public override bool TryGetMember(GetMemberBinder binder, out object result) => throw Absent();
        public override bool TrySetMember(SetMemberBinder binder, object value) => throw Absent();
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) => throw Absent();
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) => throw Absent();
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) => throw Absent();
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) => throw Absent();
        public override bool TryDeleteMember(DeleteMemberBinder binder) => throw Absent();
        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) => throw Absent();
        public override bool TryConvert(ConvertBinder binder, out object result) => throw Absent();
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result) => throw Absent();
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) => throw Absent();
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result) => throw Absent();
        public override IEnumerable<string> GetDynamicMemberNames() => throw Absent();

        // The proxy's own Equals/GetHashCode/ToString forward to these, so they explain
        // themselves too rather than answering for the stand-in.
        public override string ToString() => throw Absent();
        public override bool Equals(object obj) => throw Absent();
        public override int GetHashCode() => throw Absent();
    }
}
