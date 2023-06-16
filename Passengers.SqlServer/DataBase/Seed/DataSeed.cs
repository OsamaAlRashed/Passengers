using Bogus;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passengers.Models.Base;
using Passengers.Models.Location;
using Passengers.Models.Main;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passengers.SqlServer.DataBase.Seed
{
    public static class DataSeed
    {
        private static readonly Faker _faker = new Faker();

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetService<UserManager<AppUser>>();
            var context = services.GetService<PassengersDbContext>();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var newRole = await CreateNewRoles(roleManager);
                await ClearRoles(roleManager);

                await SeedSettings(context);

                await SeedLocations(context);
                
                await SeedCategories(context);
                
                await SeedTags(context);

                await SeedUsers(userManager, roleManager, newRole, context);

                await SeedProducts(context);

                await SeedOrders(context);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


        }

        private static async Task SeedOrders(PassengersDbContext context)
        {
            if (context.Orders.Any())
            {
                return;
            }


            for (int i = 0; i < 10; i++)
            {
                await SeedOrders(context, OrderStatus.Sent);
                await SeedOrders(context, OrderStatus.Refused);
                await SeedOrders(context, OrderStatus.Accepted);
                await SeedOrders(context, OrderStatus.Assigned);
                await SeedOrders(context, OrderStatus.Completed);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedOrders(PassengersDbContext context, OrderStatus orderStatus)
        {
            context.Orders.Add(new Order
            {
                AddressId = await context.GetRandomId<Address>(),
                DeliveryCost = Random.Shared.Next(2, 10),
                DriverId = await context.GetRandomUserId(UserType.Driver),
                ExpectedCost = Random.Shared.Next(2, 10),
                DriverNote = _faker.Lorem.Text(),
                ExpectedTime = Random.Shared.Next(20, 60),
                ShopNote = _faker.Lorem.Text(),
                IsShopReady = orderStatus > OrderStatus.Accepted ? true : false,
                SerialNumber = "A" + Helpers.GetNumberToken(5),
                OrderType = OrderType.Instant,
                OrderDetails = new List<OrderDetails>()
                    {
                        new OrderDetails
                        {
                            ProductId = await context.GetRandomId<Product>(),
                            Quantity = Random.Shared.Next(1, 5),
                        },
                        new OrderDetails
                        {
                            ProductId = await context.GetRandomId<Product>(),
                            Quantity = Random.Shared.Next(1, 5),
                        }
                    },
                OrderStatusLogs = orderStatus switch
                {
                    OrderStatus.Sent => SentStatus(_faker),
                    OrderStatus.Refused => RefusedStaus(_faker),
                    OrderStatus.Accepted => AcceptedStatus(_faker),
                    OrderStatus.Assigned => AssignedStatus(_faker),
                    OrderStatus.Completed => CompletedStatus(_faker),
                    _ => SentStatus(_faker)
                },
            });
        }


        #region Order Status

        private static List<OrderStatusLog> SentStatus(Faker faker)
        {
            return new List<OrderStatusLog>()
            {
                new OrderStatusLog()
                {
                    Status = OrderStatus.Sent,
                    Note = faker.Lorem.Text(),
                }
            };
        }

        private static List<OrderStatusLog> AcceptedStatus(Faker faker)
        {
            return new List<OrderStatusLog>()
            {
                new OrderStatusLog()
                {
                    Status = OrderStatus.Sent,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Accepted,
                    Note = faker.Lorem.Text(),
                },
            };
        }

        private static List<OrderStatusLog> AssignedStatus(Faker faker)
        {
            return new List<OrderStatusLog>()
            {
                new OrderStatusLog()
                {
                    Status = OrderStatus.Sent,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Accepted,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Assigned,
                    Note = faker.Lorem.Text(),
                },
            };
        }

        private static List<OrderStatusLog> CompletedStatus(Faker faker)
        {
            return new List<OrderStatusLog>()
            {
                new OrderStatusLog()
                {
                    Status = OrderStatus.Sent,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Accepted,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Assigned,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Completed,
                    Note = faker.Lorem.Text(),
                },
            };
        }

        private static List<OrderStatusLog> RefusedStaus(Faker faker)
        {
            return new List<OrderStatusLog>()
            {
                new OrderStatusLog()
                {
                    Status = OrderStatus.Sent,
                    Note = faker.Lorem.Text(),
                },
                new OrderStatusLog()
                {
                    Status = OrderStatus.Refused,
                    Note = faker.Lorem.Text(),
                },
            };
        }

        #endregion

        private static async Task SeedProducts(PassengersDbContext context)
        {
            if (context.Products.Any())
            {
                return;
            }

            for (int i = 0; i < 50; i++)
            {
                context.Products.Add(new Product
                {
                    Avilable = true,
                    Description = _faker.Lorem.Paragraph(20),
                    PrepareTime = Random.Shared.Next(10, 30),
                    Name = _faker.Lorem.Paragraph(10),
                    TagId = await context.GetRandomId<Tag>(),
                    PriceLogs = new List<PriceLog>()
                    {
                        new PriceLog()
                        {
                            Price = Random.Shared.Next(10, 50),
                            DateCreated = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddYears(1),
                        }
                    }
                });
            }

            await context.SaveChangesAsync();
        }

        public static async Task<Guid> GetRandomUserId(this PassengersDbContext context, UserType userType)
        {
            var count = await context.Set<AppUser>().Where(x => x.UserType == userType).CountAsync();

            return (await context.Set<AppUser>().Where(x => x.UserType == userType)
                .Skip(count - 1).FirstOrDefaultAsync()).Id;

        }

        public static async Task<Guid> GetRandomId<T>(this PassengersDbContext context)
            where T : class, IBaseEntity
        {
            var count = await context.Set<T>()
                .CountAsync();

            return (await context.Set<T>().Skip(count - 1).FirstOrDefaultAsync()).Id;
        }

        #region Seed Users

        private static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IEnumerable<string> newRoles, PassengersDbContext context)
        {
            #region Admin
            var admin = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.UserName == "admin");
            if (admin is null)
            {
                admin = new AppUser()
                {
                    UserName = "admin",
                    FullName = "admin admin",
                    Email = "admin@admin",
                    UserType = UserType.Admin,
                    AccountStatus = AccountStatus.Accepted,
                    DOB = new DateTime(2000, 1, 1),
                    AddressText = "Syria - Aleppo",
                };
                var createResult = await userManager.CreateAsync(admin, "passengers");
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(admin, UserType.Admin.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            #endregion

            for (int i = 0; i < 10; i++)
            {
                await SeedCustomers(userManager, context, _faker.Phone.PhoneNumber(), _faker.Name.FullName());
                await SeedDrivers(userManager, context, _faker.Phone.PhoneNumber(), _faker.Name.FullName());
                await SeedShops(userManager, context, _faker.Phone.PhoneNumber(), $"Passengers Shop {i + 1}");
            }
        }

        private static async Task SeedCustomers(UserManager<AppUser> userManager, PassengersDbContext context, string phoneNumber, string fullName)
        {
            string randomValue = Guid.NewGuid().ToString().Substring(0, 8);
            var customer = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (customer is null)
            {
                customer = new AppUser()
                {
                    UserName = $"Customer{randomValue}",
                    FullName = fullName,
                    Email = $"C{randomValue}@passenges.com",
                    UserType = UserType.Customer,
                    AccountStatus = AccountStatus.Accepted,
                    DOB = new DateTime(2000, 1, 1),
                    Addresses = new List<Address>()
                    {
                        new Address
                        {
                            AreaId = await context.GetRandomId<Area>(),
                            Building = _faker.Address.BuildingNumber(),
                            IsActive = true,
                            IsCurrentLocation = true,
                            Lat = _faker.Address.Latitude().ToString(),
                            Long = _faker.Address.Longitude().ToString(),
                            Note = _faker.Lorem.Text(),
                            Title = "Home",
                            PhoneNumber = _faker.Phone.PhoneNumber()
                        }
                    }
                };
                var createResult = await userManager.CreateAsync(customer, "passengers");
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(customer, UserType.Customer.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
        }

        private static async Task SeedDrivers(UserManager<AppUser> userManager, PassengersDbContext context, string phoneNumber, string fullName)
        {
            string randomValue = Guid.NewGuid().ToString().Substring(0, 8);
            var driver = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (driver is null)
            {
                driver = new AppUser()
                {
                    UserName = $"Driver{randomValue}",
                    FullName = fullName,
                    Email = $"D{randomValue}@passenges.com",
                    UserType = UserType.Driver,
                    AccountStatus = AccountStatus.Accepted,
                    DOB = new DateTime(2000, 1, 1),
                    BloodType = BloodType.ONegative,
                    AddressText = "Albarsha, street 14, unit 602",
                };
                var createResult = await userManager.CreateAsync(driver, "passengers");
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(driver, UserType.Customer.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
        }

        private static async Task SeedShops(UserManager<AppUser> userManager, PassengersDbContext context,
            string phoneNumber,
            string name
            )
        {
            var tagsNames = context.Set<Tag>()
                 .AsEnumerable()
                 .OrderBy(x => Random.Shared.Next(10))
                 .Take(3)
                 .Select(x => x.Name)
                 .ToList();
            string randomValue = Guid.NewGuid().ToString().Substring(0, 8);
            var shop = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            if (shop is null)
            {
                shop = new AppUser()
                {
                    Name = name,
                    UserName = $"Shop{randomValue}",
                    Email = $"S{randomValue}@passenges.com",
                    UserType = UserType.Shop,
                    AccountStatus = AccountStatus.Accepted,
                    Description = "",
                    OwnerName = "Owner",
                    CategoryId = await context.GetRandomId<Category>(),
                    Tags = tagsNames.Select(x => new Tag
                    {
                        Name = x
                    }).ToList(),
                    Address = new Address
                    {
                        Lat = _faker.Address.Latitude().ToString(),
                        Long = _faker.Address.Latitude().ToString(),
                        Text = _faker.Address.FullAddress(),
                        AreaId = await context.GetRandomId<Area>()
                    },
                    ShopSchedules = new List<ShopSchedule>
                    {
                        new ShopSchedule
                        {
                            Days = "1,2,3,4,5",
                            FromTime = new TimeSpan(9,0,0),
                            ToTime = new TimeSpan(23,0,0),
                        }
                    },
                };
                var createResult = await userManager.CreateAsync(shop, "passengers");
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(shop, UserType.Customer.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
        }

        #endregion

        #region Seed Roles
        private static async Task<IEnumerable<string>> CreateNewRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = Enum.GetValues(typeof(UserType)).Cast<UserType>().Select(a => a.ToString());
            var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
            var newRoles = roles.Except(identityRoles);

            foreach (var @new in newRoles)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>() { Name = @new });
            }

            return newRoles;
        }

        private static async Task<IEnumerable<string>> ClearRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = Enum.GetValues(typeof(UserType)).Cast<UserType>().Select(a => a.ToString());
            var identityRoles = roleManager.Roles.ToList();

            var clearRoles = identityRoles.Where(x => !roles.Contains(x.Name));

            foreach (var @new in clearRoles)
            {
                await roleManager.DeleteAsync(@new);
            }

            return clearRoles.Select(x => x.Name);
        }

        #endregion

        #region Seed Tags

        private static async Task SeedTags(PassengersDbContext context)
        {
            if (context.Tags.Any())
            {
                return;
            }

            AddTag(context, "Burger 🍔");
            AddTag(context, "Pizza 🍕");
            AddTag(context, "Ice cream🍦");
            AddTag(context, "Chicken 🍗");
            AddTag(context, "meat 🍖");
            AddTag(context, "Fruit 🍓");
            AddTag(context, "Soft drinks 🍹");
            AddTag(context, "Sandwich 🥪");
            AddTag(context, "Tea 🍵");
            AddTag(context, "Soft drinks 🍹");
            AddTag(context, "Fatayer 🥟");
            AddTag(context, "Cup cake 🧁");
            AddTag(context, "Doughnuts 🍩");
            AddTag(context, "Cake 🍰");
            AddTag(context, "Bavarage 🍹");

            await context.SaveChangesAsync();
        }

        private static void AddTag(PassengersDbContext context, string name)
        {
            var tag1 = new Tag
            {
                Name = name,
            };
            context.Tags.Add(tag1);
        }

        #endregion

        #region Seed Categories
        private static async Task SeedCategories(PassengersDbContext context)
        {
            if (context.Categories.Any())
            {
                return;
            }

            AddCategory(context, "Fast Food");
            AddCategory(context, "Café");
            AddCategory(context, "Ice cream");
            AddCategory(context, "Pastries");
            AddCategory(context, "Drinks");
            AddCategory(context, "Grills");

            await context.SaveChangesAsync();
        }

        private static void AddCategory(PassengersDbContext context, string name)
        {
            var category1 = new Category
            {
                Name = name,
                LogoPath = "",
                ParentId = null,
            };
            context.Categories.Add(category1);
        }
        #endregion

        private static async Task SeedLocations(PassengersDbContext context)
        {
            if (context.Countries.Any())
            {
                return;
            }

            var syria = new Country
            {
                Name = "Syria",
                Cities = new List<City>()
                    {
                        new City
                        {
                            Name = "Aleppo",
                            Areas = new List<Area>()
                            {
                                new Area
                                {
                                    Name = "Alforqan"
                                }
                            }
                        }
                    }
            };

            var uae = new Country
            {
                Name = "UAE",
                Cities = new List<City>()
                    {
                        new City
                        {
                            Name = "Dubai",
                            Areas = new List<Area>()
                            {
                                new Area
                                {
                                    Name = "Albarsha"
                                }
                            }
                        }
                    }
            };

            context.Countries.Add(syria);
            context.Countries.Add(uae);

            await context.SaveChangesAsync();
        }

        private static async Task SeedSettings(PassengersDbContext context)
        {
            if (context.Settings.Any())
            {
                return;
            }

            var setting = new Setting
            {
                KMPrice = 500
            };
            context.Settings.Add(setting);
            await context.SaveChangesAsync();
        }
    }
}
