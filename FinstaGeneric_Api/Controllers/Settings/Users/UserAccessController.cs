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
using FinstaApi.Security.Hashing;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Settings.Users
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class UserAccessController : ControllerBase
    {
        private readonly IPasswordHasher _passwordHasher;
        IUserAccess objUserAccess = new UserAccessDAL();
        List<ContactEmployeeDTO> lstContactEmployeeDTO { set; get; }
        List<UserAccessDTO> lstUserAccessDTO { set; get; }

        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public UserAccessController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment, IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get All Un Registered Employees (i.e Un Registered Users)
        /// </summary>

        [Route("api/Settings/Users/UserAccess/GetAllEmployees")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            lstContactEmployeeDTO = new List<ContactEmployeeDTO>();
            try
            {
                lstContactEmployeeDTO =await objUserAccess.GetContactDetails(Con);
                if (lstContactEmployeeDTO != null && lstContactEmployeeDTO.Count > 0)
                {
                    return Ok(lstContactEmployeeDTO);
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
        /// Get All Registred User View
        /// </summary>

        [Route("api/Settings/Users/UserAccess/GetUserView")]
        [HttpGet]
        public async Task<IActionResult> GetUserView()
        {
            lstUserAccessDTO = new List<UserAccessDTO>();
            try
            {
                lstUserAccessDTO =await objUserAccess.GetAllUsersView(Con);
                if (lstUserAccessDTO != null && lstUserAccessDTO.Count > 0)
                {
                    return Ok(lstUserAccessDTO);
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
        /// Get  Employee Role Based On Employee ID
        /// </summary>

        [Route("api/Settings/Users/UserAccess/GetEmployeeRole")]
        [HttpGet]
        public async Task<IActionResult> GetEmployeeRole(long Employeeid)
        {
            string RoleName = string.Empty;
            try
            {
                RoleName = await objUserAccess.GetRoleName(Employeeid, Con);
                if (RoleName != null)
                {
                    return Ok(RoleName);
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
        /// Save User Registration
        /// </summary>

        [HttpPost]
        [Route("api/Settings/Users/UserAccess/SaveUserAccess")]
        public IActionResult SaveUserAccess(UserAccessDTO UserAccessDTO)
        {
            bool isSaved = false;
            try
            {
                string Password =objUserAccess.GetDeafultPassword(Con);
                UserAccessDTO.pPassword = _passwordHasher.HashPassword(Password);
                isSaved =objUserAccess.SaveUserAccess(UserAccessDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

        /// <summary>
        /// Retuns User Count Based On User Name .if count is greater then 0 user Name Already Exist 
        /// </summary>
        [HttpGet]
        [Route("api/Settings/Users/UserAccess/CheckUserName")]
        public async Task<IActionResult> CheckUserName(string UserName)
        {
            int count = 0;
            try
            {              
                count =await objUserAccess.CheckUserName(UserName,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }


        /// <summary>
        /// Retuns User Count Based On Contact Reference ID .if count is greater then 0 Contact Reference ID Already Exist 
        /// </summary>
        [HttpGet]
        [Route("api/Settings/Users/UserAccess/CheckUsercontactRefID")]
        public async Task<IActionResult> CheckUsercontactRefID(string Contactrefid)
        {
            int count = 0;
            try
            {
                count =await objUserAccess.CheckUsercontactRefID(Contactrefid,Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        /// <summary>
        /// Check User Login
        /// </summary>
        [HttpGet]
        [Route("api/Settings/Users/UserAccess/checkUserLogin")]
        public IActionResult checkUserLogin(string UserName, string Password)
        {
            UserAccessDTO user = new UserAccessDTO();
            try
            {
                user = objUserAccess.CheckUser(UserName, Password, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(user);
        }

        /// <summary>
        /// Registred User Change Password Based On User Name
        /// </summary>

        [Route("api/Settings/Users/UserAccess/ChangePassword")]
        [HttpPost]
        public  IActionResult ChangePassword(string Username, string password)
        {

            bool isSaved = false;
            try
            {
                string GenPass =_passwordHasher.HashPassword(password);
                isSaved = objUserAccess.UpdateuserPassword(Username, GenPass, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

        /// <summary>
        /// Registred User IN-ACTIVE / ACTIVE Based on User Id
        /// </summary>
        [HttpPost]
        [Route("api/Settings/Users/UserAccess/UserActiveInactive")]
        public async Task<IActionResult> UserActiveInactive(long Userid,bool Status)
        {
            bool isSaved = false;
            try
            {              
                isSaved =await objUserAccess.DeleteUser(Userid,Status,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

        /// <summary>
        /// Reset Password Based On User Name
        /// </summary>
        [Route("api/Settings/Users/UserAccess/ResetPassword")]
        [HttpPost]
        public IActionResult ResetPassword(string Username)
        {

            bool isSaved = false;
            try
            {
                string Password = objUserAccess.GetDeafultPassword(Con);
                string GenPass = _passwordHasher.HashPassword(Password);
                isSaved = objUserAccess.UpdateuserPassword(Username,GenPass,Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }

    }
}