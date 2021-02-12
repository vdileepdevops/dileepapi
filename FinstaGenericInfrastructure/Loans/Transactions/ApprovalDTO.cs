using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Transactions
{
    public class ApprovalDTO : CommonDTO
    {
        public Int64 pLoantypeid { get; set; }
        public string pLoantype { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoanname { get; set; }
        public string pApplicantname { get; set; }
        public string papprovestatus { get; set; }
        public Int64 pApplicationid { get; set; }
        public string pVchapplicationid { get; set; }
        public DateTime? pApproveddate { get; set; }
        public Int64 pApprovedby { get; set; }
        public DateTime? pDisbursmentdate { get; set; }
        public decimal? pAmountrequested { get; set; }
        public decimal? pApprovedloanamount { get; set; }
        public string pLoanpayin { get; set; }
        public string pInteresttype { get; set; }
        public decimal? pRateofinterest { get; set; }
        public decimal? pTenureofloan { get; set; }
        public string pTenuretype { get; set; }
        public decimal? pVchaccintrest { get; set; }
        public string pLoaninstalmentpaymentmode { get; set; }
        public DateTime? pOldInstallmentstartdate { get; set; }
        public decimal? pInstallmentamount { get; set; }
        public DateTime? pInstallmentstartdate { get; set; }
        public string pHolidayperiodpayin { get; set; }
        public decimal? pHolidayperiodvalue { get; set; }
        public string pMoratoriumperiodpayin { get; set; }
        public decimal? pMoratoriumperiodvalue { get; set; }
        public string pRemarks { get; set; }
        public string pDisbursementpayinmode { get; set; }
        public decimal? pGraceperiod { get; set; }
        public Int16 pInterevels { get; set; }
        public List<approvalpaymentstagesDTO> lstStagewisepayments { get; set; }
        public List<approvedloanchargesDTO> lstApprovedloancharges { get; set; }

    }
    public class approvalpaymentstagesDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public string pStageno { get; set; }
        public string pStagename { get; set; }
        public string pPaymentreleasetype { get; set; }
        public decimal? pPaymentreleasepercentage { get; set; }
        public decimal? pPaymentreleaseamount { get; set; }
    }
    public class approvedloanchargesDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public string pChargestype { get; set; }
        public string pChargename { get; set; }
        public decimal? pChargereceivableamount { get; set; }
        public decimal? pChargewaiveoffamount { get; set; }
        public string pChargepaymentmode { get; set; }
        public string pGsttype { get; set; }
        public string pGstcaltype { get; set; }
        public decimal? pGstpercentage { get; set; }
        public decimal? pCgstpercentage { get; set; }
        public decimal? pSgstpercentage { get; set; }
        public decimal? pCgstamount { get; set; }
        public decimal? pSgstamount { get; set; }
        public decimal? pTotalgstamount { get; set; }
        public decimal? pTotalchargeamount { get; set; }
        public decimal? pUtsgtpercentage { get; set; }
        public decimal? pIgstpercentage { get; set; }
        public decimal? pUtgstamount { get; set; }
        public decimal? pIgstamount { get; set; }
        public decimal? pActualchargeamount { get; set; }
    }
    public class ViewapplicationsDTO
    {
        public decimal? pGraceperiod { get; set; }

        public string pContacttype { get; set; }
        public string pContactreferenceid { get; set; }
        public Int64 pLoantypeid { get; set; }
        public string pLoantype { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoanname { get; set; }
        public string pApplicanttype { get; set; }
        public Int64 pApplicationid { get; set; }
        public string pVchapplicationid { get; set; }
        public string pDateofapplication { get; set; }
        public string pApplicantname { get; set; }
        public decimal? pAmountrequested { get; set; }
        public string pAproveddate { get; set; }
        public string pApprovedby { get; set; }
        public decimal? pApprovedloanamount { get; set; }
        public string pLoanpayin { get; set; }
        public string pInteresttype { get; set; }
        public string pLoaninstalmentpaymentmode { get; set; }
        public decimal? pTenureofloan { get; set; }
        public decimal? pRateofinterest { get; set; }
        public string pPurposeofloan { get; set; }
        public string pMobileno { get; set; }
        public decimal? pInstalmentamount { get; set; }
        public Int16 pInterevels { get; set; }
        public decimal pschemeid { get; set; }
    }
    public class LoanwisechargeDTO
    {
        public string pChargename { get; set; }
        public decimal? pChargeamount { get; set; }
        public string pGsttype { get; set; }
        public string pGstcaltype { get; set; }
        public decimal? pGstpercentage { get; set; }
        public decimal? pCgstpercentage { get; set; }
        public decimal? pSgstpercentage { get; set; }
        public decimal? pCgstamount { get; set; }
        public decimal? pSgstamount { get; set; }
        public decimal? pTotalgstamount { get; set; }
        public decimal? pTotalchargeamount { get; set; }
        public decimal? pUtsgtpercentage { get; set; }
        public decimal? pIgstpercentage { get; set; }
        public decimal? pUtgstamount { get; set; }
        public decimal? pIgstamount { get; set; }
        public decimal? pActualchargeamount { get; set; }

    }
    public class CashflowDTO
    {
        public string pLoanname { get; set; }
        public decimal? pInstalmentamount { get; set; }
        public string pName { get; set; }
        public decimal? pSavingsamount { get; set; }
    }
}
