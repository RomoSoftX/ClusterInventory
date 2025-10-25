namespace ClusterInventory.Repositories
{
    using Models;
    using Settings;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class ItemRepository : IItemRepository
    {
        private readonly IMongoCollection<Item> _items;

        public ItemRepository(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _items = db.GetCollection<Item>(settings.ItemsCollection);

            // Índice único en SKU
            var idxKeys = Builders<Item>.IndexKeys.Ascending(x => x.Sku);
            var idxModel = new CreateIndexModel<Item>(idxKeys, new CreateIndexOptions { Unique = true });
            _items.Indexes.CreateOne(idxModel);
        }

        public async Task<List<Item>> GetAllAsync() =>
            await _items.Find(_ => true).ToListAsync();

        public async Task<Item?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return null;
            return await _items.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Item?> GetBySkuAsync(string sku) =>
            await _items.Find(x => x.Sku == sku).FirstOrDefaultAsync();

        public async Task<Item> CreateAsync(Item item)
        {
            item.Id = null;
            if (string.IsNullOrWhiteSpace(item.Sku)) throw new ArgumentException("SKU is required");
            await _items.InsertOneAsync(item);
            return item;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return false;
            var result = await _items.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
