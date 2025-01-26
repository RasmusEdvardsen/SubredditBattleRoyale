using Nethereum.Contracts;

namespace Backend.Functionality;

public interface ILog
{
    string BlockHash { get; set; }
    string TransactionHash { get; set; }
    ulong LogIndex { get; set; }
    ulong BlockNumber { get; set; }
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
    public required string Id { get; set; }
    public required string Buyer { get; set; }
    public required string Subreddit { get; set; }
    public ulong Tokens { get; set; }
    public required string BlockHash { get; set; }
    public required string TransactionHash { get; set; }
    public ulong LogIndex { get; set; }
    public ulong BlockNumber { get; set; }

    public static TokensBurned FromBlockChain(EventLog<TokensBurnedEvent> eventItem)
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

public record SeasonWon : ILog
{
    public required string Id { get; set; }
    public required string Subreddit { get; set; }
    public required string Tokens { get; set; }
    public ulong Season { get; set; }
    public required string BlockHash { get; set; }
    public required string TransactionHash { get; set; }
    public ulong LogIndex { get; set; }
    public ulong BlockNumber { get; set; }

    public static SeasonWon FromBlockChain(EventLog<SeasonWonEvent> eventItem)
    {
        return new()
        {
            Id = $"{eventItem.Log.BlockHash}_{eventItem.Log.TransactionHash}_{eventItem.Log.LogIndex}",
            Subreddit = eventItem.Event.Subreddit!,
            Tokens = eventItem.Event.Tokens!,
            Season = (ulong)eventItem.Event.Season,
            BlockHash = eventItem.Log.BlockHash,
            TransactionHash = eventItem.Log.TransactionHash,
            LogIndex = (ulong)eventItem.Log.LogIndex.Value,
            BlockNumber = (ulong)eventItem.Log.BlockNumber.Value
        };
    }
}

public record struct BlockchainSync
{
    public int Id { get; set; }
    public DateTime SyncedAt { get; set; }
}

public record VoidTokenCount
{
    public int Id { get; set; } = 1;
    public required ulong Balance { get; set; }
}