using Passengers.Repository.Base;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.OfferService
{
    public class OfferRepository : BaseRepository , IOfferRepository
    {
        public OfferRepository(PassengersDbContext context): base(context)
        {

        }
    }
}
