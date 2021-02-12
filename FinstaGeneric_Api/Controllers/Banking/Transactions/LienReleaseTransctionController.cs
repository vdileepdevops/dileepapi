using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Cors;
using FinstaInfrastructure.Banking.Transactions;
using Microsoft.Extensions.Configuration;
using FinstaApi.Common;



namespace FinstaApi.Controllers.Banking.Transactions
{
   // [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class LienReleaseTransctionController : ControllerBase
    {
               
        ILienReleaseTransaction Objlienrelease = new LienReleaseTransactionDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;


        public LienReleaseTransctionController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [Route("api/banking/Transactions/LienReleaseTransactions/LienReleaseMembercode")]
        [HttpGet]

        public IActionResult LienReleaseMembercode(string Branchname)
        {
            List<LienRelasegetmemberDTO> lstLienreleasemember = new List<LienRelasegetmemberDTO>();
            try
            {
                lstLienreleasemember = Objlienrelease.LienReleasemembercode(Branchname,Con);
                return lstLienreleasemember != null ? Ok(lstLienreleasemember) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        [Route("api/banking/Transactions/LienReleaseTransactions/GetBranches")]
        [HttpGet]

        public IActionResult GetBranches()
        {
            List<LienRelaseBranchrDTO> lstLienreleaseBranches = new List<LienRelaseBranchrDTO>();
            try
            {
                lstLienreleaseBranches = Objlienrelease.GetBranches(Con);
                return lstLienreleaseBranches != null ? Ok(lstLienreleaseBranches) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }


        [Route("api/banking/Transactions/LienReleaseTransactions/LienReleaseMemberfd")]
        [HttpGet]

        public async Task<IActionResult> LienReleaseMemberfd(string Membercode,string BranchName)
        {

            
            List<LienRelasegetmemberDTO> lstLienrelasememberfd = new List<LienRelasegetmemberDTO>();
            try
            {
                lstLienrelasememberfd = await Objlienrelease.Getlienreleasefd(Membercode, BranchName, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLienrelasememberfd);


        }


        [Route("api/banking/Transactions/LienReleaseTransactions/GetLienreleasedata")]
        [HttpGet]
        public async Task<IActionResult> GetLienreleasedata(string Membercode,string Fdraccountno, string LienDate)
        {

            List<LienReleasememberfdDTO> listliendata = new List<LienReleasememberfdDTO>();
            try
            {
                listliendata = await Objlienrelease.GetLienrelasedata(Membercode, Fdraccountno, LienDate, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(listliendata);


        }



        [Route("api/banking/Transactions/LienReleaseTransactions/SaveLienreleaseentry")]
        [HttpPost]
        public IActionResult SaveLienreleaseentry(LienreleaseSaveDTO _LienreleasesaveDTO)
        {
            bool isSaved = false;

            try
            {
                isSaved = Objlienrelease.SaveLienreleaseentry(_LienreleasesaveDTO, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }


        [Route("api/banking/Transactions/LienReleaseTransactions/DeleteLienreleaseentry")]
        [HttpGet]
        public IActionResult DeleteLienreleaseentry(long Lienid)
        {
            try
            {
                if (Objlienrelease.DeleteLienreleaseentry(Lienid, Con))
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



      
        [Route("api/banking/Transactions/LienReleaseTransactions/Lienreleaseviewdata")]
        [HttpGet]
        public IActionResult Lienreleaseviewdata()
        {
            LienReleaseviewDTO lstlienreleaseView = new LienReleaseviewDTO();
            try
            {
                lstlienreleaseView = Objlienrelease.Lienreleaseviewdata(Con);
                return lstlienreleaseView != null ? Ok(lstlienreleaseView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


    }
}