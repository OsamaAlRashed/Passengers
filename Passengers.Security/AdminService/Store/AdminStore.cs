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
                Address = c.Address?.Text,
                UserName = c.UserName,
                Age = c.DOB is null ? null : (DateTime.Today.Year - c.DOB.Value.Year),
                FullName = c.FullName,
                GenderType = c.GenderType,
                IdentifierImagePath = c.IdentifierImagePath,
                PhoneNumber = c.PhoneNumber,
                Salary = 0,
                IsBlocked = c.DateBlocked.HasValue
            };

            public static Func<SetAdminDto, AppUser> AdminDtoToAdmin => c => new AppUser
            {
                UserName = c.UserName,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                Address = new Address
                {
                    Text = c.Address,
                    AreaId = new Guid("b85a6cd6-f4c4-4a2c-7446-08d9fac0f696"),
                },
                //Salary = 
                DOB = c.DOB,
                GenderType = c.GenderType,
            };

            public static Action<AppUser, SetAdminDto> AssignDtoToAdmin => (entity, dto) =>
            {
                entity.UserName = dto.UserName;
                entity.FullName = dto.FullName;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.Address = new Address
                {
                    Text = dto.Address,
                    AreaId = new Guid("b85a6cd6-f4c4-4a2c-7446-08d9fac0f696"),
                };
                entity.DOB = dto.DOB;
                entity.GenderType = dto.GenderType;
                entity.AccountStatus = AccountStatus.Accepted;
            };

        }
    }
}
