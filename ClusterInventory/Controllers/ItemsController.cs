namespace ClusterInventory.Controllers
{
    using Repositories;
    using Models;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Driver;


    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _repo;

        public ItemsController(IItemRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetById(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item is null) return NotFound(new { error = "Not found or invalid id" });
            return Ok(item);
        }

        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<Item>> GetBySku(string sku)
        {
            var item = await _repo.GetBySkuAsync(sku);
            if (item is null) return NotFound(new { error = "Not found" });
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> Create([FromBody] Item input)
        {
            if (string.IsNullOrWhiteSpace(input.Sku)) return BadRequest(new { error = "SKU is required" });
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { error = "Name is required" });
            if (input.Price < 0) return BadRequest(new { error = "Price >= 0" });
            if (input.Stock < 0) return BadRequest(new { error = "Stock >= 0" });
            if (input.MinStock < 0) return BadRequest(new { error = "MinStock >= 0" });

            try
            {
                var created = await _repo.CreateAsync(input);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (MongoWriteException mwx) when (mwx.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                return Conflict(new { error = "SKU already exists" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound(new { error = "Not found or invalid id" });
            return NoContent();
        }
    }
}
