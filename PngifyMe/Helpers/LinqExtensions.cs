using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Helpers;

public static class LinqExtensions
{
    private static readonly Random _rng = new Random();

    [Obsolete]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source
            .Select(x => new { Value = x, Order = _rng.Next() })
            .OrderBy(x => x.Order)
            .Select(x => x.Value);
    }
}