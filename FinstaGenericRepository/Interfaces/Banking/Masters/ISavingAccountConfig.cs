using FinstaInfrastructure.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace FinstaRepository.Interfaces.Banking.Masters
{
   public interface ISavingAccountConfig
    {
        int SaveSavingAccountNameAndCode(SavingAccountNameandCodeDTO SavingAccountNameandCodelist, string Connectionstring);
        bool SaveSavingAccountConfiguration(SavingAccountConfigDTO SavingAccountConfiglist, string con);
        bool SaveLoanFacility(LoanFacilityDTO objLoanFacility, string con);
        bool SaveIdentificationdocuments(IdentificationDocumentsDTO obIdentificationDocuments, string con);
        bool SaveReferralCommission(ReferralCommissionDTO objReferralCommission, string con);
        int checkInsertAccNameandCodeDuplicates(string checkparamtype, string Accname, string Acccode, Int64 SavingAccid, string connectionstring);
        Task<List<SavingAccountNameandCodeDTO>> GetSavingAccountConfigData(string ConnectionString);
        Task<List<SavingAccountConfigDTO>> GetSavingAccountConfigMasterDetails(string ConnectionString, Int64 SavingAccountId);
        bool DeleteSavingAccountConfig(Int64 savingAccountid, int modifiedby, string con);
        SavingschemeandcodeCount GetSavingAccNameCodeCount(Int64 SavingAccid, string SavingAccName, string SavingAccCode, string ConnectionString);
    }
}
