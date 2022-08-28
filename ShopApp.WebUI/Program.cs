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
options.UseSqlServer(@"Server=DESKTOP-9IJKPL9\SQLDERS; Database=ShopApp; uid=sa;pwd=1"));

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

    options.Lockout.MaxFailedAccessAttempts = 5; //5 kere yanlýþ giriþte kitle
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5); //5 kere yanlýþ giriþte 5 dakika kitle
    options.Lockout.AllowedForNewUsers = true;

    //user
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = true;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";              //Giriþ Sayfasý
    options.LogoutPath = "/Account/Logout";            //Çýkýþ Sayfasý
    options.AccessDeniedPath = "/account/accessdenied";//Yetkisiz Giriþ Sayfasý
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //Cookie Süresi(default 20dk)
    options.SlidingExpiration = true;
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,                               //Tarayýcýda tutulsun
        Name = "ShopApp.Security.Cookie"               //Cookie adý
    };
});


//AddScoped: Gelen her bir web request için bir instance oluþturur ve gelen her ayný request te ayný instance'ý kullanýlýr, farklý web requestleri içinde yeniden instance alýr.


//Dependency Injection: Baðýmlýlýk yönetimi
builder.Services.AddScoped<IProductDal, EfCoreProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager> ();







//MVC Mimarisini Tanýmladým.
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
app.CustomStaticFiles(); //middleWare: Bootstrap kütüphanesini npm ile indirilecek ve nodemodules içerisindeki static dosyalarý ile dýþarýya açma iþlemidir.
app.UseAuthentication(); //Sýralama Önemli
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
});

app.Run();


