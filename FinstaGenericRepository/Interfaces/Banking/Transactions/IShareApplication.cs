using FinstaInfrastructure.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Masters;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IShareApplication
    {
        Task<List<shareMembersDetails>> GetshareMembers(string Membertype, string Receipttype, string ConnectionString);
        Task<List<ShareviewDTO>> GetSharNames(string Membertype, string Applicanttype, string ConnectionString);
        Task<List<shareconfigDetails>> GetSharconfigdetails(long shareconfigid, string Applicanttype, string Membertype, string ConnectionString);
        string SaveshareApplication(ShareApplicationDTO ShareApplicationDTO, string ConnectionString, out long pShareaccountid);
        bool SaveshareJointMembersandNomineeData(savejointandnomiee JointandNomineeSaveDTO, string ConnectionString);
        bool SaveReferralData(Referrals referralDetails, string con);
        Task<List<ShareApplicationDTO>> BindShareApplicationView(string ConnectionString);
        bool DeleteShareDetails(long ShareApplicationId, string Connectionstring);
        //Task<List<ShareApplicationDTO>> BindShareDetailsBasedonApplicationid(long ShareApplicationid,string ConnectionString);



    }
}
