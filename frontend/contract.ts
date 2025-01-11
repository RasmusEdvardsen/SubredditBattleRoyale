// consider uninstall web3, install web3-eth
import { Web3 } from "web3";
import * as fs from 'fs';

const INFURA_API_KEY = "INFURA_API_KEY";
const CONTRACT_ADDRESS = "0xea8831bcb719914ab97131f48d9b2dc737dbd25a";

const jsonContractFilePath = "..\\ignition\\deployments\\mainnet-deployment\\artifacts\\SubredditBattleRoyaleModule#SubredditBattleRoyale.json";

async function main() {
    const provider = new Web3.providers.HttpProvider(`https://mainnet.infura.io/v3/${INFURA_API_KEY}`);
    const web3 = new Web3(provider);

    const jsonContractFile = await fs.promises.readFile(jsonContractFilePath, 'utf8');
    const jsonContract = JSON.parse(jsonContractFile);

    const contract = new web3.eth.Contract(jsonContract.abi, CONTRACT_ADDRESS);

    let currentSeason = await contract.methods.currentSeason().call();
    let initialSupply = await contract.methods.INITIAL_SUPPLY().call();
    let voidTokenCount = await contract.methods.voidTokenCount().call();

    console.log("Current season: ", currentSeason);
    console.log("Initial supply: ", initialSupply);
    console.log("Void token count: ", voidTokenCount);

    const event = await contract.events.TokensPurchased({fromBlock: 0});

    event.on('data', (data) => {
        console.log(`\nTokens purchased!\nBuyer: ${data.returnValues.buyer}\nSubreddit: ${data.returnValues.subreddit}\nAmount: ${data.returnValues.amount}`);
    });
    event.on('error', (err: Error) => {
        console.log("\nError occurred: ", err);
    });
}

main()
    .then()
    .catch(console.error);
