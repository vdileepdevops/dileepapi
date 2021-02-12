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
    //[Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class FIMemberController : ControllerBase
    {
        IFIMember _FIMemberDAL = new FIMemberDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public List<FiMemberContactDetails> _FiMemberContactDetailsList { get; set; }
        public List<FIMembertypeDTO> _FIMembersList { get; set; }
        public List<FIApplicantTypeDTO> _FIApplicantTypesList { get; set; }
        public FIMemberandApplicantList _FIMemberandApplicantList { get; set; }
        public List<FIMeberRefIdAndID> _FIMeberRefIdAndIDList { get; set; }
        public FIMemberController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        /// <summary>
        /// Save Fi Member ( Not in Use) // All Tabs Save in Single Transaction
        /// </summary>
        /// <param name="_FIMemberDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FIMember/SaveandUpdateFIMember")]
        public IActionResult SaveFIMember([FromBody]  FIMemberDTO _FIMemberDTO)
        {
            try
            {
                if (_FIMemberDAL.SaveFIMember(_FIMemberDTO, Con))
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
        /// Get Member Details on Passing Reference ID
        /// </summary>
        /// <param name="MemberReferenceId"></param>
        /// <returns></returns>
        [HttpGet]        
        [Route("api/Banking/Masters/FIMember/GetFIMemberData")]
        public async Task< IActionResult> GetFIMemberData( string MemberReferenceId)
        {      
                try
                { 
                    FIMemberDTO FIMemberList = await _FIMemberDAL.GetFIMemberData(MemberReferenceId, Con);
                    if (FIMemberList != null && FIMemberList._FiMemberContactDetailsDTO!=null)
                    {
                        return Ok(FIMemberList);
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.NoContent);                    
                    }
                }
                catch (Exception)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                    //throw new FinstaAppException(Ex.ToString());
                }                
           
        }

        /// <summary>
        /// Get all FI Members Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/GetallFIMembers")]
        public async Task<IActionResult> GetallFIMembersDetails()
        {           
            try
            {
                List<FiMemberContactDetails> _FiMemberContactDetailsList = await _FIMemberDAL.GetallFIMembersDetails(Con);
                if(_FiMemberContactDetailsList!=null && _FiMemberContactDetailsList.Count>0)
                {
                    return Ok(_FiMemberContactDetailsList);
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
        
        /// <summary>
        /// Checks Duplicates whether Contact Exists as Member Or Not
        /// </summary>
        /// <param name="ContactReferenceId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/checkMemberCountinMaster")]
        public int checkMemberCountinMaster(string ContactReferenceId)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(ContactReferenceId) != string.Empty)
                {                   
                   count = _FIMemberDAL.checkMemberCountinMaster(ContactReferenceId, Con);                  
                }
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }

        /// <summary>
        /// API to delete Fi Member
        /// </summary>
        /// <param name="MemberReferenceID"></param>
        /// <param name="Userid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FIMember/DeleteFIMember")]
        public IActionResult DeleteFIMember(string MemberReferenceID,long Userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(MemberReferenceID) && Userid > 0)
                {
                    if (_FIMemberDAL.DeleteFIMember(MemberReferenceID, Userid, Con))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        /// <summary>
        /// API to Get Applicant Types based on Contact type
        /// </summary>
        /// <param name="ContactType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FIMember/GetapplicantTypes")]
        public async Task<IActionResult> GetFIMembersAndApplicantTypesDetails(string ContactType)
        {
            _FIMemberandApplicantList = new FIMemberandApplicantList();
            try
            {
                _FIMemberandApplicantList._FIMembertypeDTOList = await _FIMemberDAL._GetFIMembersTypesListDetails(Con);
                if(!string.IsNullOrEmpty(ContactType))
                _FIMemberandApplicantList. _FIApplicantTypeDTO = await _FIMemberDAL.GetFIMembersApplicantListDetails(ContactType, Con);
                if(_FIMemberandApplicantList!=null)
                {
                    return Ok(_FIMemberandApplicantList);
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


        #region Tabwise Save FI Member
        /// <summary>
        /// Save FIrst Tab Data of Member
        /// </summary>
        /// <param name="_FiMemberContactDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FIMember/SaveFIMemberMasterData")]
        public IActionResult SaveFIMemberMasterData([FromBody] FiMemberContactDetails _FiMemberContactDetails)
        {
            FIMeberRefIdAndID _FIMeberRefIdAndID = new FIMeberRefIdAndID();
            try
            {
                long MemberID = 0;
                string Dob = string.Empty;
                string MemberReferenceId = _FIMemberDAL.SaveFIMemberMasterData(_FiMemberContactDetails, Con,out MemberID, out Dob);
                if (!string.IsNullOrEmpty( MemberReferenceId ))
                {
                    _FIMeberRefIdAndID.pMemberReferenceId = MemberReferenceId;
                    _FIMeberRefIdAndID.pMemberId = MemberID;
                    _FIMeberRefIdAndID.pDob = Dob;
                    return Ok(_FIMeberRefIdAndID);
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
        /// Save Member Reference Data
        /// </summary>
        /// <param name="_FIMembersaveReferences"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FIMember/SaveFIMemberReferenceData")]
        public IActionResult SaveFIMemberReferenceData([FromBody] FIMembersaveReferences _FIMembersaveReferences)
        {            
            try
            {                
                if(_FIMemberDAL.SaveFIMemberReferenceData(_FIMembersaveReferences, Con))
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
        /// Save Member Referral Data
        /// </summary>
        /// <param name="_FiMemberReferralDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Masters/FIMember/SaveFIMemberReferralData")]
        public IActionResult SaveFIMemberReferralData([FromBody] FiMemberReferralDTO _FiMemberReferralDTO)
        {            
            try
            {
                if (_FIMemberDAL.SaveFIMemberReferralData(_FiMemberReferralDTO, Con))
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
        #endregion


        #region Get Individual Data

        /// <summary>
        /// Get Referral Data on Passing MemberCode
        /// </summary>
        /// <param name="Applicationid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FIMember/GetFIMemberReferraldetails")]
        public IActionResult GetFIMemberReferraldetails(string Applicationid)
        {           
            try
            {
                FiMemberReferralDTO _FiMemberReferralDTO = _FIMemberDAL.GetFIMemberReferralInformation(Applicationid, Con);
                if (_FiMemberReferralDTO != null)
                {
                    return Ok(_FiMemberReferralDTO);
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

        /// <summary>
        /// Get Reference Data on passing Membercode
        /// </summary>
        /// <param name="Applicationid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FIMember/GetFIMemberReferenceInformation")]
        public IActionResult GetFIMemberReferenceInformation(string Applicationid)
        {
            var _FIMembersaveReferences = new FIMembersaveReferences();
            try
            {
                _FIMembersaveReferences.lobjAppReferences = _FIMemberDAL.GetFIMemberReferenceInformation(Applicationid, Con);
                _FIMembersaveReferences.pIsreferencesapplicable= _FIMemberDAL.GetIsReferencesapplicableOrnot(Applicationid, Con);
                if (_FIMembersaveReferences.lobjAppReferences != null)
                {
                    return Ok(_FIMembersaveReferences);
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

        /// <summary>
        /// Get Guardian Details for Members who are having Applicant type as Minor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/GetFIMembersAsGuardians")]
        public async Task<IActionResult> GetFIMembersforasGuardians()
        {           
            try
            {
                List<FIMeberRefIdAndID> _FIMeberRefIdAndIDList = await _FIMemberDAL.GetFIMembersforasGuardians(Con);
                if(_FIMeberRefIdAndIDList!=null && _FIMeberRefIdAndIDList.Count>0)
                {
                    return Ok(_FIMeberRefIdAndIDList);
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


        /// <summary>
        /// Get Contact Details
        /// </summary>
        /// <param name="contactType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Masters/FIMember/getContactDetails")]
        public IActionResult getContactDetails(string contactType)
        {
            List<ContactDetailsDTO> lstContactDetails = new List<ContactDetailsDTO>();
            try
            {
                lstContactDetails = _FIMemberDAL.getContactDetails(contactType, Con);
                if (lstContactDetails != null)
                {
                    return Ok(lstContactDetails);
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