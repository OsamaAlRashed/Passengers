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
        public AddressRepository(PassengersDbContext context) : base(context) { }

        public async Task<OperationResult<CustomerAddressDto>> Add(CustomerAddressDto dto)
        {
            var entity = AddressStore.Query.InverseSelectCustomerAddressDto.Compile()(dto);
            entity.CustomerId = Context.CurrentUserId;
            entity.AreaId = Context.Areas.FirstOrDefault().Id;
            Context.Addresses.Add(entity);
            await Context.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<OperationResult<List<CustomerAddressDto>>> GetByCustomerId(Guid? id)
        {
            var result = await Context.Addresses
                .Where(x => x.IsActive && (id.HasValue ? x.CustomerId == id : x.CustomerId == Context.CurrentUserId))
                .Select(AddressStore.Query.SelectCustomerAddressDto)
                .ToListAsync();
            return result;
        }

        public async Task<OperationResult<CustomerAddressDto>> GetById(Guid id)
        {
            var result = await Context.Addresses
                .Where(x => x.Id == id).Select(AddressStore.Query.SelectCustomerAddressDto)
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

        public async Task<OperationResult<ShopAddressDto>> Update(ShopAddressDto dto)
        {
            var entity = await Context.Addresses.Where(x => x.ShopId == dto.ShopId).FirstOrDefaultAsync();
            if (entity == null)
                return (OperationResultTypes.NotExist, "");
            AddressStore.Query.AssignShopAddressDtoToAddress(entity, dto);
            await Context.SaveChangesAsync();
            return dto;
        }

        public async Task<OperationResult<CustomerAddressDto>> Update(CustomerAddressDto dto)
        {
            var entity = await Context.Addresses.Where(x => x.Id == dto.Id).FirstOrDefaultAsync();
            if (entity == null)
                return (OperationResultTypes.NotExist, "");
            entity.IsActive = false;
            dto.Id = Guid.Empty;
            return await Add(dto);
        }

        public async Task<OperationResult<ShopAddressDto>> Add(ShopAddressDto dto)
        {
            var entity = AddressStore.Query.InverseSelectShopAddressDto.Compile()(dto);
            entity.ShopId = Context.CurrentUserId;
            entity.AreaId = Context.Areas.FirstOrDefault().Id;
            Context.Addresses.Add(entity);
            await Context.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }
    }
}
