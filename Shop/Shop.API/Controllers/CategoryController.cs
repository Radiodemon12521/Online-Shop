using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Common.DTOs;
using Shop.Database;
using Shop.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private AppDbContext _appDbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;

        }
        [HttpGet("Flat")]
        public async Task<IActionResult> GetAllCat()
        {
            var categories = await _appDbContext.Categories.ToListAsync();
            categories = categories.Select(x => { x.Parent = null; return x; }).ToList();


            return Ok(categories);
        }
        [HttpGet("Children")]
        public async Task<IActionResult> GetChildren([FromQuery]Guid? parentId)
        {
            var categories = await _appDbContext.Categories
                .Where(x=>x.ParentId==parentId)
                .Select(x=>new GetCategoriesResponse()
                {
                    Name= x.Name,
                    Description= x.Description,
                    Id= x.Id,
                    ParentId=x.ParentId  
                })
                .ToListAsync();
            foreach (var category in categories)
            {
                category.Products = await _appDbContext.Products.Where(x => x.CategoryId == category.Id).Select(x=>new ProductDto()
                {
                    CategoryId= x.CategoryId,
                    Name= x.Name,
                    Description= x.Description,
                    Id= x.Id,
                    ImageSource=x.ImageSource,
                    Price= x.Price
                }).ToListAsync();
            }
            

            return Ok(categories);
            
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody, Required] Category category)
        {
        
            _appDbContext.Categories.Add(category);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody, Required] Category category)
        {

            var entity = await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (entity is null)
                return NotFound();
            entity.Name = category.Name;
            entity.Description = category.Description;

            _appDbContext.Categories.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return Ok();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null) return NotFound();
            _appDbContext.Categories.Remove(category);
            await _appDbContext.SaveChangesAsync();
            return Ok();

        }
        [HttpPut("{categoryId}/AddProduct/{productId}")]
        public async Task<IActionResult> AddProduct(Guid productId, Guid categoryId)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
            if (product is null) return NotFound();
            var category = await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(x => x.Id == categoryId);
            if (category is null) return NotFound();
            category.Products.Add(product);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
        

    }
}
