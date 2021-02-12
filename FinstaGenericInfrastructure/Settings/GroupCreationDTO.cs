using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;
using FinstaInfrastructure;

namespace FinstaInfrastructure.Settings
{
    public class  GroupCreationDTO : CommonDTO
    {
        public List<GroupCreation> pListGroupDetails { get; set; }
        public Int64 pGroupID { get; set; }
        public string pGroupType { get; set; }
        public string pGroupName { get; set; }
        public string pGroupCode { get; set; }
        public decimal pMembersCount { get; set; }
        public decimal pGroupSeries { get; set; }
        public string pGroupNo { get; set; }
        public string pGroupMembersRole { get; set; }
        public string pContactNo { get; set; }
        public string pTransactionType { get; set; }
    }
    public class GroupCreation 
    {
        public Int64 pContactID { get; set; }
        public string pContactRefId { get; set; }
        public Int64 pMemberId { get; set; }
        public string pContactName { get; set; }
        public string pContactNo { get; set; }
        public Int64 pGrouproleID { get; set; }
        public string pRoleInGroup { get; set; }
        public  string pTypeofOperation { get; set; }
        public Int64 pRecordId { get; set; }
    }
    public class GroupRoleDTO : CommonDTO
    {
        public Int64 pGroupRoleId { get; set; }
        public string pGroupRoleName { get; set; }
    }
}
