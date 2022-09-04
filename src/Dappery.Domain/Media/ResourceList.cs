using System.Collections.Generic;
using System.Linq;

namespace Dappery.Domain.Media;

public class ResourceList<T>
{
    public ResourceList(IEnumerable<T> items)
    {
        Items = items;
        Count = Items.Count();
    }

    public IEnumerable<T> Items { get; }

    public int Count { get; }
}
