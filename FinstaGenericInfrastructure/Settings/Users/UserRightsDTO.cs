using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Settings.Users
{
  public class UserRightsDTO
    {
        public long pUserID { set; get; }
        public string pUserName { set; get; }
        public string pName { set; get; }
        public long pRoleid { set; get; }
        public string pDesignation { set; get; }
        public string pImagepath { set; get; }
        public List<string> pImage { set; get; }

        public List<ModuleDTO> ModuleDTOList { set; get; }
        public int pCreateby { set; get; }
    }

    public class UserRightsFunctionsDTO
    {
        public long pUserID { set; get; }
        public string pUserName { set; get; }
        public string pName { set; get; }
        public int pCreateby { set; get; }
        public List<FunctionsDTO> FunctionsDTOList { set; get; }
        
    }

    public class RoleDTO
    {
        public long proleid { set; get; }
        public string prolename { set; get; }
    }
    public class UserDTO
    {
        public long pUserID { set; get; }
        public string pUserName { set; get; }
    }
    public class ModuleDTO
    {
            public long pmoduleid { set; get; }
            public string pmodulename { set; get; }
        public string pcssclass { set; get; }

        //public long psubmoduleid { set; get; }
        //public string psubmodulename { set; get; }
        public List<SubModuleDTO> lstSubModuleDTO { set; get; }
           
   }
   
    public class SubModuleDTO
    {
        //public long pmoduleid { set; get; }
        //public string pmodulename { set; get; }
        public long psubmoduleid { set; get; }
        public string psubmodulename { set; get; }
        public List<FunctionsDTO> FunctionsDTOList { set; get; }
    }
    public class FunctionsDTO
    {
        public long pmoduleid { set; get; }
        public string pmodulename { set; get; }
        public long psubmoduleid { set; get; }
        public string psubmodulename { set; get; }
        public long pFunctionID { set; get; }
        public string pFunctionName { set; get; }
        public string pFunctionUrl { set; get; }
        public string pCssclass { set; get; }
        public bool pIsviewpermission { set; get; }
        public bool pIscreatepermission { set; get; }
        public bool pIsupdatepermission { set; get; }
        public bool pIsdeletepermission { set; get; }
    }
}
