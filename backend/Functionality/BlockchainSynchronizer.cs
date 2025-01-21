using Dapper;
using Microsoft.Data.Sqlite;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Z.Dapper.Plus;

namespace Backend.Functionality
{
    public class BlockchainSynchronizer(string ContractAddress = "", string AlchemyApiKey = "")
    {
        private readonly Web3 web3 = new($"https://eth-mainnet.g.alchemy.com/v2/{AlchemyApiKey}");

        public async Task SyncBlockchainIfNeeded()
        {
            using var connection = new SqliteConnection("Data Source=hello.db");

            var blockchainSync = await connection.QueryAsync<BlockchainSync>("SELECT * FROM BlockchainSync");
            if (!blockchainSync.Any() || blockchainSync.Max(s => s.SyncedAt) < DateTime.UtcNow - TimeSpan.FromSeconds(15))
            {
                await Task.WhenAll([
                    SyncEvents<TokensPurchasedEvent, TokensPurchased>("TokensPurchased", TokensPurchased.FromBlockChain),
                    SyncEvents<TokensBurnedEvent, TokensBurned>("TokensBurned", TokensBurned.FromBlockChain),
                    SyncEvents<SeasonWonEvent, SeasonWon>("SeasonWon", SeasonWon.FromBlockChain)
                ]);

                await connection.ExecuteAsync("INSERT INTO BlockchainSync (SyncedAt) VALUES (DATETIME('now'))");
            }
        }

        private async Task SyncEvents<TEventDTO, TEvent>(string tableName, Func<EventLog<TEventDTO>, TEvent> objectMapper) where TEventDTO : IEventDTO, new()
        {
            using var connection = new SqliteConnection("Data Source=hello.db");

            var existingEntries = await connection.QueryAsync<ulong>($"SELECT COUNT(BlockNumber) FROM {tableName}");

            var fromBlock = existingEntries.Any() ? existingEntries.Max() + 1 : 0;

            var eventsBlockChain = await GetEventLogs<TEventDTO>(ContractAddress, fromBlock);

            var events = eventsBlockChain.Select(objectMapper);

            await connection.UseBulkOptions(s => s.InsertIfNotExists = true).BulkInsertAsync(events);
        }

        private async Task<IEnumerable<EventLog<TEventDTO>>> GetEventLogs<TEventDTO>(string contractAddress, ulong fromBlock) where TEventDTO : IEventDTO, new()
        {
            var eventHandler = web3.Eth.GetEvent<TEventDTO>(contractAddress);

            // todo: does this actually work?
            var filter = eventHandler.CreateFilterInput();
            filter.FromBlock = new BlockParameter(fromBlock);

            return await eventHandler.GetAllChangesAsync(filter);
        }
    }
}
