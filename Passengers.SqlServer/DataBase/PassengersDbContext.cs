using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Passengers.Models.Base;
using Passengers.Models.Location;
using Passengers.Models.Main;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SharedKernel.Services.LangService;
using Passengers.SharedKernel.Services.LangService.Contant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Passengers.SqlServer.DataBase
{
    public class PassengersDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        public string CurrentLang { get; }
        public Guid? CurrentUserId { get; }

        public PassengersDbContext(DbContextOptions<PassengersDbContext> options,
            ICurrentUserService currentUserService, ILangService langService) : base(options)
        {
            _currentUserService = currentUserService;
            CurrentLang = langService?.CurrentLang ?? LangConstant.En;
            CurrentUserId = currentUserService.UserId;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var entities = builder.Model
                .GetEntityTypes()
                .Where(e => e.ClrType.GetInterface(typeof(IBaseEntity).Name) != null)
                .Select(e => e.ClrType);

            foreach (var entity in entities)
            {
                builder.Entity(entity).HasIndex(nameof(IBaseEntity.DateCreated));
                Expression<Func<IBaseEntity, bool>> expression = b => !b.DateDeleted.HasValue;
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                builder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }

            builder.Entity<Tag>().HasQueryFilter(x => !x.DateDeleted.HasValue && !x.IsHidden);
            builder.Entity<Product>().HasQueryFilter(x => !x.DateDeleted.HasValue && !x.Tag.IsHidden);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                IBaseEntity entity = entry.Entity.AsTo<IBaseEntity>();
                if (entity is null) continue;

                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entity.DeletedBy = _currentUserService.UserId;
                        break;
                    case EntityState.Modified:
                        entity.UpdatedBy = _currentUserService.UserId;
                        entity.DateUpdated = DateTime.Now.ToLocalTime();
                        break;
                    case EntityState.Added:
                        entity.CreatedBy = _currentUserService.UserId;
                        entity.DateCreated = DateTime.Now.ToLocalTime();
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Favorite> Favorites { get; set; }


        public DbSet<Discount> Discounts { get; set; }
        public DbSet<PriceLog> PriceLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<ShopCategory> ShopCategories { get; set; }
        public DbSet<ShopContact> ShopContacts { get; set; }
        public DbSet<ShopSchedule> ShopSchedules { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
