using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source is List<T> list)
        {
            list.ForEach(action);
            return;
        }

        foreach (var item in source)
        {
            action(item);
        }
    }
}
