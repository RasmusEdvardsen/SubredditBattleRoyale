// uninstall web3, install web3-eth
var Eth = require('web3-eth');

// dont do this, really don't want to expose private keys
// Web3.eth.accounts.privateKeyToAccount("PRIVATE_KEY")

// contract.methods.currentSeason().call()

// dunno how to set this correctly
// Contract.setProvider("https://mainnet.infura.io/v3/2c3e5f5cd3fd4fdf8e15562f1ddb3e83")

// ignition\deployments\mainnet-deployment\artifacts\SubredditBattleRoyaleModule#SubredditBattleRoyale.json use "abi" property on object 

// hide this somehow
var INFURA_API_KEY = "INFURA_API_KEY";

console.log(Eth);
