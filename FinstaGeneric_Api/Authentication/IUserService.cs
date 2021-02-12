using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Security.Tokens;
using FinstaInfrastructure.Settings.Users;
namespace FinstaApi.Authentication
{
    public interface IUserService
    {
        UserAccessDTO Authenticate(string username, string password,string Con);
        IEnumerable<UserAccessDTO> GetAll();
        UserAccessDTO VerifyOTP(UserAccessDTO userParam, string Con);


    }
}
