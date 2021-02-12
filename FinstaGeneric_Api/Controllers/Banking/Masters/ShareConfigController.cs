using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Settings.Users;
using FinstaRepository.Interfaces.Settings.Users;
using FinstaApi.Common;
using FinstaRepository.DataAccess.Settings.Users;
using System.Net;
using System.Net.Http;
using FinstaInfrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Banking.Masters;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Banking.Masters;

namespace FinstaApi.Controllers.Banking.Masters
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ShareConfigController : ControllerBase
    {
        IShareConfig ObjShareConfig = new ShareConfigDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ShareConfigController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        ///// <summary>
        ///// Save's Share Configuration Details .
        ///// </summary>     
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/Banking/Masters/ShareConfig/SaveShareConfig")]
        //public IActionResult SaveShareConfig([FromBody]  ShareConfigDTO ShareConfigDTO)
        //{
        //    try
        //    {
        //        if (ObjShareConfig.SaveShareConfig(ShareConfigDTO, Con))
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
        /// Save's Share Name And Code .
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/ShareConfig/SaveShareNameAndCode")]
        public IActionResult SaveShareNameAndCode([FromBody]  ShareConfigDTO ShareConfigDTO)
        {
            try
            {
                if (ObjShareConfig.SaveShareNameANdcode(ShareConfigDTO, Con))
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
        /// Get Share Name And Code Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/GetShareNameAndCodeDetails")]
        public async Task<IActionResult> GetShareNameAndCodeDetails(string ShareName,string ShareCode)
        {
            ShareConfigDTO ShareConfigDTO = new ShareConfigDTO();
            try
            {
                ShareConfigDTO = await ObjShareConfig.GetShareNameANdcode(ShareName, ShareCode, Con);
                return ShareConfigDTO != null ? Ok(ShareConfigDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        /// <summary>
        /// Save's Share Configuration Details .
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/ShareConfig/SaveShareConfigurationDeatils")]
        public IActionResult SaveShareConfigurationDeatils([FromBody]  ShareConfigDetails ShareConfigDetails)
        {
            try
            {
                if (ObjShareConfig.SaveShareConfigDetails(ShareConfigDetails, Con))
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
        /// Get Share Configuration Details .
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/GetShareConfigurationDetails")]
        public async Task<IActionResult> GetShareConfigurationDetails(string ShareName, string ShareCode)
        {

            ShareConfigDetails ShareConfigDetails = new ShareConfigDetails();
            try
            {
                ShareConfigDetails = await ObjShareConfig.GetShareConfigDetails(ShareName, ShareCode, Con);
                return ShareConfigDetails != null ? Ok(ShareConfigDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }




        /// <summary>
        /// Save's Share Configuration Referral Details .
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/ShareConfig/SaveShareConfigurationReferralDeatils")]
        public IActionResult SaveShareConfigurationReferralDeatils([FromBody]  ShareconfigReferralDTO ShareconfigReferralDTO)
        {
            try
            {
                if (ObjShareConfig.SaveShareConfigReferral(ShareconfigReferralDTO, Con))
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
        /// Get Share Configuration Referral Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/GetShareConfigurationReferralDetails")]
        public async Task<IActionResult> GetShareConfigurationReferralDetails(string ShareName, string ShareCode)
        {
            ShareconfigReferralDTO ShareconfigReferralDTO = new ShareconfigReferralDTO();
          
            try
            {
                ShareconfigReferralDTO =await ObjShareConfig.GetShareConfigReferralDetails(ShareName, ShareCode, Con);
                return ShareconfigReferralDTO != null ? Ok(ShareconfigReferralDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        /// <summary>
        /// Delete Share Configuration Details .
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/ShareConfig/DeleteShareConfig")]
        public IActionResult DeleteShareConfig(long ShareconfigID, string ShareName)
        {
            try
            {
                if (ObjShareConfig.DeleteShareConfiguration(ShareconfigID, ShareName,Con))
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
        ///// <summary>
        ///// Get Share Config Details.
        ///// </summary>     
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/Banking/Masters/ShareConfig/GetShareConfigDetails")]
        //public IActionResult GetShareConfigDetails(long ShareconfigID, string ShareName)
        //{
        //    ShareConfigDTO ShareConfigDTO = new ShareConfigDTO();
        //    try
        //    {
        //        ShareConfigDTO = ObjShareConfig.GetShareConfigurationDetails(ShareconfigID, ShareName,Con);
        //        return ShareConfigDTO != null ? Ok(ShareConfigDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Get Share Config View Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/GetShareConfigViewDetails")]
        public async Task<IActionResult> GetShareConfigViewDetails()
        {
            List<ShareviewDTO> lstShareviewDTO = new List<ShareviewDTO>();         
            try
            {
                lstShareviewDTO = await ObjShareConfig.GetShareview(Con);
                return lstShareviewDTO != null ? Ok(lstShareviewDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Check Duplicate Share Names 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/CheckDuplicateShareName")]
        public IActionResult CheckDuplicateShareName(string ShareName)
        {
            int count = 0;
            try
            {
                count = ObjShareConfig.GetShareNameCount(ShareName, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/CheckDuplicateShareCode")]
        public IActionResult CheckDuplicateShareCode(string ShareCode)
        {
            int count = 0;
            try
            {
                count = ObjShareConfig.GetShareCodeCount(ShareCode, Con);
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        [HttpGet]
        [Route("api/Banking/Masters/ShareConfig/CheckDuplicatShareNameCode")]
        public IActionResult GetShareNameCodeCount(Int64 shareid, string ShareName, string ShareCode)
        {
            try
            {
                ShareschemeandcodeCount _ShareschemeandcodeCount = ObjShareConfig.GetShareNameCodeCount(shareid, ShareName, ShareCode, Con);
                if (_ShareschemeandcodeCount != null)
                {
                    return Ok(_ShareschemeandcodeCount);
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