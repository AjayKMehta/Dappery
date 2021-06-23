using Dappery.Domain.Dtos;

namespace Dappery.Domain.Media
{
    public class BreweryResource : Resource<BreweryDto>
    {
        public BreweryResource(BreweryDto resource)
            : base(resource)
        {
        }
    }
}
