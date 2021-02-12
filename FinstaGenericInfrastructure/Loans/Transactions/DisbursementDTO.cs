using FinstaInfrastructure.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Transactions
{
    public class DisbursementDTO : ModeofPaymentDTO
    {
        public string papproveddate { get; set; }

        public string pmobileno { get; set; }
        public long pdisbursementid { get; set; }
        public string pcontactreftype { get; set; }
        public decimal ploanpayoutamount { get; set; }
        public string ploantype { get; set; }
        public long psubledgerid { get; set; }
        public decimal pcancelchequesamount { get; set; }

        public string pvoucherrefno { get; set; }
        public string ppaymentid { get; set; }
        public string ppaymentdate { get; set; }
        public decimal ptotalpaidamount { get; set; }
        public string pnarration { get; set; }
        public string pmodofPayment { get; set; }

        public string papplicantname { get; set; }
        public long papplicantid { get; set; }
        public string pcontactreferenceid { get; set; }
        public string ploanname { get; set; }
        public long papplicationid { get; set; }

        public string pvchapplicationid { get; set; }
        public string pdateofapplication { get; set; }
        public string ppurposeofloan { get; set; }
        public decimal? papprovedloanamount { get; set; }
        public decimal? ptenureofloan { get; set; }
        public string pinteresttype { get; set; }
        public decimal? prateofinterest { get; set; }

        public string ploanpayin { get; set; }
        public string ppayinduration { get; set; }
        public string ppayinnature { get; set; }
        public decimal? ploaninstallment { get; set; }
        public string pinstallmentstartdate { get; set; }
        public string papprovedstatus { get; set; }
        public string pdisbursedtype { get; set; }
        public string pdisbursedby { get; set; }
        public decimal pchargesadjustedamount { get; set; }
        public decimal? potherloansadjustedamount { get; set; }
        public decimal ploandisbusalamount { get; set; }
        public long pdebtorsaccountid { get; set; }
        public List<InstallmentdateDTO> pInstallmentdatelist { get; set; }
        public List<PaymentsDTO> ppaymentslist { get; set; }
        public List<ChargesDTO> padjustchargeslist { get; set; }
        public List<ChargesDTO> pchargereceiptslist { get; set; }

        public List<ExistingLoansDTO> pexistingloanslist { get; set; }
        public List<StageWisePaymentsDTO> pstagewisepaymentslist { get; set; }
        public List<PartPaymentsDTO> pPartPaymentslist { get; set; }
        public string ppreviousdisbursedtdate { get; set; }
        public string pexistingdisbursestatus { get; set; }
        public string ploaninstalmentpaymentmode { get; set; }
    }
    public class DisbursementViewDTO
    {
        public List<DisbursementViewDetailsDTO> papprovedloanslist { get; set; }
        public List<DisbursementViewDetailsDTO> ptotaldisbusedlist { get; set; }
        public List<DisbursementViewDetailsDTO> ppartdisbursedlist { get; set; }
        public List<DisbursementViewDetailsDTO> pstagedisbursedlist { get; set; }
    }
    public class DisbursementViewDetailsDTO
    {
        public string pvchapplicationid { get; set; }
        public string ploantype { get; set; }
        public string ploanname { get; set; }
        public string papplicantname { get; set; }
        public long pcontactno { get; set; }
        public decimal ploanapprovedamount { get; set; }
        public decimal pdisbursementamount { get; set; }
        public decimal pdisbursementbalance { get; set; }
        public int pendingstatgesno { get; set; }
    }
    public class ChargesDTO
    {
        public string pchequestatus;

        public string pgsttype { get; set; }
        public string pgstcalculationtype { get; set; }
        public decimal pgstpercentage { get; set; }

        public long pchargeid { get; set; }
        public string pchargetype { get; set; }
        public string pchargename { get; set; }
        public decimal? pchargeamount { get; set; }
        public decimal? pchargeactualamount { get; set; }
        public decimal pchargegstamount { get; set; }
        public decimal pchargecgstamount { get; set; }
        public decimal pchargesgstamount { get; set; }
        public decimal pchargeutgstamount { get; set; }
        public decimal pchargeigstamount { get; set; }

        public decimal pchargecgstpercentage { get; set; }
        public decimal pchargesgstpercentage { get; set; }
        public decimal pchargeutgstpercentage { get; set; }
        public decimal pchargeigstpercentage { get; set; }


        public decimal? pchargewaiveoffamount { get; set; }
        public decimal? pchargereceiptwaiveoffamount { get; set; }
        public decimal pchargebalance { get; set; }
        public string pchargeadjuststatus { get; set; }
        public string pchargecollectstatus { get; set; }
        public string pchargereceiptno { get; set; }
        public string pchargereceiptdate { get; set; }
    }

    public class InstallmentdateDTO
    {
        public string pinstallmentdatetype { get; set; }
        public decimal? pdisbursefromno { get; set; }
        public decimal? pdisbursetono { get; set; }
        public decimal? pinstallmentdatedueno { get; set; }
    }
    public class ExistingLoansDTO
    {
        public string pvchapplicationid { get; set; }
        public decimal poutstandingamount { get; set; }
        public decimal padjustedamount { get; set; }
    }
    public class PartPaymentsDTO
    {
        public string pvchapplicationid { get; set; }
        public decimal pdisburedamount { get; set; }
        public string pdisburseddate { get; set; }
        public string pdisbursedby { get; set; }
        public string pvoucheno { get; set; }
        public string pmodeofpayment { get; set; }
    }
    public class StageWisePaymentsDTO
    {
        public bool pstagestatus;
        public long pstageno { get; set; }
        public string pstagename { get; set; }
        public string ppaymentreleasetype { get; set; }
        public decimal ppercentage { get; set; }
        public decimal pstageamount { get; set; }
        public string paymentreleasetype { get; set; }
        public bool pstagepaidstatus { get; set; }
        public string pstagepreviouspaidstatus { get; set; }
        public string pstagepaiddate { get; set; }
        public string pstagepaidvouchernumber { get; set; }
    }

    public class InstalmentsgenerationDTO
    {
        public Int32 pInstalmentno { get; set; }
        public DateTime? pInstalmentdate { get; set; }
        public decimal? pInstalmentprinciple { get; set; }
        public decimal? pInstalmentinterest { get; set; }
        public decimal? pInstalmentamount { get; set; }
    }

    public class DisbursementReportDTO
    {

        public string pfieldname { get; set; }
        public string ptype { get; set; }
        public string pfromdate { get; set; }
        public List<DisbursementReportDetailsDTO> pdisbursedlist { get; set; }
        public List<DisbursementReportDetailsDTO> ploantypeslist { get; set; }
        public List<DisbursementReportDetailsDTO> papplicanttypeslist { get; set; }
        public List<DisbursementReportDuesDetailsDTO> pdisbursementdueslist { get; set; }
        public string ptodate { get; set; }
    }
    public class DisbursementReportDetailsDTO
    {
        public int precordid;

        public string ptype { get; set; }
        public decimal? ploancount { get; set; }
        public decimal? ploanvalue { get; set; }
        public decimal? ploanaveragevalue { get; set; }
    }
    public class DisbursementReportDuesDetailsDTO
    {
        public string pvchapplicationid { get; set; }
        public string papplicantname { get; set; }
        public decimal pdisbursedamount { get; set; }
        public decimal pinstallmentintrest { get; set; }
        public decimal ploanamount { get; set; }
        public int ptenureofloan { get; set; }
        public decimal ppaidamount { get; set; }
        public decimal pdueamount { get; set; }
        public decimal ppenaltyamount { get; set; }
        public decimal ppenaltypaidamount { get; set; }
        public decimal ppenaltydueamount { get; set; }
        public string pstatus { get; set; }
        public string psalesexecutive { get; set; }
        public string pcollectionexecutive { get; set; }
        public string ploantype { get; set; }
        public string papplicanttype { get; set; }
    }
    public class EmiChartReportDTO
    {
        public string pLoanname { set; get; }
        public decimal pLoanamount { set; get; }
        public Int32 pTenureofloan { set; get; }
        public decimal pRateofinterest { set; get; }
        public decimal pinstallmentamount { set; get; }
        public string pPayinmode { set; get; }
        public string pEmistartdate { set; get; }
        public string pApplicationid { set; get; }
        public string pApplicantname { set; get; }
        public string PLoantype { set; get; }
        public string pInstalmentpaymentmode { set; get; }
        public Int64 ppartprinciplepaidinterval { set; get; }
        public string pInteresttype { set; get; }
        public List<EmiInstalmentDetailsDTO> pEmichartlist { set; get; }
    }
    public class EmiInstalmentDetailsDTO
    {
        public Int32 pInstalmentno { get; set; }
        public string pInstalmentdate { get; set; }
        public decimal? pInstalmentprinciple { get; set; }
        public decimal? pInstalmentinterest { get; set; }
        public decimal? pInstalmentamount { get; set; }
    }

    public class EmiChartViewDTO
    {
        public string pVchapplicationID { get; set; }
        public string pApplicantname { get; set; }
        public decimal pApprovedloanamount { get; set; }
        public decimal pDisbursalamount { get; set; }
        public string pVoucherno { get; set; }
        public string pApplicantEmail { get; set; }
        public string pApplicantMobileNo { set; get; }
    }

    public class DisbursementTrendDTO
    {
        public List<DisbursementTrendDeatilsDTO> pmonthlist { get; set; }
        public List<DisbursementTrendDeatilsDTO> pdisbursementtrendlist { get; set; }
        public List<DisbursementTrendDeatilsDTO> pdisbursementtrenddailylist { get; set; }
    }

    public class DisbursementTrendDeatilsDTO
    {
       
        public int precordid { get; set; }
        public int psortorder { get; set; }
        public string ptype { get; set; }
        public string pparticulars { get; set; }
        public int pcount1 { get; set; }
        public decimal pamount1 { get; set; }

        public int pcount2 { get; set; }
        public decimal pamount2 { get; set; }

        public int pcount3 { get; set; }
        public decimal pamount3 { get; set; }

        public int pcount4 { get; set; }
        public decimal pamount4 { get; set; }

        public int pcount5 { get; set; }
        public decimal pamount5 { get; set; }

        public int pcount6 { get; set; }
        public decimal pamount6 { get; set; }

        public int pcount7 { get; set; }
        public decimal pamount7 { get; set; }

        public int pcount8 { get; set; }
        public decimal pamount8 { get; set; }

        public int pcount9 { get; set; }
        public decimal pamount9 { get; set; }

        public int pcount10 { get; set; }
        public decimal pamount10 { get; set; }

        public int pcount11 { get; set; }
        public decimal pamount11 { get; set; }

        public int pcount12 { get; set; }
        public decimal pamount12 { get; set; }
    }
}
