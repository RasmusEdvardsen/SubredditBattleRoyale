using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Functionality;

public record struct AllEvents(
    VoidTokenCount VoidTokenCount,
    IEnumerable<TokensPurchasedApiModel> TokensPurchased,
    IEnumerable<TokensBurnedApiModel> TokensBurned,
    IEnumerable<SeasonWonApiModel> SeasonWon);

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

        var voidTokenCount = connection.QuerySingleAsync<VoidTokenCount>("SELECT * FROM VoidTokenCount");
        var tokensPurchased = connection.QueryAsync<TokensPurchased>("SELECT * FROM TokensPurchased");
        var tokensBurned = connection.QueryAsync<TokensBurned>("SELECT * FROM TokensBurned");
        var seasonWon = connection.QueryAsync<SeasonWon>("SELECT * FROM SeasonWon");

        await Task.WhenAll(voidTokenCount, tokensPurchased, tokensBurned, seasonWon);

        return new AllEvents(
            voidTokenCount.Result,
            tokensPurchased.Result.Select(TokensPurchasedApiModel.FromDatabase),
            tokensBurned.Result.Select(TokensBurnedApiModel.FromDatabase),
            seasonWon.Result.Select(SeasonWonApiModel.FromDatabase)
        );
    }
}
