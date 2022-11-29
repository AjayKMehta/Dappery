using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dappery.Core.Data;
using Dappery.Core.Extensions;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;

using MediatR;

namespace Dappery.Core.Breweries.Queries.GetBreweries;

public class GetBreweriesQueryHandler : IRequestHandler<GetBreweriesQuery, BreweryResourceList>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBreweriesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BreweryResourceList> Handle(GetBreweriesQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the breweries and clean up our resources
        IEnumerable<Brewery>? breweries = await _unitOfWork.BreweryRepository.GetAllBreweries(cancellationToken).ConfigureAwait(false);
        _unitOfWork.Commit();

        // Map our breweries from the returned query
        IEnumerable<BreweryDto> mappedBreweries = breweries.Select(b => b.ToBreweryDto());

        // Map each brewery to its corresponding DTO
        return new BreweryResourceList(mappedBreweries);
    }
}
