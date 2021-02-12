using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{
    public class DuesSummaryReportDTO
    {        
        public string pLoantype { get; set; }
        public string pLoanname { get; set; }
        public string pApplicantname { get; set; }
        public string pLoanaccountno { get; set; }
        public decimal pDisbursedamount { get; set; }        
        public decimal pPresentDueamount { get; set; }
        public decimal pPreviousDueamount { get; set; }
        public long  pTotalDues { get; set; }
        public decimal  pDuesPenalty { get; set; }
        public decimal  pTotaldueamountwithpenalty { get; set; }
        public decimal pOutstandingPrinciple { get; set; }
        public long  pFutureinstallments { get; set; }
        public string pLoanstatus { get; set; }        
        public string psalesExecutive { get; set; }        
        public long pNoofInstallmentsPaid { get; set; }
        public decimal pLastPaidamount { get; set; }
        public decimal pPenaltypaid { get; set; }
        public string pLastpaidDate { get; set; }
        public string pTenure { get; set; }
        public string pLoanPayin { get; set; }
        public string pDisbursedDate { get; set; }
        public string pLoanstartdate { get; set; }
        public string pLoanenddate { get; set; }

        public List<DuesdetailedReportDTO> lstDuesdetailedReportDTO { get; set; }
        public decimal pcollection { get; set; }
        public decimal pcurrentdues { get; set; }
    }

    public class DuesdetailedReportDTO : CommonDTO
    {
        public string pInstalmentdate { get; set; }
        public string pInstalmentno { get; set; }
        public string pReceivableprinciple { get; set; }
        public string pReceivableinterest { get; set; }
        public string pReceivableamount { get; set; }
        public string pReceivablepenalty { get; set; }
        public string pParticulars { get; set; }
    }    
}
