using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductController : ControllerBase
    {
        private AppDbContext _appDbContext;
        public ProductController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _appDbContext.Products.ToListAsync();

            if (products.Count == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }
    }
}
