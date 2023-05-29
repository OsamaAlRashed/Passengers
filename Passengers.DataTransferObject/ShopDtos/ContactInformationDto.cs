using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ShopDtos
{
    public class ContactInformationDto
    {
        public ContactShopType Type { get; set; }
        public string Text { get; set; }
    }
}
