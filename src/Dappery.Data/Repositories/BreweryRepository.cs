using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dappery.Core.Data;
using Dappery.Domain.Entities;

namespace Dappery.Data.Repositories
{
    public class BreweryRepository : IBreweryRepository
    {
        private readonly IDbTransaction dbTransaction;
        private readonly IDbConnection? dbConnection;
        private readonly string rowInsertRetrievalQuery;

        public BreweryRepository(IDbTransaction dbTransaction, string rowInsertRetrievalQuery)
        {
            this.dbTransaction = dbTransaction;
            this.dbConnection = this.dbTransaction.Connection;
            this.rowInsertRetrievalQuery = rowInsertRetrievalQuery;
        }

        public async Task<Brewery?> GetBreweryById(int id, CancellationToken cancellationToken)
        {
            // Instantiate our commands
            var beersFromBreweryCommand = new CommandDefinition(
                "SELECT * FROM Beers WHERE BreweryId = @Id",
                new { Id = id },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            var breweryCommand = new CommandDefinition(
                "SELECT br.*, a.* FROM Breweries br INNER JOIN Addresses a ON a.BreweryId = br.Id WHERE br.Id = @Id",
                new { Id = id },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            var beersFromBrewery = (await this.dbConnection!.QueryAsync<Beer>(beersFromBreweryCommand).ConfigureAwait(false)).ToList();

            var breweries = await this.dbConnection!.QueryAsync<Brewery, Address, Brewery>(
                breweryCommand,
                (brewery, address) =>
                {
                    // Since breweries have a one-to-one relation with address, we can initialize that mapping here
                    brewery.Address = address;

                    foreach (var beer in beersFromBrewery)
                    {
                        brewery.Beers.Add(beer);
                    }

                    return brewery;
                }).ConfigureAwait(false);

            return breweries?.FirstOrDefault();
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweries(CancellationToken cancellationToken)
        {
            // Instantiate our commands to utilize our cancellation token
            var beersCommand = new CommandDefinition(
                "SELECT * FROM Beers",
                this.dbTransaction,
                cancellationToken: cancellationToken);

            var beersJoinedOnBreweriesCommand = new CommandDefinition(
                "SELECT * FROM Breweries br INNER JOIN Addresses a ON a.BreweryId = br.Id",
                this.dbTransaction,
                cancellationToken: cancellationToken);

            // Grab a reference to all beers so we can map them to there corresponding breweries
            var beers = (await this.dbConnection.QueryAsync<Beer>(beersCommand).ConfigureAwait(false)).ToList();

            return await this.dbConnection.QueryAsync<Brewery, Address, Brewery>(
                // We join with addresses as there's a one-to-one relation with breweries, making the query a little less intensive
                beersJoinedOnBreweriesCommand,
                (brewery, address) =>
                {
                    // Map the address to the brewery
                    brewery.Address = address;

                    // Map each beer to the beer collection for the brewery during iteration over our result set
                    if (beers.Any(b => b.BreweryId == brewery.Id))
                    {
                        foreach (var beer in beers.Where(b => b.BreweryId == brewery.Id))
                        {
                            brewery.Beers.Add(beer);
                        }
                    }

                    return brewery;
                }).ConfigureAwait(false);
        }

        public async Task<int> CreateBrewery(Brewery brewery, CancellationToken cancellationToken)
        {
            // Grab a reference to the address
            var address = brewery.Address;
            var breweryInsertSql =
                new StringBuilder("INSERT INTO Breweries (Name, CreatedAt, UpdatedAt) VALUES (@Name, @CreatedAt, @UpdatedAt)");

            // Instantiate our commands to utilize our cancellation token
            var breweryInsertCommand = new CommandDefinition(
                breweryInsertSql.Append(this.rowInsertRetrievalQuery).ToString(),
                new { brewery.Name, brewery.CreatedAt, brewery.UpdatedAt },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            // Let's add the brewery
            var breweryId = await this.dbConnection.ExecuteScalarAsync<int>(breweryInsertCommand).ConfigureAwait(false);

            var addressInsertCommand = new CommandDefinition(
                @"INSERT INTO Addresses (StreetAddress, City, State, ZipCode, CreatedAt, UpdatedAt, BreweryId)
                        VALUES (@StreetAddress, @City, @State, @ZipCode, @CreatedAt, @UpdatedAt, @BreweryId)",
                new
                {
                    address?.StreetAddress,
                    address?.City,
                    address?.State,
                    address?.ZipCode,
                    address?.CreatedAt,
                    address?.UpdatedAt,
                    BreweryId = breweryId
                },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            // One of our business rules is that a brewery must have an associated address
            await this.dbConnection.ExecuteAsync(addressInsertCommand).ConfigureAwait(false);

            return breweryId;
        }

        public async Task UpdateBrewery(Brewery brewery, CancellationToken cancellationToken, bool updateAddress = false)
        {
            // Instantiate our commands to utilize our cancellation token
            var breweryUpdateCommand = new CommandDefinition(
                "UPDATE Breweries SET Name = @Name, UpdatedAt = @UpdatedAt WHERE Id = @Id",
                new
                {
                    brewery.Name,
                    brewery.UpdatedAt,
                    brewery.Id
                },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            // Again, we'll assume the brewery details are being validated and mapped properly in the application layer
            await this.dbConnection.ExecuteAsync(breweryUpdateCommand).ConfigureAwait(false);

            if (brewery.Address is not null && updateAddress)
            {
                var addressUpdateCommand = new CommandDefinition(
                    "UPDATE Addresses SET StreetAddress = @StreetAddress, City = @City, ZipCode = @ZipCode, State = @State, UpdatedAt = @UpdatedAt WHERE Id = @Id",
                    new
                    {
                        brewery.Address.StreetAddress,
                        brewery.Address.City,
                        brewery.Address.ZipCode,
                        brewery.Address.State,
                        brewery.Address.UpdatedAt,
                        brewery.Address.Id
                    },
                    this.dbTransaction,
                    cancellationToken: cancellationToken);

                // Again, we'll assume the brewery details are being validated and mapped properly in the application layer
                // For now, we won't allow users to swap breweries address to another address
                await this.dbConnection.ExecuteAsync(addressUpdateCommand).ConfigureAwait(false);
            }
        }

        public async Task DeleteBrewery(int breweryId, CancellationToken cancellationToken)
        {
            // Because we setup out database providers to cascade delete on parent entity removal, we won't have to
            // worry about individually removing all the associated beers and address
            // NOTE: Because we don't directly expose CRUD operations on the address table, we'll validate the cascade
            // remove directly in the database for now
            var deleteBreweryCommand = new CommandDefinition(
                "DELETE FROM Breweries WHERE Id = @Id",
                new { Id = breweryId },
                this.dbTransaction,
                cancellationToken: cancellationToken);

            await this.dbConnection.ExecuteAsync(deleteBreweryCommand).ConfigureAwait(false);
        }
    }
}
