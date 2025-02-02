interface TokenTransaction {
    id: string;
    buyer: string;
    subreddit: string;
    tokens: number;
    blockHash: string;
    transactionHash: string;
    logIndex: number;
    blockNumber: number;
}

interface VoidTokenCount {
    id: number;
    balance: number;
}

interface TokensPurchased extends TokenTransaction {}

interface TokensBurned extends TokenTransaction {}

interface SeasonWon extends TokenTransaction {
    season: number;
}

// todo: DO THIS INSTEAD OF THE ABOVE
// event has id, blockhash, txnhash, logindex, blocknumber
// tokenevent has buyer, subreddit, tokens
// tokenpurchaseevent and tokenburnevent extend tokenevent with no additional fields
// seasonevent has season, subreddit, tokens and extends event