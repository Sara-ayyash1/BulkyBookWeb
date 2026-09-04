using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory = false)
        {
            if(includeCategory)
            {
                return await _context.Products.Include(p => p.Category).ToListAsync();
            }
            else
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id , bool includeCategory = false)
        {
            if(includeCategory)
            {
                return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            }
            else
            {
                return await _context.Products.FindAsync(id);
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

       
    }
}