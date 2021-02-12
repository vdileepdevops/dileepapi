using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings;
using FinstaRepository.Interfaces.Settings;
using FinstaApi.Common;
using FinstaRepository.DataAccess.Settings;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Settings
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class GroupCreationController : ControllerBase
    {
        IGroupCreation  _groupcreationDAL = new GroupCreationDAL();        
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        List<GroupRoleDTO> groupRoleObj = null;
        List<GroupCreationDTO> groupDetailsObj = null;
        public GroupCreationController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Saves Group Role
        /// </summary>
        /// <param name="groupRole"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/GroupCreation/SaveGroupRole")]
        public IActionResult SaveGroupRole([FromBody] GroupRoleDTO groupRole)
        {          
            try
            {
                if (!string.IsNullOrEmpty(groupRole.pGroupRoleName))
                {
                   if( _groupcreationDAL.SaveGroupRole(groupRole, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.NotModified);
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
                throw ex;
            }           
        }
        /// <summary>
        /// Checks Duplicate Group Roles
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/GroupCreation/checkGroupRoleCountinMaster")]
        public int checkGroupRoleCountinMaster(string roleName)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(roleName) != string.Empty)
                {                   
                   count = _groupcreationDAL.checkGroupRoleCountinMaster(roleName, Con);                    
                }               
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
        /// <summary>
        /// Save Group Configuration Data
        /// </summary>
        /// <param name="groupObj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/GroupCreation/saveGroupConfig")]      
        public IActionResult saveGroupConfiguration([FromBody] GroupCreationDTO groupObj)
        {            
            try
            {
                if( _groupcreationDAL.saveGroupConfiguration(groupObj, Con))
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }
            catch (Exception Ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
                throw Ex;
            }           
        }
        /// <summary>
        /// Checks Contact exists or not for the Group Name (Duplicate)
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="ContactId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/GroupCreation/CheckMemberinGroup")]
        public int checkContactDuplicateinGroup([FromBody] string GroupName,long ContactId)
        {
            try
            {
                return  _groupcreationDAL.checkContactDuplicateinGroup(GroupName, ContactId, Con);
            }
            catch (Exception)
            {               
                throw;
            }
        }
        /// <summary>
        /// Get Group Roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/GroupCreation/GetRoles")]
        public async Task< IActionResult> getGroupRoles()
        {
            groupRoleObj = new List<GroupRoleDTO>();
            try
            {                
                groupRoleObj = await _groupcreationDAL.getGroupRoles(Con);
                if(groupRoleObj!=null && groupRoleObj.Count>0)
                {
                    return Ok(groupRoleObj);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);                
            }
        }
        /// <summary>
        /// Get all Group Details for Main View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/GroupCreation/GetallGroupDetails")]
        public async Task<IActionResult> GetallEmployeeDetailsAsync()
        {
            groupDetailsObj = new List<GroupCreationDTO>();
            try
            {
                groupDetailsObj =await _groupcreationDAL.GetallGroupsDetailsAsync(Con);
                if(groupDetailsObj!=null && groupDetailsObj.Count>0)
                {
                    return Ok(groupDetailsObj);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);                
            }           
        }
        /// <summary>
        /// Update Group Member Details on Edit
        /// </summary>
        /// <param name="groupUpdateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/GroupCreation/UpdateGroupDetails")]
        public IActionResult UpdateGroupDetails([FromBody]  GroupCreationDTO groupUpdateDTO)
        {
            try
            {
              if( _groupcreationDAL.UpdateGroupDetails(groupUpdateDTO, Con))            
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);                
            }
        }
        /// <summary>
        ///  Fetching employee Details Based on ID 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>        
        [HttpGet]
        [Route("api/Settings/GroupCreation/GetGroupMembersDetailsOnId")]
        public async Task<IActionResult> GetGroupMembersDetailsOnIdAsync(long groupID)
        {
            GroupCreationDTO groupchildObject = new GroupCreationDTO();
            try
            {
                groupchildObject = await _groupcreationDAL.GetGroupMembersDetailsOnIdAsync(groupID, Con);
                if (groupchildObject != null)
                {
                    return Ok(groupchildObject);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);                
            }
        }      
    }
}
