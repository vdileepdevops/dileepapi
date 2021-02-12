using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
   public interface IFdConfigartion
    {
        #region Saverdconfigaration
        // bool SaveFDconfigaration(FdConfigartionDTO FdConfigartionDTO,string Connectionstring);
        Task<List<FDViewDTO>> GetFdViewDetails(string ConnectionString);
        Task<FdNameAndCodeDTO> GetFdNameAndCodeDetails(string FDName, string FdCode, string ConnectionString);
        bool SaveFdNameAndCode(FdNameAndCodeDTO FdNameAndCodeDTO, string Connectionstring);
        bool SaveFDConfigurationDetails(FdConfigDeatails FdConfigDeatails, string Connectionstring);
        Task<FdConfigDeatails> GetFDConfigurationDetails(string FdName, string FDCode, string Connectionstring);
        bool SaveLoanFacility(FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO, string Connectionstring);
        Task<FDloanfacilityDetailsDTO> GetLoanFacilityDetails(string FdName, string FDCode, string Connectionstring);
        bool SaveFdReferralDeatils(FDReferralCommissionDTO FDReferralCommissionDTO, string Connectionstring);
        Task<FDReferralCommissionDTO> GetFDReferralCommission(string FdName, string FDCode, string Connectionstring);
        bool SaveIdentificationDocumentsFD(IdentificationDocumentsDto IdentificationDocumentsDto, string Connectionstring);
        Task<IdentificationDocumentsDto> GetIdentificationDocumentsFD(string FdName, string FDCode, string Connectionstring);
        bool DeleteFdConfiguration(Int64 FdConfigId, string Connectionstring);
        FdschemeandcodeCount GetFDNameCount(Int64 FDConfigid, string FDName,string FdnameCode, string ConnectionString);
        #endregion
    }
}
