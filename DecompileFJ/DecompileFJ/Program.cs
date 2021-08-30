using System;
using System.Collections.Generic;
using System.Linq;

namespace DecompileFJ
{
    /// <summary xml:lang="ru">
    /// Представляет методы расширений по объединению последовательностей     
    /// </summary>
    public static class LinqExtensions
    {
        public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKeySelector,
            Func<TRight, TKey> rightKeySelector,
            Func<TLeft, TRight, TKey, TResult> resultSelector,
            IEqualityComparer<TKey> comparator = null,
            TLeft defaultLeft = default(TLeft),
            TRight defaultRight = default(TRight))
        {
            comparator = comparator ?? EqualityComparer<TKey>.Default;
            return FullOuterJoinIterator(left, right, leftKeySelector, rightKeySelector, resultSelector, comparator, defaultLeft, defaultRight);
        }

        internal static IEnumerable<TResult> FullOuterJoinIterator<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TKey> leftKeySelector,
            Func<TRight, TKey> rightKeySelector,
            Func<TLeft, TRight, TKey, TResult> resultSelector,
            IEqualityComparer<TKey> comparator,
            TLeft defaultLeft,
            TRight defaultRight)
        {
            var leftLookup = left.ToLookup(leftKeySelector, comparator);
            var rightLookup = right.ToLookup(rightKeySelector, comparator);
            var keys = leftLookup.Select(g => g.Key).Union(rightLookup.Select(g => g.Key), comparator);

            foreach (var key in keys)
                foreach (var leftValue in leftLookup[key].DefaultIfEmpty(defaultLeft))
                    foreach (var rightValue in rightLookup[key].DefaultIfEmpty(defaultRight))
                        yield return resultSelector(leftValue, rightValue, key);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var persons = new List<Person>
            {
                new Person { Name = "Alexey", City = "Moscow" },
                new Person { Name = "Vladimir", City = "St. Peterburg" },
                new Person { Name = "Sergey", City = "Vladimir" },
            };

            var weathers = new List<Weather>
            {
                new Weather { Now = "Solar", City = "Moscow" },
                new Weather { Now = "Rainy", City = "Tallin" },
            };

            var join = persons.FullOuterJoin(weathers, x => x.City, y => y.City, (first, second, id) => new { id, first, second });
            foreach (var j in join)
            {
                Console.WriteLine($"{ j.first?.Name ?? "NULL" } | { j.id } | { j.second?.Now ?? "NULL" }");
            }
            Console.ReadKey();
        }
    }

    class Person
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    class Weather
    {
        public string Now { get; set; }
        public string City { get; set; }
    }
}