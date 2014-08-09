using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Helper
{
    public static class EnumerableHelper
    {
        /// <summary>
        /// Split the chunk collection to several sublists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source chunk collectoin</param>
        /// <param name="size">the size of the sublist</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int size)
        {
            while (source.Any())
            {
                yield return source.Take(size);
                source = source.Skip(size);
            }
        }
    }
}
