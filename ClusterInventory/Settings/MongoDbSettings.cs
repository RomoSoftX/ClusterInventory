namespace ClusterInventory.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public string ItemsCollection { get; set; } = default!;
        public string MovementsCollection { get; set; } = default!;
    }
}
