using Microsoft.Extensions.FileProviders;

namespace ShopApp.WebUI.Middlewares
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder CustomStaticFiles(this IApplicationBuilder app)
        {
            //Directory.GetCurrentDirectory() : Çalışılan dosya dizinini(yolunu)  verir.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "node_modules"); // çalışan dizinin sonuna node_modules kelimesini ekler.

            //path=> "C:\Users\altan\source\repos\YAZILIMUZMANLIGI_16-19\WEB YAZILIM\ShopApp\ShopApp.WebUI\node_modules"

            var options = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = "/modules"
            };
            app.UseStaticFiles(options);
            return app;
        }
    }
}
