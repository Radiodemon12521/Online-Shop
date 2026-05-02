using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
   public  class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; }
         
       public Category? Parent {  get; set; }
       public Guid? ParentId { get; set; }
        public ICollection<Category> Children { get; set; }

    }
}
