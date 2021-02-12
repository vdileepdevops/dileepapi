using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Reports;
using static FinstaInfrastructure.Banking.Reports.LAReportsDTO;


namespace FinstaRepository.Interfaces.Banking.Reports
{
    public interface ILAReports
    {

        #region Cash Flow Report...
        List<LAReportsDTO> GetCashFlowSummary(string date, string months, string Connectionstring);
        List<CashFlowDetailsDTO> GetCashFlowDetails(string Asonmonth,string month,string Connectionstring);

        List<CashFlowPerticularDetailsDTO> GetCashFlowPerticularsDetails(string perticulars,string Asonmonth, string Connectionstring);

        #endregion

        #region Target Report...
        List<TargetReportDTO> GetTargetReportSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate, string Connectionstring);
        List<TargetReportDTO> GetTargetReportDetails(string branch,string Connectionstring);

        #endregion

        #region Agent Points...
        List<LAReportsDTO> GetAgentPointsSummary(string receiptfromdate, string receipttodate, string chequefromdate, string chequetodate,string Connectionstring);
        List<AgentPointsDetailsDTO> GetAgentPointsDetails(string agentname, string Connectionstring);

        #endregion


        #region Interest Payment Report...
        List<LAReportsDTO> GetInterestreportscheme(string Connectionstring);
        List<LAReportsDTO> GetInterestreportfdaccountnos(string paymenttype, string company, string brnach, long schemeid, string Connectionstring);

        #endregion

        #region Commission Payment Report...


        #endregion

        #region Maturity Intimation Report...
        List<LAReportsDTO> GetMaturitybrnach(long schemeid, string Connectionstring);
        List<LAReportsDTO> GetMaturityscheme(string Connectionstring);
        bool SaveMaturityIntimationReport(LAReportsDTO _MaturityIntimationDTO, string ConnectionString);
        List<MaturityIntimationDTO> ShowMaturityIntimationReport(long schemeid, string branchname, string fromdate, string todate, string Connectionstring);

        List<MaturityIntimationDTO> GetMaturityIntimationLetter(string FdAccountNo, string Connectionstring);
        #endregion

        #region Lien Release Report...
        List<LAReportsDTO> GetLienbrnach(string Connectionstring);
        List<LienReleaseDTO> ShowLienReleaseReport(string branchname, string fromdate, string todate, string Connectionstring);
        #endregion

        #region Self Adjustment Report...
        List<LAReportsDTO> GetSelfAdjustmentcompany(string Connectionstring);
        List<LAReportsDTO> GetSelfAdjustmentbrnach(string companyname, string Connectionstring);
        List<SelfAdjustmentDTO> ShowSelfAdjustmentReport(string paymenttype, string companyname, string branchname, string fromdate, string todate, string Connectionstring);
        #endregion

        #region Maturity Trend Report...
        List<MaturityTrendDTO> ShowMaturityTrendReport(string Connectionstring);
        List<MaturityTrendDTO> ShowMaturityTrendGridHeader(string Connectionstring);

        List<MaturityTrendDetailsDTO> ShowShemeAndDatewiseDetails(string schemenamee, string maturitydate,string Connectionstring);
        List<MaturityTrendDetailsDTO> ShowGrandTotalDatewiseDetails(string maturitydate,string Connectionstring);

        #endregion

        #region Interest Trend Report...
        List<InterestPaymentTrendDTO> ShowInterestPaymentReport(string Connectionstring);

        List<InterestPaymentTrendDetailsDTO> ShowInterestTrendShemeAndDatewiseDetails(string schemenamee, string maturitydate, string Connectionstring);
        List<InterestPaymentTrendDetailsDTO> ShowInterestTrendGrandTotalDatewiseDetails(string maturitydate, string Connectionstring);
        #endregion

        #region Pre Maturity Report...
        List<PreMaturityDetailsDTO> ShowPreMaturityReport(string fromdate, string todate, string type, string pdatecheked,string Connectionstring);

        #endregion

        #region Member wise Receipts Report...
        List<LAReportsDTO> GetMemberName(string Connectionstring);
        List<MemberwiseReceiptsDTO> ShowMemberwiseReceiptsReport(long memberid, string fromdate, string todate, string pdatecheked, string Connectionstring);

        #endregion

        #region Interest  Report...

        List<InterestreportDTO> interestreport(string Month, long Schemeid, string paymenttype, string companyname, string branchname, string Connectionstring);

        #endregion

        #region  Print Maturity Trend Details Report...
        List<PrintMaturityTrendDetailsDTO> PrintMaturityTrendDetailsReport(string maturitydate, string Connectionstring);
        #endregion

        #region Print Interest Trend Details Report...
        List<PrintInterestPaymentTrendDetailsDTO> PrintInterestTrendDetailsReport(string maturitydate, string Connectionstring);
        #endregion

        #region Application Form
        List<ApplicationFormDTO> GetApplicationFdnames(string Connectionstring);

        ApplicationFormDetailsDTO GetApplicationFormDetails(string FdAccountNo, string Connectionstring);
        #endregion















    }
}
