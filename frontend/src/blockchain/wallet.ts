import * as ethers from 'ethers';
import { abi } from './abi.ts';

const callPurchaseTokens = async (signerValue: ethers.JsonRpcSigner | undefined) => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signerValue)

    const numTokensToPurchase = 1;
    const TOKEN_PRICE = 0.0001;

    const ether = numTokensToPurchase * TOKEN_PRICE
    const gasEstimate = await contract.purchaseTokens.estimateGas("/r/dota2", numTokensToPurchase, {
        value: ethers.parseEther(ether.toString())
    });

    const txn = await contract.purchaseTokens("/r/dota2", numTokensToPurchase, {
        value: ethers.parseEther(ether.toString()),
        gasLimit: gasEstimate
    })

    console.log("Transaction hash: ", txn.hash)

    // todo: display result somehow
}

const callBurnTokens = async (signerValue: ethers.JsonRpcSigner | undefined) => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signerValue)

    const numTokensToBurn = 1;
    const TOKEN_PRICE = 0.0001;

    const ether = numTokensToBurn * TOKEN_PRICE
    const gasEstimate = await contract.burnTokens.estimateGas("/r/dota2", numTokensToBurn, {
        value: ethers.parseEther(ether.toString())
    });

    const txn = await contract.burnTokens("/r/dota2", numTokensToBurn, {
        value: ethers.parseEther(ether.toString()),
        gasLimit: gasEstimate
    })

    console.log("Transaction hash: ", txn.hash)

    // todo: display result somehow
}

export { callPurchaseTokens, callBurnTokens };