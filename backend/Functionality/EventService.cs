using Dapper;
using Microsoft.Data.Sqlite;

namespace Backend.Functionality
{
    public class Events
    {
        public async Task<AllEvents> GetAllEvents()
        {
            using var connection = new SqliteConnection("Data Source=hello.db");

            await new BlockchainSynchronizer().SyncBlockchainIfNeeded();

            var tokensPurchased = connection.QueryAsync<TokensPurchased>("SELECT * FROM TokensPurchased");
            var tokensBurned = connection.QueryAsync<TokensBurned>("SELECT * FROM TokensBurned");
            var seasonWon = connection.QueryAsync<SeasonWon>("SELECT * FROM SeasonWon");

            await Task.WhenAll(tokensPurchased, tokensBurned, seasonWon);

            return new AllEvents(tokensPurchased.Result, tokensBurned.Result, seasonWon.Result);
        }
    }
}
