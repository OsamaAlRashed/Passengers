using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.AccountService
{
    public interface IAccountRepository
    {
        Task<OperationResult<LoginResponseDto>> Login(BaseLoginDto dto);
        Task<OperationResult<CreateAccountDto>> Create(CreateAccountDto dto);
        Task<OperationResult<bool>> ChangeStatus(Guid id, AccountStatus accountStatus);
        //Task<OperationResult<bool>> ResendToken(Guid id, TokenTypes type);
        //Task<OperationResult<bool>> ForgetPassword(string email);
        //Task<OperationResult<bool>> ResetPassword(ResetPasswordDto model);
        Task<OperationResult<TokenDto>> RefreshToken(TokenRequestDto tokenDto);
        Task<bool> IsPhoneNumberUsed(string phoneNumber);
    }
}
