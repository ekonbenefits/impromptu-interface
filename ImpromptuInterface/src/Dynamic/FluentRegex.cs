using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImpromptuInterface.Dynamic
{
    /// <summary>
    /// Extension Methods for fluent Regex
    /// </summary>
    public static class FluentRegex
    {
  
        public static IEnumerable<dynamic> FluentFilter(this IEnumerable<string> list, Regex regex)
        {
            return list.Select(it => regex.Match(it)).Where(it => it.Success).Select(it => new ImpromptuMatch(it, regex)).Cast<dynamic>();
        }

        public static IEnumerable<dynamic> Matches(string inputString, Regex regex)
        {
            var tMatches = regex.Matches(inputString);

            return tMatches.Cast<Match>().Where(it => it.Success).Select(it => new ImpromptuMatch(it, regex)).Cast<dynamic>();
        }

        public static dynamic Match(string inputString, Regex regex)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new ImpromptuMatch(tMatch, regex) : null;
        }

        public static dynamic FluentMatch(this Regex regex, string inputString)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new ImpromptuMatch(tMatch,regex) : null;
        }

        public static IEnumerable<dynamic> FluentMatches(this Regex regex, string inputString)
        {
            return Matches(inputString, regex);
        }

        public static T Match<T>(string inputString, Regex regex) where T : class
        {
            var tMatch = Match(inputString, regex);
            return tMatch == null ? null : Impromptu.DynamicActLike(tMatch, typeof(T));
        }

        public static T FluentMatch<T>(this Regex regex, string inputString) where T : class
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new ImpromptuMatch(tMatch, regex).ActLike<T>() : null;
        }

        public static IEnumerable<T> FluentMatches<T>(this Regex regex,string inputString) where T : class
        {
            return Matches(inputString,regex).AllActLike<T>();
        }

        public static IEnumerable<T> FluentFilter<T>(this IEnumerable<string> list, Regex regex) where T : class
        {
            return FluentFilter(list, regex).AllActLike<T>();
        }

    }
}
