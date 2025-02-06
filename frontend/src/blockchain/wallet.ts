import * as ethers from 'ethers';
import { abi } from './abi.ts';

const callPurchaseTokens = async (signerValue: ethers.JsonRpcSigner | undefined, subreddit: string, amount: number) => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signerValue)

    const TOKEN_PRICE = 0.0001;

    const ether = (amount * TOKEN_PRICE).toFixed(4);

    const gasEstimate = await contract.purchaseTokens.estimateGas(subreddit, amount, {
        value: ethers.parseEther(ether.toString())
    });

    const txn = await contract.purchaseTokens(subreddit, amount, {
        value: ethers.parseEther(ether.toString()),
        gasLimit: gasEstimate
    })

    console.log("Transaction hash: ", txn.hash)

    // todo: display result somehow
}

const callBurnTokens = async (signerValue: ethers.JsonRpcSigner | undefined, subreddit: string, amount: number) => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signerValue)

    const TOKEN_PRICE = 0.0001;

    const ether = (amount * TOKEN_PRICE).toFixed(4);

    const gasEstimate = await contract.burnTokens.estimateGas(subreddit, amount, {
        value: ethers.parseEther(ether.toString())
    });

    const txn = await contract.burnTokens(subreddit, amount, {
        value: ethers.parseEther(ether.toString()),
        gasLimit: gasEstimate
    })

    console.log("Transaction hash: ", txn.hash)

    // todo: display result somehow
}

export { callPurchaseTokens, callBurnTokens };