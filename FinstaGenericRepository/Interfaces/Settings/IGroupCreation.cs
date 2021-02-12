using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.Interfaces.Settings
{
    public interface IGroupCreation
    {
        bool SaveGroupRole(GroupRoleDTO groupRole, string connectionstring);
        int checkGroupRoleCountinMaster(string roleName, string connectionString);
        bool saveGroupConfiguration(GroupCreationDTO groupObj, string connectionString);
        int checkContactDuplicateinGroup(string GroupName,long contactId,string connectionString);
       Task< List<GroupRoleDTO>> getGroupRoles(string connectionString);
        Task< List<GroupCreationDTO>> GetallGroupsDetailsAsync(string connectionstring);
        bool UpdateGroupDetails(GroupCreationDTO objGroup, string connectionString);
      Task<  GroupCreationDTO> GetGroupMembersDetailsOnIdAsync(long groupID, string connectionString);
    }
}
