using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;

namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface IInsurenceConfig
    {
        #region Get RDName Count
        int GetInsurenceNameCount(string InsurenceName, string ConnectionString);

        int GetInsurenceCodeCount(string InsurenceCode, string ConnectionString);

        InsuranceschemeandcodeCount GetInsuranceNameCount(Int64 Insuranceid, string InsuranceName, string InsuranceCode, string ConnectionString);
        #endregion

        #region Save Insurence Configaration
        string SaveInsurenceNameAndCode(InsurenceNameAndCodeDTO InsurenceNameAndCode, string Connectionstring, out long insurenceconfigid);
        bool SaveInsurenceConfigDetails(InsurenceConfigDTO InsurenceConfigDetails, string ConnectionString);
        bool SaveInsurenceReferralDetails(insurenceReferralCommissionDTO InsurenceReferralDetails, string ConnectionString);
        #endregion

        #region Get Insurence View Details
        List<InsurenceConfigViewDTO> GetInsurenceViewDetails(string ConnectionString);
        #endregion

        #region Get Insurence Name And Code Details
        InsurenceNameAndCodeDTO GetInsurenceNameAndCodeDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString);
        #endregion

        #region Get Insurence Configuration Details
        InsurenceConfigDTO GetInsurenceConfigurationDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString);
        #endregion

        #region Get Insurence Referral Details
        insurenceReferralCommissionDTO GetInsurenceReferralDetails(string InsurenceName, string InsurenceNameCode, string ConnectionString);
        #endregion

        #region Delete Insurence Configuration 
        bool DeleteInsurenceConfiguration(long InsurenceConfigId, string Connectionstring);
        #endregion
    }
}
