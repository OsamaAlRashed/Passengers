using Passengers.DataTransferObject.OfferDtos;
using Passengers.Models.Main;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.OfferService.Store
{
    public static class OfferStore
    {
        public static class Query
        {
            public static Expression<Func<Offer, GetOfferDto>> GetSelectOffer => c => new GetOfferDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImagePath = c.Documents.Select(x => x.Path).FirstOrDefault(),
                PrepareTime = c.PrepareTime,
                Price = c.Price,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                BuyerCount = c.OrderDetails.Sum(x => x.Quantity)
            };

            public static Expression<Func<SetOfferDto, Offer>> SetSelectOffer => c => new Offer
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PrepareTime = c.PrepareTime,
                Price = c.Price,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            };

            public static Action<Offer, SetOfferDto> AssignDtoToOffer => (entity, dto) =>
            {
                entity.Id = dto.Id;
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.PrepareTime = dto.PrepareTime;
                entity.Price = dto.Price;
                entity.StartDate = dto.StartDate;
                entity.EndDate = dto.EndDate;
            };

        }

        public static class Filter
        {
            public static Expression<Func<Offer, bool>> WhereType(OfferTypes type) => (offer)
                => type == OfferTypes.Active ? offer.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= offer.EndDate
                                             : (type == OfferTypes.Pending ? offer.StartDate > DateTime.UtcNow
                                                                          : (type == OfferTypes.Archive ? offer.EndDate < DateTime.UtcNow
                                                                                                        : true));
        }
    }
}
