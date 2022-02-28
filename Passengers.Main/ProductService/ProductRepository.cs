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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.ProductService
{
    public class ProductRepository : BaseRepository , IProductRepository
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IDocumentRepository documentRepository;

        public ProductRepository(PassengersDbContext context,ICurrentUserService currentUserService, IDocumentRepository documentRepository): base(context)
        {
            this.currentUserService = currentUserService;
            this.documentRepository = documentRepository;
        }

        public async Task<OperationResult<GetProductDto>> Add(SetProductDto dto)
        {
            var product = new Product
            {
                Avilable = true,
                Name = dto.Name,
                PrepareTime = dto.PrepareTime,
                ProductType = ProductTypes.Product,
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
            var product = await Context.Products().Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            if (product == null)
                return (OperationResultTypes.NotExist, "");
            product.Avilable = !product.Avilable;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice)
        {
            var product = await Context.Products().Where(x => x.Id == id)
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
            var product = await Context.Products()
                .Include(x => x.Tag)
                .Include(x => x.Discounts)
                .Include(x => x.Rates)
                .ThenInclude(x=> x.Customer)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            
            var discount = GetCurrentDiscount(product.Discounts);

            var result = new
            {
                TagId = product.TagId,
                TagName = product.Tag.Name,
                Name = product.Name,
                Price = product.Price,
                DiscountPrice = discount?.Price,
                Avilable = product.Avilable,
                PrepareTime = product.PrepareTime,
                RateDegree = product.Rates.Average(x => x.Degree),
                RateNumber = product.Rates.Count(),
                DiscountStartDate = discount?.StartDate,
                DiscountEndDate = discount?.EndDate ?? DateTime.MaxValue,
                Description = product.Description,
                Rates = product.Rates.Select(r => new
                {
                    Id = r.Id,
                    Degree = r.Degree,
                    Descreption = r.Descreption,
                    CustomerName = r.Customer.Name,
                    CustomerId = r.CustomerId,
                    Date = r.DateCreated
                }).OrderByDescending(x => x.Date).Take(3).ToList(),
            };
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<object>> GetFoodMenu(int pageSize = 10, int pageNumber = 1)
        {
            var tags = await Context.Tags.Include(x => x.Products)
                .Where(x => x.ShopId == currentUserService.UserId)
                .ToListAsync();
            if (tags.Count == 0)
                return _Operation.SetFailed<object>("", OperationResultTypes.NotExist);

            var result = tags.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                LogoPath = x.LogoPath,

                Products = x.Products.Select(xx => new
                {
                    Id = xx.Id,
                    Name = xx.Name,
                    Avilable = xx.Avilable,
                    Description = xx.Description,
                    ImagePath = xx.Documents?.Select(x => x.Path).FirstOrDefault(),
                    PrepareTime = xx.PrepareTime,
                    Price = xx.Price,
                    TagId = xx.TagId,
                    DiscountPrice = GetCurrentDiscount(xx.Discounts)
                }).ToList()
            });

            return _Operation.SetSuccess(result);
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
            product.ProductType = ProductTypes.Product;
            product.Price = dto.Price;
            product.TagId = dto.TagId;

            await Context.SaveChangesAsync();

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

        private Discount GetCurrentDiscount(ICollection<Discount> discounts)
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
