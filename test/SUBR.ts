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

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            let purchaseTokenResponse = await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });
            
            expect(purchaseTokenResponse).to.changeEtherBalance(subr, totalCost);

            expect(await subr.subredditTokenBalances(DOTA_2_SUBREDDIT)).to.equal(purchaseAmount);
        });

        it("Should revert if not enough ether is sent", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const insufficientCost = (await subr.TOKEN_PRICE() * purchaseAmount) - 1n;

            let purchaseTokenResponse = subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: insufficientCost });

            expect(purchaseTokenResponse).to.be.revertedWith("Incorrect Ether sent");
        });

        it("Should set subreddit to lower case", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT.toUpperCase(), purchaseAmount, { value: totalCost });

            expect(await subr.subredditTokenBalances(DOTA_2_SUBREDDIT)).to.equal(purchaseAmount);
        });

        it("Should revert if subreddit name is too long", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const TOO_LONG_SUBREDDIT_NAME = "/r/" + "a".repeat(21 + 1); // "/r/" + 21 characters + 1 character is 1 too many.

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            let purchaseTokenResponse = subr
                .connect(otherAccount)
                .purchaseTokens(TOO_LONG_SUBREDDIT_NAME, purchaseAmount, { value: totalCost });

            expect(purchaseTokenResponse).to.be.revertedWith("Subreddit name too long");
        });
    });
    
    describe("Events", function () {
        it("Should emit an event on token purchase", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            expect(await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost }))
                .to.emit(subr, "TokensPurchased")
                .withArgs(otherAccount.address, DOTA_2_SUBREDDIT, purchaseAmount);
        });

        it("Should emit an event on token burn", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount * 3n, { value: totalCost * 3n });

            expect(await subr.connect(otherAccount).burnTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost}))
                .to.emit(subr, "TokensBurned")
                .withArgs(otherAccount.address, DOTA_2_SUBREDDIT, purchaseAmount);
        });

        it("Should emit an event on season winner", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            let currentSeason = await subr.currentSeason();

            const purchaseAmount = (await subr.INITIAL_SUPPLY() / 2n) + 1n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            expect(await subr.connect(otherAccount).purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost }))
                .to.emit(subr, "SeasonWon")
                .withArgs(DOTA_2_SUBREDDIT, await subr.subredditTokenBalances(DOTA_2_SUBREDDIT), currentSeason);
        });

        it("Should increase the token supply linearly over time", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            let purchaseAmount = 500_001n;
            let totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            // Win season 1, and start season 2.
            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });
            
            let voidTokenCount =
                await subr.INITIAL_SUPPLY()
                + await subr.INITIAL_SUPPLY() + (await subr.INITIAL_SUPPLY() / await subr.currentSeason())
                - await subr.subredditTokenBalances(DOTA_2_SUBREDDIT);

            expect(await subr.voidTokenCount()).to.equal(voidTokenCount);

            purchaseAmount = 1_000_000n;
            totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            // Win season 2, and start season 3.
            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });

            voidTokenCount =
                await subr.INITIAL_SUPPLY()
                + (await subr.INITIAL_SUPPLY() + (await subr.INITIAL_SUPPLY() / (await subr.currentSeason() - 1n)))
                + (await subr.INITIAL_SUPPLY() + (await subr.INITIAL_SUPPLY() / await subr.currentSeason()))
                - await subr.subredditTokenBalances(DOTA_2_SUBREDDIT);

            expect(await subr.voidTokenCount()).to.equal(voidTokenCount);
        });
    });
    
    describe("Withdraw", function () {
        it("Should withdraw funds", async function () {
            const { subr, owner, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });

            let withdrawResponse = await subr.connect(owner).withdraw();

            expect(withdrawResponse).to.changeEtherBalance(owner, totalCost);
        });

        it("Should revert if not owner", async function () {
            const { subr, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            let withdrawResponse = subr.connect(otherAccount).withdraw();

            expect(withdrawResponse).to.be.revertedWith("Only the owner can call this function");
        });

        it("Should revert if no funds", async function () {
            const { subr, owner } = await loadFixture(deploySubredditBattleRoyaleFixture);

            let withdrawResponse = subr.connect(owner).withdraw();

            expect(withdrawResponse).to.be.revertedWith("No Ether available to withdraw");
        });

        it("Should be able to withdraw multiple times", async function () {
            const { subr, owner, otherAccount } = await loadFixture(deploySubredditBattleRoyaleFixture);

            const purchaseAmount = 10n;
            const totalCost = await subr.TOKEN_PRICE() * purchaseAmount;

            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });

            let withdrawResponse = await subr.connect(owner).withdraw();

            expect(withdrawResponse).to.changeEtherBalance(owner, totalCost);

            await subr
                .connect(otherAccount)
                .purchaseTokens(DOTA_2_SUBREDDIT, purchaseAmount, { value: totalCost });

            let withdrawResponse2 = await subr.connect(owner).withdraw();

            expect(withdrawResponse2).to.changeEtherBalance(owner, totalCost);
        });
    });
});