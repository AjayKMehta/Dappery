using System.Collections.Generic;
using Dappery.Domain.Dtos.Brewery;

namespace Dappery.Domain.Media
{
    public class BreweryResourceList : ResourceList<BreweryDto>
    {
        public BreweryResourceList(IEnumerable<BreweryDto> items)
            : base(items)
        {
        }
    }
}
