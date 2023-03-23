using System;
using System.Collections.Generic;
using System.Linq;

namespace Npa.Accounting.Common.Ext
{
    public static class ListExtensions
    {
        public static List<T> TakeLast<T>(this List<T> collection, int? n)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)} must be 0 or greater");

            LinkedList<T> temp = new LinkedList<T>();

            foreach (var value in collection)
            {
                temp.AddLast(value);
                if (temp.Count > n)
                    temp.RemoveFirst();
            }

            List<T> tempList = temp.ToList();

            return tempList;
        }
    }
}
