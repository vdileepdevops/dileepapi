using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings;
using FinstaRepository.Interfaces.Settings;
using FinstaApi.Common;
using FinstaRepository.DataAccess.Settings;
using System.Net;
using System.Net.Http;
using FinstaInfrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FinstaApi.Controllers.Settings
{
   // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]

    public class EmployeeController : ControllerBase
    {
        IEmployee _employeeDAL = new EmployeeDAL();
        List<EmployeeDTO> employeeList = null;
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        private ILogger<EmployeeController> _logger;
        public EmployeeController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment, ILogger<EmployeeController> logger)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
            _logger = logger;
        }

        /// <summary>
        /// Saves Employee Role
        /// </summary>
        /// <param name="employeeRole"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Employee/SaveEmployeeRole")]
        public IActionResult SaveEmployeeRole([FromBody] EmployeeRole employeeRole)
        {
            try
            {
                return !string.IsNullOrEmpty(employeeRole.pEmployeeRoleName)
                    ? _employeeDAL.SaveEmployeeRole(employeeRole, Con) ? Ok(true) : (IActionResult)StatusCode((int)HttpStatusCode.NotModified)
                    : StatusCode((int)HttpStatusCode.NotAcceptable);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Save Employee Details on Passing Employee DTO
        /// </summary>
        /// <param name="employeeDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Employee/SaveEmployeeDetails")]
        public IActionResult SaveEmployeeDetails([FromBody] EmployeeDTO employeeDetails)
        {
            try
            {
                // Commented due to property name unknown from front end in kyc details for filename uploaded
                if (!string.IsNullOrEmpty(Convert.ToString(employeeDetails.pListEmpKYC)))
                {
                    if (employeeDetails.pListEmpKYC.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in employeeDetails.pListEmpKYC)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {

                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (_employeeDAL.SaveEmployeeDetails(employeeDetails, Con))
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
        /// Fetch All Employee details in Main Grid (View)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/Employee/GetallEmployeeDetails")]
        public async Task<IActionResult> GetallEmployeeDetails()
        {
            employeeList = new List<EmployeeDTO>();
            try
            {
                employeeList = await _employeeDAL.GetallEmployeeDetails(Con);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(employeeList);
        }

        /// <summary>
        /// Checks Duplicates whether Contact Exists as Employee Or Not
        /// /// </summary>
        /// <param name="employeeObj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Employee/checkEmployeeCountinMaster")]
        public int checkEmployeeCountinMaster(EmployeeDTO employeeObj)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(employeeObj.pContactId) != string.Empty)
                {
                    if (employeeObj.pContactId > 0)
                    {
                        count = _employeeDAL.checkEmployeeCountinMaster(employeeObj, Con);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
        /// <summary>
        ///  Fetching employee Details Based on ID 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>        
        [HttpGet]
        [Route("api/Settings/Employee/GetEmployeeDetailsOnId")]
        public async Task<IActionResult> GetEmployeeDetailsOnId(long employeeID)
        {
            EmployeeDTO employeeListObject = new EmployeeDTO();
            try
            {
                employeeListObject = await _employeeDAL.GetEmployeeDetailsOnId(employeeID, Con);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
                //throw new FinstaAppException(Ex.ToString());
            }
            if (employeeListObject != null)
            {
                return Ok(employeeListObject);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
        /// <summary>
        /// Updates and Deletes Specific Employee Details on passing Employee DTO and Main Transaction Type.
        /// </summary>
        /// <param name="employeeUpdateDTO"></param> 
        [HttpPost]
        [Route("api/Settings/Employee/updateEmployee")]
        public IActionResult UpdateEmployeeData([FromBody]  EmployeeDTO employeeUpdateDTO)
        {
            try
            {
                // Commented due to property name unknown from front end in kyc details for filename uploaded
                if (!string.IsNullOrEmpty(Convert.ToString(employeeUpdateDTO.pListEmpKYC)))
                {
                    if (employeeUpdateDTO.pListEmpKYC.Count > 0)
                    {
                        string OldFolder = "Upload";
                        string NewFolder = "Original";
                        string webRootPath = _hostingEnvironment.ContentRootPath;
                        string OldPath = Path.Combine(webRootPath, OldFolder);
                        string newPath = Path.Combine(webRootPath, NewFolder);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }
                        foreach (documentstoreDTO kycDoc in employeeUpdateDTO.pListEmpKYC)
                        {
                            if (!string.IsNullOrEmpty(kycDoc.pDocStorePath))
                            {
                                string OldFullPath = Path.Combine(OldPath, kycDoc.pDocStorePath);
                                string NewFullPath = Path.Combine(newPath, kycDoc.pDocStorePath);
                                kycDoc.pDocStorePath = NewFullPath;
                                if (System.IO.File.Exists(OldFullPath))
                                {
                                    System.IO.File.Move(OldFullPath, NewFullPath);
                                }
                            }
                        }
                    }
                }
                if (_employeeDAL.UpdateEmployeeData(employeeUpdateDTO, Con))
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
        /// Checks Duplicate Role Name Exists or not with Active status
        /// </summary>
        /// <param name="Rolename"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Settings/Employee/checkEmployeeRoleExistsOrNot")]
        public int checkEmployeeRoleExistsOrNot(string Rolename)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(Rolename) != string.Empty)
                {
                    count = _employeeDAL.checkEmployeeRoleExistsOrNot(Rolename, Con);
                }
            }
            catch (Exception)
            {
            }
            return count;
        }
    }
}