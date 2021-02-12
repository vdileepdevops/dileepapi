using System;
using System.Collections.Generic;
using System.Text;
using Finsta_Banking_Infrastructure.Banking.Transactions;
using System.Threading.Tasks;
namespace Finsta_Banking_Repository.Interfaces.Banking.Transactions
{
   public interface IRdReceipt
    {
        List<MemberDetailsDTO> GetMemberDetails(string MemberType, string Connectionstring);
        AccountDetailsDTO GetAccountDetails(string MemberCode, string Connectionstring);
        List<AccountDetailsByIdDTO> GetAccountDetailsByid(string AccountNo, string Connectionstring);
        List<ViewDuesDTO> GetViewDues(string AccountNo, string transdate, string Connectionstring);
        bool SaveRdReceipt(RdReceiptDTO ObjRDReceiptDTO, string Connectionstring, out string OUTReceiptid);
        List<RdReceiptDetailsDTO> GetRdReceiptDetails(string FromDate, string Todate, string Connectionstring);
        List<TransactionsDTO> GetTransactionslist(int AccountNo, string Connectionstring);
        List<RDSavingsAccountDetailsDTO> GetAdjustmentDetils(String Membercode, string ConnectionString);

        //List<RDReceiptDetailsDTO> GetRDReceiptDetails(string FromDate, string Todate, string Connectionstring);



    }
}
