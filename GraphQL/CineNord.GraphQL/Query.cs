using CineNord.GraphQL.Sharding;

namespace CineNord.GraphQL;

public class Query
{
    public string Hello() => "CineNord GraphQL koerer";

    // Viser hvilken shard en kunde ville havne i
    public int ShardFor(Guid customerId, ShardRouter router) => router.GetShardIndex(customerId);
}
