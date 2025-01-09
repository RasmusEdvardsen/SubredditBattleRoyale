// This setup uses Hardhat Ignition to manage smart contract deployments.
// Learn more about it at https://hardhat.org/ignition

import { buildModule } from "@nomicfoundation/hardhat-ignition/modules";

const SubredditBattleRoyaleModule = buildModule("SubredditBattleRoyaleModule", (m) => {
  const subredditBattleRoyale = m.contract("SubredditBattleRoyale");

  return { subredditBattleRoyale };
});

export default SubredditBattleRoyaleModule;
