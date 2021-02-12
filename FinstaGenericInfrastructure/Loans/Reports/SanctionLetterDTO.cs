using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{
    public class SanctionLetter : CommonDTO
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
        public List<SanctionletterCharges> pChargesList { get; set; }
        public string pLoantype { get; set; }
        public string pApplicantMobileNo { get; set; }
        public string pApplicantEmail { get; set; }
        public long pApplicationId { get; set; }
        public decimal pInterestRate { get; set; }
        public decimal pDownpayment { get; set; }

        public List<SanctionLetterCoapplicants> pCoapplicantslist { get; set; }
        public List<SanctionLetterguarantors> pGuarantorslist { get; set; }        
        public List<SanctionLetterPromoters> pPromoterslist { get; set; }
        public List<SanctionLetterPartners> pPartnerslist { get; set; }
        public List<SanctionLetterGuardianorParent> pGuardianOrParentlist { get; set; }
        public List<SanctionLetterJointowners> pJointOwnersList { get; set; }

        public string pTitlename { get; set; }
    }

    public class SanctionletterCharges
    {
        public string pChargename { get; set; }
        public decimal pChargeAmount { get; set; }
        public string pChargetype { get; set; }
    }   

    public class SanctionLetterCounts
    {
        public string  pStatus { get; set; }
        public long pCount { get; set; }
    }
     
    public class SanctionLetterCoapplicants
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }
    public class SanctionLetterguarantors 
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }
    public class SanctionLetterPromoters
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }
    public class SanctionLetterPartners
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }
    public class SanctionLetterGuardianorParent
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }
    public class SanctionLetterJointowners
    {
        public string pTitle { get; set; }
        public string pName { get; set; }
        public string pFathername { get; set; }
    }

}
