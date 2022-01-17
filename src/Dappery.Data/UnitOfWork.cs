using System;
using System.Data;

using Dapper;

using Dappery.Core.Data;
using Dappery.Data.Repositories;

using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

using Npgsql;

namespace Dappery.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection dbConnection;
        private readonly IDbTransaction dbTransaction;

        public UnitOfWork(string? connectionString, bool isPostgres = false)
        {
            // Based on our database implementation, we'll need a reference to the last row inserted
            string rowInsertRetrievalQuery;

            // If no connection string is passed, we'll assume we're running with our SQLite database provider
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                this.dbConnection = new SqliteConnection("Data Source=:memory:");
                rowInsertRetrievalQuery = "; SELECT last_insert_rowid();";
            }
            else
            {
                this.dbConnection = isPostgres ? new NpgsqlConnection(connectionString) : new SqlConnection(connectionString);
                rowInsertRetrievalQuery = isPostgres ? "returning Id;" : "; SELECT CAST(SCOPE_IDENTITY() as int);";
            }

            // Open our connection, begin our transaction, and instantiate our repositories
            this.dbConnection.Open();
            this.dbTransaction = this.dbConnection.BeginTransaction();
            this.BreweryRepository = new BreweryRepository(this.dbTransaction, rowInsertRetrievalQuery);
            this.BeerRepository = new BeerRepository(this.dbTransaction, rowInsertRetrievalQuery);

            // Once our connection is open, if we're running SQLite for unit tests (or that actual application), let's seed some data
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                this.SeedDatabase(this.dbConnection);
            }
        }

        public IBreweryRepository BreweryRepository { get; }

        public IBeerRepository BeerRepository { get; }

        public void Commit()
        {
            try
            {
                this.dbTransaction.Commit();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Could not commit the transaction, reason: {e.Message}");
                this.dbTransaction.Rollback();
            }
            finally
            {
                this.dbTransaction.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dbTransaction?.Dispose();
                this.dbConnection?.Dispose();
            }
        }

        private void SeedDatabase(IDbConnection dbConnection)
        {
            const string createBreweriesSql = @"
                CREATE TABLE Breweries (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT(32),
                    CreatedAt DATE,
                    UpdatedAt DATE
                );
            ";

            const string createBeersSql = @"
                CREATE TABLE Beers (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT(32),
                    BeerStyle TEXT(16),
                    CreatedAt DATE,
                    UpdatedAt DATE,
                    BreweryId INT NOT NULL,
                    CONSTRAINT FK_Beers_Breweries_Id FOREIGN KEY (BreweryId)
                        REFERENCES Breweries (Id) ON DELETE CASCADE
                );
            ";

            const string createAddressSql = @"
                CREATE TABLE Addresses (
                    Id INTEGER PRIMARY KEY,
                    StreetAddress TEXT(32),
                    City TEXT(32),
                    State TEXT(32),
                    ZipCode TEXT(8),
                    CreatedAt DATE,
                    UpdatedAt DATE,
                    BreweryId INTEGER NOT NULL,
                    CONSTRAINT FK_Address_Breweries_Id FOREIGN KEY (BreweryId)
                        REFERENCES Breweries (Id) ON DELETE CASCADE
                );
            ";

            // Add our tables
            _ = dbConnection.Execute(createBreweriesSql, this.dbTransaction);
            _ = dbConnection.Execute(createBeersSql, this.dbTransaction);
            _ = dbConnection.Execute(createAddressSql, this.dbTransaction);

            // Seed our data
            _ = dbConnection.Execute(@"
                INSERT INTO Breweries (Name, CreatedAt, UpdatedAt)
                VALUES
                    (
                        'Fall River Brewery',
                        CURRENT_DATE,
                        CURRENT_DATE
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Breweries (Name, CreatedAt, UpdatedAt)
                VALUES
                    (
                        'Sierra Nevada Brewing Company',
                        CURRENT_DATE,
                        CURRENT_DATE
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Addresses (StreetAddress, City, State, ZipCode, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        '1030 E Cypress Ave Ste D',
                        'Redding',
                        'CA',
                        '96002',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        1
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Addresses (StreetAddress, City, State, ZipCode, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        '1075 E 20th St',
                        'Chico',
                        'CA',
                        '95928',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        2
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        'Hexagenia',
                        'Ipa',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        1
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        'Widowmaker',
                        'DoubleIpa',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        1
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        'Hooked',
                        'Lager',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        1
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        'Pale Ale',
                        'PaleAle',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        2
                    );",
                transaction: this.dbTransaction);

            _ = dbConnection.Execute(@"
                INSERT INTO Beers (Name, BeerStyle, CreatedAt, UpdatedAt, BreweryId)
                VALUES
                    (
                        'Hazy Little Thing',
                        'NewEnglandIpa',
                        CURRENT_DATE,
                        CURRENT_DATE,
                        2
                    );",
                transaction: this.dbTransaction);
        }
    }
}
