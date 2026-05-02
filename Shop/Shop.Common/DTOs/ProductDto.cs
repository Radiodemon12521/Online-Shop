using Shop.Entities;

namespace Shop.Common.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImageSource { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }


    }
}
