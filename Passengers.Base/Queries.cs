using Microsoft.EntityFrameworkCore;
using Passengers.Models.Base;
using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Pagnation;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Base
{
    public static class Queries
    {
        public static IQueryable<AppUser> Drivers(this PassengersDbContext context)
            => context.Users(UserType.Driver);

        public static IQueryable<AppUser> Admins(this PassengersDbContext context)
            => context.Users(UserType.Admin);

        public static IQueryable<AppUser> Customers(this PassengersDbContext context)
            => context.Users(UserType.Customer);

        public static IQueryable<AppUser> Shops(this PassengersDbContext context, AccountStatus status = AccountStatus.Accepted)
            => context.Users(UserType.Shop, status);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, UserType type = UserType.Admin, AccountStatus accountStatus = AccountStatus.Accepted)
            => context.Users.Where(x => x.UserType == type && x.AccountStatus == accountStatus);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, AccountStatus accountStatus = AccountStatus.Accepted)
            => context.Users.Where(x => x.AccountStatus == accountStatus);

        public static IQueryable<AppUser> Users(this PassengersDbContext context, UserType type = UserType.Admin)
            => context.Users.Where(x => x.UserType == type);

        public static IQueryable<T> QueryNoTracking<T>(this PassengersDbContext context) where T : BaseEntity
            => context.Set<T>().AsNoTracking();
        public static IQueryable<T> QueryIgnoreFilter<T>(this PassengersDbContext context) where T : BaseEntity
            => context.Set<T>().IgnoreQueryFilters();

        public static IQueryable<TSource> SortBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> by, bool? isDes) where TSource : IBaseEntity
            => (!isDes.HasValue || isDes.Value) ? source.OrderByDescending(by)
                                                : source.OrderBy(by);
    }
}
