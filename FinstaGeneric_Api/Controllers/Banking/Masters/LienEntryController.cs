using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaRepository.Interfaces.Banking.Masters;
using FinstaRepository.DataAccess.Banking.Masters;
using Microsoft.AspNetCore.Cors;
using FinstaInfrastructure.Banking.Masters;
using Microsoft.Extensions.Configuration;
using FinstaApi.Common;

namespace FinstaApi.Controllers.Banking.Masters
{
    //[Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class LienEntryController : ControllerBase
    {


        ILienEntry Objlienentry = new LienEntryDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;


        public LienEntryController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/banking/masters/LienEntry/SaveLienentry")]
        [HttpPost]
        public IActionResult SaveLienentry(LienEntryDTO _LienEntry)
        {
            bool isSaved = false ;

            try
            {
                isSaved = Objlienentry.SaveLienentry(_LienEntry, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }


        [Route("api/banking/masters/LienEntry/Getfddetails")]
        [HttpGet]

        public async Task<IActionResult> Getfddetails(Int64 Memberid, string chitbranchname, string type)
        {

            List<GetfdraccountnoDTO> lstGetfdraccountnodetails = new List<GetfdraccountnoDTO>();
            try
            {
                lstGetfdraccountnodetails = await Objlienentry.Getfddetails(Memberid, chitbranchname, type, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetfdraccountnodetails);


        }

        [Route("api/banking/masters/LienEntry/GetMemberDetails")]
        [HttpGet]

        public async Task<IActionResult> GetMemberDetails(string chitbranchname)
        {

            List<FiMemberContactDetails> lstGetMemberDetails = new List<FiMemberContactDetails>();
            try
            {
                lstGetMemberDetails = await Objlienentry.GetMemberDetails(chitbranchname, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstGetMemberDetails);


        }


        [Route("api/banking/masters/LienEntry/Getmemberfddetails")]
        [HttpGet]

        public IActionResult Getmemberfddetails(Int64 Memberid, string Fdraccountno)
        {

         GetmemberfddetailsDTO lstgetmemberfddetails = new GetmemberfddetailsDTO();
            try
            {
                lstgetmemberfddetails = Objlienentry.Getmemberfddetails(Memberid, Fdraccountno, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstgetmemberfddetails);


        }

        [HttpGet]
        [Route("api/banking/masters/LienEntry/Lienviewdata")]
        public IActionResult Lienviewdata()
        {
            List<LienEntryViewDTO> lstfdView = new List<LienEntryViewDTO>();
            try
            {
                lstfdView = Objlienentry.Lienviewdata(Con);
                return lstfdView != null ? Ok(lstfdView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/banking/masters/LienEntry/GetLiendata")]
        public IActionResult GetLiendata(string Fdaccountno)
        {
            List<LienEntryDTO> Liendata = new List<LienEntryDTO>();
            try
            {
                Liendata = Objlienentry.GetLiendata(Fdaccountno,Con);
                return Liendata != null ? Ok(Liendata) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }



        [HttpGet]
        [Route("api/banking/masters/LienEntry/DeleteLienEntry")]
        public IActionResult DeleteLienEntry(long Lienid)
        {
            try
            {
                if (Objlienentry.DeleteLienentry(Lienid, Con))
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

        [Route("api/banking/masters/LienEntry/GetLienentryforEdit")]
        [HttpGet]

        public IActionResult GetLienentryforEdit(Int64 Lienid)
        {

            LienEntryDetailsForEdit lstLeinentryedit = new LienEntryDetailsForEdit();
            try
            {
                lstLeinentryedit = Objlienentry.GetLienentryforEdit(Lienid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLeinentryedit);


        }

        [HttpGet]
        [Route("api/banking/masters/LienEntry/GetFDBranchDetails")]
        public async Task<IActionResult> GetFDBranchDetails()
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await Objlienentry.GetFDBranchDetails(Con);
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