using FinstaInfrastructure.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface ISavingAccountTransaction
    {
        Task<List<SavingAccountConfigBind>> GetSavingAccountDetails(long Membertypeid, string Applicanttype,string ConnectionString);
        Task<List<SavingAccountConfigDetailsBind>> GetSavingAccountConfigDetails(long Savingconfigid, long Membertypeid, string Applicanttype, string ConnectionString);
        Task<ContactDetails> GetContactDetails(long MemberID, string ConnectionString);
        Task<List<MemberDetails>> GetMemberDetails(long MemberTypeId, string contactType, string ConnectionString);
        string SaveSavingAccountTransaction(SavingAccountTransactionSave SavingAccountTransactionsave, string ConnectionString, out long pSavingaccountid);
        bool SaveJointmemberAndNominee(JointmemberAndNomineeSave _JointmemberAndNomineeSave, string ConnectionString);
        bool SaveReferral(ReferralSave _ReferralSave, string ConnectionString);
        int CheckMemberDuplicates(long Memberid, long savingaccountid, string connectionstring);
        Task<List<SavingAccountTransactionMainGrid>> GetSavingAccountTransactionMainGrid(string ConnectionString);
        Task<SavingsTransactionDataEdit> GetSavingAccountTransactionEditDetails(string ConnectionString, Int64 SavingAccountId, string accounttype, string savingsaccountNo);
        bool DeleteSavingTransaction(Int64 savingAccountid, long modifiedby, string connectionstring);

    }
}
