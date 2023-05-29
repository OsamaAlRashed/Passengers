using Passengers.Models.Base;
using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace Passengers.Models.Shared
{
    public class Document : BaseEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Length { get; set; }
        public DocumentType Type { get; set; }

        public Guid? ShopId { get; set; }
        public AppUser Shop { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? OfferId { get; set; }
        public Offer Offer { get; set; }

    }
}
