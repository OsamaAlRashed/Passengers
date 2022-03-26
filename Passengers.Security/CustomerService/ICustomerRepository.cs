using Microsoft.AspNetCore.Http;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.Pagnation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.CustomerService
{
    public interface ICustomerRepository
    {
        Task<OperationResult<object>> Login(LoginCustomerDto dto);
        Task<OperationResult<CreateAccountCustomerDto>> SignUp(CreateAccountCustomerDto dto);
        Task<OperationResult<string>> UploadImage(IFormFile file);
        Task<OperationResult<CustomerInformationDto>> Details();
        Task<OperationResult<bool>> UpdateInformation(CustomerInformationDto dto);
        Task<OperationResult<PagedList<GetProductDto>>> GetMyFavorite(int pageNumber = 1, int pageSize = 10);
        Task<OperationResult<bool>> UnFavorite(Guid productId);
        Task<OperationResult<bool>> UnFollow(Guid shopId);
        Task<OperationResult<bool>> Favorite(Guid productId);
        Task<OperationResult<bool>> Follow(Guid shopId);
        Task<OperationResult<PagedList<GetProductDto>>> GetProducts(CustomerProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10);
        Task<OperationResult<PagedList<ShopCustomerDto>>> GetShops(CustomerShopFilterDto filterDto, bool? topShop, int pageNumber = 1, int pageSize = 10);
    }
}
