using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{
  public class CollectionReportDTO
    {
        public string pApplicantname { get; set; }
        public string pLoantype { get; set; }
        public string pVchapplicationid { get; set; }
        public decimal? pPrinciple { get; set; }
        public decimal? pInterest { get; set; }
        public decimal? pPenality { get; set; }
        public decimal? pCharges { get; set; }
        public decimal? pTotalamount { get; set; }
        public string pApplicanttype { get; set; }
    }
    public class CollectionReportDetailsDTO
    {
        public string pApplicantname { get; set; }
        public string pLoantype { get; set; }
        public string pVchapplicationid { get; set; }
        public string pReciptdate { get; set; }
        public string pReciptNo { get; set; }
        public string pModeofpayment { get; set; }
        public string pParticulars { get; set; }
        public decimal? pReceiptamount { get; set; }
        public string pNarration { get; set; }
    }
}
