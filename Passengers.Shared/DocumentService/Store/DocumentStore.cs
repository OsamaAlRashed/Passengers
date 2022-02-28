using Microsoft.AspNetCore.Http;
using Passengers.DataTransferObject.SharedDtos.DocumentDto;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Constants;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.DocumentService.Store
{
    public static class DocumentStore
    {
        public static class Filter
        {
        }

        public static class Query
        {
            public static Expression<Func<Document, GetDocumentDto>> GetSelectDocument => c => new GetDocumentDto
            {
                Id = c.Id,
                Path = c.Path
            };
        }
    }
}
