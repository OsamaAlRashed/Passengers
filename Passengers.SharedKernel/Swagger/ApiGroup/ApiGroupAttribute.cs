using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Swagger.ApiGroup
{
    public class ApiGroupAttribute : System.Attribute
    {
        public ApiGroupAttribute(params ApiGroupNames[] name)
        {
            GroupName = name;
        }
        public ApiGroupNames[] GroupName { get; set; }
    }
}
