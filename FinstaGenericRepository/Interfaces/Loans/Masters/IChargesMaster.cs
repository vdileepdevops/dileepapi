using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaRepository.Interfaces.Loans.Masters
{
    public interface IChargesMaster
    {
        #region SaveChargesName 
        bool SaveChargesName(ChargesMasterDTO Charges, string ConnectionString);
        #endregion

        #region GetChargesName 
        List<ChargesMasterDTO> GetChargesName(string ConnectionString, string type);
        #endregion

        #region UpdateChargesName 
        bool UpdateChargesName(ChargesMasterDTO Charges, string ConnectionString);
        #endregion

        #region DeleteChargesname
        bool DeleteChargesName(ChargesMasterDTO Charges, string ConnectionString);
        #endregion
        //Loan Wise Charges Assgining
        #region SaveLoanWiseChargesName 
        bool SaveLoanWiseChargesName(ChargesMasterDTO LoanWiseCharges, string ConnectionString);
        #endregion

        #region GetLoanWiseChargesName 
        List<LoanchargetypesDTO> GetLoanWiseChargesName(string ConnectionString,string loanid);
        #endregion
        //Charges Configaration
        #region SaveLoanWiseChargesConfig
        bool SaveLoanWiseChargeConfig(ChargesMasterDTO Charges, string ConnectionString);
        #endregion

        #region GetLoanWiseApplicantTypes
        List<LoanchargetypesConfigDTO> GetLoanWiseApplicantTypes(string ConnectionString, Int64 loanid);
        #endregion
        #region GetLoanWiseChargeConfig
        List<LoanchargetypesConfigDTO> GetLoanWiseChargeConfig(string ConnectionString, Int64 loanid);
        #endregion

        #region ViewLoanWiseChargeConfig
        List<LoanchargetypesConfigDTO> ViewLoanWiseChargeConfig(string ConnectionString);
        #endregion

        #region UpdateLoanWiseChargeConfig
        bool UpdateLoanWiseChargeConfig(ChargesMasterDTO Charges, string ConnectionString);
        #endregion

        #region DeleteLoanWiseChargeConfig
        bool DeleteLoanWiseChargeConfig(LoanchargetypesConfigDTO Charges, string ConnectionString);
        #endregion
        //PreClouser Charges
        #region CheckDuplicateLoanid
        int CheckDuplicateLoanid(string ConnectionString,long Loanid);
        #endregion

        #region SavePreclouserCharges
        bool SavePreclouserCharges(PreclouserchargesDTO PreclouserCharges, string ConnectionString);
        #endregion

        #region UpdatePreclouserCharges
        bool UpdatePreclouserCharges(PreclouserchargesDTO PreclouserCharges, string ConnectionString);
        #endregion

        #region DeletePreclouserCharges
        bool DeletePreclouserCharges(string ConnectionString, long Loanid, long Recordid, long userid);
        #endregion

        #region GetePreclouserCharges
        List<PreclouserchargesDTO> GetePreclouserCharges(string ConnectionString,long Loanid,long Recordid);
        #endregion

        #region ViewPreclouserCharges
        List<PreclouserchargesDTO> ViewPreclouserCharges(string ConnectionString);
        #endregion

        #region GetLoanWiseLoanPayIn
        List<LoanchargetypesConfigDTO> GetLoanWiseLoanPayin(string ConnectionString, Int64 loanid,string applicanttype);

        #endregion

        #region CheckDuplicateChargeNames
        int CheckDuplicateChargeNames(string ChargeName, Int64 chargeid,string ConnectionString);
        #endregion

        #region CheckDuplicateChargeNamesBasedonLoanid
        int CheckDuplicateChargeNamesByLoanid(string ChargeName,Int64 loanid,string ConnectionString);
        #endregion
    }
}
