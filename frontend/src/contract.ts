// todo: consider uninstall web3, install web3-eth
// todo: either keep state up until block N off-chain and fetch events from block N+1, or just fetch all events.
// todo: Events: Tokens purchased, Tokens burned, Season won
// todo: State (Update if changed by events somehow): Current season, voidTokens, BURN_MULTIPLIER, TOKEN_PRICE
import { Contract, Web3, EventLog } from "web3";
import ABI from './abi';

const INFURA_API_KEY = "2c3e5f5cd3fd4fdf8e15562f1ddb3e83";
const CONTRACT_ADDRESS = "0xea8831bcb719914ab97131f48d9b2dc737dbd25a";
const PROVIDER = new Web3.providers.HttpProvider(`https://mainnet.infura.io/v3/${INFURA_API_KEY}`);
const WEB3 = new Web3(PROVIDER);

let _CONTRACT: Contract<any>;
async function CONTRACT() {
    if (_CONTRACT == null) {
        _CONTRACT = await new WEB3.eth.Contract(ABI.abi, CONTRACT_ADDRESS);
    }

    return _CONTRACT;
}

async function startListenTokensPurchased(onData: (data: EventLog) => void, onError: (err: Error) => void) {
    const contract = await CONTRACT();

    const latestBlock = await WEB3.eth.getBlock('latest');

    const pastEventsUnfiltered: (string | EventLog)[] = await contract.getPastEvents('TokensPurchased', {
        fromBlock: 0,
        toBlock: latestBlock.number
    });
    
    function isEventLog(item: (string | EventLog)): item is EventLog {
        return typeof item !== 'string';
    }

    const pastEvents = pastEventsUnfiltered.filter(isEventLog);

    console.log(pastEvents);

    const tokensPurchasedEvent = contract.events.TokensPurchased({ fromBlock: 'latest' });

    tokensPurchasedEvent.on('data', onData);
    tokensPurchasedEvent.on('error', onError);
}

export { startListenTokensPurchased };