using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Entities;
using Shop.Common.DTOs;
using System.ComponentModel.DataAnnotations;
using Dapper;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductController : ControllerBase
    {
        private DapperContext _dapperContext;
        private AppDbContext _appDbContext;
        public ProductController(AppDbContext dbContext, DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
            _appDbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var connection = _dapperContext.CreateConnection();
            var sql = @"SELECT * FROM ""Products""";
            var products = (await connection.QueryAsync<ProductDto>(sql)).ToList();
            return Ok(products);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateProductRequest updateProduct)
        {
           

            var entity = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == updateProduct.Id);
            if (entity is null)
                return NotFound();

           

            entity.Name = updateProduct.Name;
            entity.Description = updateProduct.Description;
            entity.Price = updateProduct.Price;
            entity.ImageSource = updateProduct.ImageSource;
            entity.CategoryId = updateProduct.CategoryId;

        

            _appDbContext.Products.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ProductDto request)
        {
            var product = new Product() { 
                Id= request.Id,
                CategoryId=request.CategoryId,
                Description= request.Description,
                Name=request.Name,ImageSource=request.ImageSource,
                Price=request.Price };
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
