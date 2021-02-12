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
    public class UserRightsController : ControllerBase
    {
        IUserRights UserRightsDAL = new UserRightsDAL();
        public UserRightsDTO UserRightsDTO { set; get; }
        List<RoleDTO> lstRoleDTO { set; get; }
        public UserRightsFunctionsDTO UserRightsFunctionsDTO { set; get; }
        List<UserDTO> lstUserDTO { set; get; }
        List<ModuleDTO> lstModuleDTO { set; get; }
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public UserRightsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Gets User Rights View Based On Passing Parameters Type (User Or Designation) and if designation then Designation Name if User Then User Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/UserRights/GetUserRights")]
        public async Task<IActionResult> GetUserRights(string Type,string UserOrDesignation)
        {
            UserRightsFunctionsDTO = new UserRightsFunctionsDTO();
            try
            {
                UserRightsFunctionsDTO = await UserRightsDAL.GetallUserModules(Type, UserOrDesignation,Con);
                return UserRightsFunctionsDTO != null  ? Ok(UserRightsFunctionsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [NonAction]
        public List<string> getConvertImagepathtobase64(string strPath)
        {
            string base64String = string.Empty;
            List<string> imagebase64stringlist = new List<string>();
            string filePath = string.Empty;
            try
            {

                // strPath = "D:\\GITHUB\\FINSTA\\FinstaGeneric_Api\\ContactImages\\03d81568-f89a-469a-8d34-92f97ca4f433.jpg";
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(strPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes1 = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes1);
                        imagebase64stringlist.Add(base64String);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return imagebase64stringlist;
        }
        /// <summary>
        /// Menu Binding Based User Name (Assigned In  User Rights Form)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Users/UserRights/GetUserRightsBasedonUserName")]
        public async Task<IActionResult> GetUserRightsBasedonUserName(string UserName)
        {
            UserRightsDTO = new UserRightsDTO();
            try
            {
                UserRightsDTO = await UserRightsDAL.GetUserModulesBasedOnroleanduserid(UserName,Con);
                if(!string.IsNullOrEmpty(UserRightsDTO.pImagepath))
                {
                    UserRightsDTO.pImage = getConvertImagepathtobase64(UserRightsDTO.pImagepath);
                }
                return UserRightsDTO != null ? Ok(UserRightsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        } 
        /// <summary>
        /// Saving User Rights  Based On Passing Parameters Type (User Or Designation) and if designation then Designation Name if User Then User Name And Function Names
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("api/Settings/Users/UserRights/SaveUserRight")]
        public async Task<IActionResult> SaveUserRight(string Type, string UserOrDesignation, UserRightsFunctionsDTO UserRightsFunctionsDTO)
        {         
                bool isSaved = false;
                try
                {
                    isSaved = await UserRightsDAL.SaveUserRight(Type, UserOrDesignation, UserRightsFunctionsDTO, Con);
                }
                catch (Exception ex)
                {
                    throw new FinstaAppException(ex.ToString());
                }
                return Ok(isSaved);
           
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        
        [Route("api/Settings/Users/UserRights/GetRoles")]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            lstRoleDTO = new List<RoleDTO>();
            try
            {
                lstRoleDTO = await UserRightsDAL.BindRoles(Con);
                if (lstRoleDTO != null && lstRoleDTO.Count > 0)
                {
                    return Ok(lstRoleDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        /// <summary>
        /// Get All Users
        /// </summary>

        [Route("api/Settings/Users/UserRights/GetUsers")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            lstUserDTO = new List<UserDTO>();
            try
            {
                lstUserDTO = await UserRightsDAL.BindUsers(Con);
                if (lstUserDTO != null && lstUserDTO.Count > 0)
                {
                    return Ok(lstUserDTO);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
    }
}