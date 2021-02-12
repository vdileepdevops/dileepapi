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
    public class RdConfigartionController : ControllerBase
    {
        IRdConfigaration objRdconfig = new RdConfigarationDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public RdConfigartionController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region Save's Recurring Deposit Configaration Type

        /// <summary>
        /// Save's Recurring Deposit Name And Code.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/SaverdNameAndCode")]
        public IActionResult SaverdNameAndCode([FromBody] RdNameAndCodeDTO Rdnameandcode)
        {
            try
            {
                if (objRdconfig.SaverdNameAndCode(Rdnameandcode, Con))
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
            //RdNameAndCodeDTO objRdnameandcode = new RdNameAndCodeDTO();
            //try
            //{
            //    long Rdconfigid = 0;
            //    long Chkcount = 0;
            //    objRdnameandcode = objRdconfig.SaverdNameAndCode(Rdnameandcode, Con);
            //    if (!string.IsNullOrEmpty(Rdname))
            //    {
            //        objRdnameandcode.pRdname = Rdname;
            //        objRdnameandcode.pRdconfigid = Rdconfigid;
            //        objRdnameandcode.pDuplicatecount = Chkcount;
            //        return Ok(objRdnameandcode);
            //    }
            //    else
            //    {
            //        return StatusCode(StatusCodes.Status304NotModified);
            //    }
            //}
            //catch (Exception)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

        }

        /// <summary>
        /// Save's Recurring Deposit Configaration Details.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/Saverdconfigarationdetails")]
        public IActionResult Saverdconfigarationdetails([FromBody] RdConfigartionDetails Rdconfiglist)
        {
            try
            {
                if (objRdconfig.Saverdconfigarationdetails(Rdconfiglist, Con))
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
        /// Save's Recurring Deposit Loan Facility Details.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/SaveRdloanfacilityDetails")]
        public IActionResult SaveRdloanfacilityDetails([FromBody] RdloanfacilityDetailsDTO RdloanDetails)
        {
            try
            {
                if (objRdconfig.SaveRdloanfacilityDetails(RdloanDetails, Con))
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
        /// Save's Recurring Deposit Referral Details.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/SaveRdReferralDetails")]
        public IActionResult SaveRdReferralDetails([FromBody] RdReferralCommissionDTO RdReferralCommission)
        {
            try
            {
                if (objRdconfig.SaveRdReferralDetails(RdReferralCommission, Con))
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
        #endregion

        #region Get Rd Config View Details
        /// <summary>
        /// Get Rd Config View Details.
        /// </summary>     
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdViewDetails")]
        public IActionResult GetRdViewDetails()
        {
            List<RdViewDTO> lstRdView = new List<RdViewDTO>();
            try
            {
                lstRdView = objRdconfig.GetRdViewDetails(Con);
                return lstRdView != null ? Ok(lstRdView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Rd Name And Code Details
        /// <summary>
        /// Get Rd Name And Code Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdNameAndCodeDetails")]
        public IActionResult GetRdNameAndCodeDetails(string RdName, string RdNameCode)
        {
            RdNameAndCodeDTO objRdNameandcode = new RdNameAndCodeDTO();
            try
            {
                objRdNameandcode = objRdconfig.GetRdNameAndCodeDetails(RdName, RdNameCode, Con);
                return objRdNameandcode != null ? Ok(objRdNameandcode) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion 

        #region Get Rd Config Details
        /// <summary>
        /// Get Rd Config Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdConfigurationDetails")]
        public IActionResult GetRdConfigurationDetails(string RdName, string RdNameCode)
        {
            RdConfigartionDetails objRdConfigdetails = new RdConfigartionDetails();
            try
            {
                objRdConfigdetails = objRdconfig.GetRdConfigurationDetails(RdName, RdNameCode, Con);
                return objRdConfigdetails != null ? Ok(objRdConfigdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Rd Loan Facilty Details
        /// <summary>
        /// Get Rd Loan Facilty Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdloanfacilityDetails")]
        public IActionResult GetRdloanfacilityDetails(string RdName, string RdNameCode)
        {
            RdloanfacilityDetailsDTO objRdLoandetails = new RdloanfacilityDetailsDTO();
            try
            {
                objRdLoandetails = objRdconfig.GetRdloanfacilityDetails(RdName, RdNameCode, Con);
                return objRdLoandetails != null ? Ok(objRdLoandetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Rd Referral Details
        /// <summary>
        /// Get Rd Referral Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdReferralDetails")]
        public IActionResult GetRdReferralDetails(string RdName, string RdNameCode)
        {
            RdReferralCommissionDTO objRdReferraldetails = new RdReferralCommissionDTO();
            try
            {
                objRdReferraldetails = objRdconfig.GetRdReferralDetails(RdName, RdNameCode, Con);
                return objRdReferraldetails != null ? Ok(objRdReferraldetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Save Identification Documents RD
        /// <summary>
        /// Save / Update Recurring Deposit Identification Documents Details 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/SaveRDIdentificationDocumentsDetails")]
        public IActionResult SaveRDIdentificationDocumentsDetails([FromBody]  IdentificationDocumentsDt IdentificationDocumentsDto)
        {
            try
            {
                if (objRdconfig.SaveIdentificationDocumentsRD(IdentificationDocumentsDto, Con))
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
        #endregion

        #region Get Identification Documents RD
        /// <summary>
        /// Get Recurring Deposit Identification Documents Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdIdentificationDocumentsDetails")]
        public IActionResult GetRdIdentificationDocumentsDetails(string RdName, string RdNameCode)
        {
            IdentificationDocumentsDt IdentificationDocumentsDto = new IdentificationDocumentsDt();
            try
            {
                IdentificationDocumentsDto = objRdconfig.GetIdentificationDocumentsRD(RdName, RdNameCode, Con);
                return IdentificationDocumentsDto != null ? Ok(IdentificationDocumentsDto) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Delete Rd Configuration
        /// <summary>
        /// Delete Rd Configuration
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/RdConfig/DeleteRdConfiguration")]
        public IActionResult DeleteRdConfiguration(long RdConfigId)
        {
            try
            {
                if (objRdconfig.DeleteRdConfiguration(RdConfigId, Con))
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
        #endregion

        #region Get RDName Count
        /// <summary>
        /// Check Duplicate RDName 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/GetRdNameCount")]
        public IActionResult GetRdNameCount(string RdName)
        {
            try
            {
                int count = objRdconfig.GetRdNameCount(RdName, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }
        #endregion
        [HttpGet]
        [Route("api/Banking/Masters/RdConfig/CheckDuplicateRDName")]
        public IActionResult CheckDuplicateRDName(Int64 RDconfigid, string RDName, string RdCode)
        {
            try
            {
                RdschemeandcodeCount _RdschemeandcodeCount = objRdconfig.GetRDNameCount(RDconfigid, RDName, RdCode, Con);
                if (_RdschemeandcodeCount != null)
                {
                    return Ok(_RdschemeandcodeCount);
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