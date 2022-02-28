using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.BaseDtos
{
    public interface IKey
    {
        public Guid Id { get; set; }
    }
}
