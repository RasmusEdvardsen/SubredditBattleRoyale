// SPDX-License-Identifier: MIT
pragma solidity ^0.8.28;

// Author: rasmusedvardsen
contract SubredditBattleRoyale {
    uint256 public constant INITIAL_SUPPLY = 1_000_000;
    uint256 public constant TOKEN_PRICE = 0.0001 ether; // In WEI
    uint256 public BURN_MULTIPLIER = 3;
    uint256 public constant MAX_SUBREDDIT_LENGTH = 3 + 21; // "/r/" + 21 chars

    struct Subreddit {
        string name;
        uint256 tokenCount;
    }

    mapping(string => uint256) public subredditTokenBalances; // Track token balances by subreddit
    uint256 public voidTokenCount; // Tokens belonging to "the void"
    uint256 public currentSeason = 1;

    address public immutable owner;

    event TokensPurchased(address indexed buyer, string subreddit, uint256 amount);
    event TokensBurned(address indexed buyer, string subreddit, uint256 amount);
    event SeasonWon(string subreddit, uint256 tokens, uint256 season);

    modifier validSubreddit(string memory subreddit) {
        bytes memory b = bytes(subreddit);
        require(b.length <= MAX_SUBREDDIT_LENGTH, "Subreddit name too long");
        require(b.length > 3, "Subreddit name too short"); // Must be at least "/r/" + 1 char
        require(b[0] == "/" || b[1] == "r" || b[2] == "/", "Subreddit must start with '/r/'");
        _;
    }

    modifier onlyOwner() {
        require(msg.sender == owner, "Only the owner can call this function");
        _;
    }

    constructor() {
        voidTokenCount = INITIAL_SUPPLY; // Initialize "the void" with the initial supply of tokens
        owner = msg.sender;

        _initState();
    }

    function purchaseTokens(string memory subreddit, uint256 amount) external payable validSubreddit(subreddit) {
        require(msg.value == amount * TOKEN_PRICE, "Incorrect Ether sent");

        subreddit = _toLowerCase(subreddit);

        subredditTokenBalances[subreddit] += amount;
        voidTokenCount -= amount;

        emit TokensPurchased(msg.sender, subreddit, amount);

        if (subredditTokenBalances[subreddit] > voidTokenCount) {
            emit SeasonWon(subreddit, subredditTokenBalances[subreddit], currentSeason);
            _startNewSeason();
        }
    }
    
    function burnTokens(string memory subreddit, uint256 amount) external payable validSubreddit(subreddit) {
        require(msg.value == amount * TOKEN_PRICE, "Incorrect Ether sent");

        subreddit = _toLowerCase(subreddit);

        require(subredditTokenBalances[subreddit] >= amount * BURN_MULTIPLIER, "Not enough tokens to burn");

        subredditTokenBalances[subreddit] -= amount * BURN_MULTIPLIER;
        voidTokenCount -= amount; // Reduce tokens in "the void"

        emit TokensBurned(msg.sender, subreddit, amount);
    }
    
    function setBurnMultiplier(uint256 newMultiplier) external onlyOwner {
        require(newMultiplier > 0, "Multiplier must be greater than 0");
        require(newMultiplier <= 100, "Multiplier must be less than  or equal to 10o");
        BURN_MULTIPLIER = newMultiplier;
    }
    
    function withdraw() external onlyOwner {
        require(address(this).balance > 0, "No Ether available to withdraw");

        payable(owner).transfer(address(this).balance);
    }

    function _startNewSeason() internal {
        // Mint new tokens and assign them to "the void"
        currentSeason++;
        voidTokenCount += INITIAL_SUPPLY + (INITIAL_SUPPLY / currentSeason);
    }

    function _toLowerCase(string memory str) internal pure returns (string memory) {
        bytes memory bStr = bytes(str);
        bytes memory bLower = new bytes(bStr.length);
        for (uint256 i = 0; i < bStr.length; i++) {
            // Uppercase character
            if ((uint8(bStr[i]) >= 65) && (uint8(bStr[i]) <= 90)) {
                bLower[i] = bytes1(uint8(bStr[i]) + 32);
            } else {
                bLower[i] = bStr[i];
            }
        }
        return string(bLower);
    }

    // 24182 tokens distributed in total at initial state.
    function _initState() internal {
        _incrementSubredditTokensDecrementVoidTokens("/r/ethereum", 576);
        _incrementSubredditTokensDecrementVoidTokens("/r/solana", 1036);
        _incrementSubredditTokensDecrementVoidTokens("/r/bitcoin", 329);
        _incrementSubredditTokensDecrementVoidTokens("/r/dogecoin", 298);
        _incrementSubredditTokensDecrementVoidTokens("/r/shib", 207);
        _incrementSubredditTokensDecrementVoidTokens("/r/liverpoolfc", 938);
        _incrementSubredditTokensDecrementVoidTokens("/r/mcfc", 857);
        _incrementSubredditTokensDecrementVoidTokens("/r/chelseafc", 913);
        _incrementSubredditTokensDecrementVoidTokens("/r/dota2", 1326);
        _incrementSubredditTokensDecrementVoidTokens("/r/leagueoflegends", 1092);
        _incrementSubredditTokensDecrementVoidTokens("/r/globaloffensive", 387);
        _incrementSubredditTokensDecrementVoidTokens("/r/pathofexile", 997);
        _incrementSubredditTokensDecrementVoidTokens("/r/diablo4", 378);
        _incrementSubredditTokensDecrementVoidTokens("/r/rust", 2029);
        _incrementSubredditTokensDecrementVoidTokens("/r/csharp", 1290);
        _incrementSubredditTokensDecrementVoidTokens("/r/python", 3829);
        _incrementSubredditTokensDecrementVoidTokens("/r/cpp", 1027);
        _incrementSubredditTokensDecrementVoidTokens("/r/java", 1726);
        _incrementSubredditTokensDecrementVoidTokens("/r/solidity", 27);
        _incrementSubredditTokensDecrementVoidTokens("/r/javascript", 4093);
        _incrementSubredditTokensDecrementVoidTokens("/r/golang", 827);
    }
    
    function _incrementSubredditTokensDecrementVoidTokens(string memory subreddit, uint256 amount) internal {
        subredditTokenBalances[subreddit] += amount;
        voidTokenCount -= amount;

        emit TokensPurchased(msg.sender, subreddit, amount);
    }
}
