namespace ClusterInventory.Repositories
{
    using Models;
    using Settings;
    using MongoDB.Bson;
    using MongoDB.Driver;
    public class MovementRepository : IMovementRepository
    {
        private readonly IMongoCollection<Item> _items;
        private readonly IMongoCollection<Movement> _movs;

        public MovementRepository(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _items = db.GetCollection<Item>(settings.ItemsCollection);
            _movs = db.GetCollection<Movement>(settings.MovementsCollection);

            var idx = Builders<Movement>.IndexKeys
                .Ascending(x => x.ItemId)
                .Descending(x => x.CreatedAt);
            _movs.Indexes.CreateOne(new CreateIndexModel<Movement>(idx));
        }

        public async Task<(bool Ok, string? Error, Movement? Saved)> CreateAsync(Movement m)
        {
            if (!ObjectId.TryParse(m.ItemId, out _))
                return (false, "Invalid itemId", null);
            if (m.Quantity <= 0)
                return (false, "Quantity must be > 0", null);
            if (m.Type != "in" && m.Type != "out")
                return (false, "Type must be 'in' or 'out'", null);

            // Filtro base: por Id del item
            var filter = Builders<Item>.Filter.Eq(x => x.Id, m.ItemId);

            // Inicializa update con valor neutro para evitar CS0165
            UpdateDefinition<Item> update = Builders<Item>.Update.Inc(x => x.Stock, 0);

            if (m.Type == "in")
            {
                // Entrada: suma stock
                update = Builders<Item>.Update.Inc(x => x.Stock, m.Quantity);
            }
            else // "out"
            {
                // Salida: exige stock suficiente y resta
                filter = Builders<Item>.Filter.And(
                    filter,
                    Builders<Item>.Filter.Gte(x => x.Stock, m.Quantity)
                );
                update = Builders<Item>.Update.Inc(x => x.Stock, -m.Quantity);
            }

            var updated = await _items.UpdateOneAsync(filter, update);
            if (updated.ModifiedCount != 1)
                return (false, m.Type == "out" ? "Insufficient stock" : "Item not found", null);

            m.Id = null;
            m.CreatedAt = DateTime.UtcNow;
            await _movs.InsertOneAsync(m);

            return (true, null, m);
        }

        public async Task<List<Movement>> GetByItemAsync(string itemId, int limit = 50)
        {
            if (!ObjectId.TryParse(itemId, out _)) return new();
            return await _movs.Find(x => x.ItemId == itemId)
                              .SortByDescending(x => x.CreatedAt)
                              .Limit(limit)
                              .ToListAsync();
        }
    }
}
