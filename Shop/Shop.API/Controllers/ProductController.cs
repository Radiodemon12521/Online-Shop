using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Entities;
using System.ComponentModel.DataAnnotations;

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

            //if (products.Count == 0)
            //{
            //    return NotFound();
            //}

            return Ok(products);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody, Required] Product product)
        {

            var entity = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            if (entity is null)
                return NotFound();
            entity.Name = product.Name;
            entity.Description = product.Description;
            entity.Price = product.Price;
            entity.ImageSource = product.ImageSource;

            _appDbContext.Products.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return Ok();

        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody, Required] Product product)
        {
            _appDbContext.Products.Add(product);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product is null) return NotFound();
            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return Ok();

        }
        [HttpGet("ByCategory")] 
        public async Task<IActionResult> ByCategory([FromQuery,Required]Guid id)
        {
            var products = await _appDbContext.Products.Where(x => x.CategoryId == id).ToListAsync();
            return Ok(products);
        }
    }
}
