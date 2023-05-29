using Passengers.DataTransferObject.GeneralDtos;
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
        Task<OperationResult<bool>> Logout(string refreshToken);
        Task<OperationResult<CreateAccountDto>> Create(CreateAccountDto dto);
        Task<OperationResult<bool>> ChangeStatus(Guid id, AccountStatus accountStatus);
        Task<OperationResult<TokenDto>> RefreshToken(string accessToken, string refreshToken);
        Task<OperationResult<bool>> ChangePassword(Guid id, string oldPassword, string newPassword);
        Task<OperationResult<bool>> ChangePassword(Guid id, string newPassword);
        Task<bool> IsPhoneNumberUsed(string phoneNumber);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteAllShop(List<Guid> ids);
        Task<OperationResult<bool>> Block(Guid id);
        Task<OperationResult<List<SelectDto>>> Users(UserType? type);
        List<Guid> GetUserIds(UserType type);

        //Task<OperationResult<bool>> ResendToken(Guid id, TokenTypes type);
        //Task<OperationResult<bool>> ForgetPassword(string email);
        //Task<OperationResult<bool>> ResetPassword(ResetPasswordDto model);
    }
}
