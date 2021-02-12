using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Common;
using FinstaInfrastructure.Loans.Transactions;
using FinstaRepository.DataAccess.Loans.Transactions;
using FinstaRepository.Interfaces.Loans.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FinstaInfrastructure.Accounting;
using FinstaRepository.DataAccess.Accounting.Transactions;
using FinstaRepository.Interfaces.Accounting.Transactions;
using FinstaRepository.Interfaces.Settings;
using FinstaRepository.DataAccess.Settings;

namespace FinstaApi.Controllers.Loans.Transactions
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ReceiptsController : ControllerBase
    {
        IReceipts ObjReceipts = new ReceiptsDAL();
        public EmiReceiptsDTO ReceiptsDTO { get; set; }
        public List<EmiReceiptsDTO> lstReceipts { get; set; }
        public List<ParticularsDTO> lstParticulars { get; set; }
        public List<OutstandingbalDTO> lstOutstandingbal { get; set; }
        public List<LoannamesDTO> lstLoannames { get; set; }
        public List<LoandetailsDTO> lstLoandetails { get; set; }
        public List<ViewParticularsDTO> lstParticularsDetails { get; set; }
        public List<ViewtodayreceiptsDTO> lstViewtodayreceipts { get; set; }
        public GeneralReceiptReportDTO _GeneralReceiptReportDTO { get; set; }
        IAccountingTransactions _AccountingTransactionsDAL = new AccountingTransactionsDAL();
        ISettings obj = new SettingsDAL();
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ReceiptsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        #region GetLoannames
        [Route("api/loans/Transactions/Receipts/GetLoannames")]
        [HttpGet]
        public IActionResult GetApplicantionid()
        {
            lstLoannames = new List<LoannamesDTO>();
            try
            {
                lstLoannames = ObjReceipts.GetLoannames(Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoannames);

        }
        #endregion

        #region GetApplicantionid
        [Route("api/loans/Transactions/Receipts/GetApplicantionid")]
        [HttpGet]
        public IActionResult GetApplicantionid(string loanname,string formname)
        {
            lstReceipts = new List<EmiReceiptsDTO>();
            try
            {
                lstReceipts = ObjReceipts.GetApplicantionid(loanname, Con,formname);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstReceipts);

        }
        #endregion

        #region GetParticulars
        [Route("api/loans/Transactions/Receipts/GetParticulars")]
        [HttpGet]
        public IActionResult GetParticulars(string loanid, string transdate, string formname)
        {
            lstParticulars = new List<ParticularsDTO>();
            try
            {
             lstParticulars = ObjReceipts.GetParticulars(loanid, transdate, Con, formname);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstParticulars);

        }
        #endregion

        #region ViewParticularsDetails
        [Route("api/loans/Transactions/Receipts/ViewParticularsDetails")]
        [HttpGet]
        public IActionResult ViewParticularsDetails(string loanid, string transdate,string todate,string duestype)
        {
            lstParticularsDetails = new List<ViewParticularsDTO>();
            try
            {
             
                lstParticularsDetails = ObjReceipts.ViewParticularsDetails(loanid, transdate, Con,  todate,  duestype);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstParticularsDetails);

        }
        #endregion

        #region GetTransactions
        [Route("api/loans/Transactions/Receipts/GetTransactions")]
        [HttpGet]
        public IActionResult GetTransactions(string loanid)
        {
            lstOutstandingbal = new List<OutstandingbalDTO>();
            try
            {
                lstOutstandingbal = ObjReceipts.GetTransactions(loanid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstOutstandingbal);

        }
        #endregion

        #region GetLoandetails
        [Route("api/loans/Transactions/Receipts/GetLoandetails")]
        [HttpGet]
        public IActionResult GetLoandetails(string loanid)
        {
            lstLoandetails = new List<LoandetailsDTO>();
            try
            {
                lstLoandetails = ObjReceipts.GetLoandetails(loanid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoandetails);

        }
        #endregion

        #region SaveEmiReceipt
        [Route("api/loans/Transactions/Receipts/SaveEmiReceipt")]
        [HttpPost]
        public async Task<IActionResult> SaveEmiReceipt([FromBody] SaveEmireceiptsDTO SaveEmireceiptslist)
        {          
            try
            {
                string OUTReceiptid = string.Empty;
                if (ObjReceipts.SaveEmiReceipt(SaveEmireceiptslist, Con, out OUTReceiptid))
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
                    return Ok(false);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        #endregion


        #region Viewtodayreceipts
        [Route("api/loans/Transactions/Receipts/Viewtodayreceipts")]
        [HttpGet]
        public IActionResult Viewtodayreceipts(string fromdate, string todate,string formname)
        {
            lstViewtodayreceipts = new List<ViewtodayreceiptsDTO>();
            try
            {
                lstViewtodayreceipts = ObjReceipts.Viewtodayreceipts(fromdate,todate, Con, formname);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewtodayreceipts);

        }
        #endregion
    }
}