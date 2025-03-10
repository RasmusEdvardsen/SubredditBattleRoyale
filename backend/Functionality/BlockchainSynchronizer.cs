using System.Data;
using System.Numerics;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Backend.Functionality;

public interface IBlockchainSynchronizer
{
    Task SyncBlockchainIfNeeded();
}

public class BlockchainSynchronizer(IOptions<BlockchainOptions> blockchainOptions) : IBlockchainSynchronizer
{
    private readonly Web3 _web3 = new(blockchainOptions.Value.RpcUri);
    private readonly string _contractAddress = blockchainOptions.Value.ContractAddress;

    public async Task SyncBlockchainIfNeeded()
    {
        using var connection = new SqliteConnection("Data Source=hello.db");


        var blockchainSync = await connection.QueryAsync<BlockchainSync>("SELECT * FROM BlockchainSync");
        if (!blockchainSync.Any() || blockchainSync.Max(s => s.SyncedAt) < DateTime.UtcNow - TimeSpan.FromSeconds(15))
        {
            await Task.WhenAll([
                SyncEvents<TokensPurchasedEvent, TokensPurchased>("TokensPurchased", TokensPurchased.FromBlockChain),
                SyncEvents<TokensBurnedEvent, TokensBurned>("TokensBurned", TokensBurned.FromBlockChain),
                SyncEvents<SeasonWonEvent, SeasonWon>("SeasonWon", SeasonWon.FromBlockChain)
            ]);

            var voidTokenCount = await _web3.Eth
                .GetContractQueryHandler<VoidTokenCountMessage>()
                .QueryAsync<BigInteger>(_contractAddress, new VoidTokenCountMessage());

            // todo: wrap in waitall
            await connection.ExecuteAsync("UPDATE VoidTokenCount SET Balance = @Balance",
                new { Balance = (ulong)voidTokenCount }
            );
            
            await connection.ExecuteAsync("INSERT INTO BlockchainSync (SyncedAt) VALUES (DATETIME('now'))");
        }
    }

    private async Task SyncEvents<TEventDTO, TEvent>(string tableName, Func<EventLog<TEventDTO>, TEvent> objectMapper) where TEventDTO : IEventDTO, new()
    {
        using var connection = new SqliteConnection("Data Source=hello.db");

        var existingEntries = await connection.QueryAsync<ulong>($"SELECT COUNT(BlockNumber) FROM {tableName}");

        var fromBlock = existingEntries.Any() ? existingEntries.Max() + 1 : 0;

        var eventsBlockChain = await GetEventLogs<TEventDTO>(fromBlock);

        var events = eventsBlockChain.Select(objectMapper);

        await connection.BulkInsertAsync(events);
    }

    private async Task<IEnumerable<EventLog<TEventDTO>>> GetEventLogs<TEventDTO>(ulong fromBlock) where TEventDTO : IEventDTO, new()
    {
        var eventHandler = _web3.Eth.GetEvent<TEventDTO>(_contractAddress);

        // todo: does this actually work?
        var filter = eventHandler.CreateFilterInput();
        filter.FromBlock = new BlockParameter(fromBlock);

        return await eventHandler.GetAllChangesAsync(filter);
    }
}
