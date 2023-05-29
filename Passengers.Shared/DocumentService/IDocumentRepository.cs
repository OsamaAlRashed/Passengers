using Microsoft.AspNetCore.Http;
using Passengers.DataTransferObject.SharedDtos.DocumentDto;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.DocumentService
{
    public interface IDocumentRepository
    {
        Task<List<GetDocumentDto>> Get();
        Task<GetDocumentDto> GetById(Guid id);
        Task<List<GetDocumentDto>> GetByEntityId(Guid entityId, DocumentEntityType type);
        Task<GetDocumentDto> Add(IFormFile file, Guid entityId, DocumentEntityType type);
        Task<List<GetDocumentDto>> Add(List<IFormFile> files, Guid entityId, DocumentEntityType type);
        Task<GetDocumentDto> Update(IFormFile file, Guid entityId, DocumentEntityType type);
        Task<List<GetDocumentDto>> Update(List<Guid> olds, List<IFormFile> news, Guid entityId, DocumentEntityType type);
        Task<bool> Remove(Guid id);
        Task<bool> Remove(List<Guid> id);
        Task<bool> RemoveByEntityId(Guid entityId);
    }
}
