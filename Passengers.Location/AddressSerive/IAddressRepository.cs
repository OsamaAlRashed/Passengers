using Passengers.DataTransferObject.LocationDtos;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.AddressSerive
{
    public interface IAddressRepository
    {
        Task<OperationResult<CustomerAddressDto>> Add(CustomerAddressDto dto);
        Task<OperationResult<ShopAddressDto>> Add(ShopAddressDto dto);
        Task<OperationResult<ShopAddressDto>> Update(ShopAddressDto dto);
        Task<OperationResult<CustomerAddressDto>> Update(CustomerAddressDto dto);
        Task<OperationResult<bool>> Remove(Guid id);
        Task<OperationResult<bool>> RemoveByUserId(Guid id);
        Task<OperationResult<CustomerAddressDto>> GetById(Guid id);
        Task<OperationResult<List<CustomerAddressDto>>> GetByCustomerId(Guid? id);
        Task<OperationResult<ShopAddressDto>> GetByShopId(Guid id);
    }
}
