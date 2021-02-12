using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings.Users;
using FinstaRepository.Interfaces.Settings.Users;
using FinstaApi.Common;
using FinstaRepository.DataAccess.Settings.Users;
using System.Net;
using System.Net.Http;
using FinstaInfrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Settings.Users
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class RolesCreationController : ControllerBase
    {
        IRoles _RolesDAL = new RolesDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public List<WorkGroupinrolesDTO> WorkgroupinrolesList { get; set; }
        public List<RolemodulesDTO> _RolemodulesDTOList { get; set; }
        public List<RolesubmodulesDTO> _RolesubmodulesDTOList { get; set; }
        public List<RolesDTO> _RolesDTOList { get; set; }

        public List<MenuandNavigationDTO> _MenuandSubmenuDetails { set; get; }
        public RolesCreationController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Save's Work Group in Role.
        /// </summary>
        /// <param name="_WorkGroupinrolesDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Users/RolesCreation/SaveGroupinRole")]
        public IActionResult SaveWorkGroupinRole([FromBody]  WorkGroupinrolesDTO _WorkGroupinrolesDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(_WorkGroupinrolesDTO.pGroupname))
                {
                    if (_RolesDAL.SaveWorkGroupinRole(_WorkGroupinrolesDTO, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Gets all Work Groups for Role.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetallWorkGroupsinRoles")]
        public async Task<IActionResult> GetallWorkGroupsinRoles()
        {
            WorkgroupinrolesList = new List<WorkGroupinrolesDTO>();
            try
            {
                WorkgroupinrolesList = await _RolesDAL.GetallWorkGroupsinRoles(Con);
                return WorkgroupinrolesList != null && WorkgroupinrolesList.Count > 0 ? Ok(WorkgroupinrolesList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Save's Role
        /// </summary>
        /// <param name="_RolesDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Users/RolesCreation/SaveRole")]
        public IActionResult SaveRole([FromBody] RolesDTO _RolesDTO)
        {
            try
            {
                bool IsValid = false;
                if (!string.IsNullOrEmpty(_RolesDTO.ptypeofoperation))
                {
                    if (_RolesDTO.ptypeofoperation.Trim().ToUpper() == "CREATE")
                    {
                        if (!string.IsNullOrEmpty(_RolesDTO.pGroupname) && !string.IsNullOrEmpty(_RolesDTO.pRoledesignation))
                        {
                            IsValid = true;
                        }
                    }
                    if (_RolesDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE" || _RolesDTO.ptypeofoperation.Trim().ToUpper() == "DELETE")
                    {
                        if (Convert.ToString(_RolesDTO.pRoleID) != string.Empty && _RolesDTO.pRoleID > 0)
                        {
                            IsValid = true;
                        }
                    }
                }
                if (IsValid)
                {
                    if (_RolesDAL.SaveRole(_RolesDTO, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Saves Role Module on passing Model object.
        /// </summary>
        /// <param name="_RolemodulesDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Users/RolesCreation/SaveRoleModule")]
        public IActionResult SaveRoleModule([FromBody] RolemodulesDTO _RolemodulesDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(_RolemodulesDTO.pModulename))
                {
                    if (_RolesDAL.SaveRoleModule(_RolemodulesDTO, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Saves Role Submodule on passing Model Object.
        /// </summary>
        /// <param name="_RolesubmodulesDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Users/RolesCreation/SaveRoleSubModule")]
        public IActionResult SaveRoleSubModule([FromBody] RolesubmodulesDTO _RolesubmodulesDTO)
        {
            try
            {
                if (!string.IsNullOrEmpty(_RolesubmodulesDTO.pModulename) && !string.IsNullOrEmpty(_RolesubmodulesDTO.pSubmodulename))
                {
                    if (_RolesDAL.SaveRoleSubModule(_RolesubmodulesDTO, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Gets all Role Modules.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetallRolesModules")]
        public async Task<IActionResult> GetallRolesModules()
        {
            _RolemodulesDTOList = new List<RolemodulesDTO>();
            try
            {
                _RolemodulesDTOList = await _RolesDAL.GetallRolesModules(Con);
                return _RolemodulesDTOList != null && _RolemodulesDTOList.Count > 0 ? Ok(_RolemodulesDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Gets Role Submodules on passing Module ID.
        /// </summary>
        /// <param name="Moduleid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetRolesSubModulesbyModule")]
        public async Task<IActionResult> GetRolesSubModulesbyModule(long Moduleid)
        {
            _RolesubmodulesDTOList = new List<RolesubmodulesDTO>();
            try
            {              
                    
                    if (Moduleid > 0)
                    {
                        _RolesubmodulesDTOList = await _RolesDAL.GetRolesSubModulesbyModule(Moduleid, Con);
                        return _RolesubmodulesDTOList != null && _RolesubmodulesDTOList.Count > 0 ? Ok(_RolesubmodulesDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
                    }
                
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        ///  Saves Role Functions .
        /// </summary>
        /// <param name="_MenuandNavigationDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Users/RolesCreation/SaveRoleFunction")]
        public IActionResult SaveRoleFunction([FromBody] MenuandNavigationDTO _MenuandNavigationDTO)
        {
            try
            {
                bool IsValid = false;

                if(!string.IsNullOrEmpty(_MenuandNavigationDTO.ptypeofoperation))
                {
                    if(_MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper()=="CREATE")
                    {
                        if (!string.IsNullOrEmpty(_MenuandNavigationDTO.pModulename) && !string.IsNullOrEmpty(_MenuandNavigationDTO.pSubmodulename) && !string.IsNullOrEmpty(_MenuandNavigationDTO.pFunctionname) && !string.IsNullOrEmpty(_MenuandNavigationDTO.pFunctionurl))
                        {
                            IsValid = true;
                        }                       
                    }
                    if (_MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper() == "UPDATE" || _MenuandNavigationDTO.ptypeofoperation.Trim().ToUpper() == "DELETE")
                    {
                        if(Convert.ToString(_MenuandNavigationDTO.pFunctionId) !=string.Empty && _MenuandNavigationDTO.pFunctionId>0)
                        {
                            IsValid = true;
                        }
                    }
                }
               if (IsValid)  
               {
                   if (_RolesDAL.SaveRoleFunction(_MenuandNavigationDTO, Con))
                   {
                       return Ok(true);
                   }
                   else
                   {
                       return StatusCode(StatusCodes.Status304NotModified);
                   }
               }
               else
               {
                   return StatusCode(StatusCodes.Status406NotAcceptable);
               }            
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Gets all Work-Groups and Designations (Both active and Inactive).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetWorkgroupandDesignations")]
        public async Task<IActionResult> GetWorkgroupandDesignations()
        {
            _RolesDTOList = new List<RolesDTO>();
            try
            {
                _RolesDTOList = await _RolesDAL.GetWorkgroupandDesignations(Con);
                return _RolesDTOList != null && _RolesDTOList.Count > 0 ? Ok(_RolesDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        ///  Get Work Group Count
        /// </summary>
        /// <param name="Workgroup"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetWorkgroupcount")]
        public int GetWorkgroupcount(string Workgroup)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(Workgroup) != string.Empty)
                {
                   
                   count = _RolesDAL.GetWorkgroupcount(Workgroup, Con);
                   
                }
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
        /// <summary>
        /// Checks Work-Group and Designation Combination count
        /// </summary>
        /// <param name="Workgroup"></param>
        /// <param name="Rolename"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetWorkgroupandDesignationcount")]
        public int GetWorkgroupandDesignationcount(string Workgroup,string Rolename)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(Workgroup) != string.Empty && !string.IsNullOrEmpty(Rolename))
                {
                    count = _RolesDAL.GetWorkgroupandDesignationcount(Workgroup, Rolename,Con);
                }
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
        /// <summary>
        ///  Get Module Count
        /// </summary>
        /// <param name="Modulename"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetModulecount")]
        public int GetModulecount(string Modulename)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(Modulename) != string.Empty)
                {
                    count = _RolesDAL.GetModulecount(Modulename, Con);
                }                
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
        /// <summary>
        /// Checks Module-SubModule Combination count
        /// </summary>
        /// <param name="Modulename"></param>
        /// <param name="Submodulename"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetSubmenucountbyMenu")]
        public int GetSubmenucountbyMenu(string Modulename, string Submodulename)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(Modulename) != string.Empty && !string.IsNullOrEmpty(Submodulename))
                {
                    count = _RolesDAL.GetSubmenucountbyMenu(Modulename, Submodulename, Con);
                }
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }

        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetMenuandSubmenuDetails")]
        public async Task<IActionResult> GetMenuandSubmenuDetails()
        {
            _MenuandSubmenuDetails = new List<MenuandNavigationDTO>();
            try
            {
                _MenuandSubmenuDetails = await _RolesDAL.GetMenuandsubmenulist(Con);
                return _MenuandSubmenuDetails != null && _MenuandSubmenuDetails.Count > 0 ? Ok(_MenuandSubmenuDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Checks Function Saving Duplicates on passing Menu,Submenu Id's and functionname,function url.
        /// </summary>
        /// <param name="MenuId"></param>
        /// <param name="SubMenuId"></param>
        /// <param name="functionname"></param>
        /// <param name="functionurl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/RolesCreation/GetMenuandfunctionCount")]
        public int GetMenuandfunctionCount(long MenuId, long SubMenuId, string functionname, string functionurl)
        {
            try
            {
                if (MenuId > 0 && SubMenuId > 0 && !string.IsNullOrEmpty(functionname) && !string.IsNullOrEmpty(functionurl))
                    return _RolesDAL.GetMenufunctionscount(MenuId, SubMenuId, functionname, functionurl, Con);
                else
                    return 0;
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
        }
    }
}