using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Common.DTOs;
using Shop.Database;
using Shop.Entities;

namespace Shop.API.Controllers
{

    [ApiController]
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private DapperContext _dapperContext;
        private AppDbContext _appDbContext;

        public CategoryController(AppDbContext dbContext, DapperContext dapperContext)
        {
            _appDbContext = dbContext;
            _dapperContext = dapperContext;

        }
        [HttpGet("Flat")]
        public async Task<IActionResult> GetAllCat()
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"SELECT * FROM ""Categories""";

            var categories = (await connection.QueryAsync<GetCategoriesResponse>(sql)).ToList();
            return Ok(categories);
        }

        [HttpGet("Children")]
        public async Task<IActionResult> GetChildren([FromQuery] Guid? parentId)
        {
            using var connection = _dapperContext.CreateConnection();

            var categories = (
     await connection.QueryAsync<GetCategoriesResponse>(
         """
        SELECT *
        FROM "Categories" c
        WHERE (@parentId IS NULL AND c."ParentId" IS NULL)
           OR c."ParentId" = @parentId
        """,
         new { parentId }))
            .ToList();
         var categoryIds = categories
            .Select(x => x.Id)
            .ToArray();
            var products = (
    await connection.QueryAsync<ProductDto>(
        """
        SELECT *
        FROM "Products" p
        WHERE p."CategoryId" = ANY(@categoryIds)
        """,
        new { categoryIds }))
    .ToList();

            var lookup = products.ToLookup(x => x.CategoryId);

            foreach (var category in categories)
            {
                category.Products = lookup[category.Id]
                    .ToList();
            }
            return Ok(categories);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateCategoryRequest dto)
        {
            var entity = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                ParentId = dto.ParentId
            };

            _appDbContext.Categories.Add(entity);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateCategoryRequest dto)
        {
            var entity = await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return NotFound();

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ParentId = dto.ParentId;

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
