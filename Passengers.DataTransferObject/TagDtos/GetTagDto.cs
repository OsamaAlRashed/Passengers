using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.TagDtos
{
    public class GetTagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public Guid? ShopId { get; set; }
    }
}
