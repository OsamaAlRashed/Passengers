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

        public async Task<GetDocumentDto> Add(IFormFile file, Guid entityId, DocumentEntityTypes type)
        {
            var path = Upload(file, type);
            if (path.IsNullOrEmpty())
                return null;
            var document = new Document
            {
                Name = file.Name,
                Length = file.Length,
                Path = path,
                Type = file.Name.GetDocumentType()
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

        public async Task<List<GetDocumentDto>> Add(List<IFormFile> files, Guid entityId, DocumentEntityTypes type)
        {
            List<GetDocumentDto> documents = new List<GetDocumentDto>();
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

        public Task<List<GetDocumentDto>> GetByEntityId(Guid entityId, DocumentEntityTypes type)
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

        public async Task<GetDocumentDto> Update(IFormFile file, Guid entityId, DocumentEntityTypes type)
        {
            var path = Upload(file, type);
            if (path.IsNullOrEmpty())
                return null;

            var document = await Context.Documents
                .Where(x => type == DocumentEntityTypes.Product ? x.ProductId == entityId : x.ShopId == entityId).FirstOrDefaultAsync();
            document.Path = path;
            document.Name = file.Name;
            document.Length = file.Length;

            Context.Update(document);
            await Context.SaveChangesAsync();
            return new GetDocumentDto
            {
                Id = document.Id,
                Path = document.Path
            };
        }

        public async Task<List<GetDocumentDto>> Update(List<Guid> olds, List<IFormFile> news, Guid entityId, DocumentEntityTypes type)
        {
            throw new NotImplementedException();
        }
        private void SetEntityId(Document model, Guid entityId, DocumentEntityTypes type)
        {
            if (type == DocumentEntityTypes.Product)
                model.ProductId = entityId;
            else if (type == DocumentEntityTypes.Shop)
                model.ShopId = entityId;
        }

        private DocumentEntityTypes? Map(string name)
        {
            if (name == FolderNames.Product)
                return DocumentEntityTypes.Product;
            else if (name == FolderNames.Shop)
                return DocumentEntityTypes.Shop;
            return null;
        }

        private string Map(DocumentEntityTypes type)
        {
            if (type == DocumentEntityTypes.Product)
                return FolderNames.Product;
            else if (type == DocumentEntityTypes.Shop)
                return FolderNames.Shop;
            return null;
        }

        private string Upload(IFormFile file, DocumentEntityTypes type)
        {
            var folderName = Map(type);
            if (folderName is null) return null;
            return file.TryUploadImage(folderName, host.WebRootPath);
        }

        private List<string> Upload(List<IFormFile> files, DocumentEntityTypes type)
        {
            var folderName = Map(type);
            if (folderName is null) return null;
            return files.TryUploadImages(folderName, host.WebRootPath);
        }
    }
}
