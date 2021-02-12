using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Accounting
{
    public class AccountsMasterDTO
    {
        public List<Modeoftransaction> modeofTransactionslist { get; set; }
        public List<BankDTO> banklist { get; set; }
        public List<AccountsDTO> accountslist { get; set; }
        public List<PartyDTO> partylist { get; set; }
        public List<GstDTo> Gstlist { get; set; }
        public List<TdsSectionDTO> lstTdsSectionDetails { get; set; }
        public List<BankDTO> bankdebitcardslist { get; set; }
        public List<BankUPI> bankupilist { get; set; }
        public List<ChequesDTO> chequeslist { get; set; }
        public List<GstDTo> statelist { get; set; }
        public decimal accountbalance { get; set; }
        public decimal cashbalance { get; set; }
        public decimal bankbalance { get; set; }
        public decimal bankpassbookbalance { get; set; }
        //public List<PartyDTO> walletlist { get; set; }
    }
    public class AccountsDTO : CommonDTO
    {
        public long psubledgerid { get; set; }
        public string psubledgername { get; set; }
        public long pledgerid { get; set; }
        public string pledgername { get; set; }

        public long id { get; set; }
        public string text { get; set; }

        public string ptranstype { get; set; }
        public decimal accountbalance { get; set; }
        public string pAccounttype { get; set; }
    }

    public class PartyDTO
    {
        public string ppartypannumber;

        public long ppartyid { get; set; }
        public string ppartyname { get; set; }
        public string ppartyemailid { get; set; }
        public long ppartycontactno { get; set; }
        public string ppartyreferenceid { get; set; }
        public string ppartyreftype { get; set; }
    }

    public class BankDTO
    {
        public string pCardNumber;

        public string pbankname { get; set; }
        public long pbankid { get; set; }
        public string pbranchname { get; set; }
        public long pdepositbankid { get; set; }
        public string pdepositbankname { get; set; }
        public decimal pbankbalance { get; set; }
        public string pbankaccountnumber { get; set; }
        public string pfrombrsdate { get; set; }
        public string ptobrsdate { get; set; }
        public decimal pbankpassbookbalance { get; set; }
    }

    public class BankDebitCardsDTO
    {
        public string pCardNumber { get; set; }
    }
    public class TypeofPaymentDTO
    {
        public string ptypeofpayment { get; set; }
    }
    public class ReceiptsDTO : AccountsDTO
    {
        public decimal pamount { get; set; }
        public string pgsttype { get; set; }
        public string pgstcalculationtype { get; set; }
        public decimal pgstpercentage { get; set; }
        public decimal pigstamount { get; set; }
        public decimal pcgstamount { get; set; }
        public decimal psgstamount { get; set; }
        public decimal putgstamount { get; set; }
        public string pState { get; set; }
        public long pStateId { get; set; }
        public string pgstno { get; set; }

        public bool IsGstapplicable { get; set; }

        public decimal ptdsamountindividual { get; set; }

        public string ptdssection { get; set; }
        public decimal ptdspercentage { get; set; }
    }

    public class PaymentsDTO : ReceiptsDTO
    {
        public bool pisgstapplicable;

        public string pgstnumber { get; set; }

        public string ppartyname { get; set; }
        public long ppartyid { get; set; }
        public string ppartyreferenceid { get; set; }
        public string ppartyreftype { get; set; }
        public Boolean pistdsapplicable { get; set; }
        public string pTdsSection { get; set; }
        public decimal pTdsPercentage { get; set; }
        public decimal ptdsamount { get; set; }
        public string ptdscalculationtype { get; set; }
        public long ptdsaccountId { get; set; }
        public string ppartypannumber { get; set; }
        public string ptdsrefjvnumber { get; set; }
    }

    public class ModeofPaymentDTO : BankUPI
    {
        public string pbankname { get; set; }
        public string pbranchname { get; set; }
        public string ptranstype { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public string pchequedate { get; set; }
        public long pbankid { get; set; }
        public string pCardNumber { get; set; }
        public long pdepositbankid { get; set; }
        public string pdepositbankname { get; set; }
    }
    public class GeneralreceiptDTO : ModeofPaymentDTO
    {
        public string preceiptid { get; set; }
        public string preceiptdate { get; set; }
        public string pmodofreceipt { get; set; }
        public decimal ptotalreceivedamount { get; set; }
        public string pnarration { get; set; }
        public string ppartyname { get; set; }
        public long ppartyid { get; set; }
        public Boolean pistdsapplicable { get; set; }
        public string pTdsSection { get; set; }
        public decimal pTdsPercentage { get; set; }
        public decimal ptdsamount { get; set; }
        public string ptdscalculationtype { get; set; }
        public string ppartypannumber { get; set; }
        public string ppartyreftype { get; set; }

        public string ppartyreferenceid { get; set; }

        public List<ReceiptsDTO> preceiptslist { get; set; }
        public string pFilename { get; set; }
        public string pFilepath { get; set; }
        public string pFileformat { get; set; }

        public string pCleardate { get; set; }
        public string pDepositeddate { get; set; }
    }

    public class PaymentVoucherDTO : ModeofPaymentDTO
    {
        public string ppaymentid { get; set; }
        public string ppaymentdate { get; set; }
        public decimal ptotalpaidamount { get; set; }
        public string pnarration { get; set; }
        public string pmodofPayment { get; set; }
        public List<PaymentsDTO> ppaymentslist { get; set; }
        public string pFilename { get; set; }
        public string pFilepath { get; set; }
        public string pFileformat { get; set; }
    }

    public class PaymentVoucherReportDTO
    {
        public string pChequenumber;
        public string ptypeofpayment;
        public string pcontactid;
        public string pcontactname;

        public string ppaymentid { get; set; }
        public string ppaymentdate { get; set; }

        public string pnarration { get; set; }
        public string pmodofPayment { get; set; }
        public string pemployeename { get; set; }
        public List<GeneralReceiptSubDetails> ppaymentslist { get; set; }
        public CompanyInfoDTO pCompanyInfoforReports { get; set; }

    }
    public class JournalVoucherDTO : CommonDTO
    {
        public string pjvnumber { get; set; }
        public string pjvdate { get; set; }
        public decimal ptotalpaidamount { get; set; }
        public string pnarration { get; set; }
        public string pmodoftransaction { get; set; }

        public List<PaymentsDTO> pJournalVoucherlist { get; set; }
        public string pFilename { get; set; }
        public string pFilepath { get; set; }
        public string pFileformat { get; set; }
    }
    public class ReceiptReferenceDTO : GeneralreceiptDTO
    {
        //public string pchequesonhandstatus { get; set; }
        public Boolean pdepositstatus { get; set; }
        public Boolean pcancelstatus { get; set; }
        public Boolean preturnstatus { get; set; }
        public string pchequestatus { get; set; }
        //public long pdepositedBankid { get; set; }
        //public string pdepositedBankName { get; set; }
        public decimal pcancelcharges { get; set; }
        public decimal pactualcancelcharges { get; set; }
        public string pledger { get; set; }
        public string pdepositeddate { get; set; }
        public string pcleardate { get; set; }
    }

    public class ChequesOnHandDTO
    {
        public string ptransactiondate { get; set; }
        public string pfrombrsdate { get; set; }
        public string ptobrsdate { get; set; }
        public decimal _BankBalance { get; set; }
        public List<ReceiptReferenceDTO> pchequesOnHandlist { get; set; }
        public List<ReceiptReferenceDTO> pchequesclearreturnlist { get; set; }
    }

    public class GstDTo
    {
        public long pRecordId { get; set; }
        public decimal pgstpercentage { get; set; }
        public decimal pigstpercentage { get; set; }
        public decimal pcgstpercentage { get; set; }
        public decimal psgstpercentage { get; set; }
        public decimal putgstpercentage { get; set; }
        public string pState { get; set; }
        public int pStateId { get; set; }

        public string pgstno { get; set; }
        public string pgsttype { get; set; }
        public int pisgstapplicable { get; set; }
    }

    public class AccountTreeDTO : CommonDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public Int64? pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
    }
    public class AccountCreationDTO : CommonDTO
    {
        public long pRecordId { get; set; }
        public long pAccountid { set; get; }
        public string pAccountname { set; get; }
        public long pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal pOpeningamount { set; get; }
        public decimal? pPartyname { set; get; }
        public string pFormtype { set; get; }
        public string pOpeningdate { set; get; }
        public string pOpeningBalanceType { set; get; }
        public object pistdsapplicable { get; set; }
        public object ptdscalculationtype { get; set; }
        public object pisgstapplicable { get; set; }
        public object pgsttype { get; set; }
        public object pgstpercentage { get; set; }
    }



    public class Modeoftransaction : CommonDTO
    {
        public long pRecordid { get; set; }
        public string pmodofPayment { get; set; }
        public string pmodofreceipt { get; set; }

        public string ptranstype { get; set; }
        public string ptypeofpayment { get; set; }
        public string pchqonhandstatus { get; set; }
        public string pchqinbankstatus { get; set; }
        public string pchqclearstatus { get; set; }
    }

    public class GeneralReceiptReportDTO
    {
        public string pReceiptId { get; set; }
        public string pReceiptdate { get; set; }
        public long pCrdAccountId { get; set; }
        public decimal pLedgeramount { get; set; }
        public string pGstno { get; set; }
        public string pContactname { get; set; }
        public string pNarration { get; set; }
        public string pAccountname { get; set; }
        public string pAmountinWords { get; set; }
        public List<GeneralReceiptSubDetails> pGeneralReceiptSubDetailsList { set; get; }
        public string pModeofreceipt { get; set; }
        public string pReferenceorChequeNo { get; set; }
        public string pTypeofpayment { get; set; }
        public string pPostedby { get; set; }

        public CompanyInfoDTO pCompanyInfoforReports { get; set; }
        public string pChequedate { get; set; }
        public string pCleardate { get; set; }
        public string pDepositeddate { get; set; }
    }
    public class GeneralReceiptSubDetails
    {
        public decimal pLedgeramount { get; set; }
        public string pAccountname { get; set; }
        public string pgstcalculationtype { get; set; }
        public decimal pcgstamount { get; set; }
        public decimal psgstamount { get; set; }
        public decimal pigstamount { get; set; }
        public string ptdscalculationtype { get; set; }
        public decimal ptdsamount { get; set; }
    }
    public class JournalVoucherReportDTO
    {
        public string pJvnumber { get; set; }
        public string pJvDate { get; set; }
        public double pCreditAmount { get; set; }
        public double pDebitamount { get; set; }
        public string pNarration { get; set; }
        public string pParticulars { get; set; }
        public List<JournalVoucherReportDTO> plstJournalVoucherReportDTO { get; set; }
      

    }
    #region Account Tree New
    public class AccountTreeNewDTO : CommonDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public object Main_accountname { set; get; }
        public object pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
        public List<AccountsMasterTwotypeDTO> Children { get; set; }
    }
    public class AccountsMasterTwotypeDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public object Main_accountname { set; get; }
        public object pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
        public List<AccountsMasterThreetypeDTO> Children { get; set; }
    }
    public class AccountsMasterThreetypeDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public object Main_accountname { set; get; }
        public object pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
        public List<AccountsMasterFourtypeDTO> Children { get; set; }
    }
    public class AccountsMasterFourtypeDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public object Main_accountname { set; get; }
        public object pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
        public List<AccountsMasterFivetypeDTO> Children { get; set; }
    }
    public class AccountsMasterFivetypeDTO
    {
        public Int64 pRecordId { get; set; }
        public Int64 pAccountid { set; get; }
        public string pAccountname { set; get; }
        public object Main_accountname { set; get; }
        public object pParentId { set; get; }
        public string pParrentAccountname { set; get; }
        public string pAcctType { set; get; }
        public string pChracctype { set; get; }
        public decimal? pAccountBalance { set; get; }
        public string pOpeningdate { set; get; }
        public string pFontcolor { set; get; }
        public string pAmounttype { set; get; }
        public long pRownum { set; get; }
        public long pLevel { set; get; }
        public bool pHaschild { set; get; }
    }
    public class TdsSectionNewDTO
    {
        public object istdsapplicable;

        public object pRecordid { get; set; }
        public object pTdsSection { get; set; }
        public object sectionname { get; set; }
        public object pTdsPercentage { get; set; }
        public object withpanpercentage { get; set; }
        public object withcompanypanpercentage { get; set; }
        public object withoutpanpercentage { get; set; }
        public object monthlylimitamount { get; set; }
        public object yearlylimitamount { get; set; }
    }
    #endregion

    #region Account Tree By Sai Mahesh

    public class AccountCreationNewDTO : CommonDTO
    {
        public object pRecordId { get; set; }
        public object pAccountid { set; get; }
        public object pAccountname { set; get; }
        public object pParentId { set; get; }
        public object pParrentAccountname { set; get; }
        public object pAcctType { set; get; }
        public object pChracctype { set; get; }
        public decimal pOpeningamount { set; get; }
        public object pPartyname { set; get; }
        public object pFormtype { set; get; }
        public object pOpeningdate { set; get; }
        public object pOpeningBalanceType { set; get; }

        public object schema { set; get; }

        public object pistdsapplicable { get; set; }
        public object ptdscalculationtype { get; set; }
        public object pisgstapplicable { get; set; }
        public object pgsttype { get; set; }
        public object pgstpercentage { get; set; }
        public object debit_restriction_status { get; set; }
        public object credit_restriction_status { get; set; }

    }

    public class JournalVoucherNewDTO : CommonDTO
    {
        public string pjvnumber { get; set; }
        public string pjvdate { get; set; }
        public object ptotalpaidamount { get; set; }
        public string pnarration { get; set; }
        public string pmodoftransaction { get; set; }
        public List<PaymentsNewDTO> pJournalVoucherlist { get; set; }
        public string pFilename { get; set; }
        public string pFilepath { get; set; }
        public string pFileformat { get; set; }
        public object referenceno { get; set; }
    }

    public class PaymentsNewDTO : ReceiptsDTO
    {
        //public object pisgstapplicable;
        public object pgstnumber { get; set; }
        public object ppartyname { get; set; }
        public string ppartyid { get; set; }
        public string ppartyreferenceid { get; set; }
        public string ppartyreftype { get; set; }
        public Boolean pistdsapplicable { get; set; }
        //public object pTdsSection { get; set; }
        //public object pTdsPercentage { get; set; }
        public decimal ptdsamount { get; set; }
        public object ptdscalculationtype { get; set; }
        public long ptdsaccountId { get; set; }
        public object ppartypannumber { get; set; }
        public object ptdsrefjvnumber { get; set; }
        public object ledgeramount { get; set; }
        public object totalreceivedamount { get; set; }

        public object pFilename { get; set; }

        public object agentcode { get; set; }

        public object ticketno { get; set; }
        public object chitgroupid { get; set; }
        public object schemesubscriberid { get; set; }
    }
    public class AccountsTreeDTO
    {
        public object rowid { get; set; }
        public object tbl_mst_account_id { set; get; }
        public object account_id { set; get; }
        public object account_name { set; get; }
        public object balance { set; get; }
        public object Main_accountname { set; get; }
        public object parent_id { set; get; }
        public object parent_account_name { set; get; }
        public object account_balance { set; get; }
        public object chracc_type { set; get; }
        public object level { set; get; }
        public object amount_type { set; get; }
        public object haschild { set; get; }
        public object fcolor { set; get; }
        public object CreatedBy { set; get; }
        public decimal AccountHeadSum { set; get; }

        public object Subcategorycreationstatus { set; get; }

        public List<AccountsMasterTwotypeNewDTO> Children { get; set; }
    }

    public class AccountsMasterTwotypeNewDTO
    {
        public object rowid { get; set; }
        public object tbl_mst_account_id { set; get; }
        public object account_id { set; get; }
        public object account_name { set; get; }
        public object Main_accountname { set; get; }
        public object parent_id { set; get; }
        public object parent_account_name { set; get; }
        public object account_balance { set; get; }
        public object chracc_type { set; get; }
        public object level { set; get; }
        public object amount_type { set; get; }
        public object haschild { set; get; }
        public object fcolor { set; get; }
        public object CreatedBy { set; get; }
        public decimal AccountHeadSum { set; get; }

        public object Subcategorycreationstatus { set; get; }
        public List<AccountsMasterThreetypeNewDTO> Children { get; set; }
    }
    public class AccountsMasterThreetypeNewDTO
    {
        public object rowid { get; set; }
        public object tbl_mst_account_id { set; get; }
        public object account_id { set; get; }
        public object account_name { set; get; }

        public object Main_accountname { set; get; }
        public object parent_id { set; get; }
        public object parent_account_name { set; get; }
        public object account_balance { set; get; }
        public object chracc_type { set; get; }
        public object level { set; get; }
        public object amount_type { set; get; }
        public object haschild { set; get; }
        public object fcolor { set; get; }
        public object CreatedBy { set; get; }
        public decimal AccountHeadSum { set; get; }


        public object Subcategorycreationstatus { set; get; }

        public List<AccountsMasterFourtypeNewDTO> Children { get; set; }
    }

    public class AccountsMasterFourtypeNewDTO
    {
        public object rowid { get; set; }
        public object tbl_mst_account_id { set; get; }
        public object account_id { set; get; }
        public object account_name { set; get; }

        public object Main_accountname { set; get; }
        public object parent_id { set; get; }
        public object parent_account_name { set; get; }
        public object account_balance { set; get; }
        public object chracc_type { set; get; }
        public object level { set; get; }
        public object amount_type { set; get; }
        public object haschild { set; get; }
        public object fcolor { set; get; }
        public object CreatedBy { set; get; }
        public decimal AccountHeadSum { set; get; }

        public object Subcategorycreationstatus { set; get; }

        public List<AccountsMasterFivetypeNewDTO> Children { get; set; }
    }

    public class AccountsMasterFivetypeNewDTO
    {
        public object rowid { get; set; }
        public object tbl_mst_account_id { set; get; }
        public object account_id { set; get; }
        public object account_name { set; get; }
        public object Main_accountname { set; get; }
        public object parent_id { set; get; }
        public object parent_account_name { set; get; }
        public object account_balance { set; get; }
        public object chracc_type { set; get; }
        public object level { set; get; }
        public object amount_type { set; get; }
        public object haschild { set; get; }
        public object fcolor { set; get; }
        public object CreatedBy { set; get; }

        public decimal AccountHeadSum { set; get; }
        public object Subcategorycreationstatus { set; get; }

    }

    public class AccountHeadsDTO
    {
        public object RecordId { get; set; }
        public object Accountid { set; get; }
        public object Accountname { set; get; }
        public object ParentId { set; get; }
        public object ParrentAccountname { set; get; }
        public object AcctType { set; get; }
        public object Chracctype { set; get; }
        public object Openingamount { set; get; }
        public object Partyname { set; get; }
        public object Formtype { set; get; }
        public object Openingdate { set; get; }
        public object OpeningBalanceType { set; get; }
        public object CreatedBy { set; get; }

    }
    #endregion
}
