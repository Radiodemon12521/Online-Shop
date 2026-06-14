using Shop.Entities;

namespace ShopWASM
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Product.Price * Quantity;
    }

    public class CartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public decimal TotalPrice => _items.Sum(i => i.Total);
        public int TotalCount => _items.Sum(i => i.Quantity);

        public event Action? OnChange;

        public void Add(Product product)
        {
            var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing is not null)
                existing.Quantity++;
            else
                _items.Add(new CartItem { Product = product, Quantity = 1 });

            OnChange?.Invoke();
        }

        public void Remove(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item is not null)
            {
                _items.Remove(item);
                OnChange?.Invoke();
            }
        }

        public void DecreaseQuantity(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item is null) return;

            if (item.Quantity > 1)
                item.Quantity--;
            else
                _items.Remove(item);

            OnChange?.Invoke();
        }

        public bool Contains(Guid productId) => _items.Any(i => i.Product.Id == productId);
    }
}