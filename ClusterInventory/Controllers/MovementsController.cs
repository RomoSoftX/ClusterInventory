namespace ClusterInventory.Controllers
{
    using Models;
    using Repositories;
    using Microsoft.AspNetCore.Mvc;
    [ApiController]
    [Route("api/movements")]
    public class MovementsController : ControllerBase
    {
        private readonly IMovementRepository _repo;

        public MovementsController(IMovementRepository repo) => _repo = repo;

        // POST /api/movements  { itemId, type: "in"|"out", quantity, note }
        [HttpPost]
        public async Task<ActionResult<Movement>> Create([FromBody] Movement dto)
        {
            var (ok, error, saved) = await _repo.CreateAsync(dto);
            if (!ok) return BadRequest(new { error });
            return CreatedAtAction(nameof(GetByItem), new { itemId = dto.ItemId }, saved);
        }

        // GET /api/movements?itemId=xxx&limit=50
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movement>>> GetByItem([FromQuery] string itemId, [FromQuery] int limit = 50)
        {
            var items = await _repo.GetByItemAsync(itemId, limit);
            return Ok(items);
        }
    }
}
