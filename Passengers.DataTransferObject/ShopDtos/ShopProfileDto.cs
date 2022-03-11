using Passengers.DataTransferObject.ShopDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ShopDtos
{
    public class ShopProfileDto
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int FollowerCount { get; set; }
        public double Rate { get; set; }
        public string ImagePath { get; set; }
        public List<ContactInformationDto> Contacts { get; set; }
    }
}
