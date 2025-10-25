namespace ClusterInventory.Repositories
{
    using Models;
    public interface IItemRepository
    {
        Task<List<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(string id);
        Task<Item?> GetBySkuAsync(string sku);
        Task<Item> CreateAsync(Item item);
        Task<bool> DeleteAsync(string id);
    }
}
