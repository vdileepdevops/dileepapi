using FinstaInfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finsta_Banking_Infrastructure.Banking.Transactions
{
    public class MemberReceiptDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pConid { get; set; }
        public string pContactid { get; set; }
        public string pMembername { get; set; }
        public string pMembercode { get; set; }
        public string pReceiptdate { get; set; }
        public string pModeofreceipt { get; set; }
        public Decimal pReceivedamount { get; set; }
        public string pNarration { get; set; }
        public string pBank { get; set; }
        public Int64 pBankid { get; set; }
        public Int64 pdepositbankid { get; set; }
        public string pBranch { get; set; }
        public string pTranstype { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public string pchequedate { get; set; }
        public string pCardnumber { get; set; }
        public string pUpiid { get; set; }
        public Int64 pSubledgerid { get; set; }
        public Int64 pMemberid { get; set; }
        public string pReceiptno { get; set; }
        public Boolean pStatus { get; set; }      

    }


    public class MemberreceiptViewDTO : CommonDTO
    {
        public Int64 pmemberid { get; set; }
        public string pmembername { get; set; }
        public string pmembercode { get; set; }
        public Int64 pmembertypeid { get; set; }
        public string pmembertype { get; set; }
        public Int64 pcontactid { get; set; }
        public string pcontacttype { get; set; }
        public string pcontactreferenceid { get; set; }
        public string preceiptdate { get; set; }
        public Decimal preceivedamount { get; set; }
        public string pmodeofreceipt { get; set; }
        public string preceiptno { get; set; }
        public string pnarration { get; set; }
        public string pChequeStatus { get; set; }
    }

    public class MembersandContactDetails
    {
        public object pMemberName { get; set; }
        public object pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public object pContactrefid { get; set; }
        public object pContacttype { get; set; }
        public object pTypeofOperation { get; set; }
        public long precordid { get; set; }
        public object pContactnumber { set; get; }
        public object pcontactaddress { get; set; }
        
        public List<DocumentsDetailslist> MemberDocumentsDetailsDTO { get; set; }
    }
    public class DocumentsDetailslist
    {
        public object pdocumentname { get; set; }
        public object pdocreferenceno { get; set; }
    }

    public class SavingAccNameDetails
    {
        public Int64 pSavingConfigid { get; set; }
        public string pSavingAccname { get; set; }
        public string pSavingAccNameCode { get; set; }
    }

    public class SavingAccDetails
    {
        public Int64 psavingaccountid { get; set; }
        public string psavingaccountno { get; set; }
        public string ptransdate { get; set; }
        public Int64 pmembertypeid { get; set; }
        public string pmembertype { get; set; }
        public Int64 pmemberid { get; set; }
        public string pmembercode { get; set; }
        public string pmembername { get; set; }
        public Int64 pcontactid { get; set; }
        public string pcontacttype { get; set; }
        public string pcontactno { get; set; }
        public string pcontactreferenceid { get; set; }
        public string papplicanttype { get; set; }
        public Int64 psavingconfigid { get; set; }
        public string psavingaccname { get; set; }
        public Decimal psavingsamount { get; set; }
        public string psavingsamountpayin { get; set; }
        public string pinterestcompound { get; set; }
        public Decimal pminsavingamount { get; set; }
        public Decimal pminopenamount { get; set; }
        public Decimal pminmaintainbalance { get; set; }
        public string pinterestpayout { get; set; }
        public Decimal pinterestrate { get; set; }
        public Decimal pmaxwithdrawallimit { get; set; }
        public Decimal ppenaltyvalue { get; set; }
        public string psavingspayinmode { get; set; }
        public Decimal psavingmindepositamount { get; set; }
        public Decimal psavingmaxdepositamount { get; set; }
        public Int64 pAccountid { get; set; }
    }

    public class SAReceiptDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pConid { get; set; }
        public string pContactid { get; set; }
        public string pMembername { get; set; }
        public string pMembercode { get; set; }
        public string pReceiptdate { get; set; }
        public string pModeofreceipt { get; set; }
        public Decimal pReceivedamount { get; set; }
        public Decimal ppenaltyamount { get; set; }
        public string pNarration { get; set; }
        public string pBank { get; set; }
        public Int64 pBankid { get; set; }
        public Int64 pdepositbankid { get; set; }
        public string pBranch { get; set; }
        public string pTranstype { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public string pchequedate { get; set; }
        public string pCardnumber { get; set; }
        public string pUpiid { get; set; }
        public Int64 pSubledgerid { get; set; }
        public Int64 pMemberid { get; set; }
        public Int64 psavingaccountid { get; set; }
        public string pReceiptno { get; set; }
        public Boolean pStatus { get; set; }
    }

    public class SavingreceiptViewDTO : CommonDTO
    {
        public Int64 psavingaccountreceiptid { get; set; }
        public string psavingaccountno { get; set; }
        public Int64 pmemberid { get; set; }
        public string pmembername { get; set; }
        public string pmembercode { get; set; }
        public Int64 pmembertypeid { get; set; }
        public string pmembertype { get; set; }
        public Int64 pcontactid { get; set; }
        public string pcontacttype { get; set; }
        public string pcontactreferenceid { get; set; }
        public string preceiptdate { get; set; }
        public Decimal preceivedamount { get; set; }
        public string pmodeofreceipt { get; set; }
        public string preceiptno { get; set; }
        public string pnarration { get; set; }
        public string pChequeStatus { get; set; }
    }

    public class SavingTransactionDTO
    {
        public string ptransdate { get; set; }
        public string preceiptno { get; set; }
        public Decimal preceivedamount { get; set; }
        public string pmodeofreceipt { get; set; }
    }

    #region Share Receipt
    public class ShareAccNameDetails
    {
        public Int64 pShareconfigid { get; set; }
        public string pSharename { get; set; }
        public string pShareNameCode { get; set; }
    }

    public class ShareAccDetails
    {
        public Int64 pshareaccountid { get; set; }
        public string pshareaccountnumber { get; set; }
        public string ptransdate { get; set; }
        public Int64 pmembertypeid { get; set; }
        public string pmembertype { get; set; }
        public Int64 pmemberid { get; set; }
        public string pmembercode { get; set; }
        public string pmembername { get; set; }
        public Int64 pcontactid { get; set; }
        public string pcontacttype { get; set; }
        public string pcontactno { get; set; }
        public string pcontactreferenceid { get; set; }
        public string papplicanttype { get; set; }
        public Int64 pshareconfigid { get; set; }
        public string psharename { get; set; }
        public string preferenceno { get; set; }
        public Decimal pfacevalue { get; set; }
        public Decimal ptotalamount { get; set; }
        public Int64 pnoofsharesissued { get; set; }
        public Int64 pminshares { get; set; }
        public Int64 pmaxshares { get; set; }
        public Int64 pdistinctivefrom { get; set; }
        public Int64 pdistinctiveto { get; set; }
        public string psharesissuedate { get; set; }
        public string pdivedendpayout { get; set; }
        public string pnomineename { get; set; }
        public Int64 pAccountid { get; set; }
    }

    public class ShareReceiptDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public Int64 pConid { get; set; }
        public string pContactid { get; set; }
        public string pMembername { get; set; }
        public string pMembercode { get; set; }
        public string pReceiptdate { get; set; }
        public string pModeofreceipt { get; set; }
        public Decimal pReceivedamount { get; set; }
        public Decimal ppenaltyamount { get; set; }
        public string pNarration { get; set; }
        public string pBank { get; set; }
        public Int64 pBankid { get; set; }
        public Int64 pdepositbankid { get; set; }
        public string pBranch { get; set; }
        public string pTranstype { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public string pchequedate { get; set; }
        public string pCardnumber { get; set; }
        public string pUpiid { get; set; }
        public Int64 pSubledgerid { get; set; }
        public Int64 pMemberid { get; set; }
        public Int64 pshareaccountid { get; set; }
        public string pReceiptno { get; set; }
        public Boolean pStatus { get; set; }
        public long pSavingsMemberAccountid { get; set; }
    }

    public class ShareTransactionDTO
    {
        public string ptransdate { get; set; }
        public string preceiptno { get; set; }
        public Decimal preceivedamount { get; set; }
        public string pmodeofreceipt { get; set; }
    }

    public class SharereceiptViewDTO : CommonDTO
    {
        public Int64 psharereceiptid { get; set; }
        public string pshareaccountno { get; set; }
        public Int64 pmemberid { get; set; }
        public string pmembername { get; set; }
        public string pmembercode { get; set; }
        public Int64 pmembertypeid { get; set; }
        public string pmembertype { get; set; }
        public Int64 pcontactid { get; set; }
        public string pcontacttype { get; set; }
        public string pcontactreferenceid { get; set; }
        public string preceiptdate { get; set; }
        public Decimal preceivedamount { get; set; }
        public string pmodeofreceipt { get; set; }
        public string preceiptno { get; set; }
        public string pnarration { get; set; }
        public string pChequeStatus { get; set; }
    }
    #endregion
}
