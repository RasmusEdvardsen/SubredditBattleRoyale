import { loadFixture } from "@nomicfoundation/hardhat-toolbox/network-helpers";
import { expect } from "chai";
import hre from "hardhat";

describe("SubredditBattleRoyale", function () {
    const DOTA_2_SUBREDDIT = "/r/dota2";

    async function deploySubredditBattleRoyaleFixture() {
        const [owner, otherAccount] = await hre.ethers.getSigners();

        const SUBR = await hre.ethers.getContractFactory("SubredditBattleRoyale");
        const subr = await SUBR.deploy();

        return { subr, owner, otherAccount };
    }

    describe("Deployment", function () {
        it("Should have minted the correct amount of tokens", async function () {
            const { subr } = await loadFixture(deploySubredditBattleRoyaleFixture);

            expect(await subr.voidTokenCount()).to.equal(await subr.INITIAL_SUPPLY());
        });

        it("Should have set the correct token price", async function () {
            const { subr } = await loadFixture(deploySubredditBattleRoyaleFixture);

            expect(await subr.TOKEN_PRICE()).to.equal(1000000000000000);
        });

        it("Should set the right owner", async function () {
            const { subr, owner } = await loadFixture(deploySubredditBattleRoyaleFixture);

            expect(await subr.owner()).to.equal(owner.address);
        });

        it("Should have no initial funds", async function () {
            const { subr } = await loadFixture(deploySubredditBattleRoyaleFixture);

            expect(await hre.ethers.provider.getBalance(subr.target)).to.equal(0);
        });
    });

    describe("Purchase tokens", function () {
        it("Should purchase tokens, and send price to contract", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt(10);
            const totalCost = tokenPrice * purchaseAmount;

            let purchaseTokenResponse = await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });
            
            expect(purchaseTokenResponse).to.changeEtherBalance(subr, totalCost);

            expect(await subr.subredditTokenBalances(DOTA_2_SUBREDDIT)).to.equal(purchaseAmount);
        });

        it("Should revert if not enough ether is sent", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt(10);
            const insufficientCost = (tokenPrice * purchaseAmount) - BigInt(1);

            let purchaseTokenResponse = subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: insufficientCost });

            expect(purchaseTokenResponse).to.be.revertedWith("Incorrect Ether sent");
        });
    });
    
    describe("Events", function () {
        it("Should emit an event on token purchase", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt(10);
            const totalCost = tokenPrice * purchaseAmount;

            expect(await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost }))
                .to.emit(subr, "TokensPurchased")
                .withArgs(otherAccount.address, DOTA_2_SUBREDDIT, purchaseAmount);
        });

        it("Should emit an event on token burn", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt(10);
            const totalCost = tokenPrice * purchaseAmount;

            await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount * BigInt(3), { value: totalCost * BigInt(3) });

            expect(await subr.connect(otherAccount).burnTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost}))
                .to.emit(subr, "TokensBurned")
                .withArgs(otherAccount.address, DOTA_2_SUBREDDIT, purchaseAmount);
        });

        it("Should emit an event on season winner", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            let currentSeason = await subr.currentSeason();

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt((await subr.INITIAL_SUPPLY() / BigInt(2)) + BigInt(1));
            const totalCost = tokenPrice * purchaseAmount;

            expect(await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost }))
                .to.emit(subr, "SeasonWon")
                .withArgs(DOTA_2_SUBREDDIT, await subr.subredditTokenBalances(DOTA_2_SUBREDDIT), currentSeason);
        });
    });
    
    describe("Withdraw", function () {
        it("Should withdraw funds", async function () {
            const { subr, owner, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const tokenPrice = BigInt(await subr.TOKEN_PRICE());
            const purchaseAmount = BigInt(10);
            const totalCost = tokenPrice * purchaseAmount;

            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });

            let withdrawResponse = await subr.connect(owner).withdraw();

            expect(withdrawResponse).to.changeEtherBalance(owner, totalCost);
        });
    });
});
