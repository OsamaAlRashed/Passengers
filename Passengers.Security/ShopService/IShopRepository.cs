using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.DataTransferObject.ShopDtos;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.ShopService
{
    public interface IShopRepository
    {
        Task<OperationResult<CreateShopAccountDto>> SignUp(CreateShopAccountDto dto);
        Task<OperationResult<object>> Login(LoginMobileDto dto);
        Task<OperationResult<bool>> CompleteInfo(CompleteInfoShopDto dto);
        Task<OperationResult<List<ShopDto>>> Get(AccountStatus accountStatus);
    }
}
