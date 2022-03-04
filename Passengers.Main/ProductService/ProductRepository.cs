using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.Shared.DocumentService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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
            var product = new Product
            {
                Avilable = true,
                Name = dto.Name,
                PrepareTime = dto.PrepareTime,
                Price = dto.Price,
                TagId = dto.TagId,
            };

            Context.Products.Add(product);
            await Context.SaveChangesAsync();
            await documentRepository.Add(dto.ImageFile, product.Id, DocumentEntityTypes.Product);
            return _Operation.SetSuccess(new GetProductDto
            {
                Id = product.Id,
                Avilable = product.Avilable,
                Description = product.Description,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                PrepareTime = product.PrepareTime,
                Name = product.Name,
                Price = product.Price,
                TagId = product.TagId
            });
        }

        public async Task<OperationResult<bool>> ChangeAvilable(Guid id)
        {
            var product = await Context.Products.Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (product == null)
                return (OperationResultTypes.NotExist, "");
            product.Avilable = !product.Avilable;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice)
        {
            var product = await Context.Products.Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (product == null)
                return (OperationResultTypes.NotExist, "ProductNotFound");
            product.Price = newPrice;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public Task<OperationResult<List<GetProductDto>>> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<object>> GetById(Guid id)
        {
            var product = await Context.Products
                .Include(x => x.Tag)
                .Include(x => x.Discounts)
                .Include(x => x.Rates)
                .ThenInclude(x=> x.Customer)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            
            var discount = GetCurrentDiscount(product.Discounts);

            var result = new
            {
                product.TagId,
                TagName = product.Tag.Name,
                product.Name,
                product.Price,
                DiscountPrice = discount?.Price,
                product.Avilable,
                product.PrepareTime,
                RateDegree = product.Rates.Average(x => x.Degree),
                RateNumber = product.Rates.Count,
                DiscountStartDate = discount?.StartDate,
                DiscountEndDate = discount?.EndDate ?? DateTime.MaxValue,
                product.Description,
                Rates = product.Rates.Select(r => new
                {
                    r.Id,
                    r.Degree,
                    r.Descreption,
                    CustomerName = r.Customer.Name,
                    r.CustomerId,
                    Date = r.DateCreated
                }).OrderByDescending(x => x.Date).Take(3).ToList(),
            };
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<object>> GetFoodMenu(Guid tagId, int pageSize = 10, int pageNumber = 1)
        {
            var products = await Context.Products
                .Where(x => x.TagId == tagId)
                .Pagnation(pageSize, pageNumber)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Avilable,
                    x.Description,
                    ImagePath = x.Documents.Select(x => x.Path).FirstOrDefault(),
                    x.PrepareTime,
                    x.Price,
                    x.TagId,
                    DiscountPrice = GetCurrentDiscount(x.Discounts)
                }).ToListAsync();

            return _Operation.SetSuccess(products);
        }

        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var product = await Context.Products.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (product == null)
                return (OperationResultTypes.NotExist, "");

            Context.Products.Remove(product);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<GetProductDto>> Update(SetProductDto dto)
        {
            var product = await Context.Products.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (product == null)
                return (OperationResultTypes.NotExist, "");

            product.Avilable = dto.Avilable;
            product.Name = dto.Name;
            product.PrepareTime = dto.PrepareTime;
            product.Price = dto.Price;
            product.TagId = dto.TagId;

            await Context.SaveChangesAsync();

            if(dto.ImageFile != null)
                await documentRepository.Update(dto.ImageFile, product.Id, DocumentEntityTypes.Product);

            return _Operation.SetSuccess(new GetProductDto
            {
                Id = product.Id,
                Avilable = product.Avilable,
                Description = product.Description,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                PrepareTime = product.PrepareTime,
                Name = product.Name,
                Price = product.Price,
                TagId = product.TagId
            });
        }

        private static Discount GetCurrentDiscount(ICollection<Discount> discounts)
        {
            return discounts.Where(x => x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate).FirstOrDefault();
        }

        //public async Task<OperationResult<object>> FilterProduct(FilterFoodMenuDto dto)
        //{
        //    await Context.Products()
        //        .Where(x => dto.Rates.Any(r => x.Rates.Select(r => r.Degree).Contains((int)Math.Round(x.Rates.Average(xx => xx.Degree)))))
        //        .Where(x => dto.TagIds.Any(tagId => tagId == x.TagId))
        //}
    }
}
