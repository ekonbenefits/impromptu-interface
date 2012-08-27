using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using ImpromptuInterface.Internal.Support;
using ImpromptuInterface.Optimization;

namespace ImpromptuInterface.Dynamic
{

    public interface IFluentMatch
    {
        string Value { get;}
    }

    [Serializable]
    public class ImpromptuMatch : ImpromptuObject, IFluentMatch
    {
        private readonly Match _match;
        private readonly Regex _regex;
        public ImpromptuMatch(Match match, Regex regex =null)
        {
            _match = match;
            _regex = regex;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuList"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ImpromptuMatch(SerializationInfo info, 
           StreamingContext context):base(info,context)
        {
            _match = info.GetValue<Match>("_match");
            _regex = info.GetValue<Regex>("_regex");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
         
            base.GetObjectData(info,context);
            info.AddValue("_match", _match);
            info.AddValue("_regex", _regex);
        }
#endif

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (_regex == null)
                return Enumerable.Empty<string>();
            return _regex.GetGroupNames();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var tGroup = _match.Groups[binder.Name];
            Type outType;
            if (!TryTypeForName(binder.Name, out outType))
                outType = typeof (string);

            if (!tGroup.Success)
            {
                result = null;
                if (outType.IsValueType)
                    result = Impromptu.InvokeConstructor(outType);
                return true;
            }

            result = Impromptu.CoerceConvert(tGroup.Value, outType);
            return true;
        }

        public string this[int value]
        {
            get
            {
                var tGroup = _match.Groups[value];

                if (!tGroup.Success)
                {
                    return null;
                }
                return tGroup.Value;
            }
        }

        public string this[string value]
        {
            get
            {
                var tGroup = _match.Groups[value];

                if (!tGroup.Success)
                {
                    return null;
                }
                return tGroup.Value;
            }
        }

        string IFluentMatch.Value
        {
            get { return _match.Value; }
        }

        public override string ToString()
        {
            return _match.ToString();
        }
    }
}
