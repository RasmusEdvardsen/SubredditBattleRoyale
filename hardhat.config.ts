import { HardhatUserConfig, vars } from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";

// npx hardhat vars set INFURA_API_KEY <INFURA_API_KEY>
const INFURA_API_KEY = vars.get("INFURA_API_KEY");

// npx hardhat vars set SEPOLIA_PRIVATE_KEY <SEPOLIA_PRIVATE_KEY>
const SEPOLIA_PRIVATE_KEY = vars.get("SEPOLIA_PRIVATE_KEY");

// npx hardhat vars set ETHERSCAN_API_KEY <ETHERSCAN_API_KEY>
const ETHERSCAN_API_KEY = vars.get("ETHERSCAN_API_KEY");

// npx hardhat vars set COINMARKETCAP_API_KEY <COINMARKETCAP_API_KEY>
const COINMARKETCAP_API_KEY = vars.get("COINMARKETCAP_API_KEY");

const config: HardhatUserConfig = {
  solidity: {
    version: "0.8.28",
    settings: {
      optimizer: {
        enabled: true,
        runs: 200,
      },
    },
  },
  networks: {
    sepolia: {
      url: `https://sepolia.infura.io/v3/${INFURA_API_KEY}`,
      accounts: [SEPOLIA_PRIVATE_KEY],
    },
    mainnet: {
      url: `https://mainnet.infura.io/v3/${INFURA_API_KEY}`,
      accounts: [SEPOLIA_PRIVATE_KEY],
    },
  },
  etherscan: {
    apiKey: {
      sepolia: ETHERSCAN_API_KEY,
    },
  },
  gasReporter: {
    coinmarketcap: COINMARKETCAP_API_KEY,
    L1Etherscan: ETHERSCAN_API_KEY
  },
};

export default config;
