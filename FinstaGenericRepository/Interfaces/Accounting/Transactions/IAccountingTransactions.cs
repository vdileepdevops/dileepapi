using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Accounting.Transactions
{
    public interface IAccountingTransactions
    {
        Task<List<TypeofPaymentDTO>> GetTypeofpaymentList(string ConnectionString);
        Task<List<BankDTO>> GetBankntList(string ConnectionString);
        Task<List<AccountsDTO>> GetLedgerAccountList(string ConnectionString, string formname);

        Task<List<AccountsDTO>> GetSubLedgerAccountList(long ledgerid, string ConnectionString);

        Task<List<PartyDTO>> GetPartyList(string ConnectionString);
        bool SaveGeneralReceipt(GeneralreceiptDTO GeneralreceiptDTO, string ConnectionString, out string ReceiptId);
        bool SavePaymentVoucher(PaymentVoucherDTO _PaymentVoucherDTO, string ConnectionString,out string Paymentid);

        Task<List<GstDTo>> GetGstPercentages(string ConnectionString);

        Task<List<Modeoftransaction>> GetModeoftransactions(string ConnectionString);
        Task<List<BankDTO>> GetDebitCardNumbers(string ConnectionString);
        Task<List<BankUPI>> GetUpiNames(long bankid, string ConnectionString);
        Task<List<ChequesDTO>> GetChequeNumbers(long bankid, string ConnectionString);

        Task<List<GstDTo>> getStatesbyPartyid(long ppartyid, string ConnectionString, int id);
        Task<List<TdsSectionDTO>> getTdsSectionsbyPartyid(long ppartyid, string ConnectionString);
        Task<decimal> getpartyAccountbalance(long ppartyid, string ConnectionString);
        Task<decimal> getcashbalance( string ConnectionString);
        decimal GetBankBalance(long recordid, string con);
        Task<List<PaymentVoucherDTO>> GetPaymentVoucherExistingData(string ConnectionString);

        Task<List<GeneralreceiptDTO>> GetReceiptsData(string ConnectionString);

       bool SaveJournalVoucher_All(JournalVoucherDTO _JournalVoucherDTO, string ConnectionString, out string Jvnumber);
        Task<List<GeneralReceiptReportDTO>> GetgeneralreceiptReportData(string ReceiptId, string Connectionstring);

        Task<List<JournalVoucherDTO>> GetJournalVoucherData(string ConnectionString);


        Task<GeneralReceiptReportDTO> GetgeneralreceiptReportData1(string ReceiptId, string Connectionstring);
        Task<PaymentVoucherDTO> GetPaymentVoucherReportDataById(string paymentId, string Connectionstring);
        Task<List<PaymentVoucherReportDTO>> GetPaymentVoucherReportData(string paymentId, string Connectionstring);
        Task<List<JournalVoucherReportDTO>> GetJournalVoucherReportData(string con, string jvnumber);
    }
}
