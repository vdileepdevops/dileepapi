using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Loans.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Loans.Reports;
using FinstaRepository.Interfaces.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.Loans.Reports
{
    [Authorize]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class DuesSummaryReportController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        IDuesSummaryReport _DuesSummaryReportDAL = new DuesSummaryReportDAL();        
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;        
        List<DuesSummaryReportDTO> _DuesSummaryReportDTOList { get; set; }
        public DuesSummaryReportController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Get Dues Summary Report onm passing Loan Account No.
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="recordid"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldtype"></param>
        /// <param name="duestype"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetDuesSummaryReport")]
        public async Task<IActionResult> GetDuesSummaryReport(string FromDate, string ToDate,int recordid,string fieldname, string fieldtype,string duestype)
        {
            _DuesSummaryReportDTOList = new List<DuesSummaryReportDTO>();
            try
            {
                if (Convert.ToString(FromDate) !=string.Empty && Convert.ToString(ToDate) != string.Empty)
                {
                    _DuesSummaryReportDTOList = await _DuesSummaryReportDAL.GetDuesSummaryReport(Con,  FromDate,  ToDate, recordid, fieldname, fieldtype, duestype);
                    return _DuesSummaryReportDTOList != null && _DuesSummaryReportDTOList.Count>0 ? Ok(_DuesSummaryReportDTOList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}