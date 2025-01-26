using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Functionality;

public static class DatabaseSetup
{
    public async static Task SetupTeardownDatabase()
    {
        using var connection = new SqliteConnection("Data Source=hello.db");

        connection.Open();

        var createTokensPurchasedTable = @"
            CREATE TABLE IF NOT EXISTS TokensPurchased (
                Id TEXT PRIMARY KEY,
                Buyer TEXT NOT NULL,
                Subreddit TEXT NOT NULL,
                Tokens INTEGER NOT NULL,
                BlockHash TEXT NOT NULL,
                TransactionHash TEXT NOT NULL,
                LogIndex INTEGER NOT NULL,
                BlockNumber INTEGER NOT NULL
            )";
        await connection.ExecuteAsync(createTokensPurchasedTable);

        var createTokensBurnedTable = @"
            CREATE TABLE IF NOT EXISTS TokensBurned (
                Id TEXT PRIMARY KEY,
                Buyer TEXT NOT NULL,
                Subreddit TEXT NOT NULL,
                Tokens INTEGER NOT NULL,
                BlockHash TEXT NOT NULL,
                TransactionHash TEXT NOT NULL,
                LogIndex INTEGER NOT NULL,
                BlockNumber INTEGER NOT NULL
            )";
        await connection.ExecuteAsync(createTokensBurnedTable);

        var createSeasonWonTable = @"
            CREATE TABLE IF NOT EXISTS SeasonWon (
                Id TEXT PRIMARY KEY,
                Subreddit TEXT NOT NULL,
                Tokens INTEGER NOT NULL,
                Season INTEGER NOT NULL,
                BlockHash TEXT NOT NULL,
                TransactionHash TEXT NOT NULL,
                LogIndex INTEGER NOT NULL,
                BlockNumber INTEGER NOT NULL
            )";
        await connection.ExecuteAsync(createSeasonWonTable);

        var createLastBlockchainUpdateTable = @"
            CREATE TABLE IF NOT EXISTS BlockchainSync (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SyncedAt DATETIME NOT NULL
            )";
        await connection.ExecuteAsync(createLastBlockchainUpdateTable);

        var createVoidTokenCountTable = @"
            CREATE TABLE IF NOT EXISTS VoidTokenCount (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Balance INTEGER NOT NULL
            );
            INSERT INTO VoidTokenCount (Balance) VALUES (0);";
        await connection.ExecuteAsync(createVoidTokenCountTable);
    }
}
