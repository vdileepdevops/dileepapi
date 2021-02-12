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
    public class BranchController : ControllerBase
    {
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        private ILogger<EmployeeController> _logger;
        public BranchDTO BranchDTOdetails { set; get; }
        IBranch objBranch = new BranchDAL();
        public BranchController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment, ILogger<EmployeeController> logger)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
            _logger = logger;
        }
        [HttpPost]
        [Route("api/Settings/Branch/SaveBranchDetails")]
        public IActionResult SaveBranchDetails([FromBody] BranchDTO BranchDTO)
        {
            bool isSaved = false;
            try
            {
                if (BranchDTO.lstBranchDocStoreDTO.Count > 0)
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
                    for (int i = 0; i < BranchDTO.lstBranchDocStoreDTO.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH))
                        {
                            string OldFullPath = Path.Combine(OldPath, BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH);
                            string NewFullPath = Path.Combine(newPath, BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH);
                            BranchDTO.lstBranchDocStoreDTO[i].pDOCSTOREPATH = NewFullPath;
                            if (System.IO.File.Exists(OldFullPath))
                            {
                                System.IO.File.Move(OldFullPath, NewFullPath);
                            }
                        }
                    }
                }
                isSaved = objBranch.SaveBranchDetails(BranchDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }

        [Route("api/Settings/Branch/getBranchDetails")]
        [HttpGet]
        public IActionResult getBranchDetails()
        {
            BranchDTOdetails = new BranchDTO();
            try
            {
                BranchDTOdetails = objBranch.getBranchDetails(Con);
                if (BranchDTOdetails != null)
                {
                    return Ok(BranchDTOdetails);
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

        [Route("api/Settings/Branch/checkbranchnameDuplicates")]
        [HttpGet]
        public IActionResult checkbranchnameDuplicates(string branchname, string branchcode, int branchid)
        {
            string errormessage = string.Empty;

            try
            {
                errormessage = objBranch.checkbranchnameDuplicates(branchname.ToUpper(), branchcode, branchid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(errormessage);
        }

    }
}