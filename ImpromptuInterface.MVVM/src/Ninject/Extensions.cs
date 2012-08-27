using System.Collections.Generic;

namespace ImpromptuInterface.MVVM.Ninject
{
    /// <summary>
    /// A set of extensions for setting up Ninject with NoMvvm
    /// </summary>
    public static class Extensions
    {
        internal static IEnumerable<TElement> Flatten<TElement>(this IEnumerable<IEnumerable<TElement>> sequences)
        {
            foreach (var sequence in sequences)
            {
                foreach (var element in sequence)
                {
                    yield return element;
                }
            }
        }
    }
}
