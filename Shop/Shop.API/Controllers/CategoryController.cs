using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shop.API.Controllers
{
        [ApiController]
        [Route("Category")]
    public class CategoryController:ControllerBase
    {
        private AppDbContext _appDbContext;
        
        public CategoryController(AppDbContext dbContext)
        {
            _appDbContext= dbContext;

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _appDbContext.Categories.ToListAsync();

         

            return Ok(categories);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody,Required]Category category)
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
    }
}
