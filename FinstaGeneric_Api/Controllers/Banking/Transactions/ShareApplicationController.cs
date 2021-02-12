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
using FinstaInfrastructure.Banking.Masters;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Transactions
{
    //  [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ShareApplicationController : ControllerBase
    {
        IShareApplication objshareApplication = new ShareApplicationDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ShareApplicationController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Get Member Names Based Member type and Receipt type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/ShareApplication/GetshareMembers")]
        public async Task<IActionResult> GetshareMembers(string Membertype, string Receipttype)
        {
            List<shareMembersDetails> lstShareMembersDTO = new List<shareMembersDetails>();
            try
            {
                lstShareMembersDTO = await objshareApplication.GetshareMembers(Membertype, Receipttype, Con);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(lstShareMembersDTO);
        }
        /// <summary>
        /// Get Share Names Based Member type and Applicant type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/ShareApplication/GetShareNames")]
        public async Task<IActionResult> GetShareNames(string Membertype, string Applicanttype)
        {
            List<ShareviewDTO> lstShareviewDTO = new List<ShareviewDTO>();
            try
            {
                lstShareviewDTO = await objshareApplication.GetSharNames(Membertype, Applicanttype, Con);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(lstShareviewDTO);
        }
        /// <summary>
        /// Get Share Config Details Based On Config Id and Applicant type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/ShareApplication/GetSharconfigdetails")]
        public async Task<IActionResult> GetSharconfigdetails(long shareconfigid, string Applicanttype, string Membertype)
        {
            List<shareconfigDetails> lstShareconfigdetails = new List<shareconfigDetails>();
            try
            {
                lstShareconfigdetails = await objshareApplication.GetSharconfigdetails(shareconfigid, Applicanttype, Membertype,Con);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(lstShareconfigdetails);
        }


        /// <summary>
        /// Save / Update Share Application//1st Tab
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Transactions/ShareApplication/SaveShareApplication")]
        public IActionResult SaveShareApplication([FromBody]  ShareApplicationDTO ShareApplicationDTO)
        {
            shareComfigurationIdandName _shareComfigurationIdandName = new shareComfigurationIdandName();
            try
            {
                long pShareaccountid = 0;
                string shareaccountNo = objshareApplication.SaveshareApplication(ShareApplicationDTO, Con, out pShareaccountid);
                    if (!string.IsNullOrEmpty(shareaccountNo))
                    {
                    _shareComfigurationIdandName.pShareAccountNo = shareaccountNo;
                    _shareComfigurationIdandName.pshareapplicationid = pShareaccountid;
                        return Ok(_shareComfigurationIdandName);
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
        /// Share Joint Members and Nominees Save
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/SaveshareJointMembersandNomineeData")]
        public IActionResult SaveshareJointMembersandNomineeData([FromBody] savejointandnomiee _JointandNomineeSave)
        {
            try
            {
                if (objshareApplication.SaveshareJointMembersandNomineeData(_JointandNomineeSave, Con))
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
            }
        }

        /// <summary>
        /// Save Referral Data
        /// </summary>
        /// <param name="ReferralDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/SaveReferralData")]
        public IActionResult SaveReferralData([FromBody] Referrals ReferralDetails)
        {
            try
            {
                if (objshareApplication.SaveReferralData(ReferralDetails, Con))
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
            }
        }
        /// <summary>
        /// Delete Share application
        /// </summary>
        /// <param name="ReferralDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Transactions/ShareApplication/DeleteShareDetails")]
        public IActionResult DeleteShareDetails(long ShareApplicationId)
        {
            try
            {
                if (objshareApplication.DeleteShareDetails(ShareApplicationId, Con))
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
        /// Get Share Application View Details.
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/ShareApplication/GetShareApplicationViewDetails")]
        public async Task<IActionResult> GetShareApplicationViewDetails()
        {
            List<ShareApplicationDTO> lstShareApplicationDTO = new List<ShareApplicationDTO>();
            try
            {
                lstShareApplicationDTO = await objshareApplication.BindShareApplicationView(Con);
                return lstShareApplicationDTO != null ? Ok(lstShareApplicationDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }





    }
}