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
        Task<OperationResult<AddressDto>> Add(AddressDto dto);
        Task<OperationResult<AddressDto>> UpdateShopAddress(AddressDto dto);
        Task<OperationResult<bool>> Remove(Guid id);
        Task<OperationResult<bool>> RemoveByUserId(Guid id);
        Task<OperationResult<AddressDto>> GetById(Guid id);
        Task<OperationResult<List<CustomerAddressDto>>> GetByCustomerId(Guid id);
        Task<OperationResult<ShopAddressDto>> GetByShopId(Guid id);
    }
}
