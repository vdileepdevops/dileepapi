using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;
using FinstaInfrastructure.Banking.Masters;
using FiMemberContactDetails = FinstaInfrastructure.Banking.Transactions.FiMemberContactDetails;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
   public interface IRDTransaction
    {
        Task<List<RdMembersandContactDetails>> GetRDMembers(string Contacttype, string MemberType, string ConnectionString);
        Task<RDdetailsFromScheme> GetRdSchemeDetails(string ApplicantType, string MemberType, long RdconfigID, string Rdname, long Tenure, string Tenuremode, decimal instalmentamount, string ConnectionString);
        Task<List<NomineeDetails>> GetMemberNomineeDetails(string MemberCode, string ConnectionString);
        Task<List<RdNameandCode>> GetRdSchemes(string ApplicantType, string MemberType, string ConnectionString);
        string SaveRDMemberandSchemeData(RdMemberandSchemeSave RdTransSaveDTO, string ConnectionString, out long pRdAccountId);

        bool SaveRDJointMembersandNomineeData(RdJointandNomineeSave RdJointandNomineeSave, string ConnectionString);
        bool SaveRDReferralData(RdTransactionReferrals RdTransactionReferrals, string ConnectionString);

        Task<List<RdTransactionMainGridData>> GetRdTransactionData(string ConnectionString);
        Task<List<RdTransactinTenuresofTable>> GetRdTenuresofTable(string RDName, long RdconfigId, string TenureMode, string MemberType,string ConnectionString);

        Task<List<RdTransactinInstallmentsAmountofTable>> GetRdInstallmentsamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, string MemberType, string ConnectionString);

        Task<List<RdSchemeData>> GetRdSchemeDetailsforGrid(string Rdname, string ApplicantType, string MemberType, string ConnectionString);
        Task<List<RDFiMemberContactDetails>> GetallJointMembers(string membercode, string Contacttype, string ConnectionString);

        Task<List<RDTenureModes>> GetRdSchemeTenureModes(string Rdname, string ApplicantType, string MemberType, string ConnectionString);
        int GetRDDepositamountCountofInterestRate(string rdname, decimal instalmentamount, string MemberType, string con);
        RdInterestRateValidation GetRDTenureandMininterestRateofInterestRate(string rdname, decimal instalmentamount, long tenure, string tenureMode, string interestPayout, string MemberType, string con);
        List<RDMatuerityamount> GetRDMaturityamount(string pInterestMode, long pInterestTenure, decimal instalmentamount, string pInterestPayOut, string pCompoundorSimpleInterestType, decimal pInterestRate, string pCalType, string con);

        Task<List<RdTransactinInterestAmountofTable>> GetRDInterestamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, string MemberType, string ConnectionString);

        Task<RdTransactionDataEdit> GetRdTransactionDetailsforEdit(string RdAccountNo, long RdAccountId,string accounttype, string ConnectionString);

        List<RdMembersandContactDetails> GetJointMembersListofRdInEdit(string RdAccountNo, string ConnectionString);
        List<RDMemberNomineeDetails> GetNomineesListofRdInEdit(string RdAccountNo, string ConnectionString);

        Task<List<RdTransactinMaturityAmountofTable>> GetRDMaturityamountsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount, string MemberType, string ConnectionString);

        Task<List<RdInterestPayout>> GetRDPayoutsofTable(string RDName, long RdconfigId, string TenureMode, long Tenure, decimal instalmentamount, decimal Interestamount, decimal Maturityamount, string ConnectionString);

        Task<List<RDInstallmentchart>> GetRdInstallmentchart(string Rdaccountno, string ConnectionString);
    }
}
