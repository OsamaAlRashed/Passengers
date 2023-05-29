using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.SharedDtos.DocumentDto;
using Passengers.Models.Shared;
using Passengers.Repository.Base;
using Passengers.Shared.DocumentService.Store;
using Passengers.SharedKernel.Constants;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.Files;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.DocumentService
{
    public class DocumentRepository : BaseRepository, IDocumentRepository
    {
        private readonly IWebHostEnvironment host;
        public DocumentRepository(PassengersDbContext context, IWebHostEnvironment host): base(context)
        {
            this.host = host;
        }

        public async Task<GetDocumentDto> Add(IFormFile file, Guid entityId, DocumentEntityType type)
        {
            var path = Upload(file, type);
            if (path.IsNullOrEmpty())
                return null;
            var document = new Document
            {
                Name = file.Name,
                Length = file.Length,
                Path = path,
                Type = file.FileName.GetDocumentType()
            };
            SetEntityId(document, entityId, type);
            Context.Add(document);
            await Context.SaveChangesAsync();
            return new GetDocumentDto
            {
                Id = document.Id,
                Path = document.Path
            };
        }

        public async Task<List<GetDocumentDto>> Add(List<IFormFile> files, Guid entityId, DocumentEntityType type)
        {
            List<GetDocumentDto> documents = new ();
            foreach (var file in files)
            {
                await Add(file, entityId, type);
            }
            return documents;
        }

        public async Task<List<GetDocumentDto>> Get()
        {
            return await Context.Documents.Select(DocumentStore.Query.GetSelectDocument).ToListAsync();
        }

        public Task<List<GetDocumentDto>> GetByEntityId(Guid entityId, DocumentEntityType type)
        {
            throw new NotImplementedException();
        }

        public Task<GetDocumentDto> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(List<Guid> id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveByEntityId(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetDocumentDto> Update(IFormFile file, Guid entityId, DocumentEntityType type)
        {
            var path = Upload(file, type);
            if (path.IsNullOrEmpty())
                return null;

            var document = await Context.Documents
                .Where(x => type == DocumentEntityType.Product ? x.ProductId == entityId : x.ShopId == entityId).FirstOrDefaultAsync();

            if(document == null)
            {
                document = new Document()
                {
                    ShopId = entityId,
                    Type = DocumentType.Image
                };
                Context.Documents.Add(document);
            }

            document.Path = path;
            document.Name = file.Name;
            document.Length = file.Length;

            await Context.SaveChangesAsync();
            return new GetDocumentDto
            {
                Id = document.Id,
                Path = document.Path
            };
        }

        public async Task<List<GetDocumentDto>> Update(List<Guid> olds, List<IFormFile> news, Guid entityId, DocumentEntityType type)
        {
            throw new NotImplementedException();
        }
        private static void SetEntityId(Document model, Guid entityId, DocumentEntityType type)
        {
            if (type == DocumentEntityType.Product)
                model.ProductId = entityId;
            else if (type == DocumentEntityType.Shop)
                model.ShopId = entityId;
            else if (type == DocumentEntityType.Offer)
                model.ShopId = entityId;
        }

        private static DocumentEntityType? Map(string name)
        {
            if (name == FolderNames.Product)
                return DocumentEntityType.Product;
            else if (name == FolderNames.Shop)
                return DocumentEntityType.Shop;
            else if (name == FolderNames.Offer)
                return DocumentEntityType.Offer;
            return null;
        }

        private static string Map(DocumentEntityType type)
        {
            if (type == DocumentEntityType.Product)
                return FolderNames.Product;
            else if (type == DocumentEntityType.Shop)
                return FolderNames.Shop;
            else if (type == DocumentEntityType.Offer)
                return FolderNames.Offer;
            return null;
        }

        private string Upload(IFormFile file, DocumentEntityType type)
        {
            var folderName = Map(type);
            if (folderName is null) return null;
            return file.TryUploadImage(folderName, host.WebRootPath);
        }
    }
}
