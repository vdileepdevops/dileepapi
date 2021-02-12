using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FinstaRepository.Interfaces.Settings.Users;
using FinstaRepository.DataAccess.Settings.Users;
using FinstaApi.Security.Hashing;

namespace FinstaApi.Authentication
{
    public class UserService : IUserService
    {

        IUserAccess objUserAccess = new UserAccessDAL();
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<UserAccessDTO> _users = new List<UserAccessDTO>
        {
            new UserAccessDTO {pRoleid=1, pUserName="nag",pPassword="nag" }
        };

        private readonly AppSettings _appSettings;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(IOptions<AppSettings> appSettings, IPasswordHasher passwordHasher)
        {
            _appSettings = appSettings.Value;
            _passwordHasher = passwordHasher;
        }

        public UserAccessDTO Authenticate(string username, string password, string Con)
        {
            var user = objUserAccess.CheckUser(username, password, Con);
           // var encrypt = _passwordHasher.HashPassword("K@pilit$12345");
            //var decry = _passwordHasher.Decrypt(encrypt);
            // return null if user not found
            if (user == null && user.pPassword.Length > 0 || !_passwordHasher.PasswordMatches(password, user.pPassword))
                return null;

            if (user.pOtpAuthentication)
            {

                var Status = objUserAccess.SendOTP(Con, user.pMobile,Convert.ToString(user.pUserID), user.pEmail);
            }
            else { 
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.pRoleid.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.pToken = tokenHandler.WriteToken(token);

            // remove password before returning
            user.pPassword = null;
            }
            return user;
        }
        public UserAccessDTO VerifyOTP(UserAccessDTO userParam, string Con)
        {
            ValidateDTO objValidateOTP = new ValidateDTO();
            var user = objUserAccess.CheckUser(userParam.pUserName, userParam.pPassword, Con);
            objValidateOTP.pUserId = user.pUserID;
            objValidateOTP.pMobile = user.pMobile;
            objValidateOTP.pOtp = userParam.pOtp;
            var verifyOTP= objUserAccess.ValidateOTP(objValidateOTP, Con);
            // return null if user not found
            if (user == null && user.pPassword.Length > 0 || !_passwordHasher.PasswordMatches(userParam.pPassword, user.pPassword))
                return null;

            if (verifyOTP.status)
            {
                user.pStatus = Convert.ToString(verifyOTP.status);
                user.pMessage = verifyOTP.message;
                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.pRoleid.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(180),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.pToken = tokenHandler.WriteToken(token);

                // remove password before returning
                user.pPassword = null;
            }
            else
            {
                user.pMessage = verifyOTP.message;
                user.pStatus = Convert.ToString(verifyOTP.status);
            }
            return user;
        }


        public IEnumerable<UserAccessDTO> GetAll()
        {
            // return users without passwords
            return _users.Select(x =>
            {
                x.pPassword = null;
                return x;
            });
        }
    }
}
