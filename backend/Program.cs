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

    // {"buyer":"0xB6Bf1Eec596602D14acb288262C7B9b6D1B801eA","subreddit":"/r/ethereum","tokens":{"isPowerOfTwo":false,"isZero":false,"isOne":false,"isEven":true,"sign":1}}
    
    // blockhash string "0x12271ffad8006ac6d2fa1cf92d767ea30eaf443e781300aed90283af11543456"
    // txnhash string "0x8d70c8d3932323af7a7e047d65bc25d5ef8b144d5ca194bcdc04aac7d0ddfc12"
    // logindex hexbiginteger {619}
    
    // blocknumber hexbiginteger {21596321}

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