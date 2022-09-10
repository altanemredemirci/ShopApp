using Microsoft.AspNetCore.Identity;

namespace ShopApp.WebUI.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed (UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            var username = "admin";
            var email = "adminuser@shopapp.com";
            var password = "Adminuser_1234";
            var role = "admin";

            if(await userManager.FindByNameAsync(username) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));

                var user = new ApplicationUser()
                {
                    UserName = username,
                    Email = email,
                    FullName = "Admin User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
