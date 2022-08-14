using Microsoft.EntityFrameworkCore;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Categories.Count() == 0)
                {
                    context.Categories.AddRange(Categories);
                }

                if (context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                }

                context.SaveChanges();
            }
        }

        private static Category[] Categories =
        {
            new Category(){Name="Telefon"},
            new Category(){Name="Bilgisayar"}
        };

        private static Product[] Products =
        {
            new Product() { Name = "IPHONE 6S", ImageUrl = "1.jpg", Price = 12000 },
            new Product() { Name = "IPHONE 7S", ImageUrl = "2.jpg", Price = 15000 },
            new Product() { Name = "IPHONE 8S", ImageUrl = "3.jpg", Price = 16000 },
            new Product() { Name = "IPHONE 9S", ImageUrl = "4.jpg", Price = 18000 },
            new Product() { Name = "SAMSUNG Note10", ImageUrl = "5.jpg", Price = 11000 },
            new Product() { Name = "SAMSUNG Note11", ImageUrl = "6.jpg", Price = 13000 },
            new Product() { Name = "SAMSUNG Note12", ImageUrl = "7.jpg", Price = 19000 }
        };
    }
}
