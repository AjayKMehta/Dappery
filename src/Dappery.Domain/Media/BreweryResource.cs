using Dappery.Domain.Dtos.Brewery;

namespace Dappery.Domain.Media;

public class BreweryResource : Resource<BreweryDto>
{
    public BreweryResource(BreweryDto resource)
        : base(resource)
    {
    }
}
