using System;
using System.Data;

using Dapper;

using Dappery.Core.Data;
using Dappery.Data.Repositories;

using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

using Npgsql;

namespace Dappery.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public UnitOfWork(string? connectionString, bool isPostgres = false)
    {
        // Based on our database implementation, we'll need a reference to the last row inserted
        string rowInsertRetrievalQuery;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
            _dbConnection = new SqliteConnection("Data Source=:memory:");
            rowInsertRetrievalQuery = "; SELECT last_insert_rowid();";
        }
        else
        {
            _dbConnection = isPostgres ? new NpgsqlConnection(connectionString) : new SqlConnection(connectionString);
            rowInsertRetrievalQuery = isPostgres ? "returning Id;" : "; SELECT CAST(SCOPE_IDENTITY() as int);";
        }

        // Open our connection, begin our transaction, and instantiate our repositories
        _dbConnection.Open();
        _dbTransaction = _dbConnection.BeginTransaction();
        BreweryRepository = new BreweryRepository(_dbTransaction, rowInsertRetrievalQuery);
        BeerRepository = new BeerRepository(_dbTransaction, rowInsertRetrievalQuery);

        // Once our connection is open, if we're running SQLite for unit tests (or that actual application), let's seed some data
        if (string.IsNullOrWhiteSpace(connectionString))
            SeedDatabase(_dbConnection);
    }

    public IBreweryRepository BreweryRepository { get; }

    public IBeerRepository BeerRepository { get; }

    public void Commit()
    {
        try
        {
            _dbTransaction.Commit();
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"Could not commit the transaction, reason: {e.Message}");
            _dbTransaction.Rollback();
        }
        finally
        {
            _dbTransaction.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dbTransaction?.Dispose();
            _dbConnection?.Dispose();
        }
    }

    private void SeedDatabase(IDbConnection dbConnection)
    {
        const string CreateBreweriesSql = @"
                CREATE TABLE Breweries (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT(32),
                    CreatedAt DATE,
                    UpdatedAt DATE
                );
            ";

        const string CreateBeersSql = @"
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

        const string CreateAddressSql = @"
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
        _ = dbConnection.Execute(CreateBreweriesSql, _dbTransaction);
        _ = dbConnection.Execute(CreateBeersSql, _dbTransaction);
        _ = dbConnection.Execute(CreateAddressSql, _dbTransaction);

        // Seed our data
        _ = dbConnection.Execute(@"
                INSERT INTO Breweries (Name, CreatedAt, UpdatedAt)
                VALUES
                    (
                        'Fall River Brewery',
                        CURRENT_DATE,
                        CURRENT_DATE
                    );",
            transaction: _dbTransaction);

        _ = dbConnection.Execute(@"
                INSERT INTO Breweries (Name, CreatedAt, UpdatedAt)
                VALUES
                    (
                        'Sierra Nevada Brewing Company',
                        CURRENT_DATE,
                        CURRENT_DATE
                    );",
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);

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
            transaction: _dbTransaction);
    }
}
