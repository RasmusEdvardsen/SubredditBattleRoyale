// consider uninstall web3, install web3-eth
import { Web3 } from "web3";
import * as fs from 'fs';

const INFURA_API_KEY = "INFURA_API_KEY";
const CONTRACT_ADDRESS = "0xea8831bcb719914ab97131f48d9b2dc737dbd25a";

const jsonContractFilePath = "..\\ignition\\deployments\\mainnet-deployment\\artifacts\\SubredditBattleRoyaleModule#SubredditBattleRoyale.json";

// todo: stuff I need:
// Events:
// 1. Tokens purchased
// 2. Tokens burned
// 3. Season won
// State:
// 1. Current season (Update if changed)
// 2. voidTokens (Update if changed)
// 3. BURN_MULTIPLIER (Update if changed)
// 4. TOKEN_PRICE
async function main() {
    const provider = new Web3.providers.HttpProvider(`https://mainnet.infura.io/v3/${INFURA_API_KEY}`);
    const web3 = new Web3(provider);

    const jsonContractFile = await fs.promises.readFile(jsonContractFilePath, 'utf8');
    const jsonContract = JSON.parse(jsonContractFile);

    const contract = new web3.eth.Contract(jsonContract.abi, CONTRACT_ADDRESS);

    // todo: either I keep state up until block N off-chain and fetch events from block N+1, or I just fetch all events.
    const event = await contract.events.TokensPurchased({fromBlock: 0});

    event.on('data', (data) => {
        console.log(`\nTokens purchased!\nBuyer: ${data.returnValues.buyer}\nSubreddit: ${data.returnValues.subreddit}\nAmount: ${data.returnValues.amount}`);
        console.log(`Unique id: block# ${data.blockHash}, txn# ${data.transactionHash}, logIndex ${data.logIndex}`)
    });
    event.on('error', (err: Error) => {
        console.log("\nError occurred: ", err);
    });
}

main()
    .then()
    .catch(console.error);
