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
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class CompanyController : ControllerBase
    {
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        private ILogger<EmployeeController> _logger;
        ICompany objCompany = new CompanyDAL();
        public CompanyDTO CompanyDTOdetails { set; get; }
        public CompanyController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment, ILogger<EmployeeController> logger)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
            _logger = logger;
        }
        [HttpPost]
        [Route("api/Settings/Company/SaveCompanyDetails")]
        public IActionResult SaveCompanyDetails([FromBody] CompanyDTO CompanyDTO)
        {
            bool isSaved = false;

            try
            {
                if (CompanyDTO.lstCompanyDocumentsDTO.Count > 0)
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
                    for (int i = 0; i < CompanyDTO.lstCompanyDocumentsDTO.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH))
                        {
                            string OldFullPath = Path.Combine(OldPath, CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH);
                            string NewFullPath = Path.Combine(newPath, CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH);
                            CompanyDTO.lstCompanyDocumentsDTO[i].pDOCSTOREPATH = NewFullPath;
                            if (System.IO.File.Exists(OldFullPath))
                            {
                                System.IO.File.Move(OldFullPath, NewFullPath);
                            }
                        }
                    }
                }
                isSaved = objCompany.SaveCompanyDetails(CompanyDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/Settings/Company/getCompanyDetails")]
        [HttpGet]
        public IActionResult getCompanyDetails()
        {
            CompanyDTOdetails = new CompanyDTO();
            try
            {
                CompanyDTOdetails = objCompany.getCompanyDetails(Con);
                if (CompanyDTOdetails != null)
                {
                    return Ok(CompanyDTOdetails);
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