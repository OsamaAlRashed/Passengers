using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.OfferDtos;
using Passengers.Main.OfferService.Store;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.Shared.DocumentService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.OfferService
{
    public class OfferRepository : BaseRepository , IOfferRepository
    {
        private readonly IDocumentRepository documentRepository;
        private readonly ICurrentUserService currentUserService;

        public OfferRepository(PassengersDbContext context, IDocumentRepository documentRepository, ICurrentUserService currentUserService): base(context)
        {
            this.documentRepository = documentRepository;
            this.currentUserService = currentUserService;
        }

        public async Task<OperationResult<GetOfferDto>> Add(SetOfferDto dto)
        {
            var entity = OfferStore.Query.SetSelectOffer.Compile()(dto);
            await documentRepository.Add(dto.ImageFile, currentUserService.UserId.Value, DocumentEntityTypes.Offer);
            entity.ShopId = currentUserService.UserId.Value;
            Context.Offers.Add(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(OfferStore.Query.GetSelectOffer.Compile()(entity));
        }

        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var offer = await Context.Offers.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (offer == null)
                return (OperationResultTypes.NotExist, "");

            Context.Offers.Remove(offer);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<GetOfferDto>>> Get(OfferTypes type, int pageSize, int pageNumber)
        {
            var result = await Context.Offers.Where(OfferStore.Filter.WhereType(type))
                //.Pagnation(pageSize, pageNumber)
                .Select(x => OfferStore.Query.GetSelectOffer.Compile()(x))
                .ToListAsync();
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<GetOfferDto>> GetById(Guid id)
        {
            var offer = await Context.Offers.Include(x => x.OrderDetails).Where(x => x.Id == id).SingleOrDefaultAsync();
            if (offer == null)
                return (OperationResultTypes.NotExist, "");
            return _Operation.SetSuccess(OfferStore.Query.GetSelectOffer.Compile()(offer));
        }

        public async Task<OperationResult<GetOfferDto>> Update(SetOfferDto dto)
        {
            var offer = await Context.Offers.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (offer == null)
                return (OperationResultTypes.NotExist, "");

            offer = OfferStore.Query.SetSelectOffer.Compile()(dto);
            if(dto.ImageFile != null)
                await documentRepository.Update(dto.ImageFile, offer.Id, DocumentEntityTypes.Offer);

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(OfferStore.Query.GetSelectOffer.Compile()(offer));
        }

        public async Task<OperationResult<bool>> Extension(Guid id, DateTime? endDate)
        {
            var offer = await Context.Offers.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (offer == null)
                return (OperationResultTypes.NotExist, "");
            offer.EndDate = endDate ?? DateTime.Now;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }
    }
}
