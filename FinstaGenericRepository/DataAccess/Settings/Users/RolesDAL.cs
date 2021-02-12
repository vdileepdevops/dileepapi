using FinstaInfrastructure.Settings.Users;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Settings.Users;
using HelperManager;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.DataAccess.Settings.Users
{
    public class RolesDAL : SettingsDAL, IRoles
    {
        NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        NpgsqlTransaction trans = null;        
        public List<WorkGroupinrolesDTO> _WorkgroupList = null;
        public List<RolemodulesDTO> _RolemodulesDTOList = null;
        public List<RolesubmodulesDTO> _RolesubmodulesDTOList = null;
        public List<RolesDTO> _RolesDTOList = null;
        public bool SaveWorkGroupinRole(WorkGroupinrolesDTO _WorkGroupinrolesDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {                
                    con = new NpgsqlConnection(Connectionstring);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO tblmstgroup (groupname,groupdescription,issystemgroup,statusid,createdby,createddate) VALUES ('" + ManageQuote(_WorkGroupinrolesDTO.pGroupname).Trim().ToUpper() + "', '', 'N', " + Convert.ToInt32(Status.Active) + ", " + _WorkGroupinrolesDTO.pCreatedby + ", current_timestamp); ");
                    trans.Commit();
                    Issaved = true;
                               
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

            return Issaved;
        }
        public async Task<List<WorkGroupinrolesDTO>> GetallWorkGroupsinRoles(string connectionString)
        {
            await Task.Run(() =>
            {
                _WorkgroupList = new List<WorkGroupinrolesDTO>();
                try
                {
                    using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select groupid,groupname from tblmstgroup where statusid="+Convert.ToInt32(Status.Active)+";"))
                    {
                        while (dataReader.Read())
                        {
                            WorkGroupinrolesDTO _WorkGroupinrolesDTO = new WorkGroupinrolesDTO
                            {
                                pGroupname = Convert.ToString(dataReader["groupname"]),
                                pGroupid=Convert.ToInt64(dataReader["groupid"])
                            };
                            _WorkgroupList.Add(_WorkGroupinrolesDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _WorkgroupList;
        }
        public bool SaveRole(RolesDTO _RolesDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();

                if (!string.IsNullOrEmpty(_RolesDTO.ptypeofoperation))
                {
                    int RoleStatus = 2;
                    if(Convert.ToBoolean(_RolesDTO.pRoleactivestatus)==true)
                    {
                        RoleStatus = 1;
                    }
                    else
                    {
                        RoleStatus = 2;
                    }
                    if (_RolesDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "INSERT INTO tblmstgrouproles (roleid,groupid,groupname,rolename,roledescription,issystemrole,statusid,createdby,createddate) VALUES (" + _RolesDTO.pRoleID + "," + _RolesDTO.pGroupid + ", '" + ManageQuote(_RolesDTO.pGroupname) + "','" + ManageQuote(_RolesDTO.pRoledesignation).Trim().ToUpper() + "','', 'N', " + RoleStatus + ", " + _RolesDTO.pCreatedby + ", current_timestamp); ");
                    }
                    else if(_RolesDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "Update tblmstgrouproles set roleid=" + _RolesDTO.pRoleID + ",groupid=" + _RolesDTO.pGroupid + "," +
                            "groupname= '" + ManageQuote(_RolesDTO.pGroupname) + "', rolename= '" + ManageQuote(_RolesDTO.pRoledesignation).Trim().ToUpper() + "',statusid=" + RoleStatus + ",modifiedby="+ _RolesDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + _RolesDTO.pRecordId + ";");
                    }
                    else if (_RolesDTO.ptypeofoperation.Trim().ToUpper() == "DELETE")
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "Update tblmstgrouproles set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + _RolesDTO.pCreatedby + ",modifieddate=current_timestamp where recordid=" + _RolesDTO.pRecordId + ";");
                    }
                    trans.Commit();
                    Issaved = true;
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
            return Issaved;
        }      
        public bool SaveRoleModule(RolemodulesDTO _RolemodulesDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {
                    con = new NpgsqlConnection(Connectionstring);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    _ = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, cmdText: "INSERT INTO tblmstmodules (modulename,moduledescription,statusid,createdby,createddate) VALUES ('" + ManageQuote(_RolemodulesDTO.pModulename).Trim()+ "', ''," + Convert.ToInt32(Status.Active) + ", " + _RolemodulesDTO.pCreatedby + ", current_timestamp); ");
                    trans.Commit();
                    Issaved = true;
               
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

            return Issaved;
        }
        public bool SaveRoleSubModule(RolesubmodulesDTO _RolesubmodulesDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {               
                    con = new NpgsqlConnection(Connectionstring);
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    trans = con.BeginTransaction();
                    _ = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, cmdText: "INSERT INTO tblmstmodules (modulename,moduledescription,parentmoduleid,parentmodulename,statusid,createdby,createddate) VALUES ('" + ManageQuote(_RolesubmodulesDTO.pSubmodulename).Trim() + "', '', " + _RolesubmodulesDTO.pModuleId + ", '" + ManageQuote(_RolesubmodulesDTO.pModulename).Trim() + "'," + Convert.ToInt32(Status.Active) + ", " + _RolesubmodulesDTO.pCreatedby + ", current_timestamp); ");
                    trans.Commit();
                    Issaved = true;
                
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

            return Issaved;
        }
        public async Task<List<RolemodulesDTO>> GetallRolesModules(string connectionString)
        {
            await Task.Run(() =>
            {
                _RolemodulesDTOList = new List<RolemodulesDTO>();
                try
                {
                    using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select moduleid,modulename from tblmstmodules where coalesce(parentmoduleid,0)=0 and coalesce(parentmodulename,'')='' and statusid=" + Convert.ToInt32(Status.Active) + " order by modulename desc;"))
                    {
                        while (dataReader.Read())
                        {
                            RolemodulesDTO _RolemodulesDTO = new RolemodulesDTO
                            {
                                pModulename = Convert.ToString(dataReader["modulename"]),
                                pModuleId = Convert.ToInt64(dataReader["moduleid"])
                            };
                            _RolemodulesDTOList.Add(_RolemodulesDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RolemodulesDTOList;
        }
        public async Task<List<RolesubmodulesDTO>> GetRolesSubModulesbyModule(long Moduleid, string connectionString)
        {
            await Task.Run(() =>
            {
                _RolesubmodulesDTOList = new List<RolesubmodulesDTO>();
                try
                {
                    using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select moduleid,modulename from tblmstmodules where parentmoduleid= " + Moduleid + " and statusid=" + Convert.ToInt32(Status.Active) + " order by modulename desc;"))
                    {
                        while (dataReader.Read())
                        {
                            RolesubmodulesDTO _RolesubmodulesDTO = new RolesubmodulesDTO
                            {
                                pSubmodulename = Convert.ToString(dataReader["modulename"]),
                                pSubmoduleId = Convert.ToInt64(dataReader["moduleid"])
                            };
                            _RolesubmodulesDTOList.Add(_RolesubmodulesDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RolesubmodulesDTOList;
        }
        public bool SaveRoleFunction(MenuandNavigationDTO _MenuandNavigationDTO, string Connectionstring)
        {
            bool Issaved = false;
            try
            {
                con = new NpgsqlConnection(Connectionstring);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                

                if (!string.IsNullOrEmpty(_MenuandNavigationDTO.ptypeofoperation))
                {
                    string ParentFunctionid = Convert.ToString(_MenuandNavigationDTO.PFunctionParentID);
                    if (_MenuandNavigationDTO.PFunctionParentID == 0)
                    {
                        ParentFunctionid = "null";
                    }
                    if (_MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                    {
                        _ = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, cmdText: "INSERT INTO tblmstfunctions (functionparentid,parentmoduleid,submoduleid,functionname,functiondescription,functionurl,isfunctionshowinnavigation,isfunctionallowinroles,statusid,createdby,createddate) VALUES (" + ParentFunctionid + "," + _MenuandNavigationDTO.pModuleId + "," + _MenuandNavigationDTO.pSubmoduleId + ", '" + ManageQuote(_MenuandNavigationDTO.pFunctionname).Trim() + "','','" + ManageQuote(_MenuandNavigationDTO.pFunctionurl).Trim() + "','" + _MenuandNavigationDTO.pIsfunctionshowinNavigation + "','" + _MenuandNavigationDTO.pIsFunctionshowinRoles + "'," + Convert.ToInt32(Status.Active) + ", " + _MenuandNavigationDTO.pCreatedby + ", current_timestamp); ");
                    }
                    else if (_MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE")
                    {
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, "Update tblmstfunctions set parentmoduleid=" + _MenuandNavigationDTO.pModuleId + ",submoduleid= '" + ManageQuote(_MenuandNavigationDTO.pModulename).Trim() + "', functionname= '" + ManageQuote(_MenuandNavigationDTO.pFunctionname).Trim() + "',functionurl='" + ManageQuote(_MenuandNavigationDTO.pFunctionurl).Trim() + "',isfunctionshowinnavigation='" + _MenuandNavigationDTO.pIsfunctionshowinNavigation + "',isfunctionallowinroles='" + _MenuandNavigationDTO.pIsFunctionshowinRoles + "',modifiedby=" + _MenuandNavigationDTO.pCreatedby + ",modifieddate=current_timestamp,functionparentid=" + ParentFunctionid + " where functionid=" + _MenuandNavigationDTO.pFunctionId + ";");
                    }
                    else if(_MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper()=="DELETE")
                    {
                        _ = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, cmdText: "update tblmstfunctions set statusid=" + Convert.ToInt32(Status.Inactive) + ",modifiedby=" + _MenuandNavigationDTO.pCreatedby + ",modifieddate=current_timestamp where functionid="+ _MenuandNavigationDTO .pFunctionId+ ";");
                    }
                    trans.Commit();
                    Issaved = true;
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
            return Issaved;
        }
        public async Task<List<RolesDTO>> GetWorkgroupandDesignations(string Connectionstring)
        {           
            await Task.Run(() =>
            {
                _RolesDTOList = new List<RolesDTO>();
                try
                {
                    using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select recordid,roleid,groupid,groupname,rolename,case when statusid=1 then true else false end as statusid from tblmstgrouproles where statusid=" + Convert.ToInt32(Status.Active) + " order by groupname,rolename desc;"))
                    {
                        while (dataReader.Read())
                        {
                            RolesDTO _RolemodulesDTO = new RolesDTO
                            {
                                pRoleID = Convert.ToInt64(dataReader["roleid"]),
                                pGroupname = Convert.ToString(dataReader["groupname"]),
                                pGroupid = Convert.ToInt64(dataReader["groupid"]),
                                pRoleactivestatus = Convert.ToBoolean(dataReader["statusid"]),
                                pRoledesignation = Convert.ToString(dataReader["rolename"]),
                                pRecordId = Convert.ToInt64(dataReader["recordid"])
                            };
                            _RolesDTOList.Add(_RolemodulesDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _RolesDTOList;
        }
        public int GetWorkgroupcount(string Workgroup, string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstgroup where upper( groupname)='" +ManageQuote(Workgroup).Trim().ToUpper()  + " ';"));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetWorkgroupandDesignationcount(string Workgroup, string Rolename,string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstgrouproles where upper(groupname)||'-'||upper( rolename)='" + ManageQuote(Workgroup).Trim().ToUpper() + "'||'-'||'" + ManageQuote(Rolename).Trim().ToUpper() + "';"));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetModulecount(string Modulename, string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstmodules where upper( modulename)='" + ManageQuote(Modulename).Trim().ToUpper() + "' and coalesce(parentmoduleid,0)=0;"));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetSubmenucountbyMenu(string Modulename, string Submodulename, string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstmodules where upper(modulename)='" + ManageQuote(Submodulename).Trim().ToUpper() + "' and upper(parentmodulename)='" + ManageQuote(Modulename).Trim().ToUpper() + "';"));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<MenuandNavigationDTO>> GetMenuandsubmenulist(string Connectionstring)
        {
            var MenuandNavigationDTOList = new List<MenuandNavigationDTO>();
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlDataReader dataReader = NPGSqlHelper.ExecuteReader(Connectionstring, CommandType.Text, "select functionid,parentmodulename,modulename,functionname,functionurl,tf.statusid from tblmstmodules tm join tblmstfunctions tf on  tm.moduleid=tf.submoduleid where tf.statusid=" + Convert.ToInt32(Status.Active) + " order by functionid desc; "))
                    {
                        while (dataReader.Read())
                        {
                            MenuandNavigationDTO _MenuandNavigationDTO = new MenuandNavigationDTO
                            {
                                pFunctionId = Convert.ToInt64(dataReader["functionid"]),
                                pModulename = Convert.ToString(dataReader["parentmodulename"]),
                                pSubmodulename = Convert.ToString(dataReader["modulename"]),
                                pFunctionname = Convert.ToString(dataReader["functionname"]),
                                pStatusid = Convert.ToString(dataReader["statusid"]),
                                pFunctionurl = Convert.ToString(dataReader["functionurl"])
                            };
                            MenuandNavigationDTOList.Add(_MenuandNavigationDTO);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            });

            return MenuandNavigationDTOList;
        }
        public int GetMenufunctionscount(long ParentId, long SubId, string functionname, string functionurl,string connectionString)
        {
            try
            {
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from tblmstfunctions where parentmoduleid =" + ParentId + " and submoduleid=" + SubId + " and upper(functionname)='" + ManageQuote(functionname).Trim().ToUpper() + "' and functionurl= '" + ManageQuote(functionurl).Trim() + "';"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
