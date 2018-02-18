using System;
using System.Collections.Generic;
using System.Linq;

public static class LINQExtensions
{
    public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> list, int bufferSize) => list
        .Select((t, i) => new { t, i })
        .GroupBy(x => x.i / bufferSize, x => x.t);

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list) => list.OrderBy(_ => new Random().NextDouble());

    public static string ToPrettyString<T>(this IEnumerable<T> list){
        return $"[{string.Join(",", list)}]";
    }
}