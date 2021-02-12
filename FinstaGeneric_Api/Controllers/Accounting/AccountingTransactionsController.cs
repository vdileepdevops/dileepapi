using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.DataAccess.Settings;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Masters;
using FinstaRepository.DataAccess.Accounting.Masters;

using FinstaRepository.Interfaces.Settings;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using FinstaInfrastructure.Settings;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FinstaInfrastructure.Common;

namespace FinstaApi.Controllers.Accounting
{
    [Authorize]
    // [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AccountingTransactionsController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        IReferralAdvocate obReferralAdvocate = new ReferralAdvocateDAL();
        ISettings obj = new SettingsDAL();

        string Con = string.Empty;
        static IConfiguration _iconfiguration;

        IAccountMaster objaccountmaster = new AccountMasterDAL();


        public AccountsMasterDTO _accountsmasterdto { get; set; }

        public List<TypeofPaymentDTO> typeofpaymentlist { get; set; }
        public List<BankDTO> banklist { get; set; }
        public List<AccountsDTO> accountslist { get; set; }
        public List<PartyDTO> partylist { get; set; }

        public List<PaymentVoucherDTO> ppaymentslist { get; set; }
        public GeneralreceiptDTO GeneralreceiptDTO { get; set; }

        public List<GstDTo> Gstlist { get; set; }

        public List<AccountTreeDTO> accounttreelist { get; set; }

        public List<Modeoftransaction> Modeoftransactionlist { get; set; }

        public List<GeneralreceiptDTO> GeneralReceiptList { get; set; }

        public List<GeneralReceiptReportDTO> GeneralReceiptReporDataList { get; set; }
        public List<JournalVoucherDTO> JournalVoucherDataList { get; set; }

        public List<GeneralReceiptReportDTO> GeneralReceiptReportDataList { get; set; }
        public GeneralReceiptReportDTO _GeneralReceiptReportDTO { get; set; }

        public PaymentVoucherDTO pPaymentVoucherDTO { get; set; }
        public List<JournalVoucherReportDTO> lstJournalVoucherReport { get; set; }
        public CompanyInfoDTO _CompanyInfoDTO { get; set; }


        public AccountingTransactionsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetTypeofpaymentList")]

