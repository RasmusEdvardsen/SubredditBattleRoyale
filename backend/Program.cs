using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend;
using Dapper;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Data.Sqlite;
using Nethereum.Hex.HexTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

SetupTeardownDatabase();

app.MapGet("/events", async () => await new Events().GetAllEvents());

app.Run();

static void SetupTeardownDatabase()
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
    connection.Execute(createTokensPurchasedTable);

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
    connection.Execute(createTokensBurnedTable);

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
    connection.Execute(createSeasonWonTable);

    var createLastBlockchainUpdateTable = @"
        CREATE TABLE IF NOT EXISTS BlockchainSync (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SyncedAt DATETIME NOT NULL
        )";
    connection.Execute(createLastBlockchainUpdateTable);
}

// todo: implement for Events.cs tokensBurned and seasonWon
// todo: probably rate limit not the API, but calls to blockchain somehow, when API is called (stale data is fine to some extent)
// todo: allow query from-block=blockNumber&to-block=blockNumber
// todo: move Events.ContractAddress in to appsettings.json
// todo: hide Events.AlchemyApiKey apiKey somehow
