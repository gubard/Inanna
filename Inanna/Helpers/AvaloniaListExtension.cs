using Avalonia.Collections;

namespace Inanna.Helpers;

public static class AvaloniaListExtension
{
    extension<TAvaloniaList, T>(TAvaloniaList list) where TAvaloniaList : IAvaloniaList<T>
    {
        public void UpdateOrder(T[] items)
        {
            if (items.Length == 0)
            {
                list.Clear();
            }

            var index = 0;

            for (; index < items.Length; index++)
            {
                var item = items[index];

                if (index >= list.Count)
                {
                    list.Add(item);

                    continue;
                }

                if (item?.Equals(list[index]) == true)
                {
                    continue;
                }

                var lastIndex = list.LastIndexOf(item);

                if (lastIndex != -1)
                {
                    list.RemoveAt(lastIndex);
                }

                list.Insert(index, item);
            }

            if (index < list.Count)
            {
                list.RemoveRange(index, list.Count - index);
            }
        }

        private int LastIndexOf(T item)
        {
            if (list.Count == 0)
            {
                return -1;
            }

            return list.LastIndexOf(item, list.Count - 1, list.Count);
        }

        private int LastIndexOf(T item,
            int index, int count)
        {
            if (list.Count != 0 && index < 0)
            {
                throw new();
            }

            if (list.Count != 0 && count < 0)
            {
                throw new();
            }

            if (index >= list.Count)
            {
                throw new();
            }

            if (count > index + 1)
            {
                throw new();
            }

            return Array.LastIndexOf(list.ToArray(), item, index, count);
        }
    }

}