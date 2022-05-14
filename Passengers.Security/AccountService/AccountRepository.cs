using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Passengers.Base;
using Passengers.DataTransferObject.GeneralDtos;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Models.Security;
using Passengers.Repository.Base;
using Passengers.Security.Shared;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SharedKernel.Services.EmailService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.AccountService
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IPasswordHasher<AppUser> passwordHasher;
        public AccountRepository(PassengersDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IPasswordHasher<AppUser> passwordHasher) :base(context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.passwordHasher = passwordHasher;
        }

        public async Task<OperationResult<LoginResponseDto>> Login(BaseLoginDto dto)
        {
            var user = await Context.Users
                .Include(x => x.Category)
                .Include(x => x.Documents)
                .Where(x => x.UserType == dto.UserType
                    && (x.UserName == dto.UserName || x.Email == dto.UserName || x.PhoneNumber == dto.UserName))
                .SingleOrDefaultAsync();

            if (user == null || user.DateDeleted.HasValue)
                return _Operation.SetFailed<LoginResponseDto>("UserNotFound", OperationResultTypes.Unauthorized);

            if (user.AccountStatus == AccountStatus.Draft)
                return _Operation.SetFailed<LoginResponseDto>("UserNotAccepted", OperationResultTypes.Unauthorized);

            var loginResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                
            if (loginResult == SignInResult.Success)
            {
                var roles = await userManager.GetRolesAsync(user);
                var expierDate = dto.RemmberMe ? DateTime.Now.AddYears(1) : DateTime.Now.AddMinutes(30);
                if (!dto.DeviceToken.IsNullOrEmpty())
                {
                    user.DeviceTokens ??= "";
                    user.DeviceTokens = String.Join(",", user.DeviceTokens, dto.DeviceToken);
                }
                user.RefreshToken = GenerateRefreshToken();
                await Context.SaveChangesAsync();

                LoginResponseDto accountDto = new()
                {
                    User = user,
                    AccessToken = GenerateAccessToken(user, roles, expierDate),
                    RefreshToken = user.RefreshToken,
                };
                return _Operation.SetSuccess(accountDto);
            }
            return _Operation.SetFailed<LoginResponseDto>("UserNotFound", OperationResultTypes.Unauthorized);
        }

        //public async Task<bool> SendToken(CreateAccountDto dto, string token, TokenTypes type)
        //{
        //    Message message = new Message();
        //    message.To = dto.Email;

        //    if (type == TokenTypes.ConfirmEmail)
        //    {
        //        message.Subject = "تأكيد الحساب.";
        //        message.Body = @$"مرحبا .. شكرا لك على اشتراكك في تطبيقنا 
        //                  إن رمز تأكيد حسابك المؤلف من 6 أرقام هو :
        //                     {token}
        //                   انسخه وضعه في الحقل المخصص لتأكيد الحساب وابقه سريا للحفاظ على أمان حسابك.";
        //    }
        //    else if (type == TokenTypes.ResetPassword)
        //    {
        //        message.Subject = "إعادة تعيين كلمة المرور.";
        //        message.Body = $"إن رمز إعادة تعيين كلمة المرور الخاصة بحسابك هي {token}";
        //    }
        //    return await emailService.SendEmail(message);
        //}

        public string GenerateAccessToken(AppUser user, IList<string> roles, DateTime expierDate)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim("Type", user.UserType.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                  configuration["Jwt:Issuer"],
                  claims,
                  expires: expierDate,
                  signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<bool> IsPhoneNumberUsed(string phoneNumber)
        {
            return await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<OperationResult<CreateAccountDto>> Create(CreateAccountDto dto)
        {
            AppUser user = new()
            {
                UserName = dto.UserName,
                Email = dto.UserName + "@passengers.com",
                UserType = dto.Type
            };
            var identityResult = await userManager.CreateAsync(user, dto.Password);

            if (!identityResult.Succeeded)
                return _Operation.SetFailed<CreateAccountDto>(String.Join(",", identityResult.Errors.Select(error => error.Description)));

            var roleIdentityResult = await userManager.AddToRoleAsync(user, dto.Type.ToString());

            if (!roleIdentityResult.Succeeded)
                return _Operation.SetFailed<CreateAccountDto>(String.Join(",", roleIdentityResult.Errors.Select(error => error.Description)));

            await Context.SaveChangesAsync();
            dto.Id = user.Id;
            return _Operation.SetSuccess(dto);
        }

        public async Task<OperationResult<bool>> ChangeStatus(Guid id, AccountStatus accountStatus)
        {
            var requst = await Context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (requst == null)
                return _Operation.SetFailed<bool>("");
            requst.AccountStatus = accountStatus;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        //public async Task<OperationResult<bool>> ResendToken(Guid id, TokenTypes type)
        //{
        //    var user = await Context.Users.SingleOrDefaultAsync(u => u.Id == id);
        //    if (user is null)
        //        return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "المستخدم غير موجود.");

        //    var dto = new SignupDto
        //    {
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email
        //    };

        //    var result = false;
        //    if (type == TokenTypes.ConfirmEmail)
        //    {
        //        user.EmailConfirmToken = Helpers.GetRandomNumberToken(6);
        //        result = await SendToken(dto, user.EmailConfirmToken, TokenTypes.ConfirmEmail);
        //    }
        //    else if (type == TokenTypes.ResetPassword)
        //    {
        //        user.ResetPasswordToken = Helpers.GetRandomNumberToken(6);
        //        result = await SendToken(dto, user.ResetPasswordToken, TokenTypes.ResetPassword);
        //    }

        //    await Context.SaveChangesAsync();

        //    return _Operation.SetSuccess(result);
        //}
        //public async Task<OperationResult<bool>> ForgetPassword(string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

        //    var token = await userManager.GeneratePasswordResetTokenAsync(user);
        //    if (await SendToken(new SignupDto { Email = email }, token, TokenTypes.ResetPassword))
        //        return _Operation.SetSuccess(true);

        //    return _Operation.SetFailed<bool>("", OperationResultTypes.Failed);
        //}
        public async Task<OperationResult<bool>> ResetPassword(ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var token = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (result == IdentityResult.Success)
                return _Operation.SetSuccess(true);

            return _Operation.SetFailed<bool>("", OperationResultTypes.Failed);
        }

        public async Task<OperationResult<TokenDto>> RefreshToken(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = Context.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null)
                return _Operation.SetContent<TokenDto>(OperationResultTypes.NotExist, "UserNotFound");

            if (user.RefreshToken != refreshToken)
                return _Operation.SetFailed<TokenDto>("RefreshTokenUnCorrect");

            var roles = await userManager.GetRolesAsync(user);
            var newAccessToken = GenerateAccessToken(user, roles, DateTime.Now.AddMinutes(30));
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }

        public async Task<OperationResult<bool>> ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            var user = Context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
                return _Operation.SetFailed<bool>("");
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> ChangePassword(Guid id, string newPassword)
        {
            var user = Context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            if (newPassword.IsNullOrEmpty())
                return _Operation.SetFailed<bool>("");

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> Block(Guid id)
        {
            var user = await Context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (user == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            user.DateBlocked = user.DateBlocked.HasValue ? null : DateTime.Now;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            var user = await Context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (user == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            user.DateDeleted = DateTime.Now;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<SelectDto>>> Users(UserTypes? type)
        {
            var data = await Context.Users.Where(x => !type.HasValue || x.UserType == type)
                .Select(x => new SelectDto
                {
                    Id = x.Id,
                    Name = x.FullName
                }).ToListAsync();

            return _Operation.SetSuccess(data);
        }
    }
}
