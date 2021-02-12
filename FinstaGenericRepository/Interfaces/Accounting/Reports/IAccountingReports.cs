using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Accounting.Reports
{
    public interface IAccountingReports
    {
        Task<List<AccountReportsDTO>> getCashbookData(string con, string fromdate, string todate, string transType);
        Task<List<AccountReportsDTO>> getCashbookDataTotals(string con, string fromdate, string todate);
        Task<List<BankBookDTO>> GetBankNames(string con);
        Task<List<AccountReportsDTO>> GetBankBookDetails(string con, string fromDate, string toDate, long _pBankAccountId);
        Task<List<DayBookDto>> getDaybook(string con, string fromdate, string todate);
        Task<List<AccountReportsDTO>> GetAccountLedgerDetails(string con, string fromDate, string toDate, long pAccountId, long pSubAccountId);
        Task<List<AccountReportsDTO>> GetPartyLedgerDetails(string con, string fromDate, string toDate, long pAccountId, long pSubAccountId, string pPartyRefId);
        Task<List<DayBookDto>> getDaybookTotals(string con, string fromdate, string todate);
        Task<List<BRSDto>> GetBrs(string con, string fromDate, long pBankAccountId);
        Task<List<BRSDto>> GetBrs1(string con, string fromDate, long pBankAccountId);
        Task<List<AccountReportsDTO>> GetTrialBalance(string con, string fromDate, string todate, string groupType);
        Task<List<AccountReportsDTO>> GetLedgerSummary(string con, string fromDate,string todate, string pAccountId);

        Task<List<ProfitAndLossDTO>> GetProfitAndLossData(string con, string fromdate, string todate, string groupType);


        Task<List<ComparisionTb>> GetComparisionTB(string con, string fromDate, string todate);

       
        Task<List<BalanceSheetDTO>> GetBalanceSheetDetails(string con, string fromdate);
        Task<long> GetReprintCount(string con, string receiptno, long recordid);
        Task<List<AccountReportsDTO>> GetReprintBindDetails(string con);
        Task<string> GetReferenceNo(string con, string formName, string transactionNo);
        Task<List<ChequeEnquiryDTO>> GetReceivedChequesData(string con, string pChequeNo);
        Task<List<ChequeEnquiryDTO>> GetReceivedChequesDetails(string con, string pChequeNo);
        Task<List<ChequeEnquiryDTO>> GetIssuedChequesData(string con, string pChequeNo);
        Task<List<ChequeEnquiryDTO>> GetIssuedChequesDetails(string con, string pChequeNo);
        Task<List<JvListDTO>> GetJvListDetails(string con,string fromdate,string todate, string pmodeoftransaction);
        Task<List<ChequeEnquiryDTO>> GetChequeCancelDetails(string con, string fromdate, string todate);

        Task<List<ChequeEnquiryDTO>> GetChequeReturnDetails(string con, string fromdate, string todate);
        Task<List<IssuedChequeDTO>> GetIssuedChequeNumbers(string con, long bankId);

        Task<List<ledgerExtractDTO>> GetLedgerExtractDetails(string con, string fromdate, string todate);

        Task<List<subAccountLedgerDTO>> GetSubAccountLedgerDetails(string con);
        Task<List<subAccountLedgerDTO>> GetAccountLedgerData(string con, string SubLedgerName);

        //Task<List<subAccountLedgerDTO>> GetSubLedgerReportData(string con, string subLedgerName, string ledgerName, string fromDate, string toDate);
        Task<List<IssuedChequeDTO>> GetIssuedChequeDetails(string con, long bankId, long chqBookId, long chqFromNo, long _ChqToNo);

        Task<List<subAccountLedgerDTO>> GetSubLedgerReportData(string con, string subLedgerName, long parentid, string fromDate, string toDate);

        Task<List<subLedgerSummaryDTO>> GetMainAccountHeads(string con);
        Task<List<subLedgerSummaryDTO>> GetSubLedgerAccountNames(string con, string acctyp);
        Task<System.Data.DataSet> GetSubLedgerSummaryReportData(string con, long mainAccountid, string parentids, string fromDate, string toDate);
        Task<System.Data.DataSet> GetMonthlyComparisionReportData(string con, long mainAccountid, string parentids, string fromDate, string toDate);

        Task<bool> UnusedhequeCancel(string con, IssuedChequeDTO issuedChequeDTO);
        Task<MtdytdpandlDTO> GetMTDYTDPANDL(string con, string reportType, string fromDate,string[] reportSubType);
    }
}
