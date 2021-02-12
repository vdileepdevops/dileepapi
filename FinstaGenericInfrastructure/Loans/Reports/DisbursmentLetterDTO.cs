using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{
    public class DisbursmentLetterDTO : CommonDTO
    {
        public string pVchapplicationID { get; set; }
        public string pApplicantname { get; set; }
        public string pApplicantState { get; set; }
        public string pApplicantdistrict { get; set; }
        public string pApplicantcity { get; set; }
        public string pApplicantpincode { get; set; }
        public string pApprovedDate { get; set; }
        public string pLoantypeandCode { get; set; }
        public string pLoanname { get; set; }
        public string pApplicationdate { get; set; }        
        public decimal pApprovedloanamount { get; set; }
        public decimal pDisbursalamount { get; set; }
        public string pModeofpayment { get; set; }
        public decimal pTenureofloan { get; set; }
        public string pLoanpayin { get; set; }
        public string pInteresttype { get; set; }        
        public decimal pInstallmentamount { get; set; }        
        public string pLoantype { get; set; }
        public string pApplicantMobileNo { get; set; }        
        public long pApplicationId { get; set; }
        public decimal pInterestRate { get; set; }
        public string pApplicantEmail { get; set; }
        public decimal pDownpayment { get; set; }
        public string pVoucherno { get; set; }
        public long pApplicantId { get; set; }
        public string pDisbursedDate { get; set; }
        public string pDisbursedby { get; set; }
        public string pApplicantaddress1 { get; set; }
        public string pTitlename { get; set; }
    }   
    public class DisbursalLetterCount
    {
        public string pStatus { get; set; }
        public long pCount { get; set; }
    }
}
