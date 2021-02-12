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
    public class InsurenceConfigController : ControllerBase
    {
        IInsurenceConfig objInsurenceconfig = new InsurenceConfigDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public InsurenceConfigController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region Save Insurence Configaration
        /// <summary>
        /// Save's  Insurence Name And Code Configaration Type.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/InsurenceConfig/SaveInsurenceNameAndCode")]
        public IActionResult SaveInsurenceNameAndCode([FromBody]  InsurenceNameAndCodeDTO InsurenceNameAndCode)
        {
            InsurenceNameAndCodeDTO Insurencenamecode = new InsurenceNameAndCodeDTO();
            try
            {
                long insurenceconfigid = 0;
                string isurencename = objInsurenceconfig.SaveInsurenceNameAndCode(InsurenceNameAndCode, Con, out insurenceconfigid);
                if (!string.IsNullOrEmpty(isurencename))
                {
                    Insurencenamecode.pInsurencename = isurencename;
                    Insurencenamecode.pInsurenceconfigid = insurenceconfigid;
                    return Ok(Insurencenamecode);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Save's Insurence Configaration Details.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/InsurenceConfig/SaveInsurenceConfigDetails")]
        public IActionResult SaveInsurenceConfigDetails([FromBody] InsurenceConfigDTO InsurenceConfigDetails)
        {
            try
            {
                if (objInsurenceconfig.SaveInsurenceConfigDetails(InsurenceConfigDetails, Con))
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
        /// Save's Insurence Configaration Referral Details.
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/InsurenceConfig/SaveInsurenceReferralDetails")]
        public IActionResult SaveInsurenceReferralDetails([FromBody] insurenceReferralCommissionDTO InsurenceReferralDetails)
        {
            try
            {
                if (objInsurenceconfig.SaveInsurenceReferralDetails(InsurenceReferralDetails, Con))
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

        #region Get Insurence View Details
        /// <summary>
        /// Get Insurence View Details.
        /// </summary>     
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceViewDetails")]
        public IActionResult GetInsurenceViewDetails()
        {
            List<InsurenceConfigViewDTO> lstInsurenceView = new List<InsurenceConfigViewDTO>();
            try
            {
                lstInsurenceView = objInsurenceconfig.GetInsurenceViewDetails(Con);
                return lstInsurenceView != null ? Ok(lstInsurenceView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Insurence Name And Code Details
        /// <summary>
        /// Get Insurence Name And Code Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceNameAndCodeDetails")]
        public IActionResult GetInsurenceNameAndCodeDetails(string InsurenceName, string InsurenceNameCode)
        {
            InsurenceNameAndCodeDTO objInsurenceNameAndCode = new InsurenceNameAndCodeDTO();
            try
            {
                objInsurenceNameAndCode = objInsurenceconfig.GetInsurenceNameAndCodeDetails(InsurenceName, InsurenceNameCode, Con);
                return objInsurenceNameAndCode != null ? Ok(objInsurenceNameAndCode) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Insurence Configuration Details
        /// <summary>
        /// Get Insurence Configuration Detailss.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceConfigurationDetails")]
        public IActionResult GetInsurenceConfigurationDetails(string InsurenceName, string InsurenceNameCode)
        {
            InsurenceConfigDTO objInsurenceConfigdetails = new InsurenceConfigDTO();
            try
            {
                objInsurenceConfigdetails = objInsurenceconfig.GetInsurenceConfigurationDetails(InsurenceName, InsurenceNameCode, Con);
                return objInsurenceConfigdetails != null ? Ok(objInsurenceConfigdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Insurence Referral Details
        /// <summary>
        /// Get Insurence Referral Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceReferralDetails")]
        public IActionResult GetInsurenceReferralDetails(string InsurenceName, string InsurenceNameCode)
        {
            insurenceReferralCommissionDTO objinsurenceReferralCommission = new insurenceReferralCommissionDTO();
            try
            {
                objinsurenceReferralCommission = objInsurenceconfig.GetInsurenceReferralDetails(InsurenceName, InsurenceNameCode, Con);
                return objinsurenceReferralCommission != null ? Ok(objinsurenceReferralCommission) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Delete Insurence Configuration
        /// <summary>
        /// Delete Insurence Configuration
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/InsurenceConfig/DeleteInsurenceConfiguration")]
        public IActionResult DeleteInsurenceConfiguration(long InsurenceConfigId)
        {
            try
            {
                if (objInsurenceconfig.DeleteInsurenceConfiguration(InsurenceConfigId, Con))
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

        #region Get Insurence Name Count
        /// <summary>
        /// Check Duplicate RDName 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceNameCount")]
        public IActionResult GetInsurenceNameCount(string InsurenceName)
        {
            try
            {
                int count = objInsurenceconfig.GetInsurenceNameCount(InsurenceName, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Masters/InsurenceConfig/GetInsurenceCodeCount")]
        public IActionResult GetInsurenceCodeCount(string InsurenceCode)
        {
            try
            {
                int count = objInsurenceconfig.GetInsurenceCodeCount(InsurenceCode, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Masters/MemberType/CheckDuplicateInsuranceNameCode")]
        public IActionResult GetInsuranceNameCount(Int64 Insuranceid, string InsuranceName, string InsuranceCode)
        {
            try
            {
                InsuranceschemeandcodeCount _InsuranceschemeandcodeCount = objInsurenceconfig.GetInsuranceNameCount(Insuranceid, InsuranceName, InsuranceCode, Con);
                if (_InsuranceschemeandcodeCount != null)
                {
                    return Ok(_InsuranceschemeandcodeCount);
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
        #endregion
    }
}