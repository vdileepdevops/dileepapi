using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;
namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IFDReceipt
    {
        List<FDMemberDetailsDTO> GetMemberDetails(string MemberType, string BranchName, string Connectionstring);
        List<FDDetailsDTO> GetFdDetails(string MemberCode, string ChitBranch, string Connectionstring);
        List<FDDetailsByIdDTO> GetFdDetailsByid(string FdAccountNo, string Connectionstring);
        List<TransactionsDTO> GetTransactionslist(int FdAccountNo, string Connectionstring);
        List<FDReceiptDetailsDTO> GetFDReceiptDetails(string FromDate, string Todate, string Connectionstring);
        bool SaveFdReceipt(FDReceiptDTO ObjFdReceiptDTO, string Connectionstring, out string OUTReceiptid);
        Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring);
        Task<List<ChitBranchDetails>> GetFDReceiptBranchDetails(string Connectionstring);

    }
}
