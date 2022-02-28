using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.RateDtos
{
    public class RateDto
    {
        public Guid Id { get; set; }
        public int Degree { get; set; }
        public string Descreption { get; set; }
        public DateTime Date { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerName { get; set; }
    }
}
