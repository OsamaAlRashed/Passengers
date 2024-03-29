﻿using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.GeneralDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Main.ProductService.Store;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.Shared.DocumentService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Pagnation;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.ProductService
{
    public class ProductRepository : BaseRepository , IProductRepository
    {
        private readonly IDocumentRepository documentRepository;

        public ProductRepository(PassengersDbContext context, IDocumentRepository documentRepository): base(context)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<OperationResult<GetProductDto>> Add(SetProductDto dto)
        {
            var tag = await Context.Tags.Where(x => x.Id == dto.TagId).SingleOrDefaultAsync();
            if(tag == null)
                return _Operation.SetContent<GetProductDto>(OperationResultTypes.NotExist, "TagNotFound");
            if (tag.ShopId == null)
                return _Operation.SetFailed<GetProductDto>("TagNotAllowed");
            var product = new Product
            {
                Avilable = true,
                Name = dto.Name,
                PrepareTime = dto.PrepareTime,
                TagId = dto.TagId,
                Description = dto.Description,
                PriceLogs = new List<PriceLog>() {
                    new PriceLog(){ Price = dto.Price } 
                }
            };

            Context.Products.Add(product);
            await Context.SaveChangesAsync();
            await documentRepository.Add(dto.ImageFile, product.Id, DocumentEntityType.Product);
            return _Operation.SetSuccess(new GetProductDto
            {
                Id = product.Id,
                Avilable = product.Avilable,
                Description = product.Description,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                PrepareTime = product.PrepareTime,
                Name = product.Name,
                Price = product.Price(),
                TagId = product.TagId
            });
        }

        public async Task<OperationResult<bool>> ChangeAvilable(Guid id)
        {
            var product = await Context.Products.Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (product == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "ProductNotExist");
            product.Avilable = !product.Avilable;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice)
        {
            var product = await Context.Products.Include(x => x.PriceLogs).Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (product == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "ProductNotExist");

            var oldPrice = product.PriceLogs.Where(x => !x.EndDate.HasValue).FirstOrDefault();
            if (oldPrice != null)
                oldPrice.EndDate = DateTime.UtcNow;
            product.PriceLogs.Add(new PriceLog
            {
                Price = newPrice
            });

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<object>> GetById(Guid id)
        {
            var product = await Context.Products
                .Include(x => x.Tag)
                .Include(x => x.Discounts)
                .Include(x => x.Reviews)
                .ThenInclude(x=> x.Customer)
                .Include(x => x.Documents)
                .Include(x => x.PriceLogs)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (product == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "ProductNotFound");
            
            var discount = GetCurrentDiscount(product.Discounts);

            var result = new
            {
                product.Id,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                product.TagId,
                TagName = product.Tag.Name,
                product.Name,
                Price = product.Price(),
                HasDiscount = discount != null,
                DiscountPrice = discount?.Price,
                product.Avilable,
                product.PrepareTime,
                RateDegree = product.Rate(),
                RateNumber = product.Reviews.Count,
                DiscountStartDate = discount?.StartDate,
                DiscountEndDate = discount?.EndDate,
                product.Description,
                Reviews = product.Reviews.Select(r => new
                {
                    r.Id,
                    r.Rate,
                    r.Descreption,
                    CustomerName = r.Customer.Name,
                    r.CustomerId,
                    Date = r.DateCreated
                }).OrderByDescending(x => x.Date).Take(3).ToList(),
            };
            return _Operation.SetSuccess<object>(result);
        }

        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var product = await Context.Products.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (product == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "ProductNotExist");

            product.DateDeleted = DateTime.UtcNow;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<GetProductDto>> Update(SetProductDto dto)
        {
            var product = await Context.Products
                .Include(x => x.PriceLogs)
                .Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (product == null)
                return _Operation.SetContent<GetProductDto>(OperationResultTypes.NotExist, "ProductNotExist");

            product.Avilable = dto.Avilable;
            product.Name = dto.Name;
            product.PrepareTime = dto.PrepareTime;
            product.TagId = dto.TagId;
            product.Description = dto.Description;

            var oldPrice = product.PriceLogs.Where(x => !x.EndDate.HasValue).FirstOrDefault();
            if (oldPrice != null)
                oldPrice.EndDate = DateTime.UtcNow;
            product.PriceLogs.Add(new PriceLog
            {
                Price = dto.Price
            });
            
            await Context.SaveChangesAsync();

            if(dto.ImageFile != null)
                await documentRepository.Update(dto.ImageFile, product.Id, DocumentEntityType.Product);

            return _Operation.SetSuccess(new GetProductDto
            {
                Id = product.Id,
                Avilable = product.Avilable,
                Description = product.Description,
                ImagePath = product.ImagePath(),
                PrepareTime = product.PrepareTime,
                Name = product.Name,
                Price = product.Price(),
                TagId = product.TagId
            });
        }

        private static Discount GetCurrentDiscount(ICollection<Discount> discounts)
        {
            return discounts.Where(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).FirstOrDefault();
        }

        public async Task<OperationResult<PagedList<GetProductDto>>> Get(ProductFilterDto filterDto, SortProductType? sortType, bool? isDes, int pageNumber = 1, int pageSize = 10)
        {
            var products = await Context.Products
                .Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.Reviews)
                .Include(x => x.PriceLogs)
                .Where(x => x.Tag.ShopId == Context.CurrentUserId)
                .Where(ProductStore.Filter.WhereFilter(filterDto))
                .SortBy(ProductStore.Query.Sort(sortType), isDes)
                .Select(ProductStore.Query.GetSelectProduct)
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(products);
        }

    }
}
