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
    public class InsuranceMemberController : ControllerBase
    {
        IInsuranceMember _InsuranceMember = new InsuranceMemberDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public InsuranceMemberController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        public List<InsuranceMemberBind> _InsuranceMemberList { get; set; }
        public Viewmemberdetails _Viewmemberdetails { get; set; }
        public GetInsuranceMemberDataforEdit _GetInsuranceMemberDataforEdit { get; set; }        
        public List<InsuranceMembersDataforMainGrid> _InsuranceMembersDataforMainGrid { get; set; }
        public List<InsuranceSchemes> _InsuranceSchemes { get; set; }
        public List<Applicanttypesdata> _ApplicanttypesdataList { get; set; }
        


        /// <summary>
        /// Get Members Based on Membertype
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetallInsuranceMembers")]
        public async Task<IActionResult> GetallInsuranceMembers(long MembertypeID)
        {
            _InsuranceMemberList = new List<InsuranceMemberBind>();
            try
            {
                _InsuranceMemberList = await _InsuranceMember.GetallInsuranceMembers(MembertypeID,Con);
                if(_InsuranceMemberList!=null)
                {
                    return Ok(_InsuranceMemberList);
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
        /// Get Member Details on passing Member Id
        /// </summary>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetMemberDetailsforview")]
        public async Task<IActionResult> GetMemberDetailsforview(long MemberId)
        {
            _Viewmemberdetails = new Viewmemberdetails();
            try
            {
                _Viewmemberdetails = await _InsuranceMember.GetMemberDetailsforview(MemberId,Con);
                if(_Viewmemberdetails!=null)
                {
                    return Ok(_Viewmemberdetails);
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
        /// Get all scheme details of particular Scheme on passing Insurance Scheme Id.
        /// </summary>
        /// <param name="InsuranceSchemeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetInsuranceSchemeDetails")]
        public async Task<IActionResult> GetInsuranceSchemeDetails(long InsuranceSchemeId)
        {            
            try
            {
                InsuranceschemeDetails _InsuranceschemeDetails = await _InsuranceMember.GetInsuranceSchemeDetails(InsuranceSchemeId, Con);
                if(_InsuranceschemeDetails!=null)
                {
                    return Ok(_InsuranceschemeDetails);
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
        /// Get Insurance Member Nominee Details on passing member Reference ID
        /// </summary>
        /// <param name="MemberreferenceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetInsuranceMemberNomineeDetails")]
        public async Task<IActionResult> GetInsuranceMemberNomineeDetails(string MemberreferenceId)
        {           
            try
            {
                List<InsuranceMemberNomineeDetails> _InsuranceMemberNomineeDetails = await _InsuranceMember.GetInsuranceMemberNomineeDetails(MemberreferenceId, Con);
                if (_InsuranceMemberNomineeDetails != null)
                {
                    return Ok(_InsuranceMemberNomineeDetails);
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
        /// Save Insurance Member on Passing Model
        /// </summary>
        /// <param name="_InsuranceMembersave"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/InsuranceMember/SaveInsuranceMember")]
        public IActionResult SaveInsuranceMemberData([FromBody] InsuranceMembersave _InsuranceMembersave)
        {           
            try
            {
                if (_InsuranceMembersave._InsuranceMemberNomineeDetailsListSave.Count > 0)
                {
                    string OldFolder = "Upload";
                    string NewFolder = "Original";
                    string webRootPath = _hostingEnvironment.ContentRootPath;
                    string OldPath = Path.Combine(webRootPath, OldFolder);
                    string newPath = Path.Combine(webRootPath, NewFolder);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    foreach (InsuranceMemberNomineeDetails kycDoc in _InsuranceMembersave._InsuranceMemberNomineeDetailsListSave)
                    {
                        if (!string.IsNullOrEmpty(kycDoc.Idproofpath))
                        {
                            string OldFullPath = Path.Combine(OldPath, kycDoc.Idproofpath);
                            string NewFullPath = Path.Combine(newPath, kycDoc.Idproofpath);
                            kycDoc.Idproofpath = NewFullPath;
                            if (System.IO.File.Exists(OldFullPath))
                            {
                                System.IO.File.Move(OldFullPath, NewFullPath);
                            }
                        }
                    }
                }
                if (_InsuranceMember.SaveInsuranceMemberData(_InsuranceMembersave, Con))
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
        /// Get Insurance Member Data for Binding External Main Grid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetInsuranceMembersMainGrid")]
        public async Task<IActionResult> GetInsuranceMembersMainGrid()
        {
            _InsuranceMembersDataforMainGrid = new List<InsuranceMembersDataforMainGrid>();
            try
            {
                _InsuranceMembersDataforMainGrid = await _InsuranceMember.GetInsuranceMembersMainGrid(Con);
                if(_InsuranceMembersDataforMainGrid!=null)
                {
                    return Ok(_InsuranceMembersDataforMainGrid);
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
        /// Get Insurance Member data on Passing Member Reference ID for Edit
        /// </summary>
        /// <param name="MemberReferenceID"></param>
        ///  <param name="Recordid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetMemberDetailsforEdit")]
        public async Task<IActionResult> GetMemberDetailsforEdit(string MemberReferenceID,long Recordid)
        {
            _GetInsuranceMemberDataforEdit = new GetInsuranceMemberDataforEdit();
            try
            {
                _GetInsuranceMemberDataforEdit = await _InsuranceMember.GetMemberDetailsforEdit(Recordid, Con);
                _GetInsuranceMemberDataforEdit._InsuranceMemberNomineeDetailsEditList = await _InsuranceMember.GetInsuranceMemberNomineeDetails(MemberReferenceID, Con);
                _GetInsuranceMemberDataforEdit._InsuranceschemeDetailsEdit= await _InsuranceMember.GetInsuranceSchemeDetails(_GetInsuranceMemberDataforEdit.pSchemeId, Con);

                if(_GetInsuranceMemberDataforEdit!=null)
                {
                    return Ok(_GetInsuranceMemberDataforEdit);
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
        /// Deletes Insurance Member on Passing Member Reference ID
        /// </summary>
        /// <param name="MemberReferenceID"></param>
        /// <param name="Userid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/InsuranceMember/DeleteInsuranceMember")]
        public IActionResult DeleteInsuranceMember(string MemberReferenceID, long Userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(MemberReferenceID) && Userid > 0)
                {
                    if (_InsuranceMember.DeleteInsuranceMember(MemberReferenceID, Userid, Con))
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
            }
        }

        /// <summary>
        /// Get Insurance Scheme Names based on Member Type and Applicant Type
        /// </summary>
        /// <param name="Membertype"></param>
        /// <param name="Applicanttype"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetInsuranceSchemes")]
        public async Task<IActionResult> GetInsuranceSchemes(string Membertype,string Applicanttype)
        {
            _InsuranceSchemes = new List<InsuranceSchemes>();
            try
            {
                _InsuranceSchemes = await _InsuranceMember.GetInsuranceSchemes(Membertype, Applicanttype,Con);
                if (_InsuranceSchemes != null)
                {
                    return Ok(_InsuranceSchemes);
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
        /// Get all applicants
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/InsuranceMember/GetApplicants")]
        public async Task<IActionResult> GetApplicants()
        {
            _ApplicanttypesdataList = new List<Applicanttypesdata>();
            try
            {
                _ApplicanttypesdataList = await _InsuranceMember.GetApplicants(Con);
                if (_ApplicanttypesdataList != null)
                {
                    return Ok(_ApplicanttypesdataList);
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
        /// Checks Duplicates whether Member has Same Insurance under one Insurance Type Or Not with Active Status.
        /// </summary>
        /// <param name="MemberCode"></param>
        /// <param name="InsuranceconfigID"></param>
        /// <param name="InsuranceType"></param>
        /// <param name="Recordid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/InsuranceMember/checkInsuranceMemberDuplicates")]
        public int checkMemberCountinMaster(string MemberCode,long InsuranceconfigID, string InsuranceType, long Recordid)
        {
            int count = 0;
            try
            {
                if (Convert.ToString(MemberCode) != string.Empty && InsuranceconfigID > 0 && Convert.ToString(InsuranceType) != string.Empty)
                {
                    count = _InsuranceMember.checkMemberCountinMaster(MemberCode, InsuranceconfigID, Con, InsuranceType, Recordid);
                }               
            }
            catch (Exception Ex)
            {
                throw new FinstaAppException(Ex.ToString());
            }
            return count;
        }
    }
}