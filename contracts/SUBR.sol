// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SubredditBattleRoyale {
    struct Token {
        string subreddit; // Subreddit associated with the token
        uint256 version; // Game version of the token
    }

    uint256 public constant INITIAL_SUPPLY = 1_000_000; // Initial supply of tokens in the void
    uint256 public gameNumber; // Current game number
    uint256 public tokenPrice; // Current price of a token
    uint256 public tokenSupply; // Total supply of tokens for the current game
    uint256 public transactionFeeRate = 10; // Fee rate in basis points (0.1%)

    address public owner; // Contract owner
    address public voidWallet; // The void wallet
    address public feeWallet; // Wallet for collecting fees

    mapping(address => uint256) public playerBalances; // Player token balances
    mapping(uint256 => mapping(uint256 => Token)) public tokenDetails; // Mapping for token details per version
    mapping(string => uint256) public subredditBalances; // Subreddit token balances

    uint256 public nextTokenId; // Unique token ID tracker

    event TokensPurchased(address indexed purchaser, string subreddit, uint256 amount);
    event GameEnded(string winner, uint256 tokens);
    event TokensTraded(address indexed from, address indexed to, uint256 amount, string newSubreddit);

    modifier onlyOwner() {
        require(msg.sender == owner, "Not the owner");
        _;
    }

    constructor(address _feeWallet) {
        owner = msg.sender;
        feeWallet = _feeWallet;
        voidWallet = address(this); // The void wallet is the contract address itself
        gameNumber = 1;
        tokenPrice = 0.01 ether; // Initial token price
        tokenSupply = INITIAL_SUPPLY;
        subredditBalances["the void"] = INITIAL_SUPPLY;
    }

    // Buy tokens for a subreddit or the void
    function buyTokens(string memory subreddit, uint256 amount) external payable {
        require(msg.value == amount * tokenPrice, "Incorrect Ether sent");

        uint256 fee = (msg.value * transactionFeeRate) / 10000;
        uint256 netAmount = msg.value - fee;

        payable(feeWallet).transfer(fee);

        // Update subreddit balances
        if (isValidSubreddit(subreddit)) {
            subredditBalances[subreddit] += amount;
        } else {
            subreddit = "the void";
            subredditBalances[subreddit] += amount;
        }

        subredditBalances["the void"] -= amount; // Tokens leave the void
        playerBalances[msg.sender] += amount;

        // Assign tokens with details
        for (uint256 i = 0; i < amount; i++) {
            tokenDetails[gameNumber][nextTokenId] = Token(subreddit, gameNumber);
            nextTokenId++;
        }

        emit TokensPurchased(msg.sender, subreddit, amount);
        checkGameEnd();
    }

    // Check if the game has ended and determine a winner
    function checkGameEnd() public {
        if (subredditBalances["the void"] == 0) {
            startNewGame();
            return;
        }

        for (uint256 i = 0; i < nextTokenId; i++) {
            string memory subreddit = tokenDetails[gameNumber][i].subreddit;
            if (subredditBalances[subreddit] > subredditBalances["the void"]) {
                emit GameEnded(subreddit, subredditBalances[subreddit]);
                startNewGame();
                return;
            }
        }
    }

    // Start a new game
    function startNewGame() internal {
        gameNumber++;
        tokenPrice = (tokenPrice * 110) / 100; // Increase token price by 10%
        tokenSupply = INITIAL_SUPPLY + (gameNumber * 10_000); // Mint additional tokens

        subredditBalances["the void"] = tokenSupply;

        // Reset all other subreddit balances
        for (uint256 i = 0; i < nextTokenId; i++) {
            string memory subreddit = tokenDetails[gameNumber - 1][i].subreddit;
            subredditBalances[subreddit] = 0;
        }

        nextTokenId = 0; // Reset token ID tracker
    }

    // Trade tokens between players, optionally updating subreddit
    function tradeTokens(address to, uint256 amount, string memory newSubreddit) external {
        require(playerBalances[msg.sender] >= amount, "Insufficient balance");

        playerBalances[msg.sender] -= amount;
        playerBalances[to] += amount;

        // Optionally update token details
        if (isValidSubreddit(newSubreddit)) {
            for (uint256 i = 0; i < amount; i++) {
                uint256 tokenId = nextTokenId - amount + i;
                tokenDetails[gameNumber][tokenId].subreddit = newSubreddit;
            }
        }

        emit TokensTraded(msg.sender, to, amount, newSubreddit);
    }

    // Validate subreddit format
    function isValidSubreddit(string memory subreddit) internal pure returns (bool) {
        bytes memory subredditBytes = bytes(subreddit);
        return subredditBytes.length > 3 && subredditBytes[0] == '/' && subredditBytes[1] == 'r' && subredditBytes[2] == '/';
    }
}
