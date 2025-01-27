import './App.css';

import Wallet from './components/Wallet';

import { useState, useEffect } from 'react';

import axios from 'axios';

function App() {
  const [backendData, setBackendData] = useState(null);

  useEffect(() => {
    axios.get(process.env.REACT_APP_BACKEND_URI)
      .then(response => setBackendData(response.data))
      .catch(error => console.error(error));
  }, []);

  return (
    <div className="App">
      <h1>Subreddit Battle Royale</h1>

      <Wallet />

      <h3>Void Token Count</h3>
      <p>{backendData?.voidTokenCount == null ? "" : JSON.stringify(backendData.voidTokenCount)}</p>

      <h3>Tokens Purchased</h3>
      <ul>{backendData?.tokensPurchased.map((tokensPurchased, index) => <li key={index}>{tokensPurchased.id}</li>)}</ul>

      <h3>Tokens Burned</h3>
      <ul>{backendData?.tokensBurned.map(token => <li>{JSON.stringify(token)}</li>)}</ul>

      <h3>Season Won</h3>
      <ul>{backendData?.seasonWon.map(season => <li>{JSON.stringify(season)}</li>)}</ul>

    </div>
  );
}

export default App;
