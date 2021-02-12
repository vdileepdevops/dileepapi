using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;


namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface IRdConfigaration
    {
        #region Get RDName Count
        int GetRdNameCount(string RdName, string ConnectionString);
        #endregion

        #region Save's Recurring Deposit Configaration
        bool SaverdNameAndCode(RdNameAndCodeDTO Rdnameandcode, string Connectionstring);       
        bool Saverdconfigarationdetails(RdConfigartionDetails Rdconfiglist, string Connectionstring);
        bool SaveRdloanfacilityDetails(RdloanfacilityDetailsDTO RdloanDetails, string Connectionstring);
        bool SaveRdReferralDetails(RdReferralCommissionDTO RdReferralCommission, string Connectionstring);
        #endregion

        #region Get Rd View Details
        List<RdViewDTO> GetRdViewDetails(string ConnectionString);
        #endregion

        #region Get Rd Name And Code Details
        RdNameAndCodeDTO GetRdNameAndCodeDetails(string RdName, string RdNameCode, string ConnectionString);
        #endregion

        #region Get Rd Configuration Details
        RdConfigartionDetails GetRdConfigurationDetails(string RdName, string RdNameCode,string ConnectionString);
        #endregion

        #region Get Rd Loan Facility Details
        RdloanfacilityDetailsDTO GetRdloanfacilityDetails(string RdName, string RdNameCode, string ConnectionString);
        #endregion

        #region Get Rd Referral Details
        RdReferralCommissionDTO GetRdReferralDetails(string RdName, string RdNameCode, string ConnectionString);
        #endregion

        #region Save Identification Documents RD
        bool SaveIdentificationDocumentsRD(IdentificationDocumentsDt IdentificationDocumentsDto, string Connectionstring);
        #endregion

        #region Get Identification Documents RD
        IdentificationDocumentsDt GetIdentificationDocumentsRD(string RdName, string RdNameCode, string Connectionstring);
        #endregion

        #region Delete Rd Configuration
        bool DeleteRdConfiguration(long RdConfigId, string Connectionstring);
        #endregion

        RdschemeandcodeCount GetRDNameCount(Int64 RDconfigid, string RDName, string RdCode, string ConnectionString);
    }
}
