using Backend;
using Dapper;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

// SetupTeardownDatabase();

app.MapGet("/events", async () => await new Events().GetAllEvents());

app.Run();

static void SetupTeardownDatabase()
{
    var connection = new SqliteConnection("Data Source=hello.db");

    connection.Open();
    
    var createTokensPurchasedEventTable = @"
        CREATE TABLE IF NOT EXISTS TokensPurchasedEvent (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Buyer nvarchar(64) NOT NULL,
            Subreddit nvarchar(32) NOT NULL,
            Tokens bigint NOT NULL
        )";
    connection.Execute(createTokensPurchasedEventTable);

    // Clean up
    SqliteConnection.ClearAllPools();
    File.Delete("hello.db");
}

// fix: An unhandled exception of type 'System.IO.IOException' occurred in System.Private.CoreLib.dll: 'The process cannot access the file 'E:\code\subr\backend\hello.db' because it is being used by another process.'
// todo: need (blockhash+transactionhash+logindex) for ID (NEED BLOCK NUMBER TOO FOR FROMBLOCK/TOBLOCK) for each events (EventLog.Log) (maybe map to custom object that combines EventLog.Log and EventLog.Event)
// todo: create database mysqlite, store .db file in backend file. INCLUDING BLOCK NUMBER
// todo: when new request, only request events from latest block number in database
// todo: probably rate limit not the API, but calls to blockchain somehow, when API is called (stale data is fine to some extent)
// todo: allow query from-block=blockNumber&to-block=blockNumber
// todo: move Events.ContractAddress in to appsettings.json
// todo: hide Events.AlchemyApiKey apiKey somehow