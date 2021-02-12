using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaInfrastructure.TDS;
using FinstaRepository.DataAccess.TDS;
using FinstaRepository.Interfaces.TDS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.TDS
{
   
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class TdsController : ControllerBase
    {
        ITdsReport objTDSReport = new TdsReportDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public TdsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [HttpGet]
        [Route("api/TDS/GetTdsReportDetails")]
        public async Task<IActionResult> GetTdsReportDetails(string FromDate, string ToDate, string SectionName)
        {
            try
            {
                List<TdsReportDTO> _ChitBranchDetailsList = await objTDSReport.GetTdsReportDetails(Con, FromDate, ToDate, SectionName);
                if (_ChitBranchDetailsList != null && _ChitBranchDetailsList.Count > 0)
                {
                    return Ok(_ChitBranchDetailsList);
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
    }
}