using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{   
    public class DeliveryOrderLetterDTO : CommonDTO
    {
        public string pVchapplicationID { get; set; }
        public string pApplicantname { get; set; }
        public string pApprovedDate { get; set; }
        public string pLoantypeandCode { get; set; }
        public string pLoanname { get; set; }
        public string pApplicationdate { get; set; }
        public decimal pApprovedloanamount { get; set; }
        public decimal pTenureofloan { get; set; }
        public string pLoanpayin { get; set; }
        public string pInteresttype { get; set; }       
        public decimal pInstallmentamount { get; set; }        
        public string pLoantype { get; set; }
        public string pApplicantMobileNo { get; set; }
        public string pApplicantEmail { get; set; }
        public long pApplicationId { get; set; }
        public decimal pInterestRate { get; set; }
        public decimal pDownpayment { get; set; }

        public decimal pOnroadprice { get; set; }
        public decimal pRequestedamount { get; set; }

        public string pVehiclemodel { get; set; }
        public string pengineno { get; set; }
        public string pchasisno { get; set; }
        public string pregistrationno { get; set; }
        public string pyearofmake { get; set; }
        public string pfathername { get; set; }
        public string paddress1 { get; set; }
        public string paddress2 { get; set; }
        public string pstate { get; set; }
        public string pdistrict { get; set; }
        public string ppincode { get; set; }

        public string pTitlename { get; set; }
        public string pContacttype { get; set; }
    }

    public class DeliveryOrdersCount
    {
        public string pStatus { get; set; }
        public long pCount { get; set; }
    }
}
