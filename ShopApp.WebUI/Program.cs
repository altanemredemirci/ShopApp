using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.DataAccess.Concrete.Memory;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Middlewares;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
options.UseSqlServer("Server=DESKTOP-9IJKPL9\\SQLDERS; Database=ShopApp; uid=sa;pwd=1"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();




builder.Services.Configure<IdentityOptions>(options =>
{
    //password
    options.Password.RequireDigit = true;  
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    options.Lockout.MaxFailedAccessAttempts = 5; //5 kere yanl�� giri�te kitle
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5); //5 kere yanl�� giri�te 5 dakika kitle
    options.Lockout.AllowedForNewUsers = true;

    //user
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = true;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";              //Giri� Sayfas�
    options.LogoutPath = "/Account/Logout";            //��k�� Sayfas�
    options.AccessDeniedPath = "/account/accessdenied";//Yetkisiz Giri� Sayfas�
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie S�resi(default 20dk)
    options.SlidingExpiration = true;
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,                               //Taray�c�da tutulsun
        Name = "ShopApp.Security.Cookie",              //Cookie ad�
        SameSite = SameSiteMode.Strict                 //Cookie bizim taray�c� taraf�ndan server taraf�na ta��n�r.
    };
});


//AddScoped: Gelen her bir web request i�in bir instance olu�turur ve gelen her ayn� request te ayn� instance'� kullan�l�r, farkl� web requestleri i�inde yeniden instance al�r.


//Dependency Injection: Ba��ml�l�k y�netimi
builder.Services.AddScoped<IProductDal, EfCoreProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager> ();

builder.Services.AddScoped<ICartDal, EfCoreCartDal>();
builder.Services.AddScoped<ICartService, CartManager>();

builder.Services.AddScoped<IOrderDal, EfCoreOrderDal>();
builder.Services.AddScoped<IOrderService, OrderManager>();




//MVC Mimarisini Tan�mlad�m.
builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    SeedDatabase.Seed();
}
app.UseStaticFiles();
app.CustomStaticFiles(); //middleWare: Bootstrap k�t�phanesini npm ile indirilecek ve nodemodules i�erisindeki static dosyalar� ile d��ar�ya a�ma i�lemidir.
app.UseAuthentication(); //S�ralama �nemli
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");

    endpoints.MapControllerRoute(
        name: "products",
        pattern: "products/{category?}",
        defaults: new { controller = "Shop", action = "List" }
        );

    endpoints.MapControllerRoute(
       name: "cart",
       pattern: "cart",
       defaults: new { controller = "Cart", action = "Index" }
       );

    endpoints.MapControllerRoute(
       name: "adminProducts",
       pattern: "admin/products",
       defaults: new { controller = "Admin", action = "ProductList" }
       );

    endpoints.MapControllerRoute(
       name: "adminProducts",
       pattern: "admin/products/{id?}",
       defaults: new { controller = "Admin", action = "EditProduct" }
       );

    endpoints.MapControllerRoute(
      name: "adminCategories",
      pattern: "admin/categories",
      defaults: new { controller = "Admin", action = "CategoryList" }
      );

    endpoints.MapControllerRoute(
      name: "adminCategories",
      pattern: "admin/categories/{id?}",
      defaults: new { controller = "Admin", action = "EditCategory" }
      );

    endpoints.MapControllerRoute(
      name: "checkout",
      pattern: "checkout",
      defaults: new { controller = "cart", action = "Checkout" }
      );

});

app.Run();


