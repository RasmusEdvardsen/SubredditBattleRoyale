export interface BackendData {
    voidTokenCount: VoidTokenCount;
    tokensPurchased: TokensPurchasedEvent[];
    tokensBurned: TokensBurnedEvent[];
    seasonWon: SeasonWonEvent[];
}

export interface VoidTokenCount {
    id: string;
    balance: number;
}

export interface TokensPurchasedEvent {
    id: string;
    eventType: string;
    buyer: string;
    subreddit: string;
    tokens: number;
    transactionHash: string;
    blockNumber: number;
}

export interface TokensBurnedEvent {
    id: string;
    eventType: string;
    buyer: string;
    subreddit: string;
    tokens: number;
    transactionHash: string;
    blockNumber: number;
}

export interface SeasonWonEvent {
    id: string
    eventType: string;
    subreddit: string;
    tokens: number;
    season: number;
    transactionHash: string;
    blockNumber: number;
}

export interface BubbleCloudEntry {
    id: string;
    value: number;
    radius: number;
    x?: number;
    y?: number;
    fx?: number | null;
    fy?: number | null;
}
