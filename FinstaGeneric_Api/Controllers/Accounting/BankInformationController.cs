using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Accounting;
using FinstaRepository.Interfaces.Accounting.Masters;
using FinstaRepository.DataAccess.Accounting.Masters;
using FinstaApi.Common;
using Microsoft.AspNetCore.Authorization;

namespace FinstaApi.Controllers.Accounting
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BankInformationController : ControllerBase
    {
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        IBankInformation objBankInfo = new BankInformationDAL();
        public List<BankUPI> BankUPIList { get; set; }
        public List<BankInformationDTO> BankInformationList { get; set; }
        public List<ChequemanagementDTO> ChequemanagementList { get; set; }
        public BankInformationController(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        [Route("api/Accounting/Masters/SaveBankInformation")]
        [HttpPost]
        public IActionResult SaveBankInformation(BankInformationDTO lstBankInformation)
        {
            bool isSaved = false;
            try
            {
                isSaved = objBankInfo.SaveBankInformation(lstBankInformation, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Accounting/Masters/GetBankUPIDetails")]
        [HttpGet]
        public IActionResult GetBankUPIDetails()
        {
            BankUPIList = new List<BankUPI>();
            try
            {
                BankUPIList = objBankInfo.GetBankUPIDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(BankUPIList);
        }
        [Route("api/Accounting/Masters/GetBAnkDetails")]
        [HttpGet]
        public IActionResult GetBAnkDetails()
        {
            BankInformationList = new List<BankInformationDTO>();
            try
            {
                BankInformationList = objBankInfo.GetBAnkDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(BankInformationList);
        }

        [Route("api/Accounting/Masters/GetBankNames")]
        [HttpGet]
        public IActionResult GetBankNames()
        {
            BankInformationList = new List<BankInformationDTO>();
            try
            {
                BankInformationList = objBankInfo.GetBankNames(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(BankInformationList);
        }
        //[Route("api/Accounting/Masters/GenerateBookId")]
        //[HttpGet]
        //public IActionResult GenerateBookId()
        //{
        //    Int64 BookId;
        //    try
        //    {
        //        BookId = objBankInfo.GenerateBookId(Con);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new FieldAccessException(ex.ToString());
        //    }
        //    return Ok(BookId);
        //}
        [Route("api/Accounting/Masters/SaveChequeManagement")]
        [HttpPost]
        public IActionResult SaveChequeManagement(BankInformationDTO lstChequemanagement)
        {
            bool isSaved = false;
            try
            {
                isSaved = objBankInfo.SaveChequeManagement(lstChequemanagement, Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Accounting/Masters/ViewChequeManagementDetails")]
        [HttpGet]
        public IActionResult ViewChequeManagementDetails()
        {
            ChequemanagementList = new List<ChequemanagementDTO>();
            try
            {
                ChequemanagementList = objBankInfo.ViewChequeManagementDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(ChequemanagementList);
        }
        [Route("api/Accounting/Masters/ViewBankInformationDetails")]
        [HttpGet]
        public IActionResult ViewBankInformationDetails()
        {
            BankInformationList = new List<BankInformationDTO>();
            try
            {
                BankInformationList = objBankInfo.ViewBankInformationDetails(Con);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(BankInformationList);
        }
        [Route("api/Accounting/Masters/ViewBankInformation")]
        [HttpGet]
        public IActionResult ViewBankInformation(Int64 precordid)
        {
            BankInformationDTO objBankinformation = new BankInformationDTO();
            try
            {
                objBankinformation = objBankInfo.ViewBankInformation(precordid, Con);
            }
            catch(Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(objBankinformation);
        }
        [Route("api/Accounting/Masters/DeleteBankInformation")]
        [HttpPost]
        public IActionResult DeleteBankInformation(BankInformationDTO lstChequemanagement)
        {
            bool isSaved = false;
            try
            {
                isSaved = objBankInfo.DeleteBankInformation(lstChequemanagement, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/Accounting/Masters/GetExistingChequeCount")]
        [HttpGet]
        public IActionResult GetExistingChequeCount(Int64 BankId,Int64 ChqFromNo, Int64 ChqToNo)
        {
            Int64 chqCount;
            try
            {
                chqCount = objBankInfo.GetExistingChequeCount(Con, BankId, ChqFromNo, ChqToNo);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(chqCount);
        }
        [Route("api/Accounting/Masters/GetCheckDuplicateDebitCardNo")]
        [HttpPost]
        public IActionResult GetCheckDuplicateDebitCardNo(BankInformationDTO lstBankInformation)
        {
            //Int64 DebtCardCount;
            List<string> lstdata = new List<string>();
            try
            {
                lstdata = objBankInfo.GetCheckDuplicateDebitCardNo(Con, lstBankInformation);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstdata);
        }
        [Route("api/Accounting/Masters/GetCheckDuplicateUPIId")]
        [HttpGet]
        public IActionResult GetCheckDuplicateUPIId(string UPIId)
        {
            Int64 UPIIdCount;
            try
            {
                UPIIdCount = objBankInfo.GetCheckDuplicateUPIId(Con, UPIId);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(UPIIdCount);
        }
    }
}