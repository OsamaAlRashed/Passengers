using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SharedDtos.CategoryDtos
{
    public class GetCategoryDto : CategoryDto
    {
        public string Path { get; set; }
    }
}
