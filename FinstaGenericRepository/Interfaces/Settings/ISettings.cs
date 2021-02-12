using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Settings
{
    public interface ISettings
    {

        List<SettingsDTO> getContacttitles(string ConnectionString);
        List<SettingsDTO> getCountries(string ConnectionString);

        List<SettingsDTO> getStates(string ConnectionString, int id);
      
        List<SettingsDTO> getDistricts(string ConnectionString, int id);
        List<SettingsDTO> getCompanyandbranchdetails(string ConnectionString);
        List<SettingsDTO> getApplicanttypes(string contacttype,long loanid, string con);

        List<SettingsDTO> getContacttypes(int loanid, string ConnectionString);
       // bool saveRelationShip(RelationShipDTO objRelation,string connectionString);
        List<documentstoreDTO> getDocumentstoreDetails(string con, Int64 pContactId,string strapplicationid);

        CompanyInfoDTO GetcompanyNameandaddressDetails(string Connectionstring);
        Task<bool> GetDatepickerEnableStatus(string con);

        List<GenerateidMasterDTO> GetFormNames(string con);
        List<GenerateidMasterDTO> GetModeofTransaction(string Formname,string con);
        int checkTransactionCodeExist(string TransactionCode,string connectionstring);
        bool SaveGenerateIdMaster(GenerateIdDTO _GenerateIdDTO, string ConnectionString);

        List<GenerateidMasterDTO> GetGenerateidmasterList(string con);

        List<ReferralAdvocateDTO> GetAllPartyDetails(string Type, string ConnectionString);

        bool GetDateLockStatus(string ConnectionString);

    }
}
