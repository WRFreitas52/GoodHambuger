using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoodHamburger.Models;

namespace GoodHamburger.Data
{
    public class GoodHamburgerContext : DbContext
    {       
        public DbSet<Sandwich> Sandwiches { get; set; }
        public DbSet<Extra> Extras { get; set; }
        public DbSet<Order> Orders { get; set; }

        public GoodHamburgerContext(DbContextOptions<GoodHamburgerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed some initial data for testing
            modelBuilder.Entity<Sandwich>().HasData(
                new Sandwich { Id = 1, Name = "X Burger", Price = 5.00 },
                new Sandwich { Id = 2, Name = "X Egg", Price = 4.50 },
                new Sandwich { Id = 3, Name = "X Bacon", Price = 7.00 }
            );

            modelBuilder.Entity<Extra>().HasData(
                new Extra { Id = 4, Name = "Fries", Price = 2.00, IsDrink = false },
                new Extra { Id = 5, Name = "Soft drink", Price = 2.50, IsDrink = true }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
 }

