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
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;

namespace FinstaApi.Controllers.Banking.Transactions
{
    // [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class FDReceiptController : ControllerBase
    {
        IFDReceipt objFDReceipt = new FDReceiptDAL();
        public GeneralReceiptReportDTO _GeneralReceiptReportDTO { get; set; }
        public List<JournalVoucherReportDTO> _JournalVoucherReportDTO { get; set; }
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public FDReceiptController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        #region  Get Member Details
        /// <summary>
        /// Get Memeber Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetMemberDetails")]
        public IActionResult GetMemberDetails(string MemberType, string BranchName)
        {
            List<FDMemberDetailsDTO> lstMemberdetails = new List<FDMemberDetailsDTO>();
            try
            {
                lstMemberdetails = objFDReceipt.GetMemberDetails(MemberType, BranchName, Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  Get Fd Details
        /// <summary>
        /// Get Fixed Deposit Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetFdDetails")]
        public IActionResult GetFdDetails(string Membercode, string ChitBranch)
        {
            List<FDDetailsDTO> lstFDDetails = new List<FDDetailsDTO>();
            try
            {
                lstFDDetails = objFDReceipt.GetFdDetails(Membercode, ChitBranch, Con);
                return lstFDDetails != null ? Ok(lstFDDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  Get Fd Details By ID
        /// <summary>
        /// Get Fixed Deposit Details By ID
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetFdDetailsByid")]
        public IActionResult GetFdDetailsByid(string FdAccountNo)
        {
            List<FDDetailsByIdDTO> lstFDDetailsbyid = new List<FDDetailsByIdDTO>();
            try
            {
                lstFDDetailsbyid = objFDReceipt.GetFdDetailsByid(FdAccountNo, Con);
                return lstFDDetailsbyid != null ? Ok(lstFDDetailsbyid) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Transactions list
        /// <summary>
        /// Get Transactions list
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetTransactionslist")]
        public IActionResult GetTransactionslist(int FdAccountNo)
        {
            List<TransactionsDTO> lstTransactions = new List<TransactionsDTO>();
            try
            {
                lstTransactions = objFDReceipt.GetTransactionslist(FdAccountNo, Con);
                return lstTransactions != null ? Ok(lstTransactions) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  Save FD Receipt
        /// <summary>
        /// Save FD Receipt
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Transactions/FdReceipt/SaveFdReceipt")]
        public async Task<IActionResult> SaveFdReceipt([FromBody]  FDReceiptDTO ObjFdReceiptDTO)
        {
            try
            {
                string OUTReceiptid = string.Empty;
                if (objFDReceipt.SaveFdReceipt(ObjFdReceiptDTO, Con, out OUTReceiptid))
                {
                    if (!string.IsNullOrEmpty(OUTReceiptid))
                    {
                        if (ObjFdReceiptDTO.pModeofreceipt != "ADJUSTMENT")
                        {
                            _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                            _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(OUTReceiptid, Con);
                            _GeneralReceiptReportDTO.pCompanyInfoforReports = obj.GetcompanyNameandaddressDetails(Con);
                            return Ok(_GeneralReceiptReportDTO);
                        }
                        else
                        {
                            _JournalVoucherReportDTO = new List<JournalVoucherReportDTO>();
                            _JournalVoucherReportDTO = await _AccountingTransactionsDAL.GetJournalVoucherReportData(Con, OUTReceiptid);
                            return Ok(_JournalVoucherReportDTO);

                        }
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw ex;
            }
        }
        #endregion

        #region Get FdReceipt Details 
        /// <summary>
        /// Get FdReceipt Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetFDReceiptDetails")]
        public IActionResult GetFDReceiptDetails(string FromDate, string Todate)
        {
            List<FDReceiptDetailsDTO> lstFdReceiptDetails = new List<FDReceiptDetailsDTO>();
            try
            {
                lstFdReceiptDetails = objFDReceipt.GetFDReceiptDetails(FromDate, Todate,Con);
                return lstFdReceiptDetails != null ? Ok(lstFdReceiptDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetFDBranchDetails")]
        public async Task<IActionResult> GetFDBranchDetails()
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await objFDReceipt.GetFDBranchDetails(Con);
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

        [HttpGet]
        [Route("api/Banking/Transactions/FdReceipt/GetFDReceiptBranchDetails")]
        public async Task<IActionResult> GetFDReceiptBranchDetails()
        {
            try
            {
                List<ChitBranchDetails> _ChitBranchDetailsList = await objFDReceipt.GetFDReceiptBranchDetails(Con);
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