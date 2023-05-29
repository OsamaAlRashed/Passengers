using EasyRefreshToken.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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
using Passengers.Security.ShopService;
using Passengers.Shared.CategoryService;
using Passengers.Shared.DocumentService;
using Passengers.Shared.NotificationService;
using Passengers.Shared.SettingService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Filter;
using Passengers.SharedKernel.Jwt;
using Passengers.SharedKernel.Services.ConfigureServices;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SharedKernel.Services.EmailService;
using Passengers.SharedKernel.Services.LangService;
using Passengers.SharedKernel.Services.NotificationService;
using Passengers.SharedKernel.Swagger;
using Passengers.SqlServer.DataBase;
using Passengers.SqlServer.DataBase.Seed;
using System;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

builder.Services.AddHttpClient();

builder.Services.AddMvc()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
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

builder.Services.AddOpenAPI();
builder.Services.AddJwtSecurity(configuration);
builder.Services.AddDbContext<PassengersDbContext>
    (options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    });
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<ILangService, LangService>();
builder.Services.AddHttpContextAccessor();
//services.AddCors(c =>
//{
//    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials();
    });
});

builder.Services.AddHttpClient("fcm", c =>
    c.BaseAddress = new Uri("https://fcm.googleapis.com"));

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ISharedRepository, SharedRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserConnectionManager, UserConnectionManager>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();

builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

builder.Services.AddRefreshToken<PassengersDbContext, RefreshToken, AppUser, Guid>(op =>
{
    op.TokenExpiredDays = ConstantValue.ExpireRefreshTokenDay;
    op.MaxNumberOfActiveDevices = MaxNumberOfActiveDevices.Configure("UserType", (UserTypes.Driver, 1));
    op.PreventingLoginWhenAccessToMaxNumberOfActiveDevices = false;
});

//FirebaseApp.Create(new AppOptions()
//{
//    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebasekey.json"))
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.ConfigureOpenAPI();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/orderHub");

app.UseSqlServerSeed<PassengersDbContext>(async (context, provider) =>
{
    await context.Database.MigrateAsync();
    await context.Database.EnsureCreatedAsync();
    await SecuritySeed.InitializeAsync(provider);
    await DataSeed.InitializeAsync(provider);
});

app.Run();