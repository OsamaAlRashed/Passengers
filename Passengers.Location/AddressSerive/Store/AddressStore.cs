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
                CustomerId = c.Type == SharedKernel.Enums.AddressTypes.Customer ? c.EntityId : null,
                ShopId = c.Type == SharedKernel.Enums.AddressTypes.Shop ? c.EntityId : null,
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

            public static Expression<Func<Address, AddressDto>> InverseSelectAddress => c => new AddressDto
            {
                AreaId = c.AreaId,
                EntityId = c.ShopId ?? c.CustomerId.Value,
                Type = c.ShopId != null ? SharedKernel.Enums.AddressTypes.Shop : SharedKernel.Enums.AddressTypes.Customer,
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
                AreaId = c.AreaId,
                CustomerId = c.CustomerId.Value,
                Text = c.Text,
                Lat = c.Lat,
                Long = c.Long,
            };

        }
    }
}
