using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.Location.AddressSerive.Store;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.AddressSerive
{
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        public AddressRepository(PassengersDbContext context):base(context)
        {

        }

        public async Task<OperationResult<AddressDto>> Add(AddressDto dto)
        {
            dto.AreaId = Context.Areas.FirstOrDefault().Id;
            var entity = AddressStore.Query.GetSelectAddress.Compile()(dto);
            Context.Addresses.Add(entity);
            await Context.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<OperationResult<List<CustomerAddressDto>>> GetByCustomerId(Guid id)
        {
            var result = await Context.Addresses
                .Where(x => x.CustomerId == id).Select(AddressStore.Query.SelectCustomerAddressDto)
                .ToListAsync();
            return result;
        }

        public async Task<OperationResult<AddressDto>> GetById(Guid id)
        {
            var result = await Context.Addresses
                .Where(x => x.ShopId == id).Select(AddressStore.Query.InverseSelectAddress)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<OperationResult<ShopAddressDto>> GetByShopId(Guid id)
        {
            var result = await Context.Addresses
                .Where(x => x.ShopId == id).Select(AddressStore.Query.SelectShopAddressDto)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var entity = await Context.Addresses.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                return (OperationResultTypes.NotExist, "");

            entity.DateDeleted = DateTime.Now;
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<OperationResult<bool>> RemoveByUserId(Guid userId)
        {
            var entities = await Context.Addresses.Where(x => x.CustomerId == userId).ToListAsync();
            foreach (var entity in entities)
            {
                entity.DateDeleted = DateTime.Now;
            }
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<OperationResult<AddressDto>> UpdateShopAddress(AddressDto dto)
        {
            var entity = await Context.Addresses.Where(x => x.ShopId == dto.EntityId).FirstOrDefaultAsync();
            if (entity == null)
                return (OperationResultTypes.NotExist, "");
            entity = AddressStore.Query.GetSelectAddress.Compile()(dto);
            await Context.SaveChangesAsync();
            return dto;
        }
    }
}
