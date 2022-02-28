using Microsoft.AspNetCore.Identity;
using Passengers.Models.Base;
using Passengers.Models.Location;
using Passengers.Models.Main;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using OrderSet = Passengers.Models.Order.Order;
namespace Passengers.Models.Security
{
    public class AppUser : IdentityUser<Guid> , IBaseEntity
    {
        #region Base
        public DateTime DateCreated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? DeletedBy { get; set; }

        #endregion

        #region Shared
        public UserTypes UserType { get; set; }

        [DefaultValue(AccountStatus.Accepted)]
        public AccountStatus AccountStatus { get; set; }
        public string FullName { get; set; }
        public DateTime? DOB { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceTokens { get; set; }
        public GenderTypes? GenderType { get; set; }
        #endregion 

        public double? Rate { get; set; }
        public string IdentifierImagePath { get; set; }
        public BloodTypes? BloodType { get; set; }

        [InverseProperty("Customer")]
        public ICollection<OrderSet> CustomerOrders { get; set; }
        [InverseProperty("Driver")]
        public ICollection<OrderSet> DriverOrders { get; set; }

        #region Shop
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }

        public Guid? AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<ShopCategory> MainCategories { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<ShopSchedule> ShopSchedules { get; set; }
        public ICollection<ShopContact> ShopContacts { get; set; }
        public ICollection<Tag> Tags { get; set; }

        #endregion
        public ICollection<Rate> Rates { get; set; }

        [InverseProperty("Customer")]
        public ICollection<Favorite> CustomerFavorites { get; set; }

        [InverseProperty("Shop")]
        public ICollection<Favorite> ShopFavorites { get; set; }
    }
}
