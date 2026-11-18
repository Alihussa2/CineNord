using System.Security.Cryptography;

namespace CineNord.GraphQL.Sharding;

// Finder den shard en kunde hoerer til: hash(CustomerId) mod 3.
// Brug ALDRIG GetHashCode() her - den giver et nyt resultat hver gang programmet starter.
public class ShardRouter
{
    private readonly string[] _shards;

    public ShardRouter(IConfiguration config)
    {
        _shards =
        [
            config.GetConnectionString("Shard0")!,
            config.GetConnectionString("Shard1")!,
            config.GetConnectionString("Shard2")!
        ];
    }

    public int ShardCount => _shards.Length;

    public IReadOnlyList<string> AllShards => _shards;

    public int GetShardIndex(Guid customerId)
    {
        var hash = SHA256.HashData(customerId.ToByteArray());
        var value = BitConverter.ToUInt32(hash, 0);
        return (int)(value % (uint)_shards.Length);
    }

    public string GetConnectionString(Guid customerId) => _shards[GetShardIndex(customerId)];
}
