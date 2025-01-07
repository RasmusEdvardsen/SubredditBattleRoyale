# SUBR

SUBR (**SUBR**eddit **B**attle**R**oyale) is a battle royale token for subreddits to flex on each other, or for people to secure the void will win against subreddits. When a winner is found, a new and harder season is started.

## Game summary
* Some set amount of tokens (think this through. Since seasons, maybe deflationary?)
* initial all season tokens under "the void".
* all transactions needs "/r/SOME_SUBREDDIT".
* if not "/r/SOME_SUBREDDIT", then still "the void", so people can support the void kinda.
* goal is for your "/r/SOME_SUBREDDIT" to have more than the void.
* if some time expires before anyone gets more, the void wins.
* when a winner is found (the void or a subreddit), a new "season" (game) starts up.
* tokens are versioned for each season.
* somehow make it harder for people to buy tokens (sabotage others)? (maybe too hard for now to implement)
* if/when a subreddit wins, more expensive and less tokens.
* maybe "kill" subreddits with low amount of tokens?
* tokens that count negatively towards rivaling subreddit?
* battle stats will be displayed on a website.
* 'the void' could also just get stronger after every win, then old coins would still matter.

## Progress
[Deployed Lock contract for testing](https://sepolia.etherscan.io/address/0xB86DC26c7fe525Fe7938c4Ea5C8121d9192ec618#code)
[My wallet](https://etherscan.io/address/0xB6Bf1Eec596602D14acb288262C7B9b6D1B801eA)

## TODO
* read the [ethereum whitepaper](https://ethereum.org/en/whitepaper/)
* read about [hardhat](https://hardhat.org/hardhat-runner/docs/getting-started#overview)
* [hardhat ignition](https://hardhat.org/ignition/docs/getting-started#overview)
* [hardhat verifying deploys](https://hardhat.org/hardhat-runner/docs/guides/verifying)
1. Try deploy Lock.sol to testnet
2. Work on SUBR.sol
3. Create tests for SUBR.sol
4. Deploy to testnet
5. Deploy to live
6. At some point, create frontend that shows subreddit standings
7. Figure out some rivaling subreddits, and initialize contract with unequal amounts of coins for them to incentivize usage.

## Installation and preparation
### Ethereum
* VSCODE (Hardhat extension for VSCODE) (development)
* npm (and npx)
* [etherscan.io](https://etherscan.io/) account
* [metamask](https://developer.metamask.io/) account (infura sepolia)
* metamask wallet & edge/chrome/firefox extension

### Solana
* VSCODE (and extensions) (development)
* RUST (backend)
* SOLANA CLI (backend)
* ANCHOR (backend framework for accelerating development (hide away complexity))
* solana-program-test (backend test)
* typescript+@solana/web3.js (frontend to display battle stats)
