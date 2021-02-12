using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FinstaApi.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using FinstaRepository.Interfaces.Banking.Transactions;
using FinstaRepository.DataAccess.Banking.Transactions;
using FinstaInfrastructure.Banking.Transactions;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Banking.Transactions
{
    // [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class FdTransferController : ControllerBase
    {
        IFdTransfer FdtransferDAL = new FdTransferDAL();
        readonly string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public FdTransferController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        /// <summary>
        /// Get all Fd Schemes
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/GetFdSchemes")]
        public async Task<IActionResult> GetFdSchemes()
        {
            try
            {
                List<FdschemeNameandCode> _FdschemeNameandCodeList = await FdtransferDAL.GetFdSchemes(Con);
                if (_FdschemeNameandCodeList != null && _FdschemeNameandCodeList.Count > 0)
                {
                    return Ok(_FdschemeNameandCodeList);
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
        /// Get To Fd Account no's
        /// </summary>
        /// <param name="FromFdaccountno"></param>
        /// <param name="Branchname"></param>
        /// <param name="Membercode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/GetFdToDetails")]
        public IActionResult GetFdToDetails(string Branchname,string Membercode)
        {            
            try
            {
                List<FDDetailsDTO> lstFDDetails = FdtransferDAL.GetFdToDetails(Branchname, Membercode, Con);
                return lstFDDetails != null ? Ok(lstFDDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// Get From Fd Account no's
        /// </summary>
        /// <param name="Fdaccountno"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/GetFdFromDetails")]
        public IActionResult GetFdFromDetails()
        {
            try
            {
                List<FDDetailsDTO> lstFDFromDetails = FdtransferDAL.GetFdFromDetails(Con);
                return lstFDFromDetails != null ? Ok(lstFDFromDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // Branches binding API
        // api/Banking/GetchitBranchDetails

        #region  Get Fd Details By ID
        /// <summary>
        /// Get From account Fixed Deposit Details By ID
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/GetFromAccountDetailsByid")]
        public IActionResult GetFromFdDetailsByid(string FdAccountNo)
        {
            List<FDDetailsByIdDTO> lstFDDetailsbyid = new List<FDDetailsByIdDTO>();
            try
            {
                lstFDDetailsbyid = FdtransferDAL.GetFromFdDetailsByid(FdAccountNo, Con);
                return lstFDDetailsbyid != null ? Ok(lstFDDetailsbyid) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Get To account Fixed Deposit Details By ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/GetToAccountFdDetailsByid")]
        public IActionResult GetToFdDetailsByid(string FdAccountNo)
        {            
            try
            {
                List<FDDetailsByIdDTO> lstFDDetailsbyid = FdtransferDAL.GetToFdDetailsByid(FdAccountNo, Con);
                return lstFDDetailsbyid != null ? Ok(lstFDDetailsbyid) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Transfer - Save

        [HttpPost]
        [Route("api/Banking/Transactions/SaveFdTransfer")]
        public IActionResult SaveFdTransfer([FromBody]  Fdtransfersave _Fdtransfersave)
        {
            try
            {
                if (FdtransferDAL.SaveFdTransfer(_Fdtransfersave, Con))
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


        #region  Get Member Details
        /// <summary>
        /// Get Memeber Details by branch name
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdTransfer/GetMemberDetails")]
        public IActionResult GetMemberDetails( string BranchName)
        {            
            try
            {
                List<FDMemberDetailsDTO> lstMemberdetails = FdtransferDAL.GetMemberDetails(BranchName, Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);                
            }
        }
        #endregion

    }
}