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
      <p>App! {backendData}</p>
      <Wallet />
    </div>
  );
}

export default App;
