using System.Collections.Generic;

using Dappery.Domain.Dtos.Beer;

namespace Dappery.Domain.Media
{
    public class BeerResourceList : ResourceList<BeerDto>
    {
        public BeerResourceList(IEnumerable<BeerDto> items)
            : base(items)
        {
        }
    }
}
