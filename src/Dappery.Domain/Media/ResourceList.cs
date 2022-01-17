using System.Collections.Generic;
using System.Linq;

namespace Dappery.Domain.Media
{
    public class ResourceList<T>
    {
        public ResourceList(IEnumerable<T> items)
        {
            this.Items = items;
            this.Count = this.Items.Count();
        }

        public IEnumerable<T> Items { get; }

        public int Count { get; }
    }
}
