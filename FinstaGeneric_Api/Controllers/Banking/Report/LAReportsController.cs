using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaInfrastructure.Banking.Reports;
using FinstaRepository.DataAccess.Banking.Reports;
using FinstaRepository.Interfaces.Banking.Reports;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using static FinstaInfrastructure.Banking.Reports.LAReportsDTO;

namespace FinstaApi.Controllers.Banking.Report
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class LAReportsController : ControllerBase
    {
        ILAReports objLAReports = new LAReportsDAL();

        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public LAReportsController(IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }

        

        #region Cash Flow...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetCashFlowSummary")]
        public IActionResult GetCashFlowSummary(string date, string months)
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetCashFlowSummary(date, months, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetCashFlowDetails")]
        public IActionResult GetCashFlowDetails(string Asonmonth,string month)
        {
            List<CashFlowDetailsDTO> lstcashflow = new List<CashFlowDetailsDTO>();
            try
            {
                lstcashflow = objLAReports.GetCashFlowDetails(Asonmonth, month, Con);
                return lstcashflow != null ? Ok(lstcashflow) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetCashFlowPerticularsDetails")]
        public IActionResult GetCashFlowPerticularsDetails(string perticulars, string Asonmonth)
        {
            List<CashFlowPerticularDetailsDTO> lstcashflow = new List<CashFlowPerticularDetailsDTO>();
            try
            {
                lstcashflow = objLAReports.GetCashFlowPerticularsDetails(perticulars,Asonmonth, Con);
                return lstcashflow != null ? Ok(lstcashflow) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Target Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetTargetReportSummary")]
        public IActionResult GetTargetReportSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate)
        {
            List<TargetReportDTO> lstmaturity = new List<TargetReportDTO>();
            try
            {
                lstmaturity = objLAReports.GetTargetReportSummary(receiptfromdate, receipttodate, chequefromdate, chequetodate, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetTargetReportDetails")]
        public IActionResult GetTargetReportDetails(string branch)
        {
            List<TargetReportDTO> lstTargetreport = new List<TargetReportDTO>();
            try
            {
                lstTargetreport = objLAReports.GetTargetReportDetails(branch,Con);
                return lstTargetreport != null ? Ok(lstTargetreport) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Agent Points...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetAgentPointsSummary")]
        public IActionResult GetAgentPointsSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate)
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetAgentPointsSummary(receiptfromdate, receipttodate, chequefromdate, chequetodate, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetAgentPointsDetails")]
        public IActionResult GetAgentPointsDetails(string agentname)
        {
            List<AgentPointsDetailsDTO> lstmaturity = new List<AgentPointsDetailsDTO>();
            try
            {
                lstmaturity = objLAReports.GetAgentPointsDetails(agentname, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Interest Payment Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetInterestreportscheme")]
        public IActionResult GetInterestreportscheme()
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetInterestreportscheme(Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetInterestreportfdaccountnos")]
        public IActionResult GetInterestreportfdaccountnos(string paymenttype, string company, string brnach, long schemeid)
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetInterestreportfdaccountnos(paymenttype,company,brnach, schemeid, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        #endregion

        #region Maturity Intimation Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetMaturityscheme")]
        public IActionResult GetMaturityscheme()
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetMaturityscheme(Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetMaturitybrnach")]
        public IActionResult GetMaturitybrnach(long schemeid)
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetMaturitybrnach(schemeid,Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpPost]
        [Route("api/Banking/Report/LAReports/SaveMaturityIntimationReport")]
        public IActionResult SaveMaturityIntimationReport([FromBody] LAReportsDTO _MaturityIntimationDTO)
        {
            bool isSaved = false;
            LAReportsDTO objSave = new LAReportsDTO();
            try
            {

                isSaved = objLAReports.SaveMaturityIntimationReport(_MaturityIntimationDTO, Con);
                return Ok(objSave);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }



        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowMaturityIntimationReport")]
        public IActionResult ShowMaturityIntimationReport(long schemeid, string branchname, string fromdate, string todate)
        {
            List<MaturityIntimationDTO> lstmaturity = new List<MaturityIntimationDTO>();
            try
            {
                lstmaturity = objLAReports.ShowMaturityIntimationReport(schemeid, branchname, fromdate, todate, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetMaturityIntimationLetter")]
        public IActionResult GetMaturityIntimationLetter(string FdAccountNo)
        {
            List<MaturityIntimationDTO> lstmaturity = new List<MaturityIntimationDTO>();
            try
            {
                lstmaturity = objLAReports.GetMaturityIntimationLetter(FdAccountNo, Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Lien Release Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetLienbrnach")]
        public IActionResult GetLienbrnach()
        {
            List<LAReportsDTO> lstlien = new List<LAReportsDTO>();
            try
            {
                lstlien = objLAReports.GetLienbrnach(Con);
                return lstlien != null ? Ok(lstlien) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowLienReleaseReport")]
        public IActionResult ShowLienReleaseReport(string branchname, string fromdate, string todate)
        {
            List<LienReleaseDTO> lstlienrelease = new List<LienReleaseDTO>();
            try
            {
                lstlienrelease = objLAReports.ShowLienReleaseReport(branchname, fromdate, todate, Con);
                return lstlienrelease != null ? Ok(lstlienrelease) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Self Adjustment Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowSelfAdjustmentReport")]
        public IActionResult ShowSelfAdjustmentReport(string paymenttype, string companyname, string branchname, string fromdate, string todate)
        {
            List<SelfAdjustmentDTO> lstselfadjustment = new List<SelfAdjustmentDTO>();
            try
            {
                lstselfadjustment = objLAReports.ShowSelfAdjustmentReport(paymenttype, companyname, branchname, fromdate, todate, Con);
                return lstselfadjustment != null ? Ok(lstselfadjustment) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetSelfAdjustmentcompany")]
        public IActionResult GetSelfAdjustmentcompany()
        {
            List<LAReportsDTO> lstcompany = new List<LAReportsDTO>();
            try
            {
                lstcompany = objLAReports.GetSelfAdjustmentcompany(Con);
                return lstcompany != null ? Ok(lstcompany) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetSelfAdjustmentbrnach")]
        public IActionResult GetSelfAdjustmentbrnach(string companyname)
        {
            List<LAReportsDTO> lstbranch = new List<LAReportsDTO>();
            try
            {
                lstbranch = objLAReports.GetSelfAdjustmentbrnach(companyname, Con);
                return lstbranch != null ? Ok(lstbranch) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Maturity Trend Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowMaturityTrendGridHeader")]
        public IActionResult ShowMaturityTrendGridHeader()
        {
            List<MaturityTrendDTO> lsttrend = new List<MaturityTrendDTO>();
            try
            {
                lsttrend = objLAReports.ShowMaturityTrendGridHeader(Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowMaturityTrendReport")]
        public IActionResult ShowMaturityTrendReport()
        {
            List<MaturityTrendDTO> lsttrend = new List<MaturityTrendDTO>();
            try
            {
                lsttrend = objLAReports.ShowMaturityTrendReport(Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowShemeAndDatewiseDetails")]
        public IActionResult ShowShemeAndDatewiseDetails(string schemename, string maturitydate)
        {
            List<MaturityTrendDetailsDTO> lsttrend = new List<MaturityTrendDetailsDTO>();
            try
            {
                lsttrend = objLAReports.ShowShemeAndDatewiseDetails(schemename, maturitydate,Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowGrandTotalDatewiseDetails")]
        public IActionResult ShowGrandTotalDatewiseDetails(string maturitydate)
        {
            List<MaturityTrendDetailsDTO> lsttrend = new List<MaturityTrendDetailsDTO>();
            try
            {
                lsttrend = objLAReports.ShowGrandTotalDatewiseDetails(maturitydate,Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        #endregion

        #region Interest Payment Trend Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowInterestPaymentReport")]
        public IActionResult ShowInterestPaymentReport()
        {
            List<InterestPaymentTrendDTO> lstInteresttrend = new List<InterestPaymentTrendDTO>();
            try
            {
                lstInteresttrend = objLAReports.ShowInterestPaymentReport(Con);
                return lstInteresttrend != null ? Ok(lstInteresttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowInterestTrendShemeAndDatewiseDetails")]
        public IActionResult ShowInterestTrendShemeAndDatewiseDetails(string schemename, string maturitydate)
        {
            List<InterestPaymentTrendDetailsDTO> lsttrend = new List<InterestPaymentTrendDetailsDTO>();
            try
            {
                lsttrend = objLAReports.ShowInterestTrendShemeAndDatewiseDetails(schemename, maturitydate, Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowInterestTrendGrandTotalDatewiseDetails")]
        public IActionResult ShowInterestTrendGrandTotalDatewiseDetails(string maturitydate)
        {
            List<InterestPaymentTrendDetailsDTO> lsttrend = new List<InterestPaymentTrendDetailsDTO>();
            try
            {
                lsttrend = objLAReports.ShowInterestTrendGrandTotalDatewiseDetails(maturitydate, Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Pre Maturity Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowPreMaturityReport")]
        public IActionResult ShowPreMaturityReport(string fromdate, string todate, string type, string pdatecheked)
        {
            List<PreMaturityDetailsDTO> lstPreMaturity = new List<PreMaturityDetailsDTO>();
            try
            {
                lstPreMaturity = objLAReports.ShowPreMaturityReport(fromdate,todate,type, pdatecheked, Con);
                return lstPreMaturity != null ? Ok(lstPreMaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Member Wise Receipts...

        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetMemberName")]
        public IActionResult GetMemberName()
        {
            List<LAReportsDTO> lstmaturity = new List<LAReportsDTO>();
            try
            {
                lstmaturity = objLAReports.GetMemberName(Con);
                return lstmaturity != null ? Ok(lstmaturity) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        [HttpGet]
        [Route("api/Banking/Report/LAReports/ShowMemberwiseReceiptsReport")]
        public IActionResult ShowMemberwiseReceiptsReport(long memberid, string fromdate, string todate, string pdatecheked)
        {
            List<MemberwiseReceiptsDTO> lstMemberwise = new List<MemberwiseReceiptsDTO>();
            try
            {
                lstMemberwise = objLAReports.ShowMemberwiseReceiptsReport(memberid, fromdate, todate, pdatecheked, Con);
                return lstMemberwise != null ? Ok(lstMemberwise) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion


        #region Interest  Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/interestreport")]
        public IActionResult interestreport(string forthemonth, long Schemeid, string paymenttype, string companyname, string branchname)
        {
            List<InterestreportDTO> lstInterestReport = new List<InterestreportDTO>();
            try
            {
                lstInterestReport = objLAReports.interestreport(forthemonth, Schemeid, paymenttype, companyname, branchname, Con);
                return lstInterestReport != null ? Ok(lstInterestReport) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Print Maturity Trend Details Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/PrintMaturityTrendDetailsReport")]
        public IActionResult PrintMaturityTrendDetailsReport(string maturitydate)
        {
            List<PrintMaturityTrendDetailsDTO> lsttrend = new List<PrintMaturityTrendDetailsDTO>();
            try
            {
                lsttrend = objLAReports.PrintMaturityTrendDetailsReport(maturitydate, Con);
                return lsttrend != null ? Ok(lsttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region Print Interest Trend Details Report...
        [HttpGet]
        [Route("api/Banking/Report/LAReports/PrintInterestTrendDetailsReport")]
        public IActionResult PrintInterestTrendDetailsReport(string maturitydate)
        {
            List<PrintInterestPaymentTrendDetailsDTO> lstinteresttrend = new List<PrintInterestPaymentTrendDetailsDTO>();
            try
            {
                lstinteresttrend = objLAReports.PrintInterestTrendDetailsReport(maturitydate, Con);
                return lstinteresttrend != null ? Ok(lstinteresttrend) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion

        #region ApplicationForm
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetApplicationFdnames")]
        public IActionResult GetApplicationFdnames()
        {
            List<ApplicationFormDTO> lstApplicationFdnames = new List<ApplicationFormDTO>();
            try
            {
                lstApplicationFdnames = objLAReports.GetApplicationFdnames(Con);
                return lstApplicationFdnames != null ? Ok(lstApplicationFdnames) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        [HttpGet]
        [Route("api/Banking/Report/LAReports/GetApplicationFormDetails")]
        public IActionResult GetApplicationFormDetails(string FdAccountNo)
        {
            ApplicationFormDetailsDTO _ApplicationFormDetails =new ApplicationFormDetailsDTO();
            try
            {
                _ApplicationFormDetails = objLAReports.GetApplicationFormDetails(FdAccountNo,Con);
                return _ApplicationFormDetails != null ? Ok(_ApplicationFormDetails) : (IActionResult)StatusCode(StatusCodes.Status204NoContent);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
        #endregion















    }
}