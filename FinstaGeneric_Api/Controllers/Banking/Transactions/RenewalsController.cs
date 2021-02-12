using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Banking.Transactions;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Transactions
{
    // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class RenewalsController : ControllerBase
    {
        IRenewals objRenewals = new RenewalsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public RenewalsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get Scheme Type
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/Renewals/GetSchemeType")]
        public async Task<IActionResult> GetSchemeType()
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            try
            {
                lstSchemetype = await objRenewals.GetSchemeType(Con);
                return lstSchemetype != null ? Ok(lstSchemetype) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Get Fd Accounts
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/Renewals/GetFdaccounts")]
        public async Task<IActionResult> GetFdaccounts()
        {
            List<FDAccountsDTO> lstFdAccounts = new List<FDAccountsDTO>();
            try
            {
                lstFdAccounts = await objRenewals.GetFdaccounts(Con);
                return lstFdAccounts != null ? Ok(lstFdAccounts) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Get Fd Account Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/Renewals/GetFdAccountDetails")]
        public async Task<IActionResult> GetFdAccountDetails(int fdid)
        {
            List<FDAccountDetailsDTO> lstFdAccountDetails = new List<FDAccountDetailsDTO>();
            try
            {
                lstFdAccountDetails = await objRenewals.GetFdAccountDetails(fdid,Con);
                return lstFdAccountDetails != null ? Ok(lstFdAccountDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }
}