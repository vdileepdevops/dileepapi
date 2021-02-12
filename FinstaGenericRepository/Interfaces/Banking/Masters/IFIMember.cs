using FinstaInfrastructure.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface IFIMember
    {
        bool SaveFIMember(FIMemberDTO _FIMemberDTO, string ConnectionString);
       Task< FIMemberDTO > GetFIMemberData(string MemberReferenceID, string ConnectionString);
        Task <List<FiMemberContactDetails>> GetallFIMembersDetails(string ConnectionString);
        int checkMemberCountinMaster(string COntactReferenceID, string ConnectionString);
        bool DeleteFIMember(string MemberReferenceID, long Userid, string ConnectionString);

       Task<List<FIMembertypeDTO>> _GetFIMembersTypesListDetails(string ConnectionString);
        Task<List<FIApplicantTypeDTO>> GetFIMembersApplicantListDetails(string ContactType,string ConnectionString);

        string SaveFIMemberMasterData(FiMemberContactDetails _FiMemberContactDetails, string ConnectionString, out long pMemberId, out string Dob);
        bool SaveFIMemberReferenceData(FIMembersaveReferences _FIMembersaveReferences, string ConnectionString);
        bool SaveFIMemberReferralData(FiMemberReferralDTO _FiMemberReferralDTO, string ConnectionString);

        FiMemberReferralDTO GetFIMemberReferralInformation(string strapplictionid, string ConnectionString);
        List<FIMemberReferencesDTO> GetFIMemberReferenceInformation(string strapplictionid, string ConnectionString);
        bool GetIsReferencesapplicableOrnot(string strapplictionid, string ConnectionString);
        Task< List<FIMeberRefIdAndID>> GetFIMembersforasGuardians(string ConnectionString);
        List<ContactDetailsDTO> getContactDetails(string contactType, string ConnectionString);
    }
}
