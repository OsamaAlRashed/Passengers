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
        public AppUser()
        {
            DriverOrders = new HashSet<OrderSet>();
            Documents = new HashSet<Document>();
            ShopSchedules = new HashSet<ShopSchedule>();
            Tags = new HashSet<Tag>();
            Offers = new HashSet<Offer>();
            Reviews = new HashSet<Review>();
            CustomerFavorites = new HashSet<Favorite>();
            ShopFavorites = new HashSet<Favorite>();
            Addresses = new HashSet<Address>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

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
        public string DeviceTokens { get; set; }
        public GenderTypes? GenderType { get; set; }
        public DateTime? DateBlocked { get; set; }
        public decimal Salary { get; set; }
        public string AddressText { get; set; }
        #endregion 

        public double? Rate { get; set; }
        public string IdentifierImagePath { get; set; }
        public BloodTypes? BloodType { get; set; }

        public ICollection<OrderSet> DriverOrders { get; set; }

        #region Shop
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }
        public Address Address { get; set; }
        public bool? OrderStatus { get; set; }
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        
        public bool? DriverOnline { get; set; }

        public ICollection<Document> Documents { get; set; }
        public ICollection<ShopSchedule> ShopSchedules { get; set; }
        public ICollection<ShopContact> ShopContacts { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Offer> Offers { get; set; }

        #endregion
        public ICollection<Review> Reviews { get; set; }

        [InverseProperty("Customer")]
        public ICollection<Favorite> CustomerFavorites { get; set; }

        [InverseProperty("Shop")]
        public ICollection<Favorite> ShopFavorites { get; set; }

        [InverseProperty("Customer")]
        public ICollection<Address> Addresses { get; set; }

        public ICollection<Payment> Payments { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
