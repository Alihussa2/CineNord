using Npgsql;

// ETL: Extract fra 3 shards -> Transform -> Load til warehouse (upsert, ingen dubletter)
string[] shards =
[
    "Host=localhost;Port=5440;Username=cinenord;Password=cinenord;Database=shard0",
    "Host=localhost;Port=5441;Username=cinenord;Password=cinenord;Database=shard1",
    "Host=localhost;Port=5442;Username=cinenord;Password=cinenord;Database=shard2"
];
const string warehouse = "Host=localhost;Port=5432;Username=cinenord;Password=cinenord;Database=warehouse";

foreach (var (cs, i) in shards.Select((cs, i) => (cs, i)))
{
    await using var conn = new NpgsqlConnection(cs);
    await conn.OpenAsync();
    Console.WriteLine($"Shard {i}: forbundet");
    // TODO Extract: SELECT * FROM customers / reviews
}

await using (var wh = new NpgsqlConnection(warehouse))
{
    await wh.OpenAsync();
    Console.WriteLine("Warehouse: forbundet");
    // TODO Transform: tilfoej source_shard, brug (source_shard, source_review_id) som noegle
    // TODO Load: INSERT ... ON CONFLICT (...) DO UPDATE
}

Console.WriteLine("ETL faerdig");
