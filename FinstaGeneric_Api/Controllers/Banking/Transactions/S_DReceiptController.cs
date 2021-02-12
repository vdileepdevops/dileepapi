using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finsta_Banking_Infrastructure.Banking.Transactions;
using Finsta_Banking_Repository.DataAccess.Banking.Transactions;
using Finsta_Banking_Repository.Interfaces.Banking.Transactions;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Finsta_Banking_Api.Controllers.Banking.Transactions
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class S_DReceiptController : ControllerBase
    {
        IS_DReceipt objSDReceipt = new S_DReciptDAL();
        public GeneralReceiptReportDTO _GeneralReceiptReportDTO { get; set; }
        public List<JournalVoucherReportDTO> _JournalVoucherReportDTO { get; set; }
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;

        public S_DReceiptController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region  Save Member Receipt

        [HttpPost]
        [Route("api/Banking/Transactions/SDReceipt/SaveMemberReceipt")]
        public async Task<IActionResult> SaveMemberReceipt([FromBody]  MemberReceiptDTO ObjMemberReceiptDTO)
        {
            try
            {
                string OUTReceiptid = string.Empty;
                if (objSDReceipt.SaveMemberReceipt(ObjMemberReceiptDTO, Con, out OUTReceiptid))
                {
                    if (!string.IsNullOrEmpty(OUTReceiptid))
                    {
                        _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                        _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(OUTReceiptid, Con);
                        _GeneralReceiptReportDTO.pCompanyInfoforReports = obj.GetcompanyNameandaddressDetails(Con);
                        return Ok(_GeneralReceiptReportDTO);
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

        #region 
        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetMemberReceiptView")]
        public async Task<IActionResult> GetMemberReceiptView(string FromDate, string Todate)
        {
            List<MemberreceiptViewDTO> lstMemberReceiptDetails = new List<MemberreceiptViewDTO>();
            try
            {
                lstMemberReceiptDetails = objSDReceipt.GetMemberReceiptView(FromDate, Todate, Con);
                return lstMemberReceiptDetails != null ? Ok(lstMemberReceiptDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetMembers")]
        public async Task<IActionResult> GetMembers(string Contacttype, string MemberType)
        {
            List<MembersandContactDetails> lstMemberDetails = new List<MembersandContactDetails>();
            try
            {
                lstMemberDetails = objSDReceipt.GetMembers(Contacttype, MemberType, Con);
                return lstMemberDetails != null ? Ok(lstMemberDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetSavingAccountNameDetails")]
        public async Task<IActionResult> GetSavingAccountNameDetails()
        {
            List<SavingAccNameDetails> lstSavingAccNameDetails = new List<SavingAccNameDetails>();
            try
            {
                lstSavingAccNameDetails = objSDReceipt.GetSavingAccountNameDetails(Con);
                return lstSavingAccNameDetails != null ? Ok(lstSavingAccNameDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetSavingAccountNumberDetails")]
        public async Task<IActionResult> GetSavingAccountNumberDetails(Int64 SavingConfigid)
        {
            List<SavingAccDetails> lstSavingAccDetails = new List<SavingAccDetails>();
            try
            {
                lstSavingAccDetails = objSDReceipt.GetSavingAccountNumberDetails(SavingConfigid, Con);
                return lstSavingAccDetails != null ? Ok(lstSavingAccDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/Banking/Transactions/SDReceipt/SaveSavingsReceipt")]
        public async Task<IActionResult> SaveSavingsReceipt([FromBody]  SAReceiptDTO ObjSAReceiptDTO)
        {
            try
            {
                string OUTReceiptid = string.Empty;
                if (objSDReceipt.SaveSavingsReceipt(ObjSAReceiptDTO, Con, out OUTReceiptid))
                {
                    if (!string.IsNullOrEmpty(OUTReceiptid))
                    {
                        _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                        _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(OUTReceiptid, Con);
                        _GeneralReceiptReportDTO.pCompanyInfoforReports = obj.GetcompanyNameandaddressDetails(Con);
                        return Ok(_GeneralReceiptReportDTO);
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

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetSavingReceiptView")]
        public async Task<IActionResult> GetSavingReceiptView(string FromDate, string Todate)
        {
            List<SavingreceiptViewDTO> lstSavingreceiptViewDTO = new List<SavingreceiptViewDTO>();
            try
            {
                lstSavingreceiptViewDTO = objSDReceipt.GetSavingReceiptView(FromDate, Todate, Con);
                return lstSavingreceiptViewDTO != null ? Ok(lstSavingreceiptViewDTO) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetSavingTransaction")]
        public async Task<IActionResult> GetSavingTransaction(Int64 SavingAccountId)
        {
            List<SavingTransactionDTO> lstSavingTransaction = new List<SavingTransactionDTO>();
            try
            {
                lstSavingTransaction = objSDReceipt.GetSavingTransaction(SavingAccountId, Con);
                return lstSavingTransaction != null ? Ok(lstSavingTransaction) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        #region
        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetShareAccountNameDetails")]
        public async Task<IActionResult> GetShareAccountNameDetails()
        {
            List<ShareAccNameDetails> lstShareAccNameDetails = new List<ShareAccNameDetails>();
            try
            {
                lstShareAccNameDetails = objSDReceipt.GetShareAccountNameDetails(Con);
                return lstShareAccNameDetails != null ? Ok(lstShareAccNameDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetShareAccountNumberDetails")]
        public async Task<IActionResult> GetShareAccountNumberDetails(Int64 ShareConfigid)
        {
            List<ShareAccDetails> lstShareAccDetails = new List<ShareAccDetails>();
            try
            {
                lstShareAccDetails = objSDReceipt.GetShareAccountNumberDetails(ShareConfigid, Con);
                return lstShareAccDetails != null ? Ok(lstShareAccDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost]
        [Route("api/Banking/Transactions/SDReceipt/SaveShareReceipt")]
        public async Task<IActionResult> SaveShareReceipt([FromBody]  ShareReceiptDTO ObjShareReceiptDTO)
        {
            try
            {
                string OUTReceiptid = string.Empty;
                if (objSDReceipt.SaveShareReceipt(ObjShareReceiptDTO, Con, out OUTReceiptid))
                {
                    if (!string.IsNullOrEmpty(OUTReceiptid))
                    {
                        if (ObjShareReceiptDTO.pModeofreceipt != "ADJUSTMENT")
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

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetShareReceiptView")]
        public async Task<IActionResult> GetShareReceiptView(string FromDate, string Todate)
        {
            List<SharereceiptViewDTO> lstSharereceiptView = new List<SharereceiptViewDTO>();
            try
            {
                lstSharereceiptView = objSDReceipt.GetShareReceiptView(FromDate, Todate, Con);
                return lstSharereceiptView != null ? Ok(lstSharereceiptView) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Banking/Transactions/SDReceipt/GetShareTransaction")]
        public async Task<IActionResult> GetShareTransaction(Int64 ShareAccountId)
        {
            List<ShareTransactionDTO> lstShareTransaction = new List<ShareTransactionDTO>();
            try
            {
                lstShareTransaction = objSDReceipt.GetShareTransaction(ShareAccountId, Con);
                return lstShareTransaction != null ? Ok(lstShareTransaction) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
    }
}