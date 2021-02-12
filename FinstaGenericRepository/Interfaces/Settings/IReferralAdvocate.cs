using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.Interfaces.Settings
{
    public interface IReferralAdvocate
    {
        List<ReferralAdvocateDTO> getContactDetails(string contactType, string ConnectionString);
        bool saveReferral(ReferralAdvocateDTO referralavocatelist, string con);
        List<ReferralAdvocateDTO> getReferralAgentDetails(string Type, string ConnectionString);
        List<ReferralAdvocateDTO> getReferralDetails(string ConnectionString);
        bool DeleteReferralAgent(DeleteDTO objDeleteDTO, string ConnectionString);
        ReferralAdvocateDTO ViewReferralAgentDetails(Int64 refid, string ConnectionString);
        bool UpdatedReferral(ReferralAdvocateDTO referralavocatelist, string ConnectionString);
        int CheckContactDuplicate(Int64 contactId,Int64 RefId, string ConnectionString);
        List<ReferralAdvocateDTO> GetDocumentProofs(Int64 docId,  string ConnectionString);
        List<TdsSectionDTO> getTdsSectionNo(string ConnectionString);
        bool SaveTdsSectionNo(TdsSectionDTO tdsSectionNo, string con);
        List<GstTyeDTO> getGstType(string con);
        bool SaveGstType(GstTyeDTO gstType, string con);
        ReferralAdvocateDTO GetContactDetailsbyId(Int64 contactId, string con);
        bool saveAdvocate(ReferralAdvocateDTO referralavocatelist, string con);
        List<ReferralAdvocateDTO> getAdvocateLawterDetails(string type, string con);
        ReferralAdvocateDTO ViewAdvocateLawerDetails(Int64 refid, string con);
        bool DeleteAdvocateLawer(DeleteDTO objDeleteDTO, string con);
        bool UpdatedAdvocateLawer(ReferralAdvocateDTO referralavocatelist, string con);
        int CheckTdsSectionDuplicate(string tdsSecName, string con);
        int CheckGstTypeDuplicate(string strGstType, string con);
        int CheckAdvocateDuplicate(Int64 contactId, Int64 refId, string con);
        bool saveParty(ReferralAdvocateDTO referralavocatelist, string con);
        List<ReferralAdvocateDTO> getPartyDetails(string type, string con);
        ReferralAdvocateDTO ViewPartyDetails(Int64 refid, string con);
        bool DeleteParty(DeleteDTO objDeleteDTO, string con);
        bool UpdatedParty(ReferralAdvocateDTO referralavocatelist, string con);
        int CheckPartyDuplicate(Int64 contactId, Int64 refId, string con);
    }
}
