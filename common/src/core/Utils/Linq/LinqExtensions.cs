﻿
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Utils to work with Linq.
/// </summary>
namespace VVVV.Utils.Linq
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> AsDepthFirstEnumerable<T>(this T parent) where T: IEnumerable<T>
		{
			yield return parent;
			foreach (var subTree in parent)
			{
				foreach (var child in subTree.AsDepthFirstEnumerable())
				{
					yield return child;
				}
			}
		}
		
		public static IEnumerable<T> AsDepthFirstEnumerable<T>(this T parent, Func<T, IEnumerable<T>> childSelector)
		{
			yield return parent;
			foreach (var subTree in childSelector(parent))
			{
				foreach (var child in subTree.AsDepthFirstEnumerable(childSelector))
				{
					yield return child;
				}
			}
		}

        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            // none means: tell me if a certain assumption is false for all elements
            return enumerable.All(t => !predicate(t));
        }

        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            // none means: tell me if there is no element
            return !enumerable.Any();
        }

        // From: http://stackoverflow.com/questions/5039617/turning-an-ienumerableienumerablet-90-degrees
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(x => x.MoveNext()))
                {
                    yield return enumerators.Select(x => x.Current).ToArray();
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                    enumerator.Dispose();
            }
        }

	}
}
