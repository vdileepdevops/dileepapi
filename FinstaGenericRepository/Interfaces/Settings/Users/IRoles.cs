using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings.Users;

namespace FinstaRepository.Interfaces.Settings.Users
{
    public interface IRoles
    {
        bool SaveWorkGroupinRole(WorkGroupinrolesDTO _workGroupinrolesDTO, string Connectionstring);
        Task<List<WorkGroupinrolesDTO>> GetallWorkGroupsinRoles(string connectionString);
        bool SaveRole(RolesDTO _SaveRole, string Connectionstring);
        bool SaveRoleModule(RolemodulesDTO _RolemodulesDTO, string Connectionstring);
        bool SaveRoleSubModule(RolesubmodulesDTO _RolemodulesDTO, string Connectionstring);
        Task<List<RolemodulesDTO>> GetallRolesModules(string connectionString);
        Task<List<RolesubmodulesDTO>> GetRolesSubModulesbyModule(long Moduleid, string connectionString);
        bool SaveRoleFunction(MenuandNavigationDTO _MenuandNavigationDTO, string Connectionstring);

       Task< List<RolesDTO>> GetWorkgroupandDesignations(string Connectionstring);
        int GetWorkgroupcount(string Workgroup, string connectionString);
        int GetWorkgroupandDesignationcount(string Workgroup, string Rolename, string connectionString);
        int GetModulecount(string Modulename, string connectionString);
        int GetSubmenucountbyMenu(string Modulename, string Submodulename, string connectionString);

        Task<List<MenuandNavigationDTO>> GetMenuandsubmenulist(string Connectionstring);

        int GetMenufunctionscount(long ParentId, long SubId, string functionname, string functionurl, string connectionString);

    }
}
