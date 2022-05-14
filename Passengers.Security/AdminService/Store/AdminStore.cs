using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Models.Location;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.AdminService.Store
{
    public class AdminStore
    {
        public static class Filter
        {
        }
        public static class Query
        {
            public static Func<AppUser, GetAdminDto> AdminToAdminDto => c => new GetAdminDto
            {
                Id = c.Id,
                UserName = c.UserName,
                Age = c.DOB is null ? null : (DateTime.Today.Year - c.DOB.Value.Year),
                FullName = c.FullName,
                GenderType = c.GenderType,
                IdentifierImagePath = c.IdentifierImagePath,
                PhoneNumber = c.PhoneNumber,
                Salary = c.Salary,
                AddressText = c.AddressText,
                IsBlocked = c.DateBlocked.HasValue,
                DOB = c.DOB
            };

            public static Func<SetAdminDto, AppUser> AdminDtoToAdmin => c => new AppUser
            {
                UserName = c.UserName,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                AddressText = c.AddressText,
                Salary = c.Salary,
                DOB = c.DOB,
                GenderType = c.GenderType,
            };

            public static Action<AppUser, SetAdminDto> AssignDtoToAdmin => (entity, dto) =>
            {
                entity.FullName = dto.FullName;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.DOB = dto.DOB;
                entity.GenderType = dto.GenderType;
                entity.Salary = dto.Salary;
                entity.AddressText = dto.AddressText;
            };

        }
    }
}
