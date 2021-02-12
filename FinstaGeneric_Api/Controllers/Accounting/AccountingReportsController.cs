using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Masters;
using FinstaRepository.DataAccess.Accounting.Reports;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Masters;
using FinstaRepository.Interfaces.Accounting.Reports;
using FinstaRepository.Interfaces.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FinstaApi.Controllers.Accounting
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AccountingReportsController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        IAccountingReports _AccountingReportsDAL = new AccountReportsDAL();
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        IAccountMaster objaccountmaster = new AccountMasterDAL();
        public cashBookDto _CashbookDTO { get; set; }
        public DayBookDto _DaybookDTO { get; set; }
        public List<BankBookDTO> lstBankBook { get; set; }
        public BankBookDTO _BankBookDto { get; set; }
        public BRSDto _BRSDto { get; set; }
        public ProfitAndLossDTO _ProfitAndLossDTO { get; set; }
        public List<ProfitAndLossDTO> lstProfitAndLossDTO { get; set; }
        public List<BalanceSheetDTO> _lstBalanceSheet { get; set; }
        public List<AccountReportsDTO> _lstreportdetails { get; set; }
        public List<AccountReportsDTO> lstAccountReportsDTO { get; set; }
        public List<ComparisionTb> lstComparisionTb { get; set; }
        public ChequeEnquiryDTO _ChequeEnquiryDTO { get; set; }
        public JvListDTO _JvListDTO { get; set; }

        public IssuedChequeDTO _IssuedChequeDTO { get; set; }

        public ledgerExtractDTO _LedgerExtractDTO { get; set; }

        public subAccountLedgerDTO _SubAccountLedgerDTO { get; set; }
        public subLedgerSummaryDTO _SubLedgerSummaryDTO { get; set; }

        public AccountingReportsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region Cashbook
        [Route("api/Accounting/AccountingReports/getCashbookData")]
        [HttpGet]
        public async Task<IActionResult> getCashbookData(string fromdate, string todate, string transType)
        {
            _CashbookDTO = new cashBookDto();
            try
            {
                _CashbookDTO.plstcashbookdata = await _AccountingReportsDAL.getCashbookData(Con, fromdate, todate, transType);
                //_CashbookDTO.plstcashchequetotal = await _AccountingReportsDAL.getCashbookDataTotals(Con, fromdate, todate);

                return Ok(_CashbookDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion

        #region BankBook
        [Route("api/Accounting/AccountingReports/GetBankNames")]
        [HttpGet]
        public async Task<IActionResult> GetBankNames()
        {
            lstBankBook = new List<BankBookDTO>();
            try
            {
                lstBankBook = await _AccountingReportsDAL.GetBankNames(Con);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstBankBook);
        }
        [Route("api/Accounting/AccountingReports/GetBankBookDetails")]
        [HttpGet]
        public async Task<IActionResult> GetBankBookDetails(string FromDate, string ToDate, long _pBankAccountId)
        {
            _BankBookDto = new BankBookDTO();
            try
            {
                _BankBookDto.plstBankBook = await _AccountingReportsDAL.GetBankBookDetails(Con, FromDate, ToDate, _pBankAccountId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_BankBookDto.plstBankBook);
        }
        #endregion

        #region DAY BOOK

        [Route("api/Accounting/AccountingReports/getDaybook")]
        [HttpGet]
        public async Task<IActionResult> getDaybook(string fromdate, string todate)
        {
            _DaybookDTO = new DayBookDto();
            try
            {
                _DaybookDTO.plstdaybookdata = await _AccountingReportsDAL.getDaybook(Con, fromdate, todate);
                _DaybookDTO.plstdaybooktotals = await _AccountingReportsDAL.getDaybookTotals(Con, fromdate, todate);

                return Ok(_DaybookDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region GetAccountLedgerDetails
        [Route("api/Accounting/AccountingReports/GetAccountLedgerDetails")]
        [HttpGet]
        public async Task<IActionResult> GetAccountLedgerDetails(string fromDate, string toDate, long pAccountId, long pSubAccountId)
        {
            _BankBookDto = new BankBookDTO();
            try
            {
                _BankBookDto.plstBankBook = await _AccountingReportsDAL.GetAccountLedgerDetails(Con, fromDate, toDate, pAccountId, pSubAccountId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_BankBookDto.plstBankBook);
        }
        #endregion

        #region GetPartyLedgerDetails
        [Route("api/Accounting/AccountingReports/GetPartyLedgerDetails")]
        [HttpGet]
        public async Task<IActionResult> GetPartyLedgerDetails(string fromDate, string toDate, long pAccountId, long pSubAccountId, string pPartyRefId)
        {
            _BankBookDto = new BankBookDTO();
            try
            {
                _BankBookDto.plstBankBook = await _AccountingReportsDAL.GetPartyLedgerDetails(Con, fromDate, toDate, pAccountId, pSubAccountId, pPartyRefId);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

            return Ok(_BankBookDto.plstBankBook);
        }
        #endregion

        #region AccountLedger
        [Route("api/Accounting/AccountingReports/GetBrs")]
        [HttpGet]
        public async Task<IActionResult> GetBrs(string fromDate, long _pBankAccountId)
        {
            _BRSDto = new BRSDto();
            try
            {
                _BRSDto.lstBRSDto = await _AccountingReportsDAL.GetBrs(Con, fromDate, _pBankAccountId);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

            return Ok(_BRSDto.lstBRSDto);
        }
        #endregion

        #region Trialbalance
        [Route("api/Accounting/AccountingReports/GetTrialBalance")]
        [HttpGet]
        public async Task<IActionResult> GetTrialBalance(string fromDate, string todate, string GroupType)
        {
            lstAccountReportsDTO = new List<AccountReportsDTO>();
            try
            {
                lstAccountReportsDTO = await _AccountingReportsDAL.GetTrialBalance(Con, fromDate, todate, GroupType);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

            return Ok(lstAccountReportsDTO);
        }
        #endregion

        #region LedgerSummary
        [Route("api/Accounting/AccountingReports/GetLedgerSummary")]
        [HttpGet]
        public async Task<IActionResult> GetLedgerSummary(string fromDate, string todate, string pAccountId)
        {
            lstAccountReportsDTO = new List<AccountReportsDTO>();
            try
            {
                lstAccountReportsDTO = await _AccountingReportsDAL.GetLedgerSummary(Con, fromDate, todate, pAccountId);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
                //return Ok(ex.ToString());
            }

            return Ok(lstAccountReportsDTO);
        }
        #endregion

        #region ComparisionTB
        [Route("api/Accounting/AccountingReports/GetComparisionTB")]
        [HttpGet]
        public async Task<IActionResult> GetComparisionTB(string fromDate, string todate)
        {
            lstComparisionTb = new List<ComparisionTb>();
            try
            {
                lstComparisionTb = await _AccountingReportsDAL.GetComparisionTB(Con, fromDate, todate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstComparisionTb);
        }
        #endregion

        #region Profit and Loss
        [Route("api/Accounting/AccountingReports/GetProfitAndLossData")]
        [HttpGet]
        public async Task<IActionResult> GetProfitAndLossData(string fromdate, string todate, string groupType)
        {

            lstProfitAndLossDTO = new List<ProfitAndLossDTO>();
            try
            {
                lstProfitAndLossDTO = await _AccountingReportsDAL.GetProfitAndLossData(Con, fromdate, todate, groupType);


                return Ok(lstProfitAndLossDTO);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion

        #region BalanceSheet
        [Route("api/Accounting/AccountingReports/GetBalanceSheetDetails")]
        [HttpGet]
        public async Task<IActionResult> GetBalanceSheetDetails(string fromdate)
        {
            _lstBalanceSheet = new List<BalanceSheetDTO>();
            try
            {
                _lstBalanceSheet = await _AccountingReportsDAL.GetBalanceSheetDetails(Con, fromdate);

                return Ok(_lstBalanceSheet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region Reprint
        [Route("api/Accounting/AccountingReports/GetReprintCount")]
        [HttpGet]
        public async Task<IActionResult> GetReprintCount(string receiptno, long recordid)
        {
            Int64 count = 0;
            try
            {
                count = await _AccountingReportsDAL.GetReprintCount(Con, receiptno, recordid);

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/Accounting/AccountingReports/GetReprintBindDetails")]
        [HttpGet]
        public async Task<IActionResult> GetReprintBindDetails()
        {
            _lstreportdetails = new List<AccountReportsDTO>();
            try
            {
                _lstreportdetails = await _AccountingReportsDAL.GetReprintBindDetails(Con);

                return Ok(_lstreportdetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion


        #region GetReferenceNo
        [Route("api/Accounting/AccountingReports/GetReferenceNo")]
        [HttpGet]
        public async Task<IActionResult> GetReferenceNo(string FormName, string TransactionNo)
        {
            string TransReferenceNo = string.Empty;
            List<string> lstdata = new List<string>();
            try
            {
                TransReferenceNo = await _AccountingReportsDAL.GetReferenceNo(Con, FormName, TransactionNo);


                lstdata.Add(TransReferenceNo);
                return Ok(lstdata);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        #endregion

        #region ChequesEnquiry
        [Route("api/Accounting/AccountingReports/GetReceivedChequesData")]
        [HttpGet]
        public async Task<IActionResult> GetReceivedChequesData(string pChequeNo)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetReceivedChequesData(Con, pChequeNo);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        [Route("api/Accounting/AccountingReports/GetReceivedChequesDetails")]
        [HttpGet]
        public async Task<IActionResult> GetReceivedChequesDetails(string pChequeNo)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetReceivedChequesDetails(Con, pChequeNo);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        [Route("api/Accounting/AccountingReports/GetIssuedChequesData")]
        [HttpGet]
        public async Task<IActionResult> GetIssuedChequesData(string pChequeNo)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetIssuedChequesData(Con, pChequeNo);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        [Route("api/Accounting/AccountingReports/GetIssuedChequesDetails")]
        [HttpGet]
        public async Task<IActionResult> GetIssuedChequesDetails(string pChequeNo)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetIssuedChequesDetails(Con, pChequeNo);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion

        #region JvList
        [Route("api/Accounting/AccountingReports/GetJvListDetails")]
        [HttpGet]
        public async Task<IActionResult> GetJvListDetails(string fromdate,string todate,string pmodeoftransaction)
        {
            _JvListDTO = new JvListDTO();
            try
            {
                _JvListDTO.plstJvList = await _AccountingReportsDAL.GetJvListDetails(Con,fromdate,todate, pmodeoftransaction);
                return Ok(_JvListDTO.plstJvList);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion
        #region Cheque Cancel
        [Route("api/Accounting/AccountingReports/GetChequeCancelDetails")]
        [HttpGet]
        public async Task<IActionResult> GetChequeCancelDetails(string fromdate, string todate)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetChequeCancelDetails(Con, fromdate, todate);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion
        #region Cheque Return
        [Route("api/Accounting/AccountingReports/GetChequeReturnDetails")]
        [HttpGet]
        public async Task<IActionResult> GetChequeReturnDetails(string fromdate, string todate)
        {
            _ChequeEnquiryDTO = new ChequeEnquiryDTO();
            try
            {
                _ChequeEnquiryDTO.plstChequeEnquiry = await _AccountingReportsDAL.GetChequeReturnDetails(Con, fromdate, todate);
                return Ok(_ChequeEnquiryDTO.plstChequeEnquiry);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion
        #region Issued Cheques
        [Route("api/Accounting/AccountingReports/GetIssuedChequeNumbers")]
        [HttpGet]
        public async Task<IActionResult> GetIssuedChequeNumbers(long _BankId)
        {
            _IssuedChequeDTO = new IssuedChequeDTO();
            try
            {
                _IssuedChequeDTO.lstIssuedCheque = await _AccountingReportsDAL.GetIssuedChequeNumbers(Con, _BankId);
                return Ok(_IssuedChequeDTO.lstIssuedCheque);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/Accounting/AccountingReports/GetIssuedChequeDetails")]
        [HttpGet]
        public async Task<IActionResult> GetIssuedChequeDetails(long _BankId,long _ChqBookId,long _ChqFromNo,long _ChqToNo)
        {
            _IssuedChequeDTO = new IssuedChequeDTO();
            try
            {
                _IssuedChequeDTO.lstIssuedCheque = await _AccountingReportsDAL.GetIssuedChequeDetails(Con, _BankId,_ChqBookId,_ChqFromNo, _ChqToNo);
                return Ok(_IssuedChequeDTO.lstIssuedCheque);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        [Route("api/Accounting/AccountingReports/UnusedhequeCancel")]
        [HttpPost]
        public async Task<IActionResult> UnusedhequeCancel(IssuedChequeDTO issuedChequeDTO)
        {
            bool isSaved = false;
            try
            {
                isSaved = await _AccountingReportsDAL.UnusedhequeCancel(Con, issuedChequeDTO);
                return Ok(isSaved);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        #endregion

        #region Ledger Extract
        [Route("api/Accounting/AccountingReports/GetLedgerExtractDetails")]
        [HttpGet]
        public async Task<IActionResult> GetLedgerExtractDetails(string fromdate, string todate)
        {
            _LedgerExtractDTO = new ledgerExtractDTO();
            try
            {
                _LedgerExtractDTO.pLedgerExtract = await _AccountingReportsDAL.GetLedgerExtractDetails(Con, fromdate, todate);
                return Ok(_LedgerExtractDTO.pLedgerExtract);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }
        #endregion

        #region Sub Account Ledger
        [Route("api/Accounting/AccountingReports/GetSubAccountLedgerDetails")]
        [HttpGet]
        public async Task<IActionResult> GetSubAccountLedgerDetails()
        {
            _SubAccountLedgerDTO = new subAccountLedgerDTO();
            try
            {
                _SubAccountLedgerDTO.plstSubAccountLedger = await _AccountingReportsDAL.GetSubAccountLedgerDetails(Con);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_SubAccountLedgerDTO.plstSubAccountLedger);
        }

        [Route("api/Accounting/AccountingReports/GetAccountLedgerNames")]
        [HttpGet]
        public async Task<IActionResult> GetAccountLedgerNames(string SubLedgerName)
        {
            _SubAccountLedgerDTO = new subAccountLedgerDTO();
            try
            {
                _SubAccountLedgerDTO.plstSubAccountLedger = await _AccountingReportsDAL.GetAccountLedgerData(Con, SubLedgerName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_SubAccountLedgerDTO.plstSubAccountLedger);
        }

        [Route("api/Accounting/AccountingReports/GetSubAccountLedgerReportData")]
        [HttpGet]
        public async Task<IActionResult> GetSubAccountLedgerReportData(string SubLedgerName, long parentid, string fromDate,string toDate)
        {
            _SubAccountLedgerDTO = new subAccountLedgerDTO();
            try
            {
                _SubAccountLedgerDTO.plstSubAccountLedger = await _AccountingReportsDAL.GetSubLedgerReportData(Con, SubLedgerName, parentid, fromDate, toDate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_SubAccountLedgerDTO.plstSubAccountLedger);
        }
        #endregion

        #region Sub Ledger Summary
        [Route("api/Accounting/AccountingReports/GetMainAccountHeads")]
        [HttpGet]
        public async Task<IActionResult> GetMainAccountHeads()
        {
            _SubLedgerSummaryDTO = new subLedgerSummaryDTO();
            try
            {
                _SubLedgerSummaryDTO.plstSubLedgerSummary = await _AccountingReportsDAL.GetMainAccountHeads(Con);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_SubLedgerSummaryDTO.plstSubLedgerSummary);
        }

        [Route("api/Accounting/AccountingReports/GetSubLedgerAccountNames")]
        [HttpGet]
        public async Task<IActionResult> GetSubLedgerAccountNames(string acctype)
        {
            _SubLedgerSummaryDTO = new subLedgerSummaryDTO();
            try
            {
                _SubLedgerSummaryDTO.plstSubLedgerSummary = await _AccountingReportsDAL.GetSubLedgerAccountNames(Con, acctype);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(_SubLedgerSummaryDTO.plstSubLedgerSummary);
        }

        [Route("api/Accounting/AccountingReports/GetSubLedgerSummaryReportData")]
        [HttpGet]
        public async Task<IActionResult> GetSubLedgerSummaryReportData(long mainAccountid,string parentids, string fromDate, string toDate)
        {
            string jsonstring = string.Empty;
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                ds = await _AccountingReportsDAL.GetSubLedgerSummaryReportData(Con, mainAccountid, parentids, fromDate, toDate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            var result = new { gridData = ds.Tables[0], columnData = ds.Tables[1] };
            return Ok(JsonConvert.SerializeObject(result));
        }
        [Route("api/Accounting/AccountingReports/GetMonthlyComparisionReportData")]
        [HttpGet]
        public async Task<IActionResult> GetMonthlyComparisionReportData(long mainAccountid, string parentids, string fromDate, string toDate)
        {
            string jsonstring = string.Empty;
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                ds = await _AccountingReportsDAL.GetMonthlyComparisionReportData(Con, mainAccountid, parentids, fromDate, toDate);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            var result = new { gridData = ds.Tables[0], columnData = ds.Tables[1] };
            return Ok(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region MTD AND YTD ProfitandLoss
        [Route("api/Accounting/AccountingReports/GetMtdYtdPandL")]
        [HttpGet]
        public async Task<IActionResult> GetMtdYtdPandL(string ReportType, string fromDate)
        {
            MtdytdpandlDTO obj = new MtdytdpandlDTO();
            try
            {
                string[] strSubType = { "SUMMARY", "DETAILS" };
                obj = await _AccountingReportsDAL.GetMTDYTDPANDL(Con, ReportType, fromDate, strSubType);
                //obj.plstMtdytdpandldetails = await _AccountingReportsDAL.GetMTDYTDPANDL(Con, ReportType, fromDate,"DETAILS");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(obj);
        }
        #endregion
    }

}