using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, ShopContext>, IProductDal
    {
        public List<Product> GetProductsByCategory(string category,int page,int pageSize)
        {
            using(var context = new ShopContext())
            {
                var products = context.Products.AsQueryable(); //Sorgunun kopyasını tutuluyor, ne zaman ToList() denirse DB den bilgiler alınıp getiriliyor ve istendiği zaman sorgu değiştirilebiliyor.

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category)
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }

                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();
            }
        }

        public Product GetProductDetails(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(p => p.Id == id)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(c => c.Category)
                    .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using(var context = new ShopContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category)
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }

                return products.Count();
            }
        }
    }
}
