# SubredditBattleRoyale

SubredditBattleRoyale is a battle royale token for subreddits to flex on each other, or for people to secure the void will win against subreddits. When a winner is found, a new and harder season is started.

## Game summary
* Start with 1_000_000 tokens. They cost 0.0001 ether to purchase. 
* Tokens should be inscribed with a subreddit. It should always start with "/r/" followed by no less than one character.
* A "season" is won, when a subreddit has more tokens than "the void". After that, a new season starts. Old tokens still count, but new tokens are minted for the new season.
* You can also burn tokens, which will burn amount 3 times the amount of tokens of the specified subreddit.

## Future ideas
* If some time expires before anyone gets more, the void wins?
* Tokens are versioned for each season?
* Make it harder to buy more tokens?
* Maybe "kill" subreddits with low amount of tokens?
* The void could also just get stronger after every win, then old coins would still matter?
* Create ERC20 tokens. Seems difficult though, as we need metadata (subreddit) for each token trade.
* Consider Solana as a blockchain instead, and figure out what the equivalent of an ERC-20 standard token is for Solana.
* Figure out way for onlyOwner to add/remove subreddits.

## Progress
* [SubredditBattleRoyale smart contract on Ethereum mainnet](https://etherscan.io/address/0xea8831bcb719914ab97131f48d9b2dc737dbd25a)
* [SubredditBattleRoyale smart contract on Sepolia Ethereum testnet](https://sepolia.etherscan.io/address/0x47e330c6a28bb7b89eda068b8a68943e1574cce8)
* [My wallet](https://etherscan.io/address/0xB6Bf1Eec596602D14acb288262C7B9b6D1B801eA)
* [My test wallet](https://sepolia.etherscan.io/address/0xb6bf1eec596602d14acb288262c7b9b6d1b801ea)

## TODO
- [x] read the [ethereum whitepaper](https://ethereum.org/en/whitepaper/)
- [x] read about [hardhat](https://hardhat.org/hardhat-runner/docs/getting-started#overview)
- [x] [hardhat ignition](https://hardhat.org/ignition/docs/getting-started#overview)
- [x] [hardhat verifying deploys](https://hardhat.org/hardhat-runner/docs/guides/verifying)
- [x] Try deploy Lock.sol to testnet
- [x] Read up on [how to interact with smart contracts](https://www.quicknode.com/guides/ethereum-development/smart-contracts/how-to-interact-with-smart-contracts#interacting-with-smart-contracts)
- [x] Try calling Lock.sol on testnet?
- [x] Work on SubredditBattleRoyalesol
- [x] Create tests for SubredditBattleRoyale.sol
- [x] Figure out how to handle casing (subreddits are case insensitive right?)
- [x] Figure out max subreddit length
- [x] Test that _startNewSeason increases linearly over time, and that successive wins can occur
- [x] Test that successive withdrawals can occur
- [x] Add remaining folders/files from hardhat boilerplate
- [x] Find rivaling subreddits, init contract with unequal amounts of coins to incentivize usage (add test for this) (heavy on crypto/tech sites).
- [x] Allow the owner to adjust the burn multiplier as part of seasonal settings.
- [x] Deploy to testnet
- [x] Test that eth/gwei are sent to owner wallet, and that I have control over it (configure wallet during build/deploy to go to mine)
- [x] **(MAYBE)** optimize _toLowerCase, or require in validation somehow?
- [x] Deploy Lock.sol to mainnet
- [x] Lower optimization all the way down for cheaper deployment
- [x] Deploy to live
- [ ] Read up on the [web3.js](https://web3js.readthedocs.io/en/v1.10.0/) for interacting with the Ethereum Blockchain (instead of [ethers library](https://docs.ethers.org/v6/))
- [ ] Create frontend that shows subreddit standings (Take inspiration from the frontend in the [hardhat boilerplate project](https://hardhat.org/tutorial/boilerplate-project))
- [ ] Figure out how to call smart contract seamlessly (purchasing, burning, withdrawing)
- [ ] Ads/marketing

## Installation and preparation
### Ethereum
* VSCODE (Hardhat extension for VSCODE) (development)
* npm (and npx)
* [etherscan.io](https://etherscan.io/) account
* [metamask](https://developer.metamask.io/) account (infura sepolia)
* metamask wallet & edge/chrome/firefox extension
* Alchemy faucet (used instead of Infura)

### Solana
* VSCODE (and extensions) (development)
* RUST (backend)
* SOLANA CLI (backend)
* ANCHOR (backend framework for accelerating development (hide away complexity))
* solana-program-test (backend test)
* typescript+@solana/web3.js (frontend to display battle stats)
