using Passengers.Models.Base;
using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Shared
{
    public class NotificationUser : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; }
    }
}
