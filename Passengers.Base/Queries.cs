using Microsoft.EntityFrameworkCore;
using Passengers.Models.Base;
using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Base
{
    public static class Queries
    {
        public static IQueryable<AppUser> Drivers(this PassengersDbContext context)
            => context.Users(UserTypes.Driver);

        public static IQueryable<AppUser> Admins(this PassengersDbContext context)
            => context.Users(UserTypes.Admin);

        public static IQueryable<AppUser> Customers(this PassengersDbContext context)
            => context.Users(UserTypes.Customer);

        public static IQueryable<AppUser> Employees(this PassengersDbContext context)
            => context.Users(UserTypes.Stakeholder);

        public static IQueryable<AppUser> Shops(this PassengersDbContext context, AccountStatus status = AccountStatus.Accepted)
            => context.Users(UserTypes.Shop, status);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, UserTypes type = UserTypes.Admin, AccountStatus accountStatus = AccountStatus.Accepted)
            => context.Users.Where(x => x.UserType == type && x.AccountStatus == accountStatus);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, AccountStatus accountStatus = AccountStatus.Accepted)
            => context.Users.Where(x => x.AccountStatus == accountStatus);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, UserTypes type = UserTypes.Admin)
            => context.Users.Where(x => x.UserType == type);

        public static IQueryable<T> QueryNoTracking<T>(this PassengersDbContext context) where T : BaseEntity
            => context.Set<T>().AsNoTracking();
        public static IQueryable<T> QueryIgnoreFilter<T>(this PassengersDbContext context) where T : BaseEntity
            => context.Set<T>().IgnoreQueryFilters();

        public static IQueryable<T> Order<T>(this PassengersDbContext context) where T : BaseEntity
            => context.Set<T>().OrderByDescending(x => x.DateCreated);

    }
}
