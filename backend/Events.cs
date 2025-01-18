using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Dapper;
using Microsoft.Data.Sqlite;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Backend
{
    public class Events(string ContractAddress = "", string AlchemyApiKey = "")
    {
        private readonly Web3 web3 = new($"https://eth-mainnet.g.alchemy.com/v2/{AlchemyApiKey}");
        
        public async Task<AllEvents> GetAllEvents()
        {
            using var connection = new SqliteConnection("Data Source=hello.db");
            
            var tokensPurchased = await connection.QueryAsync<TokensPurchased>("SELECT * FROM TokensPurchased");
            var blockchainSync = await connection.QueryAsync<BlockchainSync>("SELECT * FROM BlockchainSync");

            if (blockchainSync.Any() && blockchainSync.Max(s => s.SyncedAt) > DateTime.Now - TimeSpan.FromMinutes(1))
            {
                return new AllEvents(tokensPurchased, [], []);
            }

            #region TokensPurchased
            var fromBlock = tokensPurchased.Any() ? tokensPurchased.Max(e => e.BlockNumber) + 1 : 0;
            
            var tokensPurchasedEventsBlockChain = await GetEventLogs<TokensPurchasedEvent>(ContractAddress, fromBlock);
            var tokensPurchasedEvents = tokensPurchasedEventsBlockChain.Select(TokensPurchased.FromBlockChain);
            
            foreach (var eventItem in tokensPurchasedEvents)
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO TokensPurchased (Id, Buyer, Subreddit, Tokens, BlockHash, TransactionHash, LogIndex, BlockNumber)
                    VALUES (@Id, @Buyer, @Subreddit, @Tokens, @BlockHash, @TransactionHash, @LogIndex, @BlockNumber)", eventItem);
            }
            
            tokensPurchasedEvents = await connection.QueryAsync<TokensPurchased>("SELECT * FROM TokensPurchased");
            #endregion

            await connection.ExecuteAsync("INSERT INTO BlockchainSync (SyncedAt) VALUES (DATETIME('now'))");

            return new AllEvents(tokensPurchasedEvents, [], []);
        }

        private async Task<IEnumerable<EventLog<TEventDTO>>> GetEventLogs<TEventDTO>(string contractAddress, ulong fromBlock) where TEventDTO : IEventDTO, new()
        {
            var eventHandler = web3.Eth.GetEvent<TEventDTO>(contractAddress);
            
            var filter = eventHandler.CreateFilterInput();
            filter.FromBlock = new BlockParameter(fromBlock);
            
            return await eventHandler.GetAllChangesAsync(filter);
        }
    }

    public interface ILog
    {
        string BlockHash { get; set; }
        string TransactionHash { get; set; }
        ulong LogIndex { get; set; }
        ulong BlockNumber { get; set; }
    }

    public record struct AllEvents(
        IEnumerable<TokensPurchased> TokensPurchased,
        IEnumerable<TokensBurned> TokensBurned,
        IEnumerable<SeasonWon> SeasonWon);


    [Event("TokensPurchased")]
    public record TokensPurchasedEvent : IEventDTO
    {
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 3, false)] public BigInteger Tokens { get; set; }
    }

    [Event("TokensBurned")]
    public record TokensBurnedEvent : IEventDTO
    {
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 3, false)] public BigInteger Tokens { get; set; }
    }

    [Event("SeasonWon")]
    public record SeasonWonEvent : IEventDTO
    {
        [Parameter("string", "subreddit", 1, true)] public string? Subreddit { get; set; }
        [Parameter("uint256", "tokens", 2, false)] public string? Tokens { get; set; }
        [Parameter("uint256", "season", 3, false)] public BigInteger Season { get; set; }
    }

    public record TokensPurchased : ILog
    {
        public required string Id { get; set; }
        public required string Buyer { get; set; }
        public required string Subreddit { get; set; }
        public ulong Tokens { get; set; }
        public required string BlockHash { get; set; }
        public required string TransactionHash { get; set; }
        public ulong LogIndex { get; set; }
        public ulong BlockNumber { get; set; }

        public static TokensPurchased FromBlockChain(EventLog<TokensPurchasedEvent> eventItem)
        {
            return new()
            {
                Id = $"{eventItem.Log.BlockHash}_{eventItem.Log.TransactionHash}_{eventItem.Log.LogIndex}",
                Buyer = eventItem.Event.Buyer!,
                Subreddit = eventItem.Event.Subreddit!,
                Tokens = (ulong)eventItem.Event.Tokens,
                BlockHash = eventItem.Log.BlockHash,
                TransactionHash = eventItem.Log.TransactionHash,
                LogIndex = (ulong)eventItem.Log.LogIndex.Value,
                BlockNumber = (ulong)eventItem.Log.BlockNumber.Value
            };
        }
    }

    public record TokensBurned : ILog
    {
        public required string Buyer { get; set; }
        public required string Subreddit { get; set; }
        public ulong Tokens { get; set; }
        public required string BlockHash { get; set; }
        public required string TransactionHash { get; set; }
        public ulong LogIndex { get; set; }
        public ulong BlockNumber { get; set; }
    }

    public record SeasonWon : ILog
    {
        public required string Subreddit { get; set; }
        public required string Tokens { get; set; }
        public ulong Season { get; set; }
        public required string BlockHash { get; set; }
        public required string TransactionHash { get; set; }
        public ulong LogIndex { get; set; }
        public ulong BlockNumber { get; set; }
    }

    public record struct BlockchainSync
    {
        public int Id { get; set; }
        public DateTime SyncedAt { get; set; }
    }
}
