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
   // [ServiceFilter(typeof(ExceptionFilter))]
    [ApiController]
    //[ExceptionFilter(logger:)]
    public class AccountStatementController : ControllerBase
    {
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;         
        IAccountStatement _AccountstmntreportDAL = new AccountStatementDAL();
        public AccountStatementDTO _AccountStatementDTO { get; set; }

        public AccountStatementController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get Account Statement Report on Passing Loan Account No.
        /// </summary>
        /// <param name="VchapplicationID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Loans/Reports/GetAccountstatementReport")]
        public async Task<IActionResult> GetAccountstatementReport(string VchapplicationID)
        {
            _AccountStatementDTO = new AccountStatementDTO();
            try
            {
                if (!string.IsNullOrEmpty(VchapplicationID))
                {
                    _AccountStatementDTO = await _AccountstmntreportDAL.GetAccountstatementReport(Con, VchapplicationID);
                    return _AccountStatementDTO != null ? Ok(_AccountStatementDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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