using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Passengers.Models.Location;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SqlServer.DataBase.Seed
{
    public class DataSeed
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetService<PassengersDbContext>();

            if (!context.Settings.Any())
            {
                var setting = new Setting
                {
                    KMPrice = 500
                };
                context.Settings.Add(setting);
                await context.SaveChangesAsync();
            }

            if (!context.Countries.Any())
            {
                var country = new Country
                {
                    Name = "Syria - سوريا"
                };
                context.Countries.Add(country);
                await context.SaveChangesAsync();

                var city = new City
                {
                    Name = "Aleppo - حلب",
                    CountryId = country.Id
                };
                context.Cities.Add(city);
                await context.SaveChangesAsync();

                var area = new Area
                {
                    Name = "Alforqan - الفرقان",
                    CityId = city.Id
                };

                context.Areas.Add(area);
                await context.SaveChangesAsync();
            }

            if (!context.Categories.Any())
            {
                var category = new Category
                {
                    Name = "Fast Food",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category);
                await context.SaveChangesAsync();
            }

        }
    }
}
