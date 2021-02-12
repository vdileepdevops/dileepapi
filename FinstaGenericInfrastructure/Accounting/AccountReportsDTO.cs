using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Accounting
{
    public class AccountReportsDTO
    {



        public string pparentname;

        public string pFormName { get; set; }
        public string ptransactiondate { get; set; }
        public string ptransactionno { get; set; }
        public string pparticulars { get; set; }
        public string pdescription { get; set; }
        public double pdebitamount { get; set; }
        public double? pcreditamount { get; set; }
        public double popeningbal { get; set; }
        public double pclosingbal { get; set; }
        public double pcashtotal { get; set; }
        public double pchequetotal { get; set; }
        public string pmodeoftransaction { get; set; }
        public long precordid { get; set; }
        public string paccountname { get; set; }
        public long paccountid { get; set; }
        public long pparentid { get; set; }
        public string pBalanceType { get; set; }
        public long pgrouprecordid { get; set; }
    }
    public class cashBookDto : AccountReportsDTO
    {
        public List<AccountReportsDTO> plstcashbookdata { get; set; }
        public List<AccountReportsDTO> plstcashchequetotal { get; set; }
    }
    public class BankBookDTO : AccountReportsDTO
    {
        public long pbankaccountid { get; set; }
        public string pbankname { get; set; }
        public List<AccountReportsDTO> plstBankBook = new List<AccountReportsDTO>();
    }
    public class DayBookDto : AccountReportsDTO
    {
        public string prcpttransactiondate { get; set; }
        public string prcptaccountname { get; set; }
        public string prcpttransactionno { get; set; }
        public double? prcptdebitamount { get; set; }
        public string prcptparticulars { get; set; }
        public List<DayBookDto> plstdaybookdata { get; set; }
        public List<DayBookDto> plstdaybooktotals { get; set; }
        public string pdatdate { get; set; }
    }
    public class BRSDto : AccountReportsDTO
    {
        public string pChequeNumber { get; set; }
        public long ptotalreceivedamount { get; set; }
        public string pBankName { get; set; }
        public string pBranchName { get; set; }
        public string pGroupType { get; set; }
        public long pBankBookBalance { get; set; }
        public List<BRSDto> lstBRSDto { get; set; }
        public List<BRSDto> lstBRSDto1 { get; set; }
    }
    public class ComparisionTb
    {
        public string pparentaccountName { get; set; }
        public string paccountName { get; set; }
        public decimal pdebitamount1 { get; set; }
        public decimal pcreditamount1 { get; set; }
        public decimal pdebitamount2 { get; set; }
        public decimal pcreditamount2 { get; set; }
        public decimal pdebittotal { get; set; }
        public decimal pcredittotal { get; set; }
    }
    public class ProfitAndLossDTO : AccountReportsDTO

    {
        public Int64? pAccountid { set; get; }
        public string pAccountname { set; get; }
        public string pAmount { set; get; }
        public Int64? pParentid { set; get; }
        public string pColorcode { set; get; }
        public Int64 pLevel { set; get; }
        public List<ProfitAndLossDTO> lstProfitAndLossDTO { get; set; }
        public double? pTotalAmount { set; get; }
    }
    public class BalanceSheetDTO : ProfitAndLossDTO
    {
        public List<BalanceSheetDTO> plstBalanceSheet { get; set; }
    }
    public class ChequeEnquiryDTO
    {
        public string preferencenumber { get; set; }
        public string pparticulars { get; set; }
        public string preceiptid { get; set; }
        public string pchequedate { get; set; }
        public string pbankname { get; set; }
        public string pdepositeddate { get; set; }
        public string pcleardate { get; set; }
        public Int64 ptotalreceivedamount { get; set; }
        public string pchequesstatus { get; set; }
        public string pdepositbankname { get; set; }
        public List<ChequeEnquiryDTO> plstChequeEnquiry { get; set; }
    }
    public class JvListDTO : AccountReportsDTO
    {
        public List<JvListDTO> plstJvList { get; set; }
    }

    public class IssuedChequeDTO : CommonDTO
    {
        public long pchequeNoFrom { get; set; }
        public long pchequeNoTo { get; set; }
        public long pchkBookId { get; set; }
        public List<IssuedChequeDTO> lstIssuedCheque { get; set; }
        public string pchqfromto { get; set; }
        public string pchequenumber { get; set; }
        public string ppaymentid { get; set; }
        public string pparticulars { get; set; }
        public string ppaymentdate { get; set; }
        public string pcleardate { get; set; }
        public string pstatus { get; set; }
        public Int64 ppaidamount { get; set; }
        public string pbankname { get; set; }
        public string pchequestatus { get; set; }
        public long pbankaccountid { get; set; }
    }
    public class ledgerExtractDTO : AccountReportsDTO
    {
        public List<ledgerExtractDTO> pLedgerExtract { get; set; }

    }
    public class subAccountLedgerDTO : AccountReportsDTO
    {
        public List<subAccountLedgerDTO> plstSubAccountLedger { get; set; }
    }
    public class subLedgerSummaryDTO : AccountReportsDTO
    {
        public string pAccountType { get; set; }
        public double pAmount { get; set; }
        public List<subLedgerSummaryDTO> plstSubLedgerSummary { get; set; }
    }
    public class MtdytdpandlDTO
    {
        public List<MtdytdpandlDetailsDTO> plstMtdytdpandl { get; set; }
        public List<MtdytdpandlDetailsDTO> plstMtdytdpandldetails { get; set; }
        public List<MtdytdpandlDetailsDTO> plstmtdytdColumnslist { get; set; }
    }
        public class MtdytdpandlDetailsDTO
    {
        public string paccountname { get; set; }
        public string plevel0 { get; set; }
        public string plevel1 { get; set; }
        public string plevel2 { get; set; }
        public string plevel3 { get; set; }
        public double pJan { get; set; }
        public double pFeb { get; set; }
        public double pMar { get; set; }
        public double pApr { get; set; }
        public double pMay { get; set; }
        public double pJun { get; set; }
        public double pJul { get; set; }
        public double pAug { get; set; }
        public double pSep { get; set; }
        public double pOct { get; set; }
        public double pNov { get; set; }
        public double pDec { get; set; }
        public string pfield { get; set; }
        public string ptitle { get; set; }
        public string ptype { get; set; }
        public string pformat { get; set; }

    }


}

