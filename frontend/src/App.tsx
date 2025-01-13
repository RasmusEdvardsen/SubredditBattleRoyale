import { useState } from 'react';
import { startListenTokensPurchased } from './contract';
import './App.css';

function App() {
  const [tokenBalances, setTokenBalances] = useState<{ [subreddit: string]: number }>({});
  startListenTokensPurchased(
    (data) => {
      setTokenBalances((prevBalances) => ({
        ...prevBalances,
        [data.returnValues.subreddit as string]: (prevBalances[data.returnValues.subreddit as string] || 0) + Number(data.returnValues.amount),
      }));
      console.log(`\nTokens purchased!\nBuyer: ${data.returnValues.buyer}\nSubreddit: ${data.returnValues.subreddit}\nAmount: ${data.returnValues.amount}`);
      console.log(`Unique id: block# ${data.blockHash}, txn# ${data.transactionHash}, logIndex ${data.logIndex}`)
    },
    (err: Error) => {
      console.log("\nError occurred: ", err);
    }
  );

  return (
    <body>
      <p>
        Subreddit Battle Royale
      </p>
      <div>
        <h2>Token Balances</h2>
        <ul>
          {Object.entries(tokenBalances).map(([subreddit, balance]) => (
            <li key={subreddit}>
              <p>{subreddit}: {Number(balance)}</p>
            </li>
          ))}
        </ul>
      </div>
    </body>
  );
}

export default App;
