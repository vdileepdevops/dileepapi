using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Finsta_Banking_Infrastructure.Banking.Transactions;
using Finsta_Banking_Repository.Interfaces.Banking.Transactions;
using Finsta_Banking_Repository.DataAccess.Banking.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;

namespace Finsta_Banking_Api.Controllers.Banking.Transactions
{
    //[Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class RDReceiptController : ControllerBase
    {
        IRdReceipt objRdReceipt = new RdReceiptDAL();
        public GeneralReceiptReportDTO _GeneralReceiptReportDTO { get; set; }
        public List<JournalVoucherReportDTO> _JournalVoucherReportDTO { get; set; }
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public RDReceiptController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
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
        [Route("api/Banking/Transactions/RDReceipt/GetMemberDetails")]
        public IActionResult GetMemberDetails(string MemberType)
        {
            List<MemberDetailsDTO> lstMemberdetails = new List<MemberDetailsDTO>();
            try
            {
                lstMemberdetails = objRdReceipt.GetMemberDetails(MemberType, Con);
                return lstMemberdetails != null ? Ok(lstMemberdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  Get Account Details
        /// <summary>
        /// Get Account  Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/RDReceipt/GetAccountDetails")]
        public IActionResult GetAccountDetails(string Membercode)
        {
            AccountDetailsDTO lstAccountDetails = new AccountDetailsDTO();
            try
            {
                lstAccountDetails = objRdReceipt.GetAccountDetails(Membercode, Con);
                return lstAccountDetails != null ? Ok(lstAccountDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  View  Account Details By Id
        /// <summary>
        /// Get Account  Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/RDReceipt/GetAccountDetailsByid")]
        public IActionResult GetAccountDetailsByid(string AccountNo)
        {
            List<AccountDetailsByIdDTO> lstAccountDetails = new List<AccountDetailsByIdDTO>();
            try
            {
                lstAccountDetails = objRdReceipt.GetAccountDetailsByid(AccountNo, Con);
                return lstAccountDetails != null ? Ok(lstAccountDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  View  Due Details
        /// <summary>
        /// View  Due Details
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/RDReceipt/GetViewDues")]
        public IActionResult GetViewDues(string AccountNo, string transdate)
        {
            List<ViewDuesDTO> lstDuedetails = new List<ViewDuesDTO>();
            try
            {
                lstDuedetails = objRdReceipt.GetViewDues(AccountNo, transdate, Con);
                return lstDuedetails != null ? Ok(lstDuedetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region  Save RD Receipt
        /// <summary>
        /// Save RD Receipt
        /// </summary>     
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banking/Transactions/RDReceipt/SaveRdReceipt")]
        public async Task<IActionResult> SaveRdReceipt([FromBody]  RdReceiptDTO ObjRdReceiptDTO)
        {
            try
            {
                string OUTReceiptid = string.Empty;
                if (objRdReceipt.SaveRdReceipt(ObjRdReceiptDTO, Con, out OUTReceiptid))
                {
                    if (!string.IsNullOrEmpty(OUTReceiptid))
                    {
                        if (ObjRdReceiptDTO.pModeofreceipt != "ADJUSTMENT")
                        {
                            _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                            _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(OUTReceiptid, Con);
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

        #region Get View RD Receipt Details 
        /// <summary>
        /// Get View RD Receipt Details 
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/RDReceipt/GetRdReceiptDetails")]
        public IActionResult GetRdReceiptDetails(string FromDate, string Todate)
        {
            List<RdReceiptDetailsDTO> lstRdReceiptDetails = new List<RdReceiptDetailsDTO>();
            try
            {
                lstRdReceiptDetails = objRdReceipt.GetRdReceiptDetails(FromDate, Todate, Con);
                return lstRdReceiptDetails != null ? Ok(lstRdReceiptDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
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
        [Route("api/Banking/Transactions/RDReceipt/GetTransactionslist")]
        public IActionResult GetTransactionslist(int AccountNo)
        {
            List<TransactionsDTO> lstTransactions = new List<TransactionsDTO>();
            try
            {
                lstTransactions = objRdReceipt.GetTransactionslist(AccountNo, Con);
                return lstTransactions != null ? Ok(lstTransactions) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Get Adjustment Detils
        /// <summary>
        /// Get Adjustment Detils
        /// </summary>     
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banking/Transactions/RDReceipt/GetAdjustmentDetils")]
        public IActionResult GetAdjustmentDetils(String Membercode)
        {
            List<RDSavingsAccountDetailsDTO> lstAdjustdetails = new List<RDSavingsAccountDetailsDTO>();
            try
            {
                lstAdjustdetails = objRdReceipt.GetAdjustmentDetils(Membercode, Con);
                return lstAdjustdetails != null ? Ok(lstAdjustdetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion


        #region RD Receipt View

        //[HttpGet]
        //[Route("api/Banking/Transactions/RDReceipt/GetRDReceiptView")]
        //public IActionResult GetRDReceiptDetails(string FromDate, string Todate)
        //{
        //    List<RDReceiptDetailsDTO> lstRDReceiptView = new List<RDReceiptDetailsDTO>();
        //    try
        //    {
        //        lstRDReceiptView = objRdReceipt.GetRDReceiptDetails(FromDate, Todate, Con);
        //        return lstRDReceiptView != null ? Ok(lstRDReceiptView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //        throw;
        //    }
        //}
        #endregion 

    }
}