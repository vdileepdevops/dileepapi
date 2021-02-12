using FinstaInfrastructure.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IFDTransaction
    {
        Task<List<FdMembersandContactDetails>> GetFDMembers(string Contacttype, string MemberType, string ConnectionString);
        Task<List<FDMemberNomineeDetails>> GetFDMemberNomineeDetails(string MemberCode, string ConnectionString);
        Task<List<FdNameandCode>> GetFdSchemes(string ApplicantType, string MemberType, string ConnectionString);
        Task< FDdetailsFromScheme> GetFdSchemeDetails(string ApplicantType, string MemberType, long FdconfigID, string Fdname, long Tenure, string Tenuremode, decimal Depositamount,string ConnectionString);
        string SaveFDMemberandSchemeData(FdMemberandSchemeSave FdMemberandSchemeData, string ConnectionString, out long pFdAccountId);

        bool SaveFDJointMembersandNomineeData(FdJointandNomineeSave FdJointandNomineeSave, string ConnectionString);
        bool SaveFDReferralData(FdTransactionReferrals FdTransactionReferrals, string ConnectionString);

        bool DeleteFdTransactions(string FdaccountNo, long pCreatedby,string ConnectionString);
        Task< List<FdTransactionMainGridData>> GetFdTransactionData(string ConnectionString);
        Task<FdTransactionDataEdit> GetFdTransactionDetailsforEdit(string FdAccountNo,  long FdAccountId, string ConnectionString, string accounttype);

        Task<List<FdTransactinTenuresofTable>> GetFdTenuresofTable(string FDName,long FdconfigId, string  TenureMode, string MemberType, string ConnectionString);
        Task<List<FdTransactinDepositAmountofTable>> GetFdDepositamountsofTable(string FDName, long FdconfigId, string TenureMode,long Tenure, string MemberType, string ConnectionString);

        Task<List<ChitBranchDetails>> GetchitBranchDetails(string Connectionstring);
        Task<object> GetchitBranchstatus(string Connectionstring);
        List<Matuerityamount> GetMaturityamount(string pInterestMode, long pInterestTenure, decimal pDepositAmount, string pInterestPayOut, string pCompoundorSimpleInterestType, decimal pInterestRate, string pCalType, string Connectionstring);


        Task<List<FdTransactinInterestAmountofTable>> GetInterestamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, string MemberType, string ConnectionString);

        Task<List<FdTransactinMaturityAmountofTable>> GetMaturityamountsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, decimal Interestamount, string MemberType, string ConnectionString);

        Task<List<FdInterestPayout>> GetPayoutsofTable(string FDName, long FdconfigId, string TenureMode, long Tenure, decimal Depositamount, decimal Interestamount,decimal Maturityamount, string ConnectionString);

        int GetDepositamountCountofInterestRate(string Fdname, decimal Depositamount, string MemberType, string ConnectionString);
        FdInterestRateValidation GetTenureandMininterestRateofInterestRate(string Fdname, decimal Depositamount, long Tenure, string TenureMode,string InterestPayout, string MemberType, string ConnectionString);

       Task< List<FdSchemeData>> GetFdSchemeDetailsforGrid(string Fdname, string ApplicantType, string MemberType, string ConnectionString);

        Task<List<FiMemberContactDetails>> GetallJointMembers(string membercode,string Contacttype,string ConnectionString);

        Task<List<TenureModes>> GetFdSchemeTenureModes(string Fdname, string ApplicantType, string MemberType, string ConnectionString);
        

    }
}
