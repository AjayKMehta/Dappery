using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Data;
using Dapper;
using Dappery.Domain.Entities;

namespace Dappery.Data.Repositories
{
    public class BeerRepository : IBeerRepository
    {
        private readonly IDbTransaction transaction;
        private readonly IDbConnection dbConnection;
        private readonly string insertRowRetrievalQuery;

        public BeerRepository(IDbTransaction dbTransaction, string insertRowRetrievalQuery)
        {
            this.transaction = dbTransaction;
            this.dbConnection = this.transaction.Connection;
            this.insertRowRetrievalQuery = insertRowRetrievalQuery;
        }

        public async Task<IEnumerable<Beer>> GetAllBeersAsync(CancellationToken token)
        {
            // Initialize our commands to utilize our cancellation token
            var addressCommand = new CommandDefinition(
                "SELECT * FROM Addresses",
                transaction: this.transaction,
                cancellationToken: token);

            var resultCommand = new CommandDefinition(
                @"SELECT b.*, br.* FROM Beers b INNER JOIN Breweries br ON br.Id = b.BreweryId",
                transaction: this.transaction,
                cancellationToken: token);

            // Retrieve the addresses, as this is a nested mapping
            var addresses = (await this.dbConnection.QueryAsync<Address>(addressCommand)).ToList();

            return await this.dbConnection.QueryAsync<Beer, Brewery, Beer>(
                resultCommand,
                (beer, brewery) =>
                {
                    // Map the brewery that Dapper returns for us to the beer
                    brewery.Address = addresses.FirstOrDefault(a => a.BreweryId == brewery.Id);
                    beer.Brewery = brewery;
                    return beer;
                }
            );
        }

        public async Task<Beer> GetBeerByIdAsync(int id, CancellationToken cancellationToken)
        {
            // Initialize our command
            var beerFromIdCommand = new CommandDefinition(
                @"SELECT b.*, br.* FROM Beers b
                INNER JOIN Breweries br ON br.Id = b.BreweryId
                WHERE b.Id = @Id",
                new { Id = id },
                this.transaction,
                cancellationToken: cancellationToken);

            // Retrieve the beer from the database
            var beerFromId = (await this.dbConnection.QueryAsync<Beer, Brewery, Beer>(
                beerFromIdCommand,
                (beer, brewery) =>
                {
                    beer.Brewery = brewery;
                    return beer;
                })).FirstOrDefault();

            // Return back to the caller if no beer is found, let the business logic decide what to do if we can't the specified beer
            if (beerFromId is null)
            {
                return null!;
            }

            // Instantiate a command for the address and brewery
            var addressCommand = new CommandDefinition(
                @"SELECT * FROM Addresses WHERE BreweryId = @BreweryId",
                new { BreweryId = beerFromId.Brewery?.Id },
                this.transaction,
                cancellationToken: cancellationToken);

            var breweryCommand = new CommandDefinition(
                @"SELECT * FROM Beers WHERE BreweryId = @BreweryId",
                new { beerFromId.BreweryId },
                this.transaction,
                cancellationToken: cancellationToken);

            // Map the address to the beer's brewery
            var address = await this.dbConnection.QueryFirstOrDefaultAsync<Address>(addressCommand);

            // Set the address found in the previous query to the beer's brewery address, if we have a brewery
            if (beerFromId.Brewery != null)
            {
                beerFromId.Brewery.Address = address;
            }

            // Let's add all the beers to our brewery attached to this beer
            var beersFromBrewery = await this.dbConnection.QueryAsync<Beer>(breweryCommand);

            // Lastly, let's add all the beers to the entity model
            foreach (var beer in beersFromBrewery)
            {
                beerFromId.Brewery?.Beers.Add(beer);
            }

            return beerFromId;
        }

        public async Task<int> CreateBeerAsync(Beer beer, CancellationToken cancellationToken)
        {
            // From our business we defined, we'll assume the brewery ID is always attached to the beer
            var beerToInsertSql = new StringBuilder(@"INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                                        VALUES (@Name, @BeerStyle, @CreatedAt, @UpdatedAt, @BreweryId)");

            var beerToCreateCommand = new CommandDefinition(
                beerToInsertSql.Append(this.insertRowRetrievalQuery).ToString(),
                new
                {
                    beer.Name,
                    beer.BeerStyle,
                    beer.CreatedAt,
                    beer.UpdatedAt,
                    beer.BreweryId
                },
                this.transaction,
                cancellationToken: cancellationToken);

            // Let's insert the beer and grab its ID
            var beerId = await this.dbConnection.ExecuteScalarAsync<int>(beerToCreateCommand);

            // Finally, we'll return the newly inserted beer Id
            return beerId;
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
                this.transaction,
                cancellationToken: cancellationToken);

            await this.dbConnection.ExecuteAsync(updateBeerCommand).ConfigureAwait(false);
        }

        public async Task DeleteBeerAsync(int beerId, CancellationToken cancellationToken)
        {
            // Our simplest command, just remove the beer directly from the database
            // Validation that the beer actually exists in the database will left to the application layer
            var deleteBeerCommand = new CommandDefinition(
                @"DELETE FROM Beers WHERE Id = @Id",
                new { Id = beerId },
                this.transaction,
                cancellationToken: cancellationToken);

            await this.dbConnection.ExecuteAsync(deleteBeerCommand).ConfigureAwait(false);
        }
    }
}
