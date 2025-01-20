import * as ethers from 'ethers';
import { useState } from 'react';
import { abi } from '../abi';

function Wallet() {
    const [signer, setSigner] = useState(null);
    const [provider, setProvider] = useState(null);

    const handleLogin = async () => {
        if (window.ethereum) {
            try {
                const browserProvider = new ethers.BrowserProvider(window.ethereum);
                setProvider(browserProvider)
                setSigner(await browserProvider.getSigner());
            } catch (error) {
                console.error(error);
            }
        } else {
            console.error("No Ethereum provider found. Install MetaMask.");
        }
    };

    const printToConsole = async () => {
        const contract = new ethers.Contract("0xea8831bcb719914ab97131f48d9b2dc737dbd25a", abi.abi, signer)

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

    if (!signer) {
        return (
            <div className="Wallet">
                <button onClick={handleLogin}>Connect to MetaMask</button>
            </div>
        );
    }

    return (
        <div className="Wallet">
            {signer ? "Connected to MetaMask! " : "Connect to MetaMask!"}
            <button onClick={printToConsole}>Print to console</button>
        </div>
    );
}

export default Wallet;
