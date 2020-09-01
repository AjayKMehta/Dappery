using System.Collections.Generic;
using System.Linq;

namespace Dappery.Domain.Media
{
    public class ResourceList<T>
    {
        public ResourceList(IEnumerable<T> items) => this.Items = items;

        public IEnumerable<T> Items { get; }

        public int Count => this.Items.Count();
    }
}
