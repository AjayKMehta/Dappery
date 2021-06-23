using Dappery.Domain.Dtos;

namespace Dappery.Domain.Media
{
    public class BeerResource : Resource<BeerDto>
    {
        public BeerResource(BeerDto resource)
            : base(resource)
        {
        }
    }
}
