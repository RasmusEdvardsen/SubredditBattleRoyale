using System.Numerics;
using System.Runtime.Serialization;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;

namespace Backend
{
    public class Events
    {
        public static async Task<AllEvents> GetAllEvents()
        {
            // todo: move in to appsettings.json
            var contractAddress = "";

            // todo: hide apiKey somehow
            var alchemyApiKey = "";

            var web3 = new Web3($"https://eth-mainnet.g.alchemy.com/v2/${alchemyApiKey}");

            var tokensPurchasedEventHandler = web3.Eth.GetEvent<TokensPurchasedEvent>(contractAddress);
            var allTokensPurchasedEventsFilter = tokensPurchasedEventHandler.CreateFilterInput();
            var tokensPurchasedEvents = await tokensPurchasedEventHandler.GetAllChangesAsync(allTokensPurchasedEventsFilter);

            var tokensBurnedEventEventHandler = web3.Eth.GetEvent<TokensBurnedEvent>(contractAddress);
            var allTokensBurnedEventEventsFilter = tokensBurnedEventEventHandler.CreateFilterInput();
            var tokensBurnedEventEvents = await tokensBurnedEventEventHandler.GetAllChangesAsync(allTokensBurnedEventEventsFilter);

            var seasonWonEventEventHandler = web3.Eth.GetEvent<SeasonWonEvent>(contractAddress);
            var allSeasonWonEventEventsFilter = seasonWonEventEventHandler.CreateFilterInput();
            var seasonWonEventPurchasedEvents = await seasonWonEventEventHandler.GetAllChangesAsync(allSeasonWonEventEventsFilter);

            var allEvents = new List<IEventDTO>();

            return new AllEvents(
                tokensPurchasedEvents.Select(s => s.Event),
                tokensBurnedEventEvents.Select(s => s.Event),
                seasonWonEventPurchasedEvents.Select(s => s.Event));
        }
    }

    public record struct AllEvents(
        IEnumerable<TokensPurchasedEvent> TokensPurchasedEvents,
        IEnumerable<TokensBurnedEvent> TokensBurnedEvents,
        IEnumerable<SeasonWonEvent> SeasonWonEvents);

    [Event("TokensPurchased")]
    public record TokensPurchasedEvent : IEventDTO
    {
        [DataMember]
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 2, false)] public BigInteger Tokens { get; set; }
    }

    [Event("TokensBurned")]
    public record TokensBurnedEvent : IEventDTO
    {
        [Parameter("address", "buyer", 1, true)] public string? Buyer { get; set; }
        [Parameter("string", "subreddit", 2, false)] public string? Subreddit { get; set; }
        [Parameter("uint256", "amount", 2, false)] public BigInteger Tokens { get; set; }
    }

    [Event("SeasonWon")]
    public record SeasonWonEvent : IEventDTO
    {
        [Parameter("string", "subreddit", 1, true)] public string? Subreddit { get; set; }
        [Parameter("uint256", "tokens", 2, false)] public string? Tokens { get; set; }
        [Parameter("uint256", "season", 2, false)] public BigInteger Season { get; set; }
    }
}
