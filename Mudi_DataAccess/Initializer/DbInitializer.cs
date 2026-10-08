using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Mudi_Models;
using Mudi_Utility;
using System;
using System.Linq;

namespace Mudi_DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, IHostEnvironment environment)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _environment = environment;
        }

        public void Initialize()
        {
            // The historic migrations target SQL Server. SQLite uses an isolated,
            // disposable development database created from the current model.
            if (_db.Database.IsSqlite())
            {
                if (!_environment.IsDevelopment())
                    throw new InvalidOperationException("SQLite setup is only supported for local Development.");
                _db.Database.EnsureCreated();
            }
            else
                _db.Database.Migrate();

            foreach (var role in new[] { WC.AdminRole, WC.CustomerRole })
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    Check(_roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult());

            if (!_db.WebSiteDetail.Any())
            {
                _db.WebSiteDetail.Add(new WebSiteDetail { AboutUs = "Mudi is your local online grocery shop.", ContactUs = "Contact us using the Contact Us page." });
                _db.SaveChanges();
            }

            if (!_environment.IsDevelopment()) return;
            var email = _configuration["LocalDevelopment:AdminEmail"];
            var password = _configuration["LocalDevelopment:AdminPassword"];
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                var user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
                if (user == null)
                {
                    user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true,
                        FullName = "Local Admin", PhoneNumber = "01700000000" };
                    Check(_userManager.CreateAsync(user, password).GetAwaiter().GetResult());
                }
                if (!_userManager.IsInRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult())
                    Check(_userManager.AddToRoleAsync(user, WC.AdminRole).GetAwaiter().GetResult());
            }

            if (_configuration.GetValue<bool>("LocalDevelopment:SeedDemoData")) SeedDemoCatalogue();
        }

        private void SeedDemoCatalogue()
        {
            var categories = new[]
            {
                ("Fresh produce", "Fruit and vegetables for everyday cooking."),
                ("Meat & poultry", "Meat and poultry for the family kitchen."),
                ("Dairy", "Butter and dairy essentials."),
                ("Pantry & snacks", "Cupboard staples and something to snack on."),
                ("Home & cleaning", "Supplies to keep your home clean."),
                ("Personal care", "Everyday personal care essentials."),
                ("Health essentials", "Useful everyday health supplies.")
            };
            // Upgrade only the exact placeholder from the original local seed.
            // Existing catalogue records and order history remain intact.
            var placeholder = _db.Product.FirstOrDefault(p => p.Name == "Sample grocery"
                && p.Description == "Local development sample product.");
            var oldCategory = _db.Category.FirstOrDefault(c => c.Name == "Groceries"
                && c.CategoryDescription == "Everyday essentials");
            if (oldCategory != null && !_db.Category.Any(c => c.Name == "Fresh produce")
                && !_db.Product.Any(p => p.CategoryId == oldCategory.Id && p.Name != "Sample grocery"))
            {
                oldCategory.Name = "Fresh produce";
                oldCategory.CategoryDescription = categories[0].Item2;
            }
            for (var i = 0; i < categories.Length; i++)
                if (!_db.Category.Local.Any(c => c.Name == categories[i].Item1)
                    && !_db.Category.Any(c => c.Name == categories[i].Item1))
                    _db.Category.Add(new Category { Name = categories[i].Item1,
                        CategoryDescription = categories[i].Item2, DisplayOrder = i + 1 });
            _db.SaveChanges();

            var products = new[]
            {
                ("Tomatoes", "Fresh produce", 100d, "kg", "167fc8d5-ebd8-4df0-a7df-0bdad9caf29a.jpg", "A colourful staple for salads, sauces and everyday cooking."),
                ("Bananas", "Fresh produce", 80d, "dozen", "05789c07-f143-44f0-b16b-919bd35cc05d.jpg", "An easy addition to breakfast or your fruit bowl."),
                ("Potatoes", "Fresh produce", 60d, "kg", "9d6fbfcd-0888-4448-9fac-087962a49c48.jpg", "A versatile kitchen staple for curries, mash and roasting."),
                ("Cucumbers", "Fresh produce", 70d, "kg", "4a75c8ee-e3c3-4f6e-97cb-34deba560f3a.jpg", "Crisp cucumbers for salads and side dishes."),
                ("Red capsicum", "Fresh produce", 180d, "500 g", "9f998c96-2f70-4a98-9b60-d7cf3f1e4db8.jpg", "Add colour to stir-fries, salads and roasted vegetables."),
                ("Red apples", "Fresh produce", 260d, "kg", "f2e23705-d322-443a-9ab6-df475972345e.jpg", "Keep your fruit bowl stocked with red apples."),
                ("Chicken", "Meat & poultry", 320d, "kg", "8455943a-4bdc-42b4-84c3-765b51bb9189.jpg", "Chicken for curries, soups and family meals."),
                ("Beef cubes", "Meat & poultry", 800d, "kg", "786036e9-ab5e-4f33-a792-699f365dcc7d.jpg", "Beef cubes for slow cooking and hearty curries."),
                ("Mutton", "Meat & poultry", 1100d, "kg", "5b38eef6-a387-46a4-8b31-45e08e86f99e.jpg", "Mutton for your favourite family recipes."),
                ("Butter", "Dairy", 220d, "200 g", "02dd52f0-f6d9-4b67-8c5c-9f34c5dfb120.jpg", "Butter for toast, baking and everyday cooking."),
                ("Peanut butter", "Pantry & snacks", 320d, "jar", "b9cb0162-81a6-438f-adf2-68ac87a130e4.jpg", "A pantry spread for toast, sandwiches and snacks."),
                ("Biscuits", "Pantry & snacks", 60d, "pack", "d966415b-b866-4ddb-91a0-41223c355fdd.jpg", "A pack of biscuits for your tea break."),
                ("Glass cleaner", "Home & cleaning", 180d, "bottle", "22341941-4975-4c1c-a292-a1d29126901c.jpg", "Glass cleaner for windows and household glass surfaces."),
                ("Floor cleaner", "Home & cleaning", 220d, "bottle", "86e09afd-5231-435e-8e66-c6dba35b7bc2.jpg", "A cleaning cupboard essential for household floors."),
                ("Hair styling gel", "Personal care", 160d, "tube", "ff3d7076-3ebd-45ac-9307-bc3cf6027de2.jpg", "Hair styling gel for your daily routine."),
                ("Face mask", "Health essentials", 25d, "piece", "cc1e719a-6b19-47a0-8196-35a7536e18cc.jpg", "A face mask for your everyday essentials kit.")
            };
            if (placeholder != null && _db.Product.Any(p => p.Name == "Tomatoes"))
            {
                placeholder.Name = "Tomatoes (demo pack)";
                placeholder.Image = products[0].Item5;
                placeholder.ShortDescription = "Tomatoes in the original local demo pack.";
                placeholder.Description = "Local demo pack. Sample price and stock; existing orders are preserved.";
                // Preserve this placeholder's ID, unit, price and stock for its existing orders.
            }
            foreach (var demo in products)
            {
                if (_db.Product.Any(p => p.Name == demo.Item1)) continue;
                var product = demo.Item1 == "Tomatoes" && placeholder != null ? placeholder : new Product();
                product.Name = demo.Item1;
                product.CategoryId = _db.Category.Single(c => c.Name == demo.Item2).Id;
                product.Price = demo.Item3;
                product.Unit = demo.Item4;
                product.Image = demo.Item5;
                product.ShortDescription = demo.Item6;
                product.Description = demo.Item6 + " Demo catalogue item for local testing; price and stock are sample values.";
                if (product.Id == 0) { product.Stock = 50; _db.Product.Add(product); }
            }
            _db.SaveChanges();
        }

        private static void Check(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
