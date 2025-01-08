// SPDX-License-Identifier: MIT
pragma solidity ^0.8.28;

// Author: rasmusedvardsen
contract SubredditBattleRoyale {
    uint256 public constant INITIAL_SUPPLY = 1_000_000;
    uint256 public constant TOKEN_PRICE = 0.001 ether; // In WEI
    uint256 public constant BURN_MULTIPLIER = 3;
    uint256 public constant MAX_SUBREDDIT_LENGTH = 3 + 21; // "/r/" + 21 chars

    struct Subreddit {
        string name;
        uint256 tokenCount;
    }

    mapping(string => uint256) public subredditTokenBalances; // Track token balances by subreddit
    uint256 public voidTokenCount; // Tokens belonging to "the void"
    uint256 public currentSeason = 1;

    address public owner;

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
    }

    function purchaseTokens(string memory subreddit, uint256 amount) public payable validSubreddit(subreddit) {
        require(msg.value == amount * TOKEN_PRICE, "Incorrect Ether sent");

        subreddit = _toLowerCase(subreddit);

        subredditTokenBalances[subreddit] += amount;
        voidTokenCount -= amount; // Reduce tokens in "the void"

        emit TokensPurchased(msg.sender, subreddit, amount);

        if (subredditTokenBalances[subreddit] > voidTokenCount) {
            emit SeasonWon(subreddit, subredditTokenBalances[subreddit], currentSeason);
            _startNewSeason();
        }
    }

    function burnTokens(string memory subreddit, uint256 amount) public payable validSubreddit(subreddit) {
        require(msg.value == amount * TOKEN_PRICE, "Incorrect Ether sent");
        require(subredditTokenBalances[subreddit] >= amount * BURN_MULTIPLIER, "Not enough tokens to burn");

        subreddit = _toLowerCase(subreddit);

        subredditTokenBalances[subreddit] -= amount * BURN_MULTIPLIER;
        voidTokenCount -= amount; // Reduce tokens in "the void"

        emit TokensBurned(msg.sender, subreddit, amount);
    }

    function _startNewSeason() internal {
        // Mint new tokens and assign them to "the void"
        uint256 newTokens = INITIAL_SUPPLY;
        voidTokenCount += newTokens;
        currentSeason++;
    }

    function withdraw() public onlyOwner {
        require(address(this).balance > 0, "No Ether available to withdraw");

        payable(owner).transfer(address(this).balance);
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
}
