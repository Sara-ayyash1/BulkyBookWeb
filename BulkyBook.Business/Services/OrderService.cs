using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int id, bool includeUser = false, bool includeDetails = false)
        {
            var query = _db.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(u => u.Product);
            }
            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<OrderHeader>> GetAllOrderAsync(string? userId = null, string? status = null,  bool includeUser = false, bool includeDetails = false)
        {
            var query = _db.OrderHeaders.AsQueryable();
            if (includeUser)
            {
                query = query.Include(o => o.ApplicationUser);
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails).ThenInclude(u => u.Product);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.ApplicationUserId == userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }
            return await query.ToListAsync();
        }

    }
}