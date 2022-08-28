using ShopApp.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100,MinimumLength =10,ErrorMessage ="Ürün ismi minimum 10 karakter olmalı")]
        public string Name { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 20, ErrorMessage = "Ürün açıklaması minimum 20 karakter olmalı")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Fiyat Belirtiniz")]
        [Range(1000,100000)]
        public decimal? Price { get; set; }
        public List<Category> SelectedCategories { get; set; }
    }
}