        public async Task<IActionResult> GetTypeofpaymentList()
        {
            typeofpaymentlist = new List<TypeofPaymentDTO>();
            try
            {
                typeofpaymentlist = await _AccountingTransactionsDAL.GetTypeofpaymentList(Con);
                return Ok(typeofpaymentlist);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetBankntList")]

        public async Task<IActionResult> GetBanknList()
        {
            banklist = new List<BankDTO>();
            try
            {
                banklist = await _AccountingTransactionsDAL.GetBankntList(Con);
                return Ok(banklist);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetLedgerAccountList")]

        public async Task<IActionResult> GetLedgerAccountList(string formname)
        {

            accountslist = new List<AccountsDTO>();
            try
            {
                accountslist = await _AccountingTransactionsDAL.GetLedgerAccountList(Con, formname);
                return Ok(accountslist);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetSubLedgerAccountList")]
        public async Task<IActionResult> GetSubLedgerAccountList(long ledgerid)
        {

            accountslist = new List<AccountsDTO>();
            try
            {
                accountslist = await _AccountingTransactionsDAL.GetSubLedgerAccountList(ledgerid, Con);
                return Ok(accountslist);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetPartyList")]

        public async Task<IActionResult> GetPartyList()
        {

            partylist = new List<PartyDTO>();
            try
            {
                partylist = await _AccountingTransactionsDAL.GetPartyList(Con);
                return Ok(partylist);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        #region General Receipt
        /// <summary>
        /// General Receipt Save
        /// </summary>
        /// <param name="GeneralreceiptDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Accounting/AccountingTransactions/SaveGeneralReceipt")]
        public async Task<IActionResult> SaveGeneralReceipt([FromBody] GeneralreceiptDTO GeneralreceiptDTO)
        {
            try
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
                if (!string.IsNullOrEmpty(GeneralreceiptDTO.pFilepath))
                {
                    string OldFullPath = Path.Combine(OldPath, GeneralreceiptDTO.pFilepath);
                    string NewFullPath = Path.Combine(newPath, GeneralreceiptDTO.pFilepath);
                    GeneralreceiptDTO.pFilepath = NewFullPath;
                    if (System.IO.File.Exists(OldFullPath))
                    {
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }

                string GeneralReceiptId = string.Empty;
                if (_AccountingTransactionsDAL.SaveGeneralReceipt(GeneralreceiptDTO, Con, out GeneralReceiptId))
                {
                    if (!string.IsNullOrEmpty(GeneralReceiptId))
                    {
                        //GeneralReceiptReportDataList = new List<GeneralReceiptReportDTO>();
                        //GeneralReceiptReportDataList = await _AccountingTransactionsDAL.GetgeneralreceiptReportData(GeneralReceiptId, Con);
                        _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                        _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(GeneralReceiptId, Con);
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
                    return Ok(false);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets Data for Main View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetGeneralReceiptsData")]
        public async Task<IActionResult> GetReceiptsData()
        {
            GeneralReceiptList = new List<GeneralreceiptDTO>();
            try
            {
                GeneralReceiptList = await _AccountingTransactionsDAL.GetReceiptsData(Con);
                return GeneralReceiptList != null && GeneralReceiptList.Count > 0 ? Ok(GeneralReceiptList) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        #endregion

        [Route("api/accounting/accountingtransactions/SavePaymentVoucher")]
        [HttpPost]
        public IActionResult SavePaymentVoucher(PaymentVoucherDTO _PaymentVoucherDTOO)
        {
            bool isSaved = false;
            List<string> lstdata = new List<string>();
            try
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
                if (!string.IsNullOrEmpty(_PaymentVoucherDTOO.pFilepath))
                {
                    string OldFullPath = Path.Combine(OldPath, _PaymentVoucherDTOO.pFilepath);
                    string NewFullPath = Path.Combine(newPath, _PaymentVoucherDTOO.pFilepath);
                    _PaymentVoucherDTOO.pFilepath = NewFullPath;
                    if (System.IO.File.Exists(OldFullPath))
                    {
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                string paymentId = string.Empty;
                isSaved = _AccountingTransactionsDAL.SavePaymentVoucher(_PaymentVoucherDTOO, Con, out paymentId);
                lstdata.Add(isSaved.ToString().ToUpper());
                lstdata.Add(paymentId);
                return Ok(lstdata);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());

            }
        }

        [HttpGet]
        [Route("api/accounting/accountingtransactions/GetPaymentVoucherExistingData")]

        public async Task<IActionResult> GetPaymentVoucherExistingData()
        {
            ppaymentslist = new List<PaymentVoucherDTO>();
            try
            {
                ppaymentslist = await _AccountingTransactionsDAL.GetPaymentVoucherExistingData(Con);
                if (ppaymentslist.Count > 0)
                {
                    return Ok(ppaymentslist);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        [HttpGet]
        [Route("api/accounting/accountingtransactions/GetGstPercentages")]

        public async Task<IActionResult> GetGstPercentages()
        {
            Gstlist = new List<GstDTo>();
            try
            {
                Gstlist = await _AccountingTransactionsDAL.GetGstPercentages(Con);
                if (Gstlist.Count > 0)
                {
                    return Ok(Gstlist);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }


        [HttpGet]
        [Route("api/accounting/accountingtransactions/GetModeoftransactions")]

        public async Task<IActionResult> GetModeoftransactions()
        {
            Modeoftransactionlist = new List<Modeoftransaction>();
            try
            {
                Modeoftransactionlist = await _AccountingTransactionsDAL.GetModeoftransactions(Con);
                if (Modeoftransactionlist != null && Modeoftransactionlist.Count > 0)
                {
                    return Ok(Modeoftransactionlist);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }



        [Route("api/accounting/accountingtransactions/GetReceiptsandPaymentsLoadingData")]
        [HttpGet]

        public async Task<IActionResult> GetReceiptsandPaymentsLoadingData(string formname)
        {

            _accountsmasterdto = new AccountsMasterDTO();
            try
            {
                _accountsmasterdto.banklist = await _AccountingTransactionsDAL.GetBankntList(Con);
                _accountsmasterdto.modeofTransactionslist = await _AccountingTransactionsDAL.GetModeoftransactions(Con); _accountsmasterdto.accountslist = await _AccountingTransactionsDAL.GetLedgerAccountList(Con, formname); _accountsmasterdto.partylist = await _AccountingTransactionsDAL.GetPartyList(Con);
                _accountsmasterdto.Gstlist = await _AccountingTransactionsDAL.GetGstPercentages(Con);
                _accountsmasterdto.bankdebitcardslist = await _AccountingTransactionsDAL.GetDebitCardNumbers(Con);
                _accountsmasterdto.cashbalance = await _AccountingTransactionsDAL.getcashbalance(Con);
                _accountsmasterdto.bankbalance = _AccountingTransactionsDAL.GetBankBalance(0, Con);
                return Ok(_accountsmasterdto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }


        [Route("api/accounting/accountingtransactions/getPartyDetailsbyid")]
        [HttpGet]

        public async Task<IActionResult> getPartyDetailsbyid(long ppartyid)
        {

            _accountsmasterdto = new AccountsMasterDTO();
            try
            {

                _accountsmasterdto.statelist = await _AccountingTransactionsDAL.getStatesbyPartyid(ppartyid, Con, 1);
                _accountsmasterdto.lstTdsSectionDetails = await _AccountingTransactionsDAL.getTdsSectionsbyPartyid(ppartyid, Con);
                _accountsmasterdto.accountbalance = await _AccountingTransactionsDAL.getpartyAccountbalance(ppartyid, Con);
                return Ok(_accountsmasterdto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }

        [Route("api/accounting/accountingtransactions/GetBankDetailsbyId")]
        [HttpGet]
        public async Task<IActionResult> GetBankDetailsbyId(long pbankid)
        {

            _accountsmasterdto = new AccountsMasterDTO();
            try
            {
                _accountsmasterdto.bankupilist = await _AccountingTransactionsDAL.GetUpiNames(pbankid, Con);
                _accountsmasterdto.chequeslist = await _AccountingTransactionsDAL.GetChequeNumbers(pbankid, Con);
                return Ok(_accountsmasterdto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }


        [Route("api/accounting/accountingtransactions/GetSubLedgerData")]
        [HttpGet]
        public async Task<IActionResult> GetSubLedgerData(long pledgerid)
        {

            accountslist = new List<AccountsDTO>();
            try
            {
                accountslist = await _AccountingTransactionsDAL.GetSubLedgerAccountList(pledgerid, Con);
                return Ok(accountslist);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }

        }


        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetAccountTreeList")]
        public IActionResult GetAccountTreeList()
        {
            accounttreelist = new List<AccountTreeDTO>();
            try
            {
                accounttreelist = objaccountmaster.GetAccountTreeDetails(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(accounttreelist);
        }
        #region Account Tree By Sai mahesh
        [Route("api/accounting/accountingtransactions/GetAccountTree")]
        [HttpGet]
        public async Task<IActionResult> GetAccountTree()
        {
            try
            {
                List<AccountsTreeDTO> _lstAccountsMasterDTO = await objaccountmaster.GetAccountTree(Con);
                return Ok(_lstAccountsMasterDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("api/accounting/accountingtransactions/AccountTreeSearch")]
        [HttpGet]
        public async Task<IActionResult> AccountTreeSearch(string searchterm)
        {
            try
            {
                List<AccountsTreeDTO> _lstAccountsMasterDTO = await objaccountmaster.AccountTreeSearch(Con, searchterm);
                return Ok(_lstAccountsMasterDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/Accounting/AccountingTransactions/SaveAccountHeads")]
        public IActionResult SaveAccountHeads(AccountCreationNewDTO accountcreate)
        {
            bool isSaved = false;
            try
            {
                isSaved = objaccountmaster.SaveAccountHeads(accountcreate, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        #endregion
        [HttpPost]
        [Route("api/Accounting/AccountingTransactions/SaveAccountMaster")]
        public IActionResult SaveAccountMaster(AccountCreationDTO accountcreate)
        {
            bool isSaved = false;
            try
            {
                isSaved = objaccountmaster.SaveAccountMaster(accountcreate, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);
        }
        [Route("api/accounting/accountingtransactions/SaveJournalVoucher")]
        [HttpPost]
        public IActionResult SaveJournalVoucher(JournalVoucherDTO _JournalVoucherDTO)
        {
            bool isSaved = false;
            List<string> lstdata = new List<string>();
            try
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
                if (!string.IsNullOrEmpty(_JournalVoucherDTO.pFilepath))
                {
                    string OldFullPath = Path.Combine(OldPath, _JournalVoucherDTO.pFilepath);
                    string NewFullPath = Path.Combine(newPath, _JournalVoucherDTO.pFilepath);
                    _JournalVoucherDTO.pFilepath = NewFullPath;
                    if (System.IO.File.Exists(OldFullPath))
                    {
                        System.IO.File.Move(OldFullPath, NewFullPath);
                    }
                }
                string Jvnumber = string.Empty;
                isSaved = _AccountingTransactionsDAL.SaveJournalVoucher_All(_JournalVoucherDTO, Con, out Jvnumber);
                lstdata.Add(isSaved.ToString().ToUpper());
                lstdata.Add(Jvnumber);
                return Ok(lstdata);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());

            }
        }
        [Route("api/accounting/accountingtransactions/checkAccountnameDuplicates")]
        //[HttpPost]
        //[HttpGet("{checkparamtype}/{loanname}/{loancode}")]
        [HttpGet]
        public IActionResult checkAccountnameDuplicates(string Accountname, string AccountType, int Parentid)
        {
            int count = 0;

            try
            {
                count = objaccountmaster.checkAccountnameDuplicates(Accountname, AccountType, Parentid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(count); ;
        }

        #region GeneralReceipts_Reports_Data
        /// <summary>
        /// API for General Receipt Report (Use for Duplicate Report also) Individual Reports.
        /// </summary>
        /// <param name="ReceiptId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetGeneralReceiptReportsIndividual")]
        public async Task<IActionResult> GetGeneralReceiptReports(string ReceiptId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ReceiptId))
                {
                    GeneralReceiptReportDataList = new List<GeneralReceiptReportDTO>();
                    GeneralReceiptReportDataList = await _AccountingTransactionsDAL.GetgeneralreceiptReportData(ReceiptId, Con);
                    if (GeneralReceiptReportDataList != null && GeneralReceiptReportDataList.Count > 0)
                    {

                        return Ok(GeneralReceiptReportDataList);
                    }
                    else
                    {
                        return NotFound();
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
        /// API for General Receipt Report (Use for Duplicate Report also) single Report with List of accounts.
        /// </summary>
        /// <param name="ReceiptId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetgeneralreceiptReportData")]
        public async Task<IActionResult> GetgeneralreceiptReportData1(string ReceiptId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ReceiptId))
                {
                    _GeneralReceiptReportDTO = new GeneralReceiptReportDTO();
                    _GeneralReceiptReportDTO = await _AccountingTransactionsDAL.GetgeneralreceiptReportData1(ReceiptId, Con);
                    _GeneralReceiptReportDTO.pCompanyInfoforReports = obj.GetcompanyNameandaddressDetails(Con);
                    if (_GeneralReceiptReportDTO != null)
                    {
                        return Ok(_GeneralReceiptReportDTO);
                    }
                    else
                    {
                        return NotFound();
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

        #endregion

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetPaymentVoucherReportDataById")]
        public async Task<IActionResult> GetPaymentVoucherReportDataById(string paymentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(paymentId))
                {
                    pPaymentVoucherDTO = new PaymentVoucherDTO();
                    pPaymentVoucherDTO = await _AccountingTransactionsDAL.GetPaymentVoucherReportDataById(paymentId, Con);
                    if (pPaymentVoucherDTO != null)
                    {

                        return Ok(pPaymentVoucherDTO);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status204NoContent);
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

        [HttpGet]
        [Route("api/Accounting/AccountingTransactions/GetPaymentVoucherReportData")]
        public async Task<IActionResult> GetPaymentVoucherReportData(string paymentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(paymentId))
                {
                    List<PaymentVoucherReportDTO> PaymentVoucherReportlist = new List<PaymentVoucherReportDTO>();
                    PaymentVoucherReportlist = await _AccountingTransactionsDAL.GetPaymentVoucherReportData(paymentId, Con);
                    if (PaymentVoucherReportlist != null)
                    {

                        return Ok(PaymentVoucherReportlist);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status204NoContent);
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
        [HttpGet]
        [Route("api/accounting/accountingtransactions/GetJournalVoucherData")]

        public async Task<IActionResult> GetJournalVoucherData()
        {
            JournalVoucherDataList = new List<JournalVoucherDTO>();

            try
            {
                JournalVoucherDataList = await _AccountingTransactionsDAL.GetJournalVoucherData(Con);
                if (JournalVoucherDataList.Count > 0)
                {
                    return Ok(JournalVoucherDataList);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }
        [HttpGet]
        [Route("api/accounting/accountingtransactions/GetJournalVoucherReportData")]

        public async Task<IActionResult> GetJournalVoucherReportData(string Jvnumber)
        {
            lstJournalVoucherReport = new List<JournalVoucherReportDTO>();

            try
            {
                lstJournalVoucherReport = await _AccountingTransactionsDAL.GetJournalVoucherReportData(Con, Jvnumber);
                if (lstJournalVoucherReport.Count > 0)
                {
                    return Ok(lstJournalVoucherReport);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw new FinstaAppException(ex.ToString());
            }
        }

        #region Account Tree New
        [Route("api/accounting/accountingtransactions/AccountTreeNew")]
        [HttpGet]
        public async Task<IActionResult> AccountTreeNew()
        {
            try
            {
                List<AccountTreeNewDTO> _lstAccountsMasterDTO = await objaccountmaster.GetAccountTreeNewDetails(Con);
                return Ok(_lstAccountsMasterDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("api/accounting/accountingtransactions/getTdsSectionNo")]
        [HttpGet]
        public IActionResult getTdsSectionNo()
        {
            List<TdsSectionNewDTO> lstTdsSectionDetails = new List<TdsSectionNewDTO>();
            try
            {
                lstTdsSectionDetails = objaccountmaster.getTdsSectionNo(Con);
            }
            catch (Exception ex)
            {
                throw new FieldAccessException(ex.ToString());
            }
            return Ok(lstTdsSectionDetails);
        }
        #endregion
    }
}