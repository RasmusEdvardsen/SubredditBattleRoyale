<script setup lang="ts">
import * as ethers from 'ethers';
import { abi } from '../abi';
import { ref } from 'vue';

declare global {
    interface Window {
        ethereum: any;
    }
}

const signer = ref<ethers.JsonRpcSigner | undefined>();
const provider = ref<ethers.BrowserProvider | undefined>();
const ethereum = window.ethereum;

const handleLogin = async () => {
    if (window.ethereum) {
        try {
            const browserProvider = new ethers.BrowserProvider(window.ethereum);
            provider.value = browserProvider;
            signer.value = await browserProvider.getSigner();
        } catch (error) {
            console.error(error);
        }
    } else {
        console.error("No Ethereum provider found. Install an ethereum-compatible wallet like MetaMask.");
    }
};

// todo: allow user to specify subreddit and amount
const purchaseTokens = async () => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signer.value)

    const numTokensToPurchase = 1;
    const TOKEN_PRICE = 0.0001;

    const ether = numTokensToPurchase * TOKEN_PRICE
    // todo: fix not allowed to use private function on contract object
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

// todo: allow user to specify subreddit and amount
const burnTokens = async () => {
    const contract = new ethers.Contract(import.meta.env.VITE_CONTRACT_ADDRESS, abi.abi, signer.value)

    const numTokensToBurn = 1;
    const TOKEN_PRICE = 0.0001;

    const ether = numTokensToBurn * TOKEN_PRICE
    // todo: fix not allowed to use private function on contract object
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

</script>

<template>
    <div v-if="!ethereum" className="Wallet">
        No Ethereum provider found. Install an ethereum-compatible wallet like MetaMask.
    </div>
    <div v-else-if="!signer" className="Wallet">
        <button @click="handleLogin">Connect to wallet</button>
    </div>
    <div v-else className="Wallet">
        {{signer ? "Connected to your wallet on! " : "Something went wrong. Please refresh this page, and try again."}}
        <button @click="purchaseTokens">Purchase tokens</button>
        <button @click="burnTokens">Burn tokens</button>
    </div>
</template>
