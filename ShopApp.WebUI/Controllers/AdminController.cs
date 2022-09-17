using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entities;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminController(IProductService productService, ICategoryService categoryService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("admin/products")]
        public IActionResult ProductList()
        {
            return View(new ProductListModel()
            {
                Products = _productService.GetAll()
            });
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, IFormFile file)
        {
            ModelState.Remove("SelectedCategories");
            if (ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price                    
                };
                if (file != null)
                {
                    entity.ImageUrl = file.FileName;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _productService.Create(entity);
                return RedirectToAction("ProductList");
            }

            return View(model);
        }

        [Route("admin/products/{id?}")]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _productService.GetByIdWithCategories((int)id);

            var model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        [Route("admin/products/{id?}")]
        public async Task<IActionResult> EditProduct(ProductModel model,int[] categoryIds, IFormFile file)
        {
            ModelState.Remove("SelectedCategories");
            ModelState.Remove("file");
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.Id);

                if (entity == null)
                {
                    return NotFound();
                }

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Price = model.Price;

                if (file != null)
                {
                    entity.ImageUrl = file.FileName;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }


                _productService.Update(entity, categoryIds);

                return RedirectToAction("ProductList");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            if (entity != null)
            {
                _productService.Delete(entity);
            }
            return RedirectToAction("ProductList");
        }


        public IActionResult CategoryList()
        {
            return View(new CategoryListModel()
            {
                Categories = _categoryService.GetAll()
            });
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            var entity = new Category()
            {
                Name = model.Name
            };
            _categoryService.Create(entity);

            return RedirectToAction("CategoryList");
        }

        public IActionResult EditCategory(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var cat = _categoryService.GetByIdWithProducts(Id.Value);
            if (cat == null)
            {
                return NotFound();
            }
            return View(new CategoryModel()
            {
                Id = cat.Id,
                Name = cat.Name,
                Products=cat.ProductCategories.Select(p=>p.Product).ToList()
            });
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            _categoryService.Update(entity);
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)
        {
            var entity = _categoryService.GetById(categoryId);

            if (entity != null)
            {
                _categoryService.Delete(entity);
            }
            return RedirectToAction("CategoryList");
        }


        [HttpPost]
        public IActionResult DeleteFromCategory(int categoryId, int productId)
        {
            _categoryService.DeleteFromCategory(categoryId, productId);
            return Redirect("/admin/categories/" + categoryId);
        }

        public async Task<IActionResult> UserList()
        {
            
            List<ApplicationUser> userList = _userManager.Users.ToList();
            List<AdminUserModel> model = new List<AdminUserModel>();
            foreach (ApplicationUser item in userList)
            {
                AdminUserModel user = new AdminUserModel();
                user.FullName = item.FullName;
                user.Username = item.UserName;
                user.EmailConfirmed = item.EmailConfirmed;
                user.Email = item.Email;
                user.IsAdmin = await _userManager.IsInRoleAsync(item, "admin");

                model.Add(user);
            }
            return View(model);
        }


        public async Task<IActionResult> UserEdit(string Email)
        {

            ApplicationUser entity = await _userManager.FindByEmailAsync(Email);
            if (entity == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı ile daha önce hesap oluşturulmamıştır.");
                return View(entity);
            }

            AdminUserModel user = new AdminUserModel();
            user.FullName = entity.FullName;
            user.Username = entity.UserName;
            user.EmailConfirmed = entity.EmailConfirmed;
            user.Email = entity.Email;
            user.IsAdmin = await _userManager.IsInRoleAsync(entity, "admin");
                        
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(AdminUserModel model)
        {

            ApplicationUser entity = await _userManager.FindByEmailAsync(model.Email);
            if (entity == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı ile daha önce hesap oluşturulmamıştır.");
                return View(entity);
            }

            entity.FullName = model.FullName;
            entity.EmailConfirmed = model.EmailConfirmed;
            entity.Email = model.Email;
            if (model.IsAdmin == true)
            {
                await _userManager.AddToRoleAsync(entity, "admin");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(entity, "admin");
            }

            await _userManager.UpdateAsync(entity);
            return RedirectToAction("UserList");
           
        }


        [HttpPost]
        public async Task<IActionResult> UserDelete(string Email)
        {

            ApplicationUser entity = await _userManager.FindByEmailAsync(Email);
            if (entity != null)
            {
                await _userManager.DeleteAsync(entity);
                return RedirectToAction("UserList");
            }
            ModelState.AddModelError("", "Silme işlemi başarısız!!");
            return View(entity);

        }
    }
}
