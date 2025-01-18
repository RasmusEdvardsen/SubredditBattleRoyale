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

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new BigIntegerConverter());
    options.SerializerOptions.Converters.Add(new HexBigIntegerConverter());
});

var app = builder.Build();

app.UseHttpsRedirection();

SetupTeardownDatabase();

app.MapGet("/events", async () => await new Events().GetAllEvents());

app.Run();

static void SetupTeardownDatabase()
{
    var connection = new SqliteConnection("Data Source=hello.db");

    connection.Open();
    
    var createTokensPurchasedEventTable = @"
        CREATE TABLE IF NOT EXISTS TokensPurchasedEvent (
            buyer TEXT,
            subreddit TEXT,
            tokens INTEGER,
            blockHash TEXT,
            transactionHash TEXT,
            logIndex INTEGER,
            blockNumber INTEGER,
            PRIMARY KEY (blockHash, transactionHash, logIndex)
        )";
    connection.Execute(createTokensPurchasedEventTable);

    var createTokensBurnedEventTable = @"
        CREATE TABLE IF NOT EXISTS TokensBurnedEvent (
            buyer TEXT,
            subreddit TEXT,
            tokens INTEGER,
            blockHash TEXT,
            transactionHash TEXT,
            logIndex INTEGER,
            blockNumber INTEGER,
            PRIMARY KEY (blockHash, transactionHash, logIndex)
        )";
    connection.Execute(createTokensBurnedEventTable);

    var createSeasonWonEventTable = @"
        CREATE TABLE IF NOT EXISTS SeasonWonEvent (
            subreddit TEXT,
            tokens INTEGER,
            Season INTEGER,
            blockHash TEXT,
            transactionHash TEXT,
            logIndex INTEGER,
            blockNumber INTEGER,
            PRIMARY KEY (blockHash, transactionHash, logIndex)
        )";
    connection.Execute(createSeasonWonEventTable);

    // Clean up
    SqliteConnection.ClearAllPools();
    File.Delete("hello.db");
}

// fix: An unhandled exception of type 'System.IO.IOException' occurred in System.Private.CoreLib.dll: 'The process cannot access the file 'E:\code\subr\backend\hello.db' because it is being used by another process.'
// todo: when new request, only request events from latest block number in database
// todo: probably rate limit not the API, but calls to blockchain somehow, when API is called (stale data is fine to some extent)
// todo: allow query from-block=blockNumber&to-block=blockNumber
// todo: move Events.ContractAddress in to appsettings.json
// todo: hide Events.AlchemyApiKey apiKey somehow

public class BigIntegerConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options) =>
        writer.WriteRawValue(value.ToString(NumberFormatInfo.InvariantInfo), false);
}

public class HexBigIntegerConverter : JsonConverter<HexBigInteger>
{
    public override HexBigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, HexBigInteger value, JsonSerializerOptions options) =>
        writer.WriteRawValue(value.Value.ToString(NumberFormatInfo.InvariantInfo), false);
}