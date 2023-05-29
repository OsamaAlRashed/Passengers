using Passengers.DataTransferObject.LocationDtos;
using Passengers.Models.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.AddressSerive.Store
{
    public static class AddressStore
    {
        public static class Filter
        {
        }

        public static class Query
        {
            public static Expression<Func<AddressDto, Address>> GetSelectAddress => c => new Address
            {
                AreaId = c.AreaId,
                CustomerId = c.Type == SharedKernel.Enums.AddressType.Customer ? c.EntityId : null,
                ShopId = c.Type == SharedKernel.Enums.AddressType.Shop ? c.EntityId : null,
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

            public static Expression<Func<Address, AddressDto>> InverseSelectAddress => c => new AddressDto
            {
                AreaId = c.AreaId,
                EntityId = c.ShopId ?? c.CustomerId.Value,
                Type = c.ShopId != null ? SharedKernel.Enums.AddressType.Shop : SharedKernel.Enums.AddressType.Customer,
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

            public static Expression<Func<Address, ShopAddressDto>> SelectShopAddressDto => c => new ShopAddressDto
            {
                AreaId = c.AreaId,
                ShopId = c.ShopId.Value,
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

            public static Expression<Func<Address, CustomerAddressDto>> SelectCustomerAddressDto => c => new CustomerAddressDto
            {
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
                Building = c.Building,
                Note = c.Note,
                PhoneNumber = c.PhoneNumber,
                Title = c.Title,
                Id = c.Id,
                IsCurrentLocation = c.IsCurrentLocation
            };

            public static Expression<Func<CustomerAddressDto, Address>> InverseSelectCustomerAddressDto => c => new Address
            {
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
                Building = c.Building,
                Note = c.Note,
                PhoneNumber = c.PhoneNumber,
                Title = c.Title,
                IsActive = true,
                IsCurrentLocation = false
            };

            public static Expression<Func<ShopAddressDto, Address>> InverseSelectShopAddressDto => c => new Address
            {
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

            public static Action<Address, CustomerAddressDto> AssignCustomerAddressDtoToAddress => (entity, dto) =>
            {
                entity.Lat = dto.Lat;
                entity.Long = dto.Long;
                entity.Note = dto.Note;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.Title = dto.Title;
                entity.Building = dto.Title;
                entity.Text = dto.Text;
                entity.IsCurrentLocation = false;
            };

            public static Action<Address, ShopAddressDto> AssignShopAddressDtoToAddress => (entity, dto) =>
            {
                entity.Lat = dto.Lat;
                entity.Long = dto.Long;
                entity.Text = dto.Text;
            };

        }
    }
}
