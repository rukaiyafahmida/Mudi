using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Mudi_Utility;
using Mudi_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mudi_DataAccess.Repository;
using Mudi_DataAccess.Repository.IRepository;
using Mudi_DataAccess.Initializer;

namespace Mudi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(
                Path.Combine(AppContext.BaseDirectory, "App_Data", "keys")));
            var provider = Configuration["Database:Provider"] ?? "SqlServer";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
                    options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
                else if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                else
                    throw new InvalidOperationException($"Unsupported database provider: {provider}");
            });
            services.AddIdentity<IdentityUser, IdentityRole>()
                       .AddDefaultTokenProviders().AddDefaultUI()
                       .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddHttpClient<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            services.AddScoped<IWishListDetailRepository, WishListDetailRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IWebSiteDetailRepository, WebSiteDetailRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            var facebook = Configuration.GetSection("Authentication:Facebook");
            if (!string.IsNullOrWhiteSpace(facebook["AppId"]) &&
                !string.IsNullOrWhiteSpace(facebook["AppSecret"]))
            {
                services.AddAuthentication().AddFacebook(options =>
                {
                    options.AppId = facebook["AppId"];
                    options.AppSecret = facebook["AppSecret"];
                });
            }

            services.AddRazorPages();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            if (!env.IsDevelopment())
                app.UseHttpsRedirection();
            Directory.CreateDirectory(Path.Combine(env.WebRootPath, "images", "product"));
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            dbInitializer.Initialize();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
