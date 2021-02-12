using FinstaInfrastructure.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface IInsuranceMember
    {
        Task<List<InsuranceMemberBind>> GetallInsuranceMembers(long MembertypeID, string ConnectionString);
        Task<Viewmemberdetails> GetMemberDetailsforview(long MemberID,string ConnectionString);
        Task<InsuranceschemeDetails> GetInsuranceSchemeDetails(long InsuranceSchemeId, string ConnectionString);
        Task<List<InsuranceMemberNomineeDetails>> GetInsuranceMemberNomineeDetails( string MemberreferenceId, string ConnectionString);
        bool SaveInsuranceMemberData(InsuranceMembersave _InsuranceMembersave, string Con);
        Task< List<InsuranceMembersDataforMainGrid>> GetInsuranceMembersMainGrid(string ConnectionString);
        Task<GetInsuranceMemberDataforEdit> GetMemberDetailsforEdit(long Recordid, string ConnectionString);
        bool DeleteInsuranceMember(string MemberReferenceID, long Userid, string ConnectionString);
        Task< List<InsuranceSchemes>> GetInsuranceSchemes(string Membertype, string Applicanttype, string ConnectionString);
        Task<List<Applicanttypesdata>> GetApplicants(string Connectionstring);

        int checkMemberCountinMaster(string ContactReferenceId,long InsuranceID, string Connectionstring,string InsuranceType, long Recordid);
    }
}
