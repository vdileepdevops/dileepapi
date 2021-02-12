using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings.Users;

namespace FinstaRepository.Interfaces.Settings.Users
{
    public interface IUserRights
    {
        Task<UserRightsFunctionsDTO> GetallUserModules(string Type, string UserOrDesignation, string connectionString);
        Task<UserRightsDTO> GetUserModulesBasedOnroleanduserid(string UserName,string connectionString);
        Task<bool> SaveUserRight(string Type, string UserOrDesignation, UserRightsFunctionsDTO UserRightsFunctionsDTO, string connectionString);
        Task<List<RoleDTO>> BindRoles(string ConnectionString);
        Task<List<UserDTO>> BindUsers(string ConnectionString);
       
    }
}
