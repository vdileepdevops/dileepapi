using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class FDReceiptDTO : CommonDTO
    {
        public int pRecordid { get; set; }
        public string pReceiptdate { get; set; }
        public int pMemberid { get; set; }
        public int pFdaccountid { get; set; }
        public string pDeposittype { get; set; }
        public string pAccountno { get; set; }
        public string pInstalmentamount { get; set; }
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
        public long pSavingsMemberAccountid { get; set; }
    }
    public class FDMemberDetailsDTO
    {
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pName { get; set; }
        public object pConid { get; set; }
        public object pContactreferenceid { get; set; }
        public object pMembertype { get; set; }
        public object pBusinessentitycontactno { get; set; }
    }
    public class FDDetailsDTO
    {
        public long pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembername { get; set; }
        public object pChitbranchname { get; set; }
        public long pMemberId { get; set; }
    }
    public class FDDetailsByIdDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembername { get; set; }
        public object pDeposiamount { get; set; }
        public object pPaidamount { get; set; }
        public object pClearedmount { get; set; }
        public object pPendingchequeamount { get; set; }
        public object pBalanceamount { get; set; }
        public object pInterestPayble { get; set; }
        public object pMaturityamount { get; set; }
        public object pDeposidate { get; set; }
        public object pMaturitydate { get; set; }
        public object pInterestPayout { get; set; }
        public object pFdschemename { get; set; }
        public object pAccountno { get; set; }
        public object pTenortype { get; set; }
        public object pTenor { get; set; }
        public object pInteresttype { get; set; }
        public object pInterestrate { get; set; }
        public object pTransdate { get; set; }
        public object pFDName { get; set; }


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
    public class FDReceiptDetailsDTO
    {
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pFdaccountno { get; set; }
        public object pReceiptdate { get; set; }
        public object pDueamount { get; set; }
        public object pReceivedAmount { get; set; }
        public object pModeOfReceipt { get; set; }
        public object pReceiptno { set; get; }
        public object pChequestatus { set; get; }
    }
}
