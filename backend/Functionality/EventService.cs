using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Functionality;

public interface IEventService
{
    Task<AllEvents> GetAllEvents();
}

public class EventService(IBlockchainSynchronizer _blockchainSynchronizer) : IEventService
{
    public async Task<AllEvents> GetAllEvents()
    {
        using var connection = new SqliteConnection("Data Source=hello.db");

        await _blockchainSynchronizer.SyncBlockchainIfNeeded();

        var tokensPurchased = connection.QueryAsync<TokensPurchased>("SELECT * FROM TokensPurchased");
        var tokensBurned = connection.QueryAsync<TokensBurned>("SELECT * FROM TokensBurned");
        var seasonWon = connection.QueryAsync<SeasonWon>("SELECT * FROM SeasonWon");

        await Task.WhenAll(tokensPurchased, tokensBurned, seasonWon);

        return new AllEvents(tokensPurchased.Result, tokensBurned.Result, seasonWon.Result);
    }
}
