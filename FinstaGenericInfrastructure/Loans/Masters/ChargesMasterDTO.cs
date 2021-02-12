using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Masters
{
    public class ChargesMasterDTO : CommonDTO
    {
        public Int64 pLoantypeid { get; set; }
        public Int64 pLoanid { get; set; }
        public Int64 pChargeid { get; set; }//Charge
        public string pChargename { get; set; }//Charge
        public List<LoanchargetypesDTO> pLoanchargetypes { get; set; }
        public List<LoanchargetypesConfigDTO> pLoanchargetypesConfig { get; set; }
       
    }

    public class LoanchargetypesDTO : CommonDTO
    {
        public Int64 pLoantypeid { get; set; } // Loan Type
        public Int64 pLoanid { get; set; }//Loan Name
        public Int64 pLoanChargeid { get; set; }//Charge
        public Int64 pChargeid { get; set; }//Charge
        public string pChargename { get; set; }//Charge
        public string pLedgername { get; set; }//Charge
        public string pParentgroupledgername { get; set; }//Charge
        public string pParentgroupledgerid { get; set; }//Charge
        public string pLoantype { get; set; }//Charge
        public string pLoanNmae { get; set; }//Charge
    }

    public class LoanchargetypesConfigDTO : CommonDTO
    {
        public int pChargeid;
        public string peditstatus;

        public Int64 precordid { get; set; }
        public string ptypeofoperation { get; set; }
        public Int64 pLoantypeid { get; set; } // Loan Type
        public string pLoantype { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoanNmae { get; set; }
        public Int64 pLoanChargeid { get; set; }//Charge
        public string pChargename { get; set; }//Charge
        public string pLoanpayin { get; set; }
        public string pIsChargedependentOnLoanRange { get; set; }//Charge is dependent on loan range
        public string pChargevaluefixedorpercentage { get; set; }//Charge is dependent on loan range
        public string pApplicanttype { get; set; }//Applicant type
        public string pIschargerangeapplicableonvalueortenure { get; set; }//On Value OR On Tenure 
        public Int64 pMinLoanAmountorTenure { get; set; }
        public Int64 pMaxLoanAmountorTenure { get; set; }
        public decimal? pChargePercentage { get; set; }
        public decimal? pMinchargesvalue { get; set; }
        public decimal? pMaxchargesvalue { get; set; }
        public string pChargecalonfield { get; set; }
        public string pGsttype { get; set; }
        public Int64 pGstvalue { get; set; }      
        public string pChargeEffectFrom { get; set; }
        public string pChargeEffectTo { get; set; }
        public string pIschargewaivedoff { get; set; }
        public Int64 pLockingperiod { get; set; }

    }

    public class PreclouserchargesDTO: CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public Int64 pLoantypeid { get; set; }
        public string pLoantype { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoanname { get; set; }
        public Int64 pLockingperiod { get; set; }
        public string pLockingperiodtype { get; set; }
        public string pIschargeapplicable { get; set; }
        public string pChargecaltype { get; set; }
        public string pChargecalonfield { get; set; }
        public decimal? pChargesvalue { get; set; }
        public string pIstaxapplicable { get; set; }
        public string pTaxtype { get; set; }
        public decimal? pTaxpercentage { get; set; }
    }

}
