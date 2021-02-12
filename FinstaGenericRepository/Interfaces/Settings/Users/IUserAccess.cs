using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings.Users;

namespace FinstaRepository.Interfaces.Settings.Users
{
   public interface IUserAccess
    {
        Task<List<ContactEmployeeDTO>> GetContactDetails(string ConnectionString);
        Task<string> GetRoleName(long EmployeeID, string ConnectionString);
        bool SaveUserAccess(UserAccessDTO UserAccessDTO,string connectionString);
        bool UpdateuserPassword(string Username,string password, string connectionString);
        UserAccessDTO CheckUser(string UserName, string PassWord, string connectionString);
        Task<int> CheckUserName(string UserName,string connectionString);
        Task<int> CheckUsercontactRefID(string Contactrefid,string connectionString);
        Task<List<UserAccessDTO>> GetAllUsersView(string connectionString);
        Task<bool> DeleteUser(long Userid, bool Status,string connectionString);
        string GetDeafultPassword(string ConnectionString);
        bool SendOTP(string connectionString, string strMobileNo, string struserid, string email);
        ValidateResponse ValidateOTP(ValidateDTO objValidateOTP, string connectionString);
    }
}
