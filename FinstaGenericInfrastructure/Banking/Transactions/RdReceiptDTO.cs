using System;
using System.Collections.Generic;
using System.Text;

namespace Finsta_Banking_Infrastructure.Banking.Transactions
{
    public class RdReceiptDTO
    {
        public int pRecordid { get; set; }
        public string pReceiptdate { get; set; }
        public int pMemberid { get; set; }
        public int pAccountid { get; set; }
        public string pDeposittype { get; set; }
        public string pAccountno { get; set; }
        public string pInstalmentamount { get; set; }
        public string pPenaltyamount { get; set; }
        public string pReceivedamount { get; set; }
        public string pModeofreceipt { get; set; }
        public string pReceiptno { get; set; }
        public string pNarration { get; set; }
        public string pBank { get; set; }
        public Int64 pBankid { get; set; }
        public string pBranch { get; set; }
        public string pReferenceno { get; set; }
        public string pTransdate { get; set; }
        public string pChrclearstatus { get; set; }
        public string pTranstype { get; set; }
        public string pTypeofpaymentonline { get; set; }
        public string pUpiid { get; set; }
        public string pCardissuebank { get; set; }
        public string pCardnumber { get; set; }
        public string pCardtype { get; set; }
        public string pCardholdername { get; set; }
        public Int64 pdepositbankid { get; set; }
        public string pFormname { get; set; }
        public Int64 pSubledgerid { get; set; }
        public string psubledgername { get; set; }
        public Int64 pConid { get; set; }
        public string pContactid { get; set; }
        public string pContacttype { get; set; }
        public string pMembername { get; set; }
        public string pchequedate { get; set; }
        public Boolean pStatus { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public int pCreatedby { get; set; }
        public long pSavingsMemberAccountid { get; set; }
    }
    public class MemberDetailsDTO
    {
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pName { get; set; }
        public object pConid { get; set; }
        public object pContactreferenceid { get; set; }
        public object pMembertype { get; set; }
        public object pBusinessentitycontactno { get; set; }

    }
    public class AccountDetailsDTO
    {
        public List<RDAccountDetailsDTO> RDAccountDetailsDTOList { get; set; }
        public List<RDSavingsAccountDetailsDTO> RDSavingsAccountDetailsDTOList { get; set; }
    }
        public class RDAccountDetailsDTO
    {
        public long paccountid { get; set; }
        public object paccountno { get; set; }
        public object pMembername { get; set; }
        public long pSubledgerid { get; set; }
    }
    public class RDSavingsAccountDetailsDTO
    {
        public object pSavingsMemberAccountno { get; set; }
        public object pSavingsMemberAccountName { get; set; }
        public object pSavingsMemberAccountid { get; set; }
        public object pSavingsMemberBalance { get; set; }
    }


    public class AccountDetailsByIdDTO
    {
        public object paccountid { get; set; }
        public object paccountno { get; set; }
        public object pMembername { get; set; }
        public object pDeposiamount { get; set; }
        public object pMaturityamount { get; set; }
        public object pInstalmentamount { get; set; }
        public object pInstalmentPayin { get; set; }
        public object pInstalmentTenor { get; set; }
        public object pInterestPayble { get; set; }
        public object pDeposidate { get; set; }
        public object pMaturitydate { get; set; }
        public object pInterestPayout { get; set; }
        public object pschemename { get; set; }
        public object pAccountno { get; set; }
        public object pInterestrate { get; set; }
        public object pTransdate { get; set; }

    }
    public class ViewDuesDTO
    {
        public object pInstalmentno { get; set; }
        public object pInstalmentdate { get; set; }
        public object pinstalmentamount { get; set; }
        public object pPenalty { get; set; }
    }
    public class RdReceiptDetailsDTO
    {
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pRdaccountno { get; set; }
        public object pReceiptdate { get; set; }
        public object pReceivedAmount { get; set; }
        public object pModeOfReceipt { get; set; }
        public object pReceiptno { set; get; }
        public object pChequestatus { set; get; }
    }

    public class TransactionsDTO
    {
        public object pReceiptamount { get; set; }
        public object pReceiptno { get; set; }
        public object pNarration { get; set; }
        public object pReceiptdate { get; set; }
        public object pChequestatus { get; set; }
        public object pModeofReceipt { get; set; }
    }
    public class AdjustdetailsDTO
    {
        public object pAccountno { get; set; }
        public object pName { get; set; }
        public object pAccountid { get; set; }
        public object pBalance { get; set; }

    }

    //public class RDReceiptDetailsDTO
    //{
    //    public object pMembername { get; set; }
    //    public object pMembercode { get; set; }
    //    public object pRdaccountno { get; set; }
    //    public object pReceiptdate { get; set; }
    //    public object pInstalmentamount { get; set; }
    //    public object pReceivedAmount { get; set; }
    //    public object pModeOfReceipt { get; set; }
    //    public object pReceiptno { set; get; }
    //    public object pChequestatus { set; get; }
    //}

   
    

}
