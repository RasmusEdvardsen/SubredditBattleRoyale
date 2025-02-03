namespace Backend.Functionality
{
    public record TokensPurchasedApiModel
    {
        public required string Id { get; set; }
        public string EventType { get; set; } = "TokensPurchased";
        public required string Buyer { get; set; }
        public required string Subreddit { get; set; }
        public ulong Tokens { get; set; }
        public required string TransactionHash { get; set; }
        public ulong BlockNumber { get; set; }

        public static TokensPurchasedApiModel FromDatabase(TokensPurchased tokensPurchased)
        {
            return new()
            {
                Id = tokensPurchased.Id,
                Buyer = tokensPurchased.Buyer,
                Subreddit = tokensPurchased.Subreddit,
                Tokens = tokensPurchased.Tokens,
                TransactionHash = tokensPurchased.TransactionHash,
                BlockNumber = tokensPurchased.BlockNumber
            };
        }
    }

    public record TokensBurnedApiModel
    {
        public required string Id { get; set; }
        public string EventType { get; set; } = "TokensBurned";
        public required string Buyer { get; set; }
        public required string Subreddit { get; set; }
        public ulong Tokens { get; set; }
        public required string TransactionHash { get; set; }
        public ulong BlockNumber { get; set; }

        public static TokensBurnedApiModel FromDatabase(TokensBurned tokensBurned)
        {
            return new()
            {
                Id = tokensBurned.Id,
                Buyer = tokensBurned.Buyer,
                Subreddit = tokensBurned.Subreddit,
                Tokens = tokensBurned.Tokens,
                TransactionHash = tokensBurned.TransactionHash,
                BlockNumber = tokensBurned.BlockNumber
            };
        }
    }

    public record SeasonWonApiModel
    {
        public required string Id { get; set; }
        public string EventType { get; set; } = "SeasonWon";
        public required string Subreddit { get; set; }
        public required string Tokens { get; set; }
        public ulong Season { get; set; }
        public required string TransactionHash { get; set; }
        public ulong BlockNumber { get; set; }

        public static SeasonWonApiModel FromDatabase(SeasonWon seasonWon)
        {
            return new()
            {
                Id = seasonWon.Id,
                Subreddit = seasonWon.Subreddit,
                Tokens = seasonWon.Tokens,
                Season = seasonWon.Season,
                TransactionHash = seasonWon.TransactionHash,
                BlockNumber = seasonWon.BlockNumber
            };
        }
    }
}