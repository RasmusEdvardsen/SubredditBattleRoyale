<script setup lang="ts">
import * as ethers from 'ethers';
import { computed, markRaw, ref } from 'vue';
import { callPurchaseTokens, callBurnTokens } from '@/blockchain/wallet.ts';

declare global {
    interface Window {
        ethereum: any;
    }
}

const signer = ref<ethers.JsonRpcSigner | undefined>();
const provider = ref<ethers.BrowserProvider | undefined>();
const ethereum = window.ethereum;

const subreddit = ref<string>('');
const subredditError = computed<string>(() =>
    !subreddit.value.startsWith('/r/') || subreddit.value.length < 4
        ? "Subreddit must start with /r/', and have at least one character after that." : ""
);

const amount = ref<number>(1);
const amountError = computed<string>(() => amount.value < 1 ? "Amount must be greater than 0." : "");

const hasValidationErrors = computed<boolean>(() => subredditError.value.length != 0 || amountError.value.length != 0);

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

const purchaseTokens = async (subreddit: string, amount: number): Promise<void> => await callPurchaseTokens(signer.value, subreddit, amount);
const burnTokens = async (subreddit: string, amount: number): Promise<void> => await callBurnTokens(signer.value, subreddit, amount);

</script>

<template>
    <div v-if="!ethereum" className="Wallet">
        No Ethereum provider found. Install an ethereum-compatible wallet like MetaMask.
    </div>
    <div v-else-if="!signer" className="Wallet">
        <button @click="handleLogin">Connect to wallet</button>
    </div>
    <div v-else className="Wallet">
        {{ signer ? "Connected to your wallet!" : "Something went wrong. Please refresh this page, and try again." }}
        <div>
            <label for="subreddit">Subreddit:</label>
            <input id="subreddit" v-model="subreddit" type="text" placeholder="Enter subreddit (e.g., /r/example)" />
            <span v-if="subreddit.length > 0 && subredditError" style="color: red;">{{ subredditError }}</span>
        </div>
        <div>
            <label for="amount">Amount:</label>
            <input id="amount" v-model="amount" type="number" min="1" placeholder="Enter amount" />
            <span v-if="amountError" style="color: red;">{{ amountError }}</span>
        </div>
        <button @click="purchaseTokens(subreddit, amount)" :disabled="hasValidationErrors">Purchase tokens</button>
        <button @click="burnTokens(subreddit, amount)" :disabled="hasValidationErrors">Burn tokens</button>
    </div>
</template>

<style scoped>
/* Dark theme styles */
.Wallet {
    background-color: #181818;
    padding: 20px;
    border-radius: 8px;
    width: 100%;
    max-width: 400px;
    margin: auto;
    text-align: center;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.3);
}

label {
    display: block;
    margin-top: 10px;
    font-size: 14px;
    color: #bbb;
    text-align: left;
}

input {
    width: 100%;
    max-width: 380px;
    padding: 10px;
    margin-top: 5px;
    border: 1px solid #333;
    border-radius: 5px;
    background-color: #252525;
    color: white;
    font-size: 16px;
}

input:focus {
    outline: none;
    border-color: #666;
}

button {
    width: 100%;
    padding: 10px;
    margin-top: 15px;
    border: none;
    border-radius: 5px;
    background-color: #6200ea;
    color: white;
    font-size: 16px;
    cursor: pointer;
    transition: background 0.3s;
}

button:disabled {
    background-color: #444;
    cursor: not-allowed;
}

button:hover:not(:disabled) {
    background-color: #7b1fa2;
}

span {
    display: block;
    margin-top: 5px;
    font-size: 12px;
    color: red;
}
</style>