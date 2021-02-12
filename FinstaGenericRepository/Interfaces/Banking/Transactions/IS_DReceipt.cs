using Finsta_Banking_Infrastructure.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finsta_Banking_Repository.Interfaces.Banking.Transactions
{
    public interface IS_DReceipt
    {
        bool SaveMemberReceipt(MemberReceiptDTO ObjMemberReceiptDTO, string ConnectionString, out string OUTReceiptid);
        List<MemberreceiptViewDTO> GetMemberReceiptView(string FromDate, string Todate, string Connectionstring);
        List<MembersandContactDetails> GetMembers(string Contacttype, string MemberType, string ConnectionString);
        List<SavingAccNameDetails> GetSavingAccountNameDetails(string ConnectionString);
        List<SavingAccDetails> GetSavingAccountNumberDetails(Int64 SavingConfigid, string ConnectionString);
        bool SaveSavingsReceipt(SAReceiptDTO ObjSAReceiptDTO, string ConnectionString, out string OUTReceiptid);
        List<SavingreceiptViewDTO> GetSavingReceiptView(string FromDate, string Todate, string Connectionstring);
        List<SavingTransactionDTO> GetSavingTransaction(Int64 SavingAccountId, string Connectionstring);
        List<ShareAccNameDetails> GetShareAccountNameDetails(string ConnectionString);
        List<ShareAccDetails> GetShareAccountNumberDetails(Int64 ShareConfigid, string ConnectionString);
        bool SaveShareReceipt(ShareReceiptDTO ObjShareReceiptDTO, string ConnectionString, out string OUTReceiptid);
        List<SharereceiptViewDTO> GetShareReceiptView(string FromDate, string Todate, string Connectionstring);
        List<ShareTransactionDTO> GetShareTransaction(Int64 ShareAccountId, string Connectionstring);
    }
}
