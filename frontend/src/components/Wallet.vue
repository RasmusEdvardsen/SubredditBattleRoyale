<script setup lang="ts">
import * as ethers from 'ethers';
import { markRaw, ref } from 'vue';
import { callPurchaseTokens, callBurnTokens } from '../blockchain/wallet.ts';

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
            provider.value = markRaw(browserProvider);
            signer.value = markRaw(await browserProvider.getSigner());
        } catch (error) {
            console.error(error);
        }
    } else {
        console.error("No Ethereum provider found. Install an ethereum-compatible wallet like MetaMask.");
    }
};

// todo: allow user to specify subreddit and amount
const purchaseTokens = async (): Promise<void> => await callPurchaseTokens(signer.value);

// todo: allow user to specify subreddit and amount
const burnTokens = async (): Promise<void> => await callBurnTokens(signer.value);

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
