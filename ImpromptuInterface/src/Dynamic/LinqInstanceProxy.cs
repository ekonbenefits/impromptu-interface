using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ImpromptuInterface.Internal.Support;

namespace ImpromptuInterface.Dynamic
{   
    
    [Serializable]
    public class LinqInstanceProxy : ExtensionToInstanceProxy, IEnumerable<object>
    {    
        
        public LinqInstanceProxy(dynamic target)
            :base((object)target, typeof(IEnumerable<>), new[]{typeof(Enumerable)}, new[]{typeof(ILinq<>), typeof(IOrderedLinq<>)})
        {

        }

        #if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuObject"/> class. when deserializing
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LinqInstanceProxy(SerializationInfo info,
                                           StreamingContext context) : base(info, context)
        {
            
        }

        protected override ExtensionToInstanceProxy CreateSelf(object target, Type extendedType, Type[] staticTypes, Type[] instanceHints)
        {
            return new LinqInstanceProxy(target);
        }

        #endif
        public IEnumerator<object> GetEnumerator()
        {
            return ((dynamic) CallTarget).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface ILinq<TSource> : IEnumerable<TSource>
    {
        TSource Aggregate(Func<TSource, TSource, TSource> func);
        TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func);
        TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector);
        Boolean All(Func<TSource, Boolean> predicate);
        Boolean Any();
        Boolean Any(Func<TSource, Boolean> predicate);
        ILinq<TSource> AsEnumerable();
        Double Average(Func<TSource, Int32> selector);
        Nullable<Double> Average(Func<TSource, Nullable<Int32>> selector);
        Double Average(Func<TSource, Int64> selector);
        Nullable<Double> Average(Func<TSource, Nullable<Int64>> selector);
        Single Average(Func<TSource, Single> selector);
        Nullable<Single> Average(Func<TSource, Nullable<Single>> selector);
        Double Average(Func<TSource, Double> selector);
        Nullable<Double> Average(Func<TSource, Nullable<Double>> selector);
        Decimal Average(Func<TSource, Decimal> selector);
        Nullable<Decimal> Average(Func<TSource, Nullable<Decimal>> selector);
        ILinq<TResult> Cast<TResult>();
        ILinq<TSource> Concat(IEnumerable<TSource> second);
        Boolean Contains(TSource value);
        Boolean Contains(TSource value, IEqualityComparer<TSource> comparer);
        Int32 Count();
        Int32 Count(Func<TSource, Boolean> predicate);
        ILinq<TSource> DefaultIfEmpty();
        ILinq<TSource> DefaultIfEmpty(TSource defaultValue);
        ILinq<TSource> Distinct();
        ILinq<TSource> Distinct(IEqualityComparer<TSource> comparer);
        TSource ElementAt(Int32 index);
        TSource ElementAtOrDefault(Int32 index);
        ILinq<TSource> Except(IEnumerable<TSource> second);
        ILinq<TSource> Except(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
        TSource First();
        TSource First(Func<TSource, Boolean> predicate);
        TSource FirstOrDefault();
        TSource FirstOrDefault(Func<TSource, Boolean> predicate);
        ILinq<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector);
        ILinq<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        ILinq<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        ILinq<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        ILinq<TResult> GroupBy<TKey, TResult>(Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector);
        ILinq<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector);
        ILinq<TResult> GroupBy<TKey, TResult>(Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer);
        ILinq<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer);
        ILinq<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, IEnumerable<TInner>, TResult> resultSelector);
        ILinq<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer);
        ILinq<TSource> Intersect(IEnumerable<TSource> second);
        ILinq<TSource> Intersect(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
        ILinq<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, TInner, TResult> resultSelector);
        ILinq<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TSource, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TSource, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer);
        TSource Last();
        TSource Last(Func<TSource, Boolean> predicate);
        TSource LastOrDefault();
        TSource LastOrDefault(Func<TSource, Boolean> predicate);
        Int64 LongCount();
        Int64 LongCount(Func<TSource, Boolean> predicate);
        TSource Max();
        Int32 Max(Func<TSource, Int32> selector);
        Nullable<Int32> Max(Func<TSource, Nullable<Int32>> selector);
        Int64 Max(Func<TSource, Int64> selector);
        Nullable<Int64> Max(Func<TSource, Nullable<Int64>> selector);
        Single Max(Func<TSource, Single> selector);
        Nullable<Single> Max(Func<TSource, Nullable<Single>> selector);
        Double Max(Func<TSource, Double> selector);
        Nullable<Double> Max(Func<TSource, Nullable<Double>> selector);
        Decimal Max(Func<TSource, Decimal> selector);
        Nullable<Decimal> Max(Func<TSource, Nullable<Decimal>> selector);
        TResult Max<TResult>(Func<TSource, TResult> selector);
        TSource Min();
        Int32 Min(Func<TSource, Int32> selector);
        Nullable<Int32> Min(Func<TSource, Nullable<Int32>> selector);
        Int64 Min(Func<TSource, Int64> selector);
        Nullable<Int64> Min(Func<TSource, Nullable<Int64>> selector);
        Single Min(Func<TSource, Single> selector);
        Nullable<Single> Min(Func<TSource, Nullable<Single>> selector);
        Double Min(Func<TSource, Double> selector);
        Nullable<Double> Min(Func<TSource, Nullable<Double>> selector);
        Decimal Min(Func<TSource, Decimal> selector);
        Nullable<Decimal> Min(Func<TSource, Nullable<Decimal>> selector);
        TResult Min<TResult>(Func<TSource, TResult> selector);
        ILinq<TResult> OfType<TResult>();
        IOrderedLinq<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector);
        IOrderedLinq<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
        IOrderedLinq<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector);
        IOrderedLinq<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
        ILinq<TSource> Reverse();
        ILinq<TResult> Select<TResult>(Func<TSource, TResult> selector);
        ILinq<TResult> Select<TResult>(Func<TSource, Int32, TResult> selector);
        ILinq<TResult> SelectMany<TResult>(Func<TSource, IEnumerable<TResult>> selector);
        ILinq<TResult> SelectMany<TResult>(Func<TSource, Int32, IEnumerable<TResult>> selector);
        ILinq<TResult> SelectMany<TCollection, TResult>(Func<TSource, Int32, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector);
        ILinq<TResult> SelectMany<TCollection, TResult>(Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector);
        Boolean SequenceEqual(IEnumerable<TSource> second);
        Boolean SequenceEqual(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
        TSource Single();
        TSource Single(Func<TSource, Boolean> predicate);
        TSource SingleOrDefault();
        TSource SingleOrDefault(Func<TSource, Boolean> predicate);
        ILinq<TSource> Skip(Int32 count);
        ILinq<TSource> SkipWhile(Func<TSource, Boolean> predicate);
        ILinq<TSource> SkipWhile(Func<TSource, Int32, Boolean> predicate);
        Int32 Sum(Func<TSource, Int32> selector);
        Nullable<Int32> Sum(Func<TSource, Nullable<Int32>> selector);
        Int64 Sum(Func<TSource, Int64> selector);
        Nullable<Int64> Sum(Func<TSource, Nullable<Int64>> selector);
        Single Sum(Func<TSource, Single> selector);
        Nullable<Single> Sum(Func<TSource, Nullable<Single>> selector);
        Double Sum(Func<TSource, Double> selector);
        Nullable<Double> Sum(Func<TSource, Nullable<Double>> selector);
        Decimal Sum(Func<TSource, Decimal> selector);
        Nullable<Decimal> Sum(Func<TSource, Nullable<Decimal>> selector);
        ILinq<TSource> Take(Int32 count);
        ILinq<TSource> TakeWhile(Func<TSource, Boolean> predicate);
        ILinq<TSource> TakeWhile(Func<TSource, Int32, Boolean> predicate);
        TSource[] ToArray();
        Dictionary<TKey, TSource> ToDictionary<TKey>(Func<TSource, TKey> keySelector);
        Dictionary<TKey, TSource> ToDictionary<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        List<TSource> ToList();
        ILookup<TKey, TSource> ToLookup<TKey>(Func<TSource, TKey> keySelector);
        ILookup<TKey, TSource> ToLookup<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        ILinq<TSource> Union(IEnumerable<TSource> second);
        ILinq<TSource> Union(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
        ILinq<TSource> Where(Func<TSource, Boolean> predicate);
        ILinq<TSource> Where(Func<TSource, Int32, Boolean> predicate);
        ILinq<TResult> Zip<TSecond, TResult>(IEnumerable<TSecond> second, Func<TSource, TSecond, TResult> resultSelector);
    }

    public interface IOrderedLinq<TSource> : ILinq<TSource>, IOrderedEnumerable<TSource>
    {
        IOrderedLinq<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector);
        IOrderedLinq<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
        IOrderedLinq<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector);
        IOrderedLinq<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
    }


}
