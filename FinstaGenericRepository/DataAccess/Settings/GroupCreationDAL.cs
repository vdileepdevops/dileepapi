using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Settings;
using FinstaRepository.Interfaces;
using FinstaRepository.Interfaces.Settings;
using HelperManager;
using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Settings
{
    public class GroupCreationDAL : SettingsDAL, IGroupCreation
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);       
        NpgsqlTransaction trans = null;
        public List<GroupCreation> groupList = null;
        public List<GroupRoleDTO> GroupRoleslist = null;
        public List<GroupCreationDTO> GroupDetailsList = null;
        GroupCreationDTO GroupCreationOBJ = null;
        public bool SaveGroupRole(GroupRoleDTO groupRole, string connectionString)
        {            
            int savedCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(groupRole.pGroupRoleName))
                {
                    savedCount = NPGSqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "insert into tblmstgrouprolesconfig(rolename,statusid,createdby,createddate) values('" + ManageQuote(groupRole.pGroupRoleName) + "'," + Convert.ToInt32(Status.Active) + "," + groupRole.pCreatedby + ",current_timestamp);");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                }
            }
            return  savedCount > 0 ? true : false;
        }
        public int checkGroupRoleCountinMaster(string roleName,string connectionString)
        {
            try
            {                
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstgrouprolesconfig where upper(rolename) ='" + ManageQuote(roleName).Trim().ToUpper() + "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                }
            }
        }
        public int checkContactDuplicateinGroup(string groupName,long contactId, string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblgroupnamewisemembers  where contactid="+ contactId + " and groupname='"+ManageQuote(groupName).Trim()+ "' and statusid=" + Convert.ToInt32(Status.Active) + ";"));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                }
            }
        }
        public bool saveGroupConfiguration(GroupCreationDTO groupDetails, string connectionString)
        {
            int groupSavedCount = 0;
            long groupId;
            StringBuilder saveGroupConfiguration = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                groupDetails.pMembersCount = Convert.ToString(groupDetails.pMembersCount) == string.Empty ? 0 : groupDetails.pMembersCount < 0 ? 0 : groupDetails.pMembersCount;
                // Master Data
                groupId = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "insert into tblmstgroupconfig(grouptype,groupname,groupcode,membercount,statusid,createdby,createddate) values('" + ManageQuote(groupDetails.pGroupType).Trim() + "','" + ManageQuote(groupDetails.pGroupName).Trim() + "','" + ManageQuote(groupDetails.pGroupCode).Trim() + "'," + groupDetails.pMembersCount + "," + Convert.ToInt32(Status.Active) + "," + groupDetails.pCreatedby + ",current_timestamp) returning groupid;"));
                // groupId = 23;

                // Child Details
                if (groupDetails.pListGroupDetails != null && groupDetails.pListGroupDetails.Count > 0)
                {
                    foreach (GroupCreation childDetails in groupDetails.pListGroupDetails)
                    {
                        int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tblgroupnamewisemembers  where contactid=" + childDetails.pContactID + " and groupname='" + ManageQuote(groupDetails.pGroupName).Trim() + "';"));
                        if (count == 0)
                        {
                            saveGroupConfiguration.Append("insert into tblgroupnamewisemembers(groupid,groupname,groupno,contactid,contactreferenceid,contactname,contactnumber,grouproleid,roleingroup,statusid,createdby,createddate) values(" + groupId + ",'" +ManageQuote( groupDetails.pGroupName).Trim() + "','" + ManageQuote(groupDetails.pGroupNo).Trim() + "'," + childDetails.pContactID + ",'" + ManageQuote(childDetails.pContactRefId) + "','" + ManageQuote(childDetails.pContactName) .Trim()+ "','" + ManageQuote(childDetails.pContactNo).Trim() + "'," + (childDetails.pGrouproleID)+ ",'" + ManageQuote( childDetails.pRoleInGroup).Trim() + "'," + Convert.ToInt32(Status.Active) + "," + groupDetails.pCreatedby + ",current_timestamp);");
                        }
                    }
                }   
                if (Convert.ToString(saveGroupConfiguration) != string.Empty)
                {
                    groupSavedCount = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, saveGroupConfiguration.ToString());
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Dispose();
                    con.Close();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return groupSavedCount > 0 ? true : false;
        }
        public async Task< List<GroupRoleDTO>> getGroupRoles(string connectionString)
        {
            await  Task.Run(() =>
            {
                GroupRoleslist = new List<GroupRoleDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select grouproleid,rolename from tblmstgrouprolesconfig where statusid=" + Convert.ToInt32(Status.Active) + " order by rolename desc;"))
                    {
                        while (dr.Read())
                        {
                            GroupRoleDTO objGroupRolesDetails = new GroupRoleDTO();
                            objGroupRolesDetails.pGroupRoleId = Convert.ToInt64(dr["grouproleid"]);
                            objGroupRolesDetails.pGroupRoleName = Convert.ToString(dr["rolename"]);
                            GroupRoleslist.Add(objGroupRolesDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return GroupRoleslist;
        }
        public async Task< List<GroupCreationDTO>> GetallGroupsDetailsAsync(string connectionString)
        {
           await Task.Run(() =>
            {
                GroupDetailsList = new List<GroupCreationDTO>();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select tc.groupid,grouptype,tc.groupcode,tc.groupname,coalesce( membercount,0) as membercount,roleingroup,contactnumber from tblmstgroupconfig tc join tblgroupnamewisemembers tn on tc.groupid=tn.groupid where roleingroup like 'Group Head%' and tc.statusid=" + Convert.ToInt32(Status.Active) + " order by tc.groupid desc; "))
                    {
                        while (dr.Read())
                        {
                            GroupCreationDTO objGroupDetails = new GroupCreationDTO();
                            objGroupDetails.pGroupID = Convert.ToInt64(dr["groupid"]);
                            objGroupDetails.pGroupType = Convert.ToString(dr["grouptype"]);
                            objGroupDetails.pGroupCode = Convert.ToString(dr["groupcode"]);
                            objGroupDetails.pGroupName = Convert.ToString(dr["groupname"]);
                            objGroupDetails.pMembersCount = Convert.ToDecimal(dr["membercount"]);
                            objGroupDetails.pGroupMembersRole = Convert.ToString(dr["roleingroup"]);
                            objGroupDetails.pContactNo = Convert.ToString(dr["contactnumber"]);
                            GroupDetailsList.Add(objGroupDetails);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return GroupDetailsList;
        }
        public bool UpdateGroupDetails(GroupCreationDTO objGRoupUpdate,string connectionString)
        {
            int UpdateCount = 0;
            StringBuilder sbUpdateGroup = new StringBuilder();
            try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (string.IsNullOrEmpty(objGRoupUpdate.pTransactionType) || objGRoupUpdate.pTransactionType.Trim().ToUpper() != "DELETE")
                {
                    sbUpdateGroup.Append("update tblmstgroupconfig set grouptype='" + ManageQuote(objGRoupUpdate.pGroupType).Trim() + "',groupname='" + ManageQuote(objGRoupUpdate.pGroupName).Trim() + "',groupcode='" + ManageQuote(objGRoupUpdate.pGroupCode).Trim() + "',membercount=" + objGRoupUpdate.pMembersCount + ", modifiedby=" + objGRoupUpdate.pCreatedby + ",modifieddate=current_timestamp where groupid=" + objGRoupUpdate.pGroupID + ";");

                    if(objGRoupUpdate.pListGroupDetails!=null && objGRoupUpdate.pListGroupDetails.Count>0)
                    {
                        foreach (GroupCreation groupStore in objGRoupUpdate.pListGroupDetails)
                        {
                            if (!string.IsNullOrEmpty(groupStore.pTypeofOperation))
                            {
                                if (groupStore.pTypeofOperation.Trim().ToUpper() == "CREATE")
                                {
                                    int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tblgroupnamewisemembers  where contactid=" + groupStore.pContactID + " and groupname='" + ManageQuote(objGRoupUpdate.pGroupName).Trim() + "';"));
                                    if (count == 0)
                                    {
                                        sbUpdateGroup.Append("insert into tblgroupnamewisemembers(groupid,groupname,groupno,contactid,contactreferenceid,contactname,contactnumber,grouproleid,roleingroup,statusid,createdby,createddate) values(" + objGRoupUpdate.pGroupID + ",'" + ManageQuote(objGRoupUpdate.pGroupName).Trim() + "','" + ManageQuote(objGRoupUpdate.pGroupNo).Trim() + "'," + groupStore.pContactID + ",'" + ManageQuote(groupStore.pContactRefId) + "','" + ManageQuote(groupStore.pContactName).Trim() + "','" + ManageQuote(groupStore.pContactNo).Trim() + "'," + (groupStore.pGrouproleID) + ",'" + ManageQuote(groupStore.pRoleInGroup).Trim() + "'," + Convert.ToInt32(Status.Active) + "," + objGRoupUpdate.pCreatedby + ",current_timestamp);");
                                    }
                                }
                                else if (groupStore.pTypeofOperation.Trim().ToUpper() == "UPDATE")
                                {
                                    sbUpdateGroup.Append("UPDATE tblgroupnamewisemembers SET  groupname ='" + ManageQuote(objGRoupUpdate.pGroupName).Trim() + "', groupno ='" + ManageQuote(objGRoupUpdate.pGroupNo).Trim() + "', contactid =" + groupStore.pContactID + ", contactreferenceid ='" + ManageQuote(groupStore.pContactRefId) + "', contactname ='" + ManageQuote(groupStore.pContactName).Trim() + "', contactnumber ='" + ManageQuote(groupStore.pContactNo).Trim() + "', grouproleid =" + (groupStore.pGrouproleID) + ", roleingroup ='" + ManageQuote(groupStore.pRoleInGroup).Trim() + "', modifiedby =" + objGRoupUpdate.pCreatedby + ", modifieddate = current_timestamp WHERE groupid=" + objGRoupUpdate.pGroupID + " and recordid=" + groupStore.pRecordId + "; ");
                                }
                                else if (groupStore.pTypeofOperation.Trim().ToUpper() == "DELETE")
                                {
                                    sbUpdateGroup.Append("Delete from tblgroupnamewisemembers where groupid=" + objGRoupUpdate.pGroupID + " and recordid=" + groupStore.pRecordId + ";");
                                }
                            }
                        }
                    }
                }
                else if(objGRoupUpdate.pTransactionType.Trim().ToUpper() == "DELETE")
                {
                    sbUpdateGroup.Append("update tblmstgroupconfig set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objGRoupUpdate .pCreatedby + ",modifieddate=current_timestamp where groupid="+objGRoupUpdate.pGroupID+" and upper(groupname)='"+ManageQuote(objGRoupUpdate.pGroupName).Trim().ToUpper()+"';");

                    sbUpdateGroup.Append("update tblgroupnamewisemembers set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + objGRoupUpdate.pCreatedby + ",modifieddate=current_timestamp where groupid=" + objGRoupUpdate.pGroupID + " and upper(groupname)='" + ManageQuote(objGRoupUpdate.pGroupName).Trim().ToUpper() + "';");
                }

                if(Convert.ToString(sbUpdateGroup)!=string.Empty)
                {
                    UpdateCount= NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbUpdateGroup.ToString());
                    trans.Commit();
                }
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
            return UpdateCount > 0 ? true : false;
        }
        public async Task< GroupCreationDTO> GetGroupMembersDetailsOnIdAsync(long groupID,string connectionString)
        {
           await Task.Run(() =>
            {
                GroupCreationOBJ = new GroupCreationDTO();
                try
                {
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select groupid,groupname,groupcode,grouptype,membercount from tblmstgroupconfig where groupid=" + groupID + " and statusid=" + Convert.ToInt32(Status.Active) + " order by  groupid asc;"))
                    {
                        while (dr.Read())
                        {                           
                            GroupCreationOBJ.pGroupID = Convert.ToInt64(dr["groupid"]);
                            GroupCreationOBJ.pGroupType = Convert.ToString(dr["grouptype"]);
                            GroupCreationOBJ.pGroupCode = Convert.ToString(dr["groupcode"]);
                            GroupCreationOBJ.pGroupName = Convert.ToString(dr["groupname"]);
                            GroupCreationOBJ.pMembersCount = Convert.ToDecimal(dr["membercount"]);
                            GroupCreationOBJ.pListGroupDetails = GetGroupChildMembersDetailsOnId(groupID, connectionString);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return GroupCreationOBJ;
        }
        public List<GroupCreation> GetGroupChildMembersDetailsOnId(Int64 groupId,string connectionString)
        {
            groupList = new List<GroupCreation>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select recordid,contactid,contactreferenceid,contactname,contactnumber,roleingroup,case when roleingroup like 'Group Head%' then 1 else 2 end as orderid from tblgroupnamewisemembers where groupid=" + groupId + " and statusid=" + Convert.ToInt32(Status.Active) + " order by orderid asc;"))
                {
                    while (dr.Read())
                    {
                        GroupCreation objGroupDetails = new GroupCreation();
                        objGroupDetails.pRecordId = Convert.ToInt64(dr["recordid"]);
                        objGroupDetails.pContactID = Convert.ToInt64(dr["contactid"]);
                        objGroupDetails.pContactRefId = Convert.ToString(dr["contactreferenceid"]);
                        objGroupDetails.pContactName = Convert.ToString(dr["contactname"]);
                        objGroupDetails.pContactNo = Convert.ToString(dr["contactnumber"]);
                        objGroupDetails.pRoleInGroup = Convert.ToString(dr["roleingroup"]);
                        groupList.Add(objGroupDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return groupList;
        }
    }
}
