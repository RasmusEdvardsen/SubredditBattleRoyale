using System.Numerics;
using System.Runtime.Serialization;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace Backend
{
    public class Events(string ContractAddress = "", string AlchemyApiKey = "")
    {
        private readonly Web3 web3 = new($"https://eth-mainnet.g.alchemy.com/v2/{AlchemyApiKey}");
        
        public async Task<AllEvents> GetAllEvents()
        {
            var tokensPurchasedEvents = await GetEventLogs<TokensPurchasedEvent>(ContractAddress);
            var tokensBurnedEventEvents = await GetEventLogs<TokensBurnedEvent>(ContractAddress);
            var seasonWonEventEvents = await GetEventLogs<SeasonWonEvent>(ContractAddress);

            return new AllEvents(tokensPurchasedEvents, tokensBurnedEventEvents, seasonWonEventEvents);
        }

        private async Task<IEnumerable<TEventDTO>> GetEventLogs<TEventDTO>(string contractAddress) where TEventDTO : IEventDTO, IUniqueLog, new()
        {
            var eventHandler = web3.Eth.GetEvent<TEventDTO>(contractAddress);
            var filter = eventHandler.CreateFilterInput();
            var events = await eventHandler.GetAllChangesAsync(filter);
            var mappedEvents = events.Select(s =>
            {
                var mappedEvent = s.Event;
                mappedEvent.BlockHash = s.Log.BlockHash;
                mappedEvent.TransactionHash = s.Log.TransactionHash;
                mappedEvent.LogIndex = s.Log.LogIndex;
                return mappedEvent;
            });
            return mappedEvents;
        }
    }

    public interface IUniqueLog
    {
        string? BlockHash { get; set; }
        string? TransactionHash { get; set; }
        HexBigInteger? LogIndex { get; set; }
    }

    public record struct AllEvents(
        IEnumerable<TokensPurchasedEvent> TokensPurchasedEvents,
        IEnumerable<TokensBurnedEvent> TokensBurnedEvents,
        IEnumerable<SeasonWonEvent> SeasonWonEvents);


    [Event("TokensPurchased")]
    public record TokensPurchasedEvent : IEventDTO, IUniqueLog
    {
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 3, false)] public BigInteger Tokens { get; set; }
        public string? BlockHash { get; set; }
        public string? TransactionHash { get; set; }
        public HexBigInteger? LogIndex { get; set; }
    }

    [Event("TokensBurned")]
    public record TokensBurnedEvent : IEventDTO, IUniqueLog
    {
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 3, false)] public BigInteger Tokens { get; set; }
        public string? BlockHash { get; set; }
        public string? TransactionHash { get; set; }
        public HexBigInteger? LogIndex { get; set; }
    }

    [Event("SeasonWon")]
    public record SeasonWonEvent : IEventDTO, IUniqueLog
    {
        [Parameter("string", "subreddit", 1, true)] public string? Subreddit { get; set; }
        [Parameter("uint256", "tokens", 2, false)] public string? Tokens { get; set; }
        [Parameter("uint256", "season", 3, false)] public BigInteger Season { get; set; }
        public string? BlockHash { get; set; }
        public string? TransactionHash { get; set; }
        public HexBigInteger? LogIndex { get; set; }
    }
}
