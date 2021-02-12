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
    public class UserRightsDAL : SettingsDAL, IUserRights
    {
        NpgsqlConnection con = null;
        NpgsqlTransaction trans = null;
        NpgsqlDataReader dr = null;
        DataSet ds = null;
        public UserRightsDTO UserRightsDTO { set; get; }
        List<ModuleDTO> lstModuleDTO { set; get; }
        List<RoleDTO> lstRoleDTO { set; get; }
        List<UserDTO> lstUserDTO { set; get; }
        public async Task<List<RoleDTO>> BindRoles(string ConnectionString)
        {
            await Task.Run(() =>
            {
                lstRoleDTO = new List<RoleDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(roleid,0) as roleid,coalesce(rolename,'') as rolename from tblmstemployeerole where statusid="+Convert.ToInt32(Status.Active)+";"))
                {
                    while (dr.Read())
                    {
                        RoleDTO RoleDTO = new RoleDTO();
                        RoleDTO.proleid = Convert.ToInt64(dr["roleid"]);
                        RoleDTO.prolename = dr["rolename"].ToString();
                        lstRoleDTO.Add(RoleDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            });
            return lstRoleDTO;
        }
        public async Task<List<UserDTO>> BindUsers(string ConnectionString)
        {
            await Task.Run(() =>
            {
                lstUserDTO = new List<UserDTO>();
            try
            {
                using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "select coalesce(userid,0) as userid,coalesce(username,'') as username from tblmstusers where  statusid ="+Convert.ToInt32(Status.Active)+";"))
                {
                    while (dr.Read())
                    {
                        UserDTO UserDTO = new UserDTO();
                        UserDTO.pUserID = Convert.ToInt64(dr["userid"]);
                        UserDTO.pUserName = dr["username"].ToString();
                        lstUserDTO.Add(UserDTO);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            });
                return lstUserDTO;
        }
        public async Task<UserRightsFunctionsDTO> GetallUserModules(string Type, string UserOrDesignation, string connectionString)
        {
            UserRightsFunctionsDTO UserRightsFunctionsDTO = new UserRightsFunctionsDTO();
            UserRightsFunctionsDTO.FunctionsDTOList = new List<FunctionsDTO>();
            List<FunctionsDTO> FunctionsDTOList = new List<FunctionsDTO>();
            List<ModuleDTO> ModuleDTOlist = new List<ModuleDTO>();
            List<SubModuleDTO> SubModuleDTOList = new List<SubModuleDTO>();
            string Query = string.Empty;
            long Userid = 0;
            int RoleFunctionsCount = 0;
            int RoleID = 0;
            ds = new DataSet();
            await Task.Run(() =>
            {
                UserRightsDTO = new UserRightsDTO();
                UserRightsDTO.ModuleDTOList = new List<ModuleDTO>();
                try
                {
                    if (string.IsNullOrEmpty(Type) && string.IsNullOrEmpty(UserOrDesignation))
                    {
                        Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where statusid=" + Convert.ToInt32(Status.Active) + ";";
                    }
                    else
                    {
                        if (Type.ToUpper().Trim() == "USER")
                        {
                            RoleFunctionsCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select count(*) from   tblmstrolefunctions where upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';"));
                            Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select coalesce(userid,0) as userid from tblmstusers where upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
                            UserRightsDTO.pUserID = Userid;
                            UserRightsDTO.pUserName = UserOrDesignation;
                            UserRightsFunctionsDTO.pUserID= Userid;
                            UserRightsFunctionsDTO.pUserName= UserOrDesignation;
                            if (RoleFunctionsCount > 0)                            {
                                // Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where functionid not in (select functionid from tblmstrolefunctions where userid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + ";";
                                Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where statusid=" + Convert.ToInt32(Status.Active) + " and functionid not in (select functionid from tblmstrolefunctions where userid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + " and t2.statusid=" + Convert.ToInt32(Status.Active) + ";";
                            }
                            else
                            {
                                RoleID = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select coalesce(roleid,0) as roleid from tblmstusers  where upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';"));
                                if(RoleID>0)
                                {
                                    Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where statusid=" + Convert.ToInt32(Status.Active) + " and functionid not in (select functionid from tblmstrolefunctions where roleid=" + RoleID + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + RoleID + " and t2.statusid=" + Convert.ToInt32(Status.Active) + ";";
                                }
                                else
                                {
                                    Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where statusid=" + Convert.ToInt32(Status.Active) + " and functionid not in (select functionid from tblmstrolefunctions where userid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + " and t2.statusid=" + Convert.ToInt32(Status.Active) + ";";
                                }
                            }
                        }
                        else
                        {
                            Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select coalesce(roleid,0) as roleid from tblmstemployeerole where statusid=" + Convert.ToInt32(Status.Active) + " and  upper(rolename)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
                            UserRightsDTO.pUserID = Userid;
                            UserRightsDTO.pUserName = UserOrDesignation;
                            UserRightsFunctionsDTO.pUserID = Userid;
                            UserRightsFunctionsDTO.pUserName = UserOrDesignation;
                            //Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where functionid not in (select functionid from tblmstrolefunctions where roleid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + Userid + ";";
                            Query = "select functionid,parentmoduleid,submoduleid,functionname,functionurl,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where statusid=" + Convert.ToInt32(Status.Active) + " and functionid not in (select functionid from tblmstrolefunctions where roleid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + Userid + " and t2.statusid=" + Convert.ToInt32(Status.Active) + ";";
                        }
                    }                   
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select moduleid,modulename from tblmstmodules where statusid=" + Convert.ToInt32(Status.Active) + " and parentmoduleid is null  order by modulesortorder asc;"))
                    {
                        while (dr.Read())
                        {
                            ModuleDTO ModuleDTO = new ModuleDTO();
                            ModuleDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
                            ModuleDTO.pmodulename = dr["modulename"].ToString();                          
                            ModuleDTO.lstSubModuleDTO = new List<SubModuleDTO>();
                            
                            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select  f1.moduleid,f1.modulename,f2.moduleid as submoduleid,f2.modulename as submodulename from tblmstmodules f1 INNER JOIN tblmstmodules f2 ON f1.moduleid = f2.parentmoduleid where f1.statusid=" + Convert.ToInt32(Status.Active) + " order by f2.modulesortorder asc;"))
                            {
                                while (dr1.Read())
                                {
                                    if (Convert.ToInt64(dr["moduleid"])== Convert.ToInt64(dr1["moduleid"]))
                                    {
                                        SubModuleDTO SubModuleDTO = new SubModuleDTO();                                      
                                        SubModuleDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
                                        SubModuleDTO.psubmodulename = dr1["submodulename"].ToString();
                                        SubModuleDTO.FunctionsDTOList = new List<FunctionsDTO>();
                                   
                                        using (NpgsqlDataReader dr2 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
                                        {
                                            while (dr2.Read())
                                            {
                                                if (Convert.ToInt64(dr2["submoduleid"]) == Convert.ToInt64(dr1["submoduleid"]))
                                                {
                                                    FunctionsDTO FunctionsDTO = new FunctionsDTO();
                                                    FunctionsDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
                                                    FunctionsDTO.pmodulename= dr["modulename"].ToString();
                                                    FunctionsDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
                                                    FunctionsDTO.psubmodulename = dr1["submodulename"].ToString();
                                                    FunctionsDTO.pFunctionID = Convert.ToInt64(dr2["functionid"]);
                                                    FunctionsDTO.pFunctionName = dr2["functionname"].ToString();
                                                    FunctionsDTO.pFunctionUrl = dr2["functionurl"].ToString();
                                                    FunctionsDTO.pIsviewpermission = Convert.ToBoolean(dr2["viewpermission"].ToString());
                                                    FunctionsDTO.pIscreatepermission = Convert.ToBoolean(dr2["createpermission"].ToString());
                                                    FunctionsDTO.pIsupdatepermission = Convert.ToBoolean(dr2["updatepermission"].ToString());
                                                    FunctionsDTO.pIsdeletepermission = Convert.ToBoolean(dr2["deletepermission"].ToString());
                                                    SubModuleDTO.FunctionsDTOList.Add(FunctionsDTO);
                                                    UserRightsFunctionsDTO.FunctionsDTOList.Add(FunctionsDTO);
                                                }
                                            }
                                        }
                                  
                                            ModuleDTO.lstSubModuleDTO.Add(SubModuleDTO);
                                    
                                    }
                                }
                            }
                           
                            UserRightsDTO.ModuleDTOList.Add(ModuleDTO);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return UserRightsFunctionsDTO;
        }
        public async Task<UserRightsDTO> GetUserModulesBasedOnroleanduserid(string UserName,string connectionString)
        {
            List<FunctionsDTO> FunctionsDTOList = new List<FunctionsDTO>();
            List<ModuleDTO> ModuleDTOlist = new List<ModuleDTO>();
            List<SubModuleDTO> SubModuleDTOList = new List<SubModuleDTO>();
            int RoleFunctionsCount = 0;
            string Query = string.Empty;
            long Userid = 0;
            long roleid = 0;
            string Name = string.Empty;
            string Desiognation = string.Empty;
            string Imagepath = string.Empty;
            ds = new DataSet();
            await Task.Run(() =>
            {

                UserRightsDTO = new UserRightsDTO();
                UserRightsDTO.ModuleDTOList = new List<ModuleDTO>();

                try
                {

                    RoleFunctionsCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(connectionString,CommandType.Text, "select count(*) from   tblmstrolefunctions where upper(username)='"+ ManageQuote(UserName.ToUpper().Trim()) + "';"));
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select coalesce(t1.roleid,0) as roleid,coalesce(t1.userid,0) as userid,coalesce(t1.employeename,t1.username) as name,coalesce(t1.designation,'') as designation,coalesce(t2.contactimagepath,'') as imagepath from tblmstusers t1 left join tblmstcontact t2 on t1.contactrefid=t2.contactreferenceid where upper(t1.username)='" + ManageQuote(UserName.ToUpper().Trim()) + "';"))
                    {
                        while (dr.Read())
                        {
                            Userid = Convert.ToInt64(dr["userid"]);
                            roleid = Convert.ToInt64(dr["roleid"]);
                            Name= dr["name"].ToString();
                            Desiognation= dr["designation"].ToString();
                            Imagepath= dr["imagepath"].ToString();
                        }
                    }                    
                    if (roleid==0||string.IsNullOrEmpty(roleid.ToString()))
                    {
                        Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t2.cssclass,'') as cssclass,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + " and t2.statusid=" + Convert.ToInt32(Status.Active) + " order by t2.submoduleid,t2.functionsortorder  asc;";
                        // Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t2.cssclass,'') as cssclass,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + ";";
                    }
                    else
                    {

                        if (RoleFunctionsCount > 0)
                        {
                            Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t2.cssclass,'') as cssclass,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + " and t2.statusid="+Convert.ToInt32(Status.Active)+ " order by t2.submoduleid,t2.functionsortorder  asc;";
                        }
                        else
                        {
                            Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t2.cssclass,'') as cssclass,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + roleid + " and t2.statusid=" + Convert.ToInt32(Status.Active) + " order by t2.submoduleid,t2.functionsortorder  asc;";
                        }
                            // Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,t2.functionurl,coalesce(t2.cssclass,'') as cssclass,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + roleid + ";";
                    }
                    UserRightsDTO.pUserID = Userid;
                    UserRightsDTO.pUserName = UserName;
                    UserRightsDTO.pName = Name;
                    UserRightsDTO.pRoleid = roleid;
                    if(RoleFunctionsCount>0)
                    {
                        UserRightsDTO.pDesignation = "USER";
                    }
                    else
                    {
                        UserRightsDTO.pDesignation =Desiognation;
                    }
                    
                    UserRightsDTO.pImagepath = Imagepath;
                    using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select moduleid,modulename,coalesce(cssclass,'') as cssclass from tblmstmodules where statusid=" + Convert.ToInt32(Status.Active) + " and parentmoduleid is null order by modulesortorder asc;"))
                    {
                        while (dr.Read())
                        {
                            ModuleDTO ModuleDTO = new ModuleDTO();
                            ModuleDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
                            ModuleDTO.pmodulename = dr["modulename"].ToString();  
                            ModuleDTO.pcssclass= dr["cssclass"].ToString();
                            ModuleDTO.lstSubModuleDTO = new List<SubModuleDTO>();
                            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select  f1.moduleid,f1.modulename,f2.moduleid as submoduleid,f2.modulename as submodulename from tblmstmodules f1 INNER JOIN tblmstmodules f2 ON f1.moduleid = f2.parentmoduleid where f1.statusid=" + Convert.ToInt32(Status.Active) + " order by f2.modulesortorder asc;"))
                            {
                                while (dr1.Read())
                                {
                                    if (Convert.ToInt64(dr["moduleid"]) == Convert.ToInt64(dr1["moduleid"]))
                                    {
                                        SubModuleDTO SubModuleDTO = new SubModuleDTO();                                      
                                        SubModuleDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
                                        SubModuleDTO.psubmodulename = dr1["submodulename"].ToString();
                                        SubModuleDTO.FunctionsDTOList = new List<FunctionsDTO>();
                                        using (NpgsqlDataReader dr2 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
                                        {
                                            while (dr2.Read())
                                            {
                                                if (Convert.ToInt64(dr2["submoduleid"]) == Convert.ToInt64(dr1["submoduleid"]))
                                                {
                                                    FunctionsDTO FunctionsDTO = new FunctionsDTO();

                                                    FunctionsDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
                                                    FunctionsDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
                                                    FunctionsDTO.pFunctionID = Convert.ToInt64(dr2["functionid"]);
                                                    FunctionsDTO.pFunctionName = dr2["functionname"].ToString();
                                                    FunctionsDTO.pFunctionUrl= dr2["functionurl"].ToString();
                                                    FunctionsDTO.pCssclass = dr2["cssclass"].ToString();
                                                    FunctionsDTO.pIsviewpermission = Convert.ToBoolean(dr2["viewpermission"].ToString());
                                                    FunctionsDTO.pIscreatepermission = Convert.ToBoolean(dr2["createpermission"].ToString());
                                                    FunctionsDTO.pIsupdatepermission = Convert.ToBoolean(dr2["updatepermission"].ToString());
                                                    FunctionsDTO.pIsdeletepermission = Convert.ToBoolean(dr2["deletepermission"].ToString());
                                                    SubModuleDTO.FunctionsDTOList.Add(FunctionsDTO);
                                                }
                                            }
                                        }
                                        if (SubModuleDTO.FunctionsDTOList.Count > 0)
                                        {
                                            ModuleDTO.lstSubModuleDTO.Add(SubModuleDTO);
                                        }
                                        else
                                        {
                                            //ModuleDTO.lstSubModuleDTO.Add(null);
                                        }
                                       
                                    }
                                }
                            }
                            if (ModuleDTO.lstSubModuleDTO.Count>0)
                            {
                                UserRightsDTO.ModuleDTOList.Add(ModuleDTO);
                            }
                        }
                    }
                  
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return UserRightsDTO;
        }

        public async Task<bool> SaveUserRight(string Type, string UserOrDesignation, UserRightsFunctionsDTO UserRightsFunctionsDTO, string connectionString)
        {
            SubModuleDTO SubModuleDTO = new SubModuleDTO();
            StringBuilder sbinsert = new StringBuilder();
            bool IsSaved = false;
            long Userid = 0;
            await Task.Run(() =>
            {
                try
            {
                con = new NpgsqlConnection(connectionString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (Type.ToUpper().Trim() == "USER")
                {
                    Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select userid from tblmstusers where upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
                    sbinsert.Append("delete from tblmstrolefunctions where userid=" + Userid + " and upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';");
                }
              else
                { 
                    Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select roleid from tblmstemployeerole where statusid=" + Convert.ToInt32(Status.Active) + " and  upper(rolename)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
                    sbinsert.Append("delete from tblmstrolefunctions where roleid=" + Userid + ";");
                }
              if(UserRightsFunctionsDTO.FunctionsDTOList.Count>0)
                {
                    for (int i = 0; i < UserRightsFunctionsDTO.FunctionsDTOList.Count; i++)
                    {
                        if (UserRightsFunctionsDTO.FunctionsDTOList[i].pIsviewpermission == true || UserRightsFunctionsDTO.FunctionsDTOList[i].pIscreatepermission == true || UserRightsFunctionsDTO.FunctionsDTOList[i].pIsupdatepermission == true || UserRightsFunctionsDTO.FunctionsDTOList[i].pIsdeletepermission == true)
                        {
                            if (Type.ToUpper().Trim() == "USER")
                            {
                                sbinsert.Append("INSERT INTO tblmstrolefunctions(functionid,moduleid,userid,username,viewpermission,createpermission,updatepermission,deletepermission,statusid,createdby,createddate)VALUES (" + UserRightsFunctionsDTO.FunctionsDTOList[i].pFunctionID + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].psubmoduleid + "," + Userid + ",'" + ManageQuote(UserOrDesignation) + "'," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsviewpermission + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIscreatepermission + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsupdatepermission + ", " + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsdeletepermission + "," + Convert.ToInt32(Status.Active) + "," + UserRightsFunctionsDTO.pCreateby + ",current_timestamp);");
                            }
                            else
                            {
                                sbinsert.Append("INSERT INTO tblmstrolefunctions(functionid,roleid,moduleid,viewpermission,createpermission,updatepermission,deletepermission,statusid,createdby,createddate)VALUES (" + UserRightsFunctionsDTO.FunctionsDTOList[i].pFunctionID + "," + Userid + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].psubmoduleid + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsviewpermission + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIscreatepermission + "," + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsupdatepermission + ", " + UserRightsFunctionsDTO.FunctionsDTOList[i].pIsdeletepermission + "," + Convert.ToInt32(Status.Active) + "," + UserRightsFunctionsDTO.pCreateby + ",current_timestamp);");
                            }
                        }
                    }
                }
                if (Convert.ToString(sbinsert) != string.Empty)
                {
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
                }
                trans.Commit();
                IsSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
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
            });
            return IsSaved;
        }




        //public async Task<UserRightsDTO> GetallUserModules(string Type, string UserOrDesignation, string connectionString)
        //{
        //    List<FunctionsDTO> FunctionsDTOList = new List<FunctionsDTO>();
        //    List<ModuleDTO> ModuleDTOlist = new List<ModuleDTO>();
        //    string Query = string.Empty;
        //    long Userid = 0;
        //    ds = new DataSet();
        //    await Task.Run(() =>
        //    {
        //        UserRightsDTO = new UserRightsDTO();
        //        UserRightsDTO.ModuleDTOList = new List<ModuleDTO>();

        //        try
        //        {
        //            if (Type == "User")
        //            {
        //                Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select userid from tblmstusers where upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
        //                UserRightsDTO.pUserID = Userid;
        //                UserRightsDTO.pUserName = UserOrDesignation;
        //                Query = "select functionid,parentmoduleid,submoduleid,functionname,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where functionid not in (select functionid from tblmstrolefunctions where userid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + ";";
        //            }
        //            else
        //            {
        //                Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select roleid from tblmstemployeerole where statusid=1 and  upper(rolename)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
        //                UserRightsDTO.pUserID = Userid;
        //                UserRightsDTO.pUserName = UserOrDesignation;
        //                Query = "select functionid,parentmoduleid,submoduleid,functionname,false as viewpermission,false as createpermission,false as updatepermission,false as deletepermission from tblmstfunctions where functionid not in (select functionid from tblmstrolefunctions where roleid=" + Userid + ") union select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + Userid + ";";
        //            }
        //            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
        //            {
        //                while (dr1.Read())
        //                {
        //                    FunctionsDTO FunctionsDTO = new FunctionsDTO();
        //                    FunctionsDTO.pmoduleid = Convert.ToInt64(dr1["parentmoduleid"]);
        //                    FunctionsDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
        //                    FunctionsDTO.pFunctionID = Convert.ToInt64(dr1["functionid"]);
        //                    FunctionsDTO.pFunctionName = dr1["functionname"].ToString();
        //                    FunctionsDTO.pIsviewpermission = Convert.ToBoolean(dr1["viewpermission"].ToString());
        //                    FunctionsDTO.pIscreatepermission = Convert.ToBoolean(dr1["createpermission"].ToString());
        //                    FunctionsDTO.pIsupdatepermission = Convert.ToBoolean(dr1["updatepermission"].ToString());
        //                    FunctionsDTO.pIsdeletepermission = Convert.ToBoolean(dr1["deletepermission"].ToString());
        //                    FunctionsDTOList.Add(FunctionsDTO);
        //                }
        //            }
        //            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select  f1.moduleid,f1.modulename,f2.moduleid as submoduleid,f2.modulename as submodulename from tblmstmodules f1 INNER JOIN tblmstmodules f2 ON f1.moduleid = f2.parentmoduleid;"))
        //            {
        //                while (dr.Read())
        //                {
        //                    ModuleDTO ModuleDTO = new ModuleDTO();
        //                    ModuleDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
        //                    ModuleDTO.pmodulename = dr["modulename"].ToString();
        //                    ModuleDTO.psubmoduleid = Convert.ToInt64(dr["submoduleid"]);
        //                    ModuleDTO.psubmodulename = dr["submodulename"].ToString();
        //                    ModuleDTOlist.Add(ModuleDTO);
        //                }
        //            }
        //            for (int i = 0; i < ModuleDTOlist.Count; i++)
        //            {
        //                for (int j = 0; j < FunctionsDTOList.Count; j++)
        //                {
        //                    if (ModuleDTOlist[i].pmoduleid == FunctionsDTOList[j].pmoduleid && ModuleDTOlist[i].psubmoduleid == FunctionsDTOList[j].psubmoduleid)
        //                    {
        //                        UserRightsDTO.ModuleDTOList.Add(new ModuleDTO
        //                        {
        //                            pmoduleid = ModuleDTOlist[i].pmoduleid,
        //                            pmodulename = ModuleDTOlist[i].pmodulename,
        //                            psubmoduleid = ModuleDTOlist[i].psubmoduleid,
        //                            psubmodulename = ModuleDTOlist[i].psubmodulename,
        //                            FunctionsDTOList = new List<FunctionsDTO>() { new FunctionsDTO { pmoduleid = ModuleDTOlist[i].pmoduleid, pmodulename = ModuleDTOlist[i].pmodulename, psubmoduleid = ModuleDTOlist[i].psubmoduleid, psubmodulename = ModuleDTOlist[i].psubmodulename, pFunctionID = FunctionsDTOList[j].pFunctionID, pFunctionName = FunctionsDTOList[j].pFunctionName, pIsviewpermission = FunctionsDTOList[j].pIsviewpermission, pIscreatepermission = FunctionsDTOList[j].pIscreatepermission, pIsupdatepermission = FunctionsDTOList[j].pIsupdatepermission, pIsdeletepermission = FunctionsDTOList[j].pIsdeletepermission } }
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    });
        //    return UserRightsDTO;
        //}
        //public async Task<List<ModuleDTO>> GetUserModulesBasedOnroleanduserid(string UserName, string connectionString)
        //{
        //    List<FunctionsDTO> FunctionsDTOList = new List<FunctionsDTO>();
        //    List<ModuleDTO> ModuleDTOlist = new List<ModuleDTO>();
        //    string Query = string.Empty;
        //    long Userid = 0;
        //    long roleid = 0;

        //    ds = new DataSet();
        //    await Task.Run(() =>
        //    {

        //        lstModuleDTO = new List<ModuleDTO>();

        //        try
        //        {
        //            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select coalesce(roleid,0) as roleid,coalesce(userid,0) as userid from tblmstusers where  upper(username)='" + ManageQuote(UserName.ToUpper().Trim()) + "';"))
        //            {
        //                while (dr.Read())
        //                {
        //                    Userid = Convert.ToInt64(dr["userid"]);
        //                    roleid = Convert.ToInt64(dr["roleid"]);
        //                }
        //            }
        //            if (roleid == 0 || string.IsNullOrEmpty(roleid.ToString()))
        //            {
        //                Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.userid=" + Userid + ";";
        //            }
        //            else
        //            {
        //                Query = "select t2.functionid,t2.parentmoduleid,t2.submoduleid,t2.functionname,coalesce(t1.viewpermission,false) as viewpermission,coalesce(t1.createpermission,false)as createpermission,coalesce(t1.updatepermission,false)as updatepermission,coalesce(t1.deletepermission,false) as deletepermission from tblmstrolefunctions t1 left join  tblmstfunctions t2 on t1.functionid=t2.functionid where t1.roleid=" + roleid + ";";
        //            }
        //            using (NpgsqlDataReader dr1 = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, Query))
        //            {
        //                while (dr1.Read())
        //                {
        //                    FunctionsDTO FunctionsDTO = new FunctionsDTO();
        //                    FunctionsDTO.pmoduleid = Convert.ToInt64(dr1["parentmoduleid"]);
        //                    FunctionsDTO.psubmoduleid = Convert.ToInt64(dr1["submoduleid"]);
        //                    FunctionsDTO.pFunctionID = Convert.ToInt64(dr1["functionid"]);
        //                    FunctionsDTO.pFunctionName = dr1["functionname"].ToString();
        //                    FunctionsDTO.pIsviewpermission = Convert.ToBoolean(dr1["viewpermission"].ToString());
        //                    FunctionsDTO.pIscreatepermission = Convert.ToBoolean(dr1["createpermission"].ToString());
        //                    FunctionsDTO.pIsupdatepermission = Convert.ToBoolean(dr1["updatepermission"].ToString());
        //                    FunctionsDTO.pIsdeletepermission = Convert.ToBoolean(dr1["deletepermission"].ToString());
        //                    FunctionsDTOList.Add(FunctionsDTO);
        //                }
        //            }
        //            using (NpgsqlDataReader dr = NPGSqlHelper.ExecuteReader(connectionString, CommandType.Text, "select  f1.moduleid,f1.modulename,f2.moduleid as submoduleid,f2.modulename as submodulename from tblmstmodules f1 INNER JOIN tblmstmodules f2 ON f1.moduleid = f2.parentmoduleid;"))
        //            {
        //                while (dr.Read())
        //                {
        //                    ModuleDTO ModuleDTO = new ModuleDTO();
        //                    ModuleDTO.pmoduleid = Convert.ToInt64(dr["moduleid"]);
        //                    ModuleDTO.pmodulename = dr["modulename"].ToString();
        //                    ModuleDTO.psubmoduleid = Convert.ToInt64(dr["submoduleid"]);
        //                    ModuleDTO.psubmodulename = dr["submodulename"].ToString();
        //                    ModuleDTOlist.Add(ModuleDTO);
        //                }
        //            }
        //            for (int i = 0; i < ModuleDTOlist.Count; i++)
        //            {
        //                for (int j = 0; j < FunctionsDTOList.Count; j++)
        //                {
        //                    if (ModuleDTOlist[i].pmoduleid == FunctionsDTOList[j].pmoduleid && ModuleDTOlist[i].psubmoduleid == FunctionsDTOList[j].psubmoduleid)
        //                    {
        //                        lstModuleDTO.Add(new ModuleDTO
        //                        {
        //                            pmoduleid = ModuleDTOlist[i].pmoduleid,
        //                            pmodulename = ModuleDTOlist[i].pmodulename,
        //                            psubmoduleid = ModuleDTOlist[i].psubmoduleid,
        //                            psubmodulename = ModuleDTOlist[i].psubmodulename,
        //                            FunctionsDTOList = new List<FunctionsDTO>() { new FunctionsDTO { pmoduleid = ModuleDTOlist[i].pmoduleid, pmodulename = ModuleDTOlist[i].pmodulename, psubmoduleid = ModuleDTOlist[i].psubmoduleid, psubmodulename = ModuleDTOlist[i].psubmodulename, pFunctionID = FunctionsDTOList[j].pFunctionID, pFunctionName = FunctionsDTOList[j].pFunctionName, pIsviewpermission = FunctionsDTOList[j].pIsviewpermission, pIscreatepermission = FunctionsDTOList[j].pIscreatepermission, pIsupdatepermission = FunctionsDTOList[j].pIsupdatepermission, pIsdeletepermission = FunctionsDTOList[j].pIsdeletepermission } }
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    });
        //    return lstModuleDTO;
        //}

        //public bool SaveUserRight(string Type, string UserOrDesignation, UserRightsDTO UserRightsDTO, string connectionString)
        //{
        //    StringBuilder sbinsert = new StringBuilder();
        //    bool IsSaved = false;
        //    long Userid = 0;
        //    try
        //    {
        //        con = new NpgsqlConnection(connectionString);
        //        if (con.State != ConnectionState.Open)
        //        {
        //            con.Open();
        //        }
        //        trans = con.BeginTransaction();
        //        if (Type == "User")
        //        {
        //            Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select userid from tblmstusers where username='" + UserOrDesignation + "';").ToString());
        //            sbinsert.Append("delete from tblmstrolefunctions where userid=" + Userid + " and upper(username)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';");
        //        }
        //        else
        //        {
        //            Userid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(connectionString, CommandType.Text, "select roleid from tblmstemployeerole where statusid=1 and  upper(rolename)='" + ManageQuote(UserOrDesignation.ToUpper().Trim()) + "';").ToString());
        //            sbinsert.Append("delete from tblmstrolefunctions where roleid=" + Userid + ";");
        //        }
        //        if (UserRightsDTO != null)
        //        {
        //            for (int i = 0; i < UserRightsDTO.ModuleDTOList.Count; i++)
        //            {
        //                if (UserRightsDTO.ModuleDTOList[i].FunctionsDTOList.Count > 0)
        //                {
        //                    for (int j = 0; j < UserRightsDTO.ModuleDTOList[i].FunctionsDTOList.Count; j++)
        //                    {
        //                        if (UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsviewpermission == true || UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIscreatepermission == true || UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsupdatepermission == true || UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsdeletepermission == true)
        //                        {
        //                            if (Type == "User")
        //                            {
        //                                sbinsert.Append("INSERT INTO tblmstrolefunctions(functionid,moduleid,userid,username,viewpermission,createpermission,updatepermission,deletepermission,statusid,createdby,createddate)VALUES (" + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pFunctionID + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pmoduleid + "," + Userid + ",'" + ManageQuote(UserOrDesignation) + "'," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsviewpermission + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIscreatepermission + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsupdatepermission + ", " + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsdeletepermission + "," + Convert.ToInt32(Status.Active) + ",1,current_timestamp);");
        //                            }
        //                            else
        //                            {
        //                                sbinsert.Append("INSERT INTO tblmstrolefunctions(functionid,roleid,moduleid,viewpermission,createpermission,updatepermission,deletepermission,statusid,createdby,createddate)VALUES (" + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pFunctionID + "," + Userid + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pmoduleid + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsviewpermission + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIscreatepermission + "," + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsupdatepermission + ", " + UserRightsDTO.ModuleDTOList[i].FunctionsDTOList[j].pIsdeletepermission + "," + Convert.ToInt32(Status.Active) + ",1,current_timestamp);");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (Convert.ToString(sbinsert) != string.Empty)
        //        {
        //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, sbinsert.ToString());
        //        }
        //        trans.Commit();
        //        IsSaved = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        trans.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Dispose();
        //            con.Close();
        //            con.ClearPool();
        //            trans.Dispose();
        //        }
        //    }
        //    return IsSaved;
        //}
    }
}
