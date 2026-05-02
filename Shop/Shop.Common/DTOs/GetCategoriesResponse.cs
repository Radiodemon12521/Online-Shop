using Shop.Entities;

namespace Shop.Common.DTOs
{
    public class GetCategoriesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<ProductDto>Products { get; set; } 
        public Guid? ParentId { get; set; }
 
    }
}
