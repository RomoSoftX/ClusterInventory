namespace ClusterInventory.Repositories
{
    using Models;
    public interface IMovementRepository
    {
        Task<(bool Ok, string? Error, Movement? Saved)> CreateAsync(Movement movement);
        Task<List<Movement>> GetByItemAsync(string itemId, int limit = 50);
    }
}
