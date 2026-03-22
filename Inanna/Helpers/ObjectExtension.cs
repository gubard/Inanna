using Avalonia.Collections;

namespace Inanna.Helpers;

public static class ObjectExtension
{
    public static void AddSorted<T, TKey>(
        this AvaloniaList<T> list,
        T item,
        Func<T, TKey> keySelector
    )
        where TKey : IComparable<TKey>
    {
        if (list.Count == 0)
        {
            list.Add(item);

            return;
        }

        var low = 0;
        var high = list.Count - 1;
        var newItemKey = keySelector(item);

        while (low <= high)
        {
            var mid = low + (high - low) / 2;
            var comparison = keySelector(list[mid]).CompareTo(newItemKey);

            if (comparison == 0)
            {
                list.Insert(mid, item);

                return;
            }

            if (comparison < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        list.Insert(low, item);
    }
}
