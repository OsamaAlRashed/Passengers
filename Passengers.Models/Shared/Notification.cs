﻿using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Shared
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
