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
    public class FdConfigartionController : ControllerBase
    {
        IFdConfigartion objFDconfig = new FdConfigartionDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public FdConfigartionController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        ///// <summary>
        ///// Save's Fixed Deposit Configaration Type.
        ///// </summary>     
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/Masters/FdConfig/SaveFDconfigaration")]
        //public IActionResult SaveFDconfigaration([FromBody]  FdConfigartionDTO FdConfigartionDTO)
        //{
        //    try
        //    {
        //        if (objFDconfig.SaveFDconfigaration(FdConfigartionDTO, Con))
        //        {
        //            return Ok(true);
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status304NotModified);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //        throw;
        //    }
        //}
        /// <summary>
        /// Save / Update Fixed Deposit Name And Code 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/SaveFDNameAndCode")]
        public IActionResult SaveFDNameAndCode([FromBody]  FdNameAndCodeDTO FdNameAndCodeDTO)
        {
            try
            {
                if (objFDconfig.SaveFdNameAndCode(FdNameAndCodeDTO, Con))
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
        /// Get Fixed Deposit Name And Code 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdNameAndCode")]
        public async Task<IActionResult> GetFdNameAndCode(string FDName, string FdNameCode)
        {
            FdNameAndCodeDTO FdNameAndCodeDTO = new FdNameAndCodeDTO();
            try
            {
                FdNameAndCodeDTO =await objFDconfig.GetFdNameAndCodeDetails(FDName, FdNameCode, Con);
                return FdNameAndCodeDTO != null ? Ok(FdNameAndCodeDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        /// <summary>
        /// Save / Update Fixed Deposit Configuration Details 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/SaveFDConfigurationDetails")]
        public IActionResult SaveFDConfigurationDetails([FromBody]  FdConfigDeatails FdConfigDeatails)
        {
            try
            {
                if (objFDconfig.SaveFDConfigurationDetails(FdConfigDeatails, Con))
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
        /// Get Fixed Deposit Configuration Details 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdConfigurationDetails")]
        public async Task<IActionResult> GetFdConfigurationDetails(string FDName, string FdNameCode)
        {
            FdConfigDeatails FdConfigDeatails = new FdConfigDeatails();
            try
            {
                FdConfigDeatails =await objFDconfig.GetFDConfigurationDetails(FDName, FdNameCode, Con);
                return FdConfigDeatails != null ? Ok(FdConfigDeatails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Save / Update Fixed Deposit Loan Facility Details 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/SaveFDLoanFacilityDetails")]
        public IActionResult SaveFDLoanFacilityDetails([FromBody]  FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO)
        {
            try
            {
                if (objFDconfig.SaveLoanFacility(FDloanfacilityDetailsDTO, Con))
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
        /// Get Fixed Deposit Loan Facility Details 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdLoanFacilityDetails")]
        public async Task<IActionResult> GetFdLoanFacilityDetails(string FDName, string FdNameCode)
        {
            FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO = new FDloanfacilityDetailsDTO();       
            try
            {
                FDloanfacilityDetailsDTO =await objFDconfig.GetLoanFacilityDetails(FDName, FdNameCode, Con);
                return FDloanfacilityDetailsDTO != null ? Ok(FDloanfacilityDetailsDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Save / Update Fixed Deposit Referral Commission Details 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/SaveFDReferralCommissionDetails")]
        public IActionResult SaveFDReferralCommissionDetails([FromBody]  FDReferralCommissionDTO FDReferralCommissionDTO)
        {
            try
            {
                if (objFDconfig.SaveFdReferralDeatils(FDReferralCommissionDTO, Con))
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
        /// Get Fixed Deposit Referral Commission Details 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdReferralCommissionDetails")]
        public async Task<IActionResult> GetFdReferralCommissionDetails(string FDName, string FdNameCode)
        {
            FDReferralCommissionDTO FDReferralCommissionDTO = new FDReferralCommissionDTO();
            try
            {
                FDReferralCommissionDTO =await objFDconfig.GetFDReferralCommission(FDName, FdNameCode, Con);
                return FDReferralCommissionDTO != null ? Ok(FDReferralCommissionDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }




        /// <summary>
        /// Save / Update Fixed Deposit Identification Documents Details 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/SaveFDIdentificationDocumentsDetails")]
        public IActionResult SaveFDIdentificationDocumentsDetails([FromBody]  IdentificationDocumentsDto IdentificationDocumentsDto)
        {
            try
            {
                if (objFDconfig.SaveIdentificationDocumentsFD(IdentificationDocumentsDto, Con))
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
        /// Get Fixed Deposit Identification Documents Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdIdentificationDocumentsDetails")]
        public async Task<IActionResult> GetFdIdentificationDocumentsDetails(string FDName, string FdNameCode)
        {
            IdentificationDocumentsDto IdentificationDocumentsDto = new IdentificationDocumentsDto();
            try
            {
                IdentificationDocumentsDto =await objFDconfig.GetIdentificationDocumentsFD(FDName, FdNameCode, Con);
                return IdentificationDocumentsDto != null ? Ok(IdentificationDocumentsDto) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        /// <summary>
        /// Get FD Config View Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/GetFdViewDetails")]
        public async Task<IActionResult> GetFdViewDetails()
        {
            List<FDViewDTO> lstFdView = new List<FDViewDTO>();
            try
            {
                lstFdView =await objFDconfig.GetFdViewDetails(Con);
                return lstFdView != null ? Ok(lstFdView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Delete Fixed Deposit Configuration 
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FdConfig/DeleteFixedDepositConfig")]
        public IActionResult DeleteFixedDepositConfig(long FdConfigId)
        {
            try
            {
                if (objFDconfig.DeleteFdConfiguration(FdConfigId, Con))
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
        /// Check Duplicate FD Names 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FdConfig/CheckDuplicateFDName")]
        public IActionResult CheckDuplicateFDName(Int64 FDConfigid, string FDName,string FdnameCode)
        {           
            try
            {
                FdschemeandcodeCount _FdschemeandcodeCount = objFDconfig.GetFDNameCount(FDConfigid, FDName, FdnameCode,Con);
                if(_FdschemeandcodeCount!=null)
                {
                    return Ok(_FdschemeandcodeCount);
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