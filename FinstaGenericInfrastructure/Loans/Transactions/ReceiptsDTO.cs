using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Transactions
{
    public class EmiReceiptsDTO
    {
        public Int64 pApplicationid { get; set; }
        public string pVchapplicationid { get; set; }
        public string pApplicantname { get; set; }
        public Int64 pAccountid { get; set; }
        public string pContactid { get; set; }
        public string pLoanname { get; set; }
        public Int64 pConid { get; set; }
        public string pContacttype { get; set; }
    }
    public class ParticularsDTO
    {
        public string pParticularstype { get; set; }
        public string pParticularsname { get; set; }
        public decimal? pPrinciplereceivable { get; set; }
        public decimal? pIntrestreceivable { get; set; }
        public decimal? pAmount { get; set; }
        public Int64 pInstalmentdues { get; set; }
        public decimal? pEmiamount { get; set; }
        public string pLastreceiptdate { get; set; }
        public decimal? pLastreceiptamount { get; set; }
        public string pGsttype { get; set; }
        public string pGstcaltype { get; set; }
        public decimal? pGstpercentage { get; set; }

        public decimal? pFutureprinciple { get; set; }
        public decimal? pFutureinterest { get; set; }
        public Int64 pFutureduecount { get; set; }

    }
    public class ViewParticularsDTO
    {
        public string pParticularsname { get; set; }
        public string pApplicationid { get; set; }
        public Int16 pInstalmentno { get; set; }
        public string pInstalmentdate { get; set; }
        public decimal? pPrinciplereceivable { get; set; }
        public decimal? pIntrestreceivable { get; set; }
        public decimal? pInstalmentdue { get; set; }
        public decimal? pPenaltyreceivable { get; set; }
        public decimal? pInstalmentamount { get; set; }
        public decimal? pAmountadusted { get; set; }


    }
    public class LoannamesDTO
    {
        public Int64 pLoanid { get; set; }
        public string pLoanname { get; set; }
    }
    public class OutstandingbalDTO
    {
        public decimal? pPrincipal { get; set; }
        public decimal? pInterest { get; set; }
        public List<TransactionsDTO> lstTransactions { get; set; }
    }
    public class TransactionsDTO
    {
        public decimal? pReceiptamount { get; set; }
        public string pReceiptno { get; set; }
        public string pNarration { get; set; }
        public string pReceiptdate { get; set; }
        public string pChequestatus { get; set; }
    }
    public class LoandetailsDTO
    {
        public string pApplicantname { get; set; }
        public string pDateofapplication { get; set; }
        public string pLoantype { get; set; }
        public string pLoanname { get; set; }
        public string pPurposeofloan { get; set; }
        public decimal? pApprovedamount { get; set; }
        public decimal? pTenureofloan { get; set; }
        public string pLoanpayin { get; set; }
        public decimal? pRateofinterest { get; set; }
        public string pInteresttype { get; set; }
        public decimal? pEmiAmount { get; set; }
        public string pLoanstatus { get; set; }
    }
    public class SaveEmireceiptsDTO : CommonDTO
    {
        public Int64 pSubledgerid { get; set; }
        public string psubledgername { get; set; }
        public Int64 pConid { get; set; }
        public string pContacttype { get; set; }
        public string pApplicantname { get; set; }
        public string pContactid { get; set; }
        public Int64 pEmiid { get; set; }
        public Int64 pLoanno { get; set; }
        public string pLoanname { get; set; }
        public Int64 pApplicationid { get; set; }
        public string pVchapplicationid { get; set; }
        public string pReceiptdate { get; set; }
        public decimal? pPrincipalreceivable { get; set; }
        public decimal? pInterestreceivable { get; set; }
        public decimal? pInstalmentreceivable { get; set; }
        public decimal? pPenaltyreceivable { get; set; }
        public decimal? pChargesreceivable { get; set; }
        public decimal? pInstallmentreceived { get; set; }
        public decimal? pPenaltyreceived { get; set; }
        public decimal? pChargesreceived { get; set; }
        public decimal? pPenaltywaiveoffamount { get; set; }
        public decimal? pChargeswaiveoffamount { get; set; }
        public decimal? pLoanadvanceamount { get; set; }
        public decimal? pTotalreceived { get; set; }
        public string pModeofreceipt { get; set; }
        public string pBank { get; set; }
        public Int64 pBankid { get; set; }
        public string pBranch { get; set; }
        public string pChequeno { get; set; }
        public string pTransdate { get; set; }
        public string pChrclearstatus { get; set; }
        public string pNarration { get; set; }
        public string pTranstype { get; set; }
        public string pTypeofpaymentonline { get; set; }
        public string pUpiid { get; set; }
        public string pVchcardissuebank { get; set; }
        public string pVchcardnumber { get; set; }
        public string pVchcardtype { get; set; }
        public string pVchcardholdername { get; set; }
        public Int64 pDeposibankid { get; set; }

        public string pFormname { get; set; }
        public List<SaveEmireceiptdetailsDTO> lstSaveEmireceiptdetails { get; set; }
    }
    public class SaveEmireceiptdetailsDTO
    {
        public string pParticularstype { get; set; }
        public string pParticularname { get; set; }
        public string pDetailsreceivabledate { get; set; }
        public decimal? pDetailsreceivableamount { get; set; }
        public decimal? pWaiveoffamount { get; set; }
        public decimal? pDetailsreceivedamount { get; set; }
        public string pGsttype { get; set; }
        public string pGstcaltype { get; set; }
        public decimal? pGstpercentage { get; set; }

        public decimal? pPrinciplereceieved { get; set; }
        public decimal? pInterestreceieved { get; set; }


    }
    public class ViewtodayreceiptsDTO
    {
        public string pReceiptdate { get; set; }
        public string pReceiptno { get; set; }
        public string pLoanno { get; set; }
        public string pCustomername { get; set; }
        public string pModeofreceipt { get; set; }
        public string pBankname { get; set; }
        public string pChequeno { get; set; } 
        public decimal? pTotalreceived { get; set; }
        public string pNarration { get; set; }
    }

    }
