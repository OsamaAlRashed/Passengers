using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Passengers.Models.Location;
using Passengers.Models.Main;
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
                var category1 = new Category
                {
                    Name = "Fast Food",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category1);

                var category2 = new Category
                {
                    Name = "Café",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category2);

                var category3 = new Category
                {
                    Name = "Ice cream",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category3);

                var category4 = new Category
                {
                    Name = "Pastries",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category4);

                var category5 = new Category
                {
                    Name = "Drinks",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category5);

                var category6 = new Category
                {
                    Name = "Grills",
                    LogoPath = "",
                    ParentId = null,
                };
                context.Categories.Add(category6);

                await context.SaveChangesAsync();
            }

            if (!context.Tags.Any())
            {
                var tag1 = new Tag
                {
                    Name = "Burger 🍔",
                };
                context.Tags.Add(tag1);

                var tag2 = new Tag
                {
                    Name = "Pizza 🍕",
                };
                context.Tags.Add(tag2);

                var tag3 = new Tag
                {
                    Name = "Ice cream🍦",
                };
                context.Tags.Add(tag3);

                var tag4 = new Tag
                {
                    Name = "Chicken 🍗",
                };
                context.Tags.Add(tag4);

                var tag5 = new Tag
                {
                    Name = "meat 🍖",
                };
                context.Tags.Add(tag5);

                var tag6 = new Tag
                {
                    Name = "Fruit 🍓",
                };
                context.Tags.Add(tag6);

                var tag7 = new Tag
                {
                    Name = "Soft drinks 🍹",
                };
                context.Tags.Add(tag7);

                var tag8 = new Tag
                {
                    Name = "Sandwich 🥪",
                };
                context.Tags.Add(tag8);

                var tag9 = new Tag
                {
                    Name = "Tea 🍵",
                };
                context.Tags.Add(tag9);

                var tag10 = new Tag
                {
                    Name = "Fatayer 🥟",
                };
                context.Tags.Add(tag10);

                var tag11 = new Tag
                {
                    Name = "Cup cake🧁",
                };
                context.Tags.Add(tag11);

                var tag12 = new Tag
                {
                    Name = "Doughnuts 🍩",
                };
                context.Tags.Add(tag12);

                var tag13 = new Tag
                {
                    Name = "Cake 🍰",
                };
                context.Tags.Add(tag13);

                var tag14 = new Tag
                {
                    Name = "Bavarage 🍹",
                };
                context.Tags.Add(tag14);

                await context.SaveChangesAsync();
            }

        }
    }
}
