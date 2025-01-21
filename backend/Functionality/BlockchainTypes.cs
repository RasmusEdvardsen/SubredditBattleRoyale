using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace Backend.Functionality
{
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
}
