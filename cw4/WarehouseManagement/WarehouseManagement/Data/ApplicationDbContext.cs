﻿using Microsoft.EntityFrameworkCore;

namespace WarehouseManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductWarehouse> ProductWarehouses { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
    }

    public class Product
    {
        public int IdProduct { get; set; }
        public string Name { get; set; } = string.Empty; // Ensure initialized
        public string Description { get; set; } = string.Empty; // Ensure initialized
        public decimal Price { get; set; }
    }

    public class Order
    {
        public int IdOrder { get; set; }
        public int IdProduct { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FulfilledAt { get; set; }
    }

    public class ProductWarehouse
    {
        public int IdProductWarehouse { get; set; }
        public int IdWarehouse { get; set; }
        public int IdProduct { get; set; }
        public int IdOrder { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Warehouse
    {
        public int IdWarehouse { get; set; }
        public string Name { get; set; } = string.Empty; // Ensure initialized
        public string Address { get; set; } = string.Empty; // Ensure initialized
    }
}