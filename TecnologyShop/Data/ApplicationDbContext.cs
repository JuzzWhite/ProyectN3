﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TecnologyShop.Models;

namespace TecnologyShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<CatalogueItem> CatalogueItem { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

    }
}
