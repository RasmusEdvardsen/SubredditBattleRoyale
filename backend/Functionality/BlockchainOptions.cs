namespace Backend.Functionality;

public class BlockchainOptions
{
    public const string Blockchain = "Blockchain";

    /// <summary>
    /// Smart contract address on the ethereum blockchain.
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;

    /// <summary>
    /// Uri of the RPC ethereum blockchain peer to use to interact with the blockchain.
    /// </summary>
    public string RpcUri { get; set; } = string.Empty;
}