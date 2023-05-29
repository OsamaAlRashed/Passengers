using Passengers.DataTransferObject.DriverDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.DriveService.Store
{
    public class DriverStore
    {

        public static class Filter
        {
        }
        public static class Query
        {
            public static Func<AppUser, GetDriverDto> DriverToDriverDto => c => new GetDriverDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Age = c.DOB is null ? null : (DateTime.Today.Year - c.DOB.Value.Year),
                FullName = c.FullName,
                GenderType = c.GenderType,
                IdentifierImagePath = c.IdentifierImagePath,
                PhoneNumber = c.PhoneNumber,
                AddressText = c.Address?.Text ?? "",
                IsBlocked = c.DateBlocked.HasValue,
                BloodType = c.BloodType,
                DOB = c.DOB,
                FixedAmount = -1 * c.Payments.Where(payment => payment.Type.IsFixed())
                    .Sum(payment => payment.Amount * payment.Type.PaymentSign()),
                Online = c.DriverOnline ?? false,
            };

            public static Action<AppUser, SetDriverDto> AssignDtoToDriver => (entity, dto) =>
            {
                entity.FullName = dto.FullName;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.DOB = dto.DOB;
                entity.GenderType = dto.GenderType;
                entity.BloodType = dto.BloodType;
            };

            public static Func<AppUser, DateTime?, DetailsDriverDto> DriverToDetailsDriverDto => (c, day) => new DetailsDriverDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Age = c.DOB is null ? null : (DateTime.Today.Year - c.DOB.Value.Year),
                FullName = c.FullName,
                GenderType = c.GenderType,
                IdentifierImagePath = c.IdentifierImagePath,
                PhoneNumber = c.PhoneNumber,
                AddressText = c.Address?.Text ?? "",
                IsBlocked = c.DateBlocked.HasValue,
                BloodType = c.BloodType,
                DOB = c.DOB,
                FixedAmount = -1 * c.Payments.Where(payment => payment.Type.IsFixed())
                    .Sum(payment => payment.Amount * payment.Type.PaymentSign()),
                DeliveryAmount = c.DriverOrders.Where(x => !day.HasValue || x.DateCreated.Date == day)
                    .Sum(x => x.DeliveryCost.GetValueOrDefault()),
                Online = c.DriverOnline ?? false,
                OnlineTime = 100,
                OrderCount = c.DriverOrders.Where(x => !day.HasValue || x.DateCreated.Date == day).Count(),
            };

            public static Func<LoginDriverDto, BaseLoginDto> DriverToBaseLoginDto => c => new BaseLoginDto
            {
                UserName = c.UserName,
                DeviceToken = c.DeviceToken,
                Password = c.Password,
                RemmberMe = false,
                UserType = UserType.Driver
            };
        }

        
    }
}
