using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SharedDtos.CategoryDtos
{
    public class TreeDto : GetCategoryDto
    {
        public List<TreeDto> Children { get; set; }
    }
}
