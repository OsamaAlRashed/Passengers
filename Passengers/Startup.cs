using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Passengers.Location.AddressSerive;
using Passengers.Location.AreaService;
using Passengers.Location.CityService;
using Passengers.Location.CountryService;
using Passengers.Location.LocationService;
using Passengers.Main.DiscountService;
using Passengers.Main.OfferService;
using Passengers.Main.PaymentService;
using Passengers.Main.ProductService;
using Passengers.Main.SalaryLogService;
using Passengers.Main.TagService;
using Passengers.Models.Security;
using Passengers.Order.OrderService;
using Passengers.Order.RealTime;
using Passengers.Order.RealTime.Hubs;
using Passengers.Security.AccountService;
using Passengers.Security.AdminService;
using Passengers.Security.CustomerService;
using Passengers.Security.DriveService;
using Passengers.Security.Shared;
using Passengers.Security.ShopService;
using Passengers.Security.TokenService;
using Passengers.Shared.CategoryService;
using Passengers.Shared.DocumentService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Filter;
using Passengers.SharedKernel.Jwt;
using Passengers.SharedKernel.Services.ConfigureServices;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SharedKernel.Services.EmailService;
using Passengers.SharedKernel.Services.LangService;
using Passengers.SharedKernel.Swagger;
using Passengers.SqlServer.DataBase;
using Passengers.SqlServer.DataBase.Seed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passengers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

            services.AddHttpClient();

            services.AddMvcCore().AddNewtonsoftJson();

            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<PassengersDbContext>()
            .AddDefaultTokenProviders();

            services.AddOpenAPI();
            services.AddJwtSecurity(Configuration);
            services.AddDbContext<PassengersDbContext>
                (options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ILangService, LangService>();
            services.AddHttpContextAccessor();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddHttpClient("fcm", c =>
                c.BaseAddress = new Uri("https://fcm.googleapis.com"));

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<ISharedRepository, SharedRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.ConfigureOpenAPI();

            app.UseCors("AllowOrigin");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<OrderHub>("/orderHub");
            });


            app.UseSqlServerSeed<PassengersDbContext>(async (context, provider) =>
            {
                await context.Database.MigrateAsync();
                await context.Database.EnsureCreatedAsync();
                await SecuritySeed.InitializeAsync(provider);
                await DataSeed.InitializeAsync(provider);
            });
        }
    }
}
