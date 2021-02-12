using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Loans.Reports;
using FinstaRepository.DataAccess.Loans.Reports;
using FinstaRepository.Interfaces.Loans.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FinstaApi.Controllers.Loans.Reports
{
    [Authorize]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionReportController : ControllerBase
    {
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;
        ICollectionReport ObjCollectionrepoort = new CollectionReportDAL();
        public List<CollectionReportDTO> lstColletionsummary { get; set; }
        public List<CollectionReportDetailsDTO> lstColletiondetails { get; set; }
        public CollectionReportController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        #region GetColletionsummary
        [HttpGet]
        [Route("api/Loans/Reports/GetColletionsummary")]
        public IActionResult GetColletionsummary(string fromdate, string todate, int recordid, string fieldname, string fieldtype)
        {
            lstColletionsummary = new List<CollectionReportDTO>();
            try
            {
                lstColletionsummary = ObjCollectionrepoort.GetColletionsummary(fromdate,todate,Con, recordid, fieldname, fieldtype);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstColletionsummary);

        }
        #endregion

        #region GetColletiondetails
        [HttpGet]
        [Route("api/Loans/Reports/GetColletiondetails")]
        public IActionResult GetColletiondetails(string fromdate, string todate, string Applicationid)
        {
            lstColletiondetails = new List<CollectionReportDetailsDTO>();
            try
            {
                lstColletiondetails = ObjCollectionrepoort.GetColletiondetails(fromdate, todate, Applicationid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstColletiondetails);

        }
        #endregion
    }
}