﻿using Passengers.Base;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.CustomerService.Store
{
    public static class CustomerStore
    {
        public static class Filter
        {
            public static Expression<Func<Product, bool>> WhereFilterProduct(CustomerProductFilterDto filter) => product =>
                product.Tag.ShopId.HasValue
             && (!filter.ShopId.HasValue || filter.ShopId == product.Tag.ShopId)
             && (!filter.TagId.HasValue || filter.TagId == product.TagId)
             && (string.IsNullOrEmpty(filter.Search) || product.Name.Contains(filter.Search))
             && (!filter.FromPrice.HasValue || filter.FromPrice <= product.Price())
             && (!filter.ToPrice.HasValue || filter.ToPrice >= product.Price())
             && (!filter.Rate.HasValue || ((int)(product.Reviews.Any() ? Math.Ceiling(product.Reviews.Average(x => x.Rate)) : 1) == filter.Rate))
             && ((!filter.WithDiscount.HasValue) ||
                (filter.WithDiscount.Value ? product.Discounts.Where(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Any()
                                       : !product.Discounts.Where(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Any()));

            public static Expression<Func<AppUser, bool>> WhereFilterShop(CustomerShopFilterDto filter, List<Guid> shopIds) => shop =>
                (string.IsNullOrEmpty(filter.Search) || shop.Name.Contains(filter.Search))
             && (shopIds == null || !shopIds.Any() | shopIds.Contains(shop.Id))
             && (!filter.CategoryId.HasValue || shop.CategoryId == filter.CategoryId)
             /// TODo && (filter.NearWithMe.HasValue) 
             && (string.IsNullOrEmpty(filter.FromTime) || shop.ShopSchedules.Any(x => TimeSpan.Parse(filter.FromTime) <= x.FromTime))
             && (string.IsNullOrEmpty(filter.ToTime) || shop.ShopSchedules.Any(x => TimeSpan.Parse(filter.ToTime) >= x.ToTime));
        }

        public static class Query
        {
            public static Func<LoginCustomerDto, BaseLoginDto> CustomerToBaseLoginDto => c => new BaseLoginDto
            {
                UserName = c.PhoneNumber,
                DeviceToken = c.DeviceToken,
                Password = c.Password,
                RemmberMe = false,
                UserType = UserType.Customer
            };

            public static Expression<Func<Product, GetProductDto>> GetSelectProduct(Guid? customerId = null) => c => new GetProductDto
            {
                Id = c.Id,
                Name = c.Name,
                Avilable = c.Avilable,
                Description = c.Description,
                ImagePath = c.ImagePath(),
                PrepareTime = c.PrepareTime,
                Price = c.Price(),
                TagId = c.TagId,
                HasDiscount = c.Discounts.Any(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate),
                Discount = c.Discounts.Any(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate) ? c.Discounts.FirstOrDefault(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Price : null,
                IsNew = DateTime.UtcNow.Day - c.DateCreated.Day <= 2,
                Rate = c.Reviews.Any() ? c.Reviews.Average(x => x.Rate) : 0,
                IsFavorite = customerId.HasValue ? c.Favorites.Any(x => x.CustomerId == customerId) : false,
            };

            public static Expression<Func<AppUser, ShopCustomerDto>> ShopToShopCustomerDto => c => new ShopCustomerDto
            {
                Id = c.Id,
                CategoryName = c.Category.Name,
                ImagePath = c.Documents.Select(x => x.Path).FirstOrDefault(),
                Name = c.Name,
                Rate = c.Tags.SelectMany(x => x.Products.SelectMany(x => x.Reviews)).Any() ? c.Tags.SelectMany(x => x.Products.SelectMany(x => x.Reviews)).Average(x => x.Rate) : 0,
                Online = c.ShopSchedules.Any(x => x.Days.Contains(DateTime.UtcNow.Day.ToString()) && x.FromTime <= DateTime.UtcNow.TimeOfDay && DateTime.UtcNow.TimeOfDay <= x.ToTime)
            };

            public static Expression<Func<AppUser, object>> SortShop(bool? topShops)
            {
                if (topShops == null || !topShops.Value)
                    return x => x.ShopSchedules.Any(x => x.Days.Contains(DateTime.UtcNow.Day.ToString()) && x.FromTime <= DateTime.UtcNow.TimeOfDay && DateTime.UtcNow.TimeOfDay <= x.ToTime);
                return x => x.Tags.SelectMany(x => x.Products.SelectMany(x => x.Reviews)).Average(x => x.Rate);
            }

            //Home
            public static Expression<Func<AppUser, ShopDto>> ShopToShopDto => c => new ShopDto
            {
                Id = c.Id,
                ImagePath = c.Documents.Select(x => x.Path).FirstOrDefault(),
                Name = c.Name,
                Online = c.ShopSchedules.Any(x => x.Days.Contains(DateTime.UtcNow.Day.ToString()) && x.FromTime <= DateTime.UtcNow.TimeOfDay && DateTime.UtcNow.TimeOfDay <= x.ToTime)
            };

            public static Func<Product, ProductDto> ProductToProductDto => c => new ProductDto
            {
                Id = c.Id,
                Name = c.Name,
                Avilable = c.Avilable,
                ImagePath = c.ImagePath(),
                Price = c.Price(),
                Rate = c.Reviews.Any() ? c.Reviews.Average(x => x.Rate) : 0,
                ShopId = c.Tag.ShopId.Value,
                ShopOnline = c.Tag.Shop.ShopSchedules.Any(x => x.Days.Contains(DateTime.UtcNow.Day.ToString()) && x.FromTime <= DateTime.UtcNow.TimeOfDay && DateTime.UtcNow.TimeOfDay <= x.ToTime),
                ShopImagePath = c.Tag.Shop.Documents.Select(x => x.Path).FirstOrDefault(),
            };
        }
    }
}
