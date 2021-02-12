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
namespace FinstaApi.Controllers.Loans.Transactions
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]

    public class ApprovalController : ControllerBase
    {
        IApproval ObjApproval = new ApprovalDAL();
        public ApprovalDTO ApprovedetailsDTO { get; set; }
        public List<ApprovalDTO> lstApprovedetails { get; set; }
        public List<ViewapplicationsDTO> lstViewapplications { get; set; }
        public List<LoanwisechargeDTO> lstLoanwisecharges { get; set; }
        public List<CashflowDTO> lstCashflow { get; set; }
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public ApprovalController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }
        #region Saveapprovedapplications
        [Route("api/loans/Transactions/Approval/Saveapprovedapplications")]
        [HttpPost]
        public IActionResult Saveapprovedapplications(ApprovalDTO ApprovalList)
        {
            bool isSaved;
            try
            {
                isSaved= ObjApproval.Saveapprovedapplications(ApprovalList, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(isSaved);

        }
        #endregion

        #region Getapprovedapplications
        [Route("api/loans/Transactions/Approval/Getapprovedapplications")]
        [HttpGet]
        public IActionResult Getapprovedapplications(string Applicationid)
        {
            lstApprovedetails = new List<ApprovalDTO>();
            try
            {
                lstApprovedetails = ObjApproval.Getapprovedapplications(Applicationid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstApprovedetails);

        }
        #endregion

        #region ViewApplications
        [Route("api/loans/Transactions/Approval/ViewApplications")]
        [HttpGet]
        public IActionResult ViewApplications(string Viewtype)
        {
            lstViewapplications = new List<ViewapplicationsDTO>();
            try
            {
                lstViewapplications = ObjApproval.ViewApplications(Viewtype, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewapplications);

        }
        #endregion

        #region GetLoanwisecharges
        [Route("api/loans/Transactions/Approval/GetLoanwisecharges")]
        [HttpGet]
        public IActionResult GetLoanwisecharges(string Loanname, decimal Amount, decimal tenor,string applicanttype,string Loanpayin, string tranddate, Int32 schemeid)
        {
            lstLoanwisecharges = new List<LoanwisechargeDTO>();
            try
            {
                lstLoanwisecharges = ObjApproval.GetLoanwisecharges(Loanname, Amount, tenor, applicanttype, Loanpayin, Con,tranddate,  schemeid);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstLoanwisecharges);

        }
        #endregion

        #region ViewApplicationsbyid
        [Route("api/loans/Transactions/Approval/ViewApplicationsbyid")]
        [HttpGet]
        public IActionResult ViewApplicationsbyid(string applicationid)
        {
            lstViewapplications = new List<ViewapplicationsDTO>();
            try
            {
                lstViewapplications = ObjApproval.ViewApplicationsbyid(applicationid, Con);

            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstViewapplications);

        }
        #endregion


        #region GetcashflowStatements
        [Route("api/loans/Transactions/Approval/GetSavingdetails")]
        [HttpGet]
        public IActionResult GetSavingdetails(string Applicationid)
        {
            lstCashflow = new List<CashflowDTO>();
            try
            {
                lstCashflow = ObjApproval.GetSavingdetails(Applicationid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstCashflow);

        }

        [Route("api/loans/Transactions/Approval/GetExstingloandetails")]
        [HttpGet]
        public IActionResult GetExstingloandetails(string Applicationid)
        {
            lstCashflow = new List<CashflowDTO>();
            try
            {
                lstCashflow = ObjApproval.GetExstingloandetails(Applicationid, Con);
            }
            catch (Exception ex)
            {
                throw new FinstaAppException(ex.ToString());
            }
            return Ok(lstCashflow);

        }
        #endregion

    }
}