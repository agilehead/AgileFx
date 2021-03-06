﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgileFx
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start)
        {
            return Slice<T>(source, start, null, null);
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start, int? stop)
        {
            return Slice<T>(source, start, stop, null);
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start, int? stop, int? step)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (step == 0) throw new ArgumentException("Step cannot be zero.", "step");
            IList<T> sourceCollection = source as IList<T>;
            
            if (sourceCollection == null) source = new List<T>(source);
            
            // nothing to slice
            if (sourceCollection.Count == 0) yield break;

            // set defaults for null arguments
            int stepCount = step ?? 1;
            int startIndex = start ?? ((stepCount > 0) ? 0 : sourceCollection.Count - 1);
            int stopIndex = stop ?? ((stepCount > 0) ? sourceCollection.Count : -1);

            // start from the end of the list if start is negitive
            if (start < 0) startIndex = sourceCollection.Count + startIndex;

            // end from the start of the list if stop is negitive
            if (stop < 0) stopIndex = sourceCollection.Count + stopIndex;

            // ensure indexes keep within collection bounds
            startIndex = Math.Max(startIndex, (stepCount > 0) ? 0 : int.MinValue);

            startIndex = Math.Min(startIndex, (stepCount > 0) ? sourceCollection.Count : sourceCollection.Count - 1);
            stopIndex = Math.Max(stopIndex, -1);
            stopIndex = Math.Min(stopIndex, sourceCollection.Count);

            for (int i = startIndex; (stepCount > 0) ? i < stopIndex : i > stopIndex; i += stepCount)
            {
                yield return sourceCollection[i];
            }
            yield break;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from name in list
                         group name by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }


        public static IEnumerable<TResult> SplitAndAggregate<TListItem, TResult>(this IEnumerable<TListItem> list,
            Func<IEnumerable<TListItem>, IEnumerable<TResult>> func, int maxItemsInSlice)
        {
            var parts = list.Split((list.Count() / maxItemsInSlice) + 1);

            var results = new List<TResult>();

            foreach (var part in parts)
            {
                results.AddRange(func(part));
            }

            return results;
        }
    }
}
