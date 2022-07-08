using EasyRefreshToken.Models;
using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Security
{
    public class RefreshToken : RefreshToken<AppUser, Guid>
    {
        public DateTime DateCreated { get; set; }
        public RefreshToken()
        {
            DateCreated = DateTime.Now;
        }
    }
}
