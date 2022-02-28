using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SharedDtos.CategoryDtos
{
    public class SetCategoryDto : CategoryDto
    {
        public IFormFile File { get; set; }
    }
}
