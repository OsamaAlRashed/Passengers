using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SharedDtos.DocumentDto
{
    public class GetDocumentDto
    {
        public Guid Id { get; set;}
        public string Path { get; set; }
    }
}
