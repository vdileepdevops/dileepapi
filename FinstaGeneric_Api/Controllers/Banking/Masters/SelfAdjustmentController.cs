using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Banking.Masters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Masters
{
    // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class SelfAdjustmentController : ControllerBase
    {
        ISelfAdjustment objSelfAdjustment = new SelfAdjustmentDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public SelfAdjustmentController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Get Company Name Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetCompanyname")]
        public async Task<IActionResult> GetCompanyname()
        {
            List<CompanyNamesDTO> lstCompanyname = new List<CompanyNamesDTO>();
            try
            {
                lstCompanyname = await objSelfAdjustment.GetCompanyname(Con);
                return lstCompanyname != null ? Ok(lstCompanyname) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get Branch Name Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetBranchName")]
        public async Task<IActionResult> GetBranchName(string Companyname)
        {
            List<BranchNamesDTO> lstBranchname = new List<BranchNamesDTO>();
            try
            {
                lstBranchname = await objSelfAdjustment.GetBranchName(Companyname, Con);
                return lstBranchname != null ? Ok(lstBranchname) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get Scheme Type Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetSchemeType")]
        public async Task<IActionResult> GetSchemeType(string BranchName)
        {
            List<SchemeTypeDTO> lstSchemetype = new List<SchemeTypeDTO>();
            try
            {
                lstSchemetype = await objSelfAdjustment.GetSchemeType(BranchName, Con);
                return lstSchemetype != null ? Ok(lstSchemetype) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get Member Name Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetMembers")]
        public async Task<IActionResult> GetMembers(string branchname, Int64 fdconfigid)
        {
            List<MembersDTO> lstMembers = new List<MembersDTO>();
            try
            {
                lstMembers = await objSelfAdjustment.GetMembers(branchname, fdconfigid, Con);
                return lstMembers != null ? Ok(lstMembers) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get FD Account Numbers
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetFdAcnumbers")]
        public async Task<IActionResult> GetFdAcnumbers(string branchname, int memberid, Int64 fdconfigid)
        {
            List<FdAccountDTO> lstFDAccountno = new List<FdAccountDTO>();
            try
            {
                lstFDAccountno = await objSelfAdjustment.GetFdAcnumbers(branchname, memberid, fdconfigid, Con);
                return lstFDAccountno != null ? Ok(lstFDAccountno) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get Bank Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetBankDetails")]
        public async Task<IActionResult> GetBankDetails(int Contactid)
        {
            List<SelfBankDetailsDTO> lstBankdetails = new List<SelfBankDetailsDTO>();
            try
            {
                lstBankdetails = await objSelfAdjustment.GetBankDetails(Contactid, Con);
                return lstBankdetails != null ? Ok(lstBankdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }





        /// <summary>
        /// Save / Update Self Or Adjustment Payments
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/SelfAdjustment/SaveSelforAdjustment")]
        public IActionResult SaveSelforAdjustment([FromBody]  SelfAdjustmentConfigDTO Selfadjustment)
        {
            try
            {
                if (objSelfAdjustment.SaveSelforAdjustment(Selfadjustment, Con))
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
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        /// <summary>
        /// View Self Or Adjustment  Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/ViewSelfAdjustmendtetails")]
        public async Task<IActionResult> GetSelfAdjustmendtetails()
        {
            List<SelfAdjustmentConfigDTO> lstSelfAdjustment = new List<SelfAdjustmentConfigDTO>();
            try
            {
                lstSelfAdjustment = await objSelfAdjustment.ViewSelfAdjustmendtetails(Con);
                return lstSelfAdjustment != null ? Ok(lstSelfAdjustment) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Get Self Or Adjustment  Details By Id
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetSelfAdjustmendtetailsByid")]
        public async Task<IActionResult> GetSelfAdjustmendtetailsByid(int memberid, int fdid)
        {
            List<SelfAdjustmentConfigDTO> lstSelfAdjustment = new List<SelfAdjustmentConfigDTO>();
            try
            {
                lstSelfAdjustment = await objSelfAdjustment.GetSelfAdjustmendtetailsByid(memberid, fdid, Con);
                return lstSelfAdjustment != null ? Ok(lstSelfAdjustment) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Masters/SelfAdjustment/GetFDBranchDetails")]
        public async Task<IActionResult> GetFDBranchDetails()
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await objSelfAdjustment.GetFDBranchDetails(Con);
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