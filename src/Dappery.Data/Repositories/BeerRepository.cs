using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Dappery.Core.Data;
using Dappery.Domain.Entities;

namespace Dappery.Data.Repositories;

public class BeerRepository : IBeerRepository
{
    private readonly IDbTransaction _transaction;
    private readonly IDbConnection _dbConnection;
    private readonly string _insertRowRetrievalQuery;

    public BeerRepository(IDbTransaction dbTransaction, string insertRowRetrievalQuery)
    {
        _transaction = dbTransaction;
        _dbConnection = _transaction.Connection!;
        _insertRowRetrievalQuery = insertRowRetrievalQuery;
    }

    public async Task<IEnumerable<Beer>> GetAllBeersAsync(CancellationToken cancellationToken)
    {
        // Initialize our commands to utilize our cancellation token
        var addressCommand = new CommandDefinition(
            "SELECT * FROM Addresses",
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var resultCommand = new CommandDefinition(
            "SELECT b.*, br.* FROM Beers b INNER JOIN Breweries br ON br.Id = b.BreweryId",
            transaction: _transaction,
            cancellationToken: cancellationToken);

        // Retrieve the addresses, as _is a nested mapping
        var addresses = (await _dbConnection.QueryAsync<Address>(addressCommand).ConfigureAwait(false)).ToList();

        return await _dbConnection.QueryAsync<Beer, Brewery, Beer>(
            resultCommand,
            (beer, brewery) =>
                {
                    // Map the brewery that Dapper returns for us to the beer
                    brewery.Address = addresses.Find(a => a.BreweryId == brewery.Id);
                    beer.Brewery = brewery;
                    return beer;
                }
            )
            .ConfigureAwait(false);
    }

    public async Task<Beer?> GetBeerByIdAsync(int id, CancellationToken cancellationToken)
    {
        // Initialize our command
        var beerFromIdCommand = new CommandDefinition(
            @"SELECT b.*, br.* FROM Beers b
                INNER JOIN Breweries br ON br.Id = b.BreweryId
                WHERE b.Id = @Id",
            new { Id = id },
            _transaction,
            cancellationToken: cancellationToken);

        // Retrieve the beer from the database
        Beer? beerFromId = (await _dbConnection.QueryAsync<Beer, Brewery, Beer>(
            beerFromIdCommand,
            (beer, brewery) =>
            {
                beer.Brewery = brewery;
                return beer;
            })
            .ConfigureAwait(false))
            .FirstOrDefault();

        // Return back to the caller if no beer is found, let the business logic decide what to do if we can't the specified beer
        if (beerFromId is null)
        {
            return null;
        }

        // Instantiate a command for the address and brewery
        var addressCommand = new CommandDefinition(
            "SELECT * FROM Addresses WHERE BreweryId = @BreweryId",
            new { BreweryId = beerFromId.Brewery?.Id },
            _transaction,
            cancellationToken: cancellationToken);

        var breweryCommand = new CommandDefinition(
            "SELECT * FROM Beers WHERE BreweryId = @BreweryId",
            new { beerFromId.BreweryId },
            _transaction,
            cancellationToken: cancellationToken);

        // Map the address to the beer's brewery
        Address? address = await _dbConnection.QueryFirstOrDefaultAsync<Address>(addressCommand).ConfigureAwait(false);

        // Set the address found in the previous query to the beer's brewery address, if we have a brewery
        _ = (beerFromId.Brewery?.Address = address);

        // Let's add all the beers to our brewery attached to _beer
        IEnumerable<Beer>? beersFromBrewery = await _dbConnection.QueryAsync<Beer>(breweryCommand).ConfigureAwait(false);

        // Lastly, let's add all the beers to the entity model
        foreach (Beer beer in beersFromBrewery)
        {
            beerFromId.Brewery?.Beers.Add(beer);
        }

        return beerFromId;
    }

    public Task<int> CreateBeerAsync(Beer beer, CancellationToken cancellationToken)
    {
        // From our business we defined, we'll assume the brewery ID is always attached to the beer
        var beerToInsertSql = new StringBuilder(@"INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                                        VALUES (@Name, @BeerStyle, @CreatedAt, @UpdatedAt, @BreweryId)");

        var beerToCreateCommand = new CommandDefinition(
            beerToInsertSql.Append(_insertRowRetrievalQuery).ToString(),
            new
            {
                beer.Name,
                beer.BeerStyle,
                beer.CreatedAt,
                beer.UpdatedAt,
                beer.BreweryId
            },
            _transaction,
            cancellationToken: cancellationToken);

        // Let's insert the beer and grab its ID
        return _dbConnection.ExecuteScalarAsync<int>(beerToCreateCommand);
    }

    public async Task UpdateBeerAsync(Beer beer, CancellationToken cancellationToken)
    {
        // Our application layer will be in charge of mapping the new properties to the entity layer,
        // as well as validating that the beer exists, so the data layer will only be responsible for
        // inserting the values into the database; separation of concerns!
        var updateBeerCommand = new CommandDefinition(
            "UPDATE Beers SET Name = @Name, BeerStyle = @BeerStyle, UpdatedAt = @UpdatedAt, BreweryId = @BreweryId WHERE Id = @Id",
            new
            {
                beer.Name,
                beer.BeerStyle,
                beer.UpdatedAt,
                beer.BreweryId,
                beer.Id
            },
            _transaction,
            cancellationToken: cancellationToken);

        _ = await _dbConnection.ExecuteAsync(updateBeerCommand).ConfigureAwait(false);
    }

    public async Task DeleteBeerAsync(int id, CancellationToken cancellationToken)
    {
        // Our simplest command, just remove the beer directly from the database
        // Validation that the beer actually exists in the database will left to the application layer
        var deleteBeerCommand = new CommandDefinition(
            "DELETE FROM Beers WHERE Id = @Id",
            new { Id = id },
            _transaction,
            cancellationToken: cancellationToken);

        _ = await _dbConnection.ExecuteAsync(deleteBeerCommand).ConfigureAwait(false);
    }
}
