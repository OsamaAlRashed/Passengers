using Microsoft.AspNetCore.Http;
using Passengers.DataTransferObject.ShopDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos
{
    public class CompleteInfoShopDto
    {
        public IFormFile Image { get; set; }
        public List<int> Days { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public List<ContactInformationDto> Contacts { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> TagIds { get; set; }
    }
}
