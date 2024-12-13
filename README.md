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

## TODO
* Consider ethereum over solana
* Try these:
* [hardhat](https://hardhat.org/hardhat-runner/docs/getting-started#overview)
* [hardhat ignition](https://hardhat.org/ignition/docs/getting-started#overview)
* [hardhat verifying deploys](https://hardhat.org/hardhat-runner/docs/guides/verifying)

### Installation and preparation
* VSCODE (and extensions) (development)
* RUST (backend)
* SOLANA CLI (backend)
* ANCHOR (backend framework for accelerating development (hide away complexity))
* solana-program-test (backend test)
* typescript+@solana/web3.js (frontend to display battle stats)
