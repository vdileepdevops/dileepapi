using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IMaturityPayment
    {
        List<MemberTypesDTO> GetMemberTypes(string ConnectionString);
        List<MemberIdsDTO> GetMemberIds(string Membertype, string Connectionstring);
        List<DepositIdsDTO> GetDepositIds(string BranchName, string MaturityType, long Schemeid, string Connectionstring);
        List<MaturityBondViewDTO> GetMaturityBondView(string Connectionstring);
        PreMaturedetailsDTO GetPreMaturityDetails(string FDAccountno, string Date, string type, string Connectionstring);
        bool SaveMaturitybond(MaturitybondsSave _MaturitybondsSaveDTO, string connectionstring);
        Task<List<SchemeTypeDTO>> GetSchemeType(string BranchName, string MaturityType, string ConnectionString);
        Task<List<ChitBranchDetails>> GetMaturityBranchDetails(string MaturityType, string Connectionstring);
        Task<List<MaturityMembersDTO>> GetMaturityMembers(string PaymentType,string ConnectionString);
        List<MaturityBondstList> GetMaturityFdDetails(string PaymentType,long Memberid, string Date, string Connectionstring);
        List<MaturityPaymentDetailsViewDTO> GetMaturityPaymentDetailsView(string Connectionstring);
        List<LienEntryViewDTO> GetLienDetails(string FdAccountNo, string ConnectionString);
        Task<FdTransactionDataEdit> GetFdTransactionDetails(string FdAccountNos,string ConnectionString);
        string SaveMaturityRenewal(MaturityRenewalSaveDTO _MaturityRenewalSaveDTO, string ConnectionString, out long pFdAccountId);

        bool SaveMaturityPayment(MaturityPaymentSaveDTO ObjMaturityPaymentDTO, string Connectionstring, out string OUTPaymentId);
    }
}
