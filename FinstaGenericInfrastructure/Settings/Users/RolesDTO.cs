using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Settings.Users
{
    public class RolesDTO : WorkGroupinrolesDTO
    {
        public string pRoledesignation { get; set; }
        public bool pRoleactivestatus { get; set; }
        public long pRecordId { get; set; }
        public long pRoleID { get; set; }
    }
    public class WorkGroupinrolesDTO : CommonDTO
    {
        public long pGroupid { get; set; }
        public string pGroupname { get; set; }
    }
    public class RolemodulesDTO : CommonDTO
    {
        public long pModuleId { get; set; }
        public string pModulename { get; set; }
    }
    public class RolesubmodulesDTO : RolemodulesDTO
    {
        public long pSubmoduleId { get; set; }
        public string pSubmodulename { get; set; }
    }

    public class MenuandNavigationDTO : RolesubmodulesDTO 
    {
        public string pFunctionname { get; set; }
        public string pFunctionurl { get; set; }
        public bool pIsfunctionshowinNavigation { get; set; }
        public bool pIsFunctionshowinRoles { get; set; }
        public long pFunctionId { get; set; }
        public long PFunctionParentID { get; set; }
    } 
}
