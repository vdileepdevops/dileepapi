using FinstaInfrastructure.Accounting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Accounting.Transactions
{
    public interface IChequesOnHand
    {
        Task<List<ReceiptReferenceDTO>> GetChequesOnHandDetails(string con);

        Task<List<ReceiptReferenceDTO>> GetDepositedCancelledCheques(string con, string BrsFromDate, string BrsTodate, Int64 _BankId);

        bool SaveChequesOnHand(ChequesOnHandDTO ChequesOnHanddto, string ConnectionString);
        ChequesOnHandDTO GetBankBalance(long recordid, string con);
        Task<List<ReceiptReferenceDTO>> GetIssuedCancelledCheques(string con, string BrsFromDate, string BrsTodate, Int64 _BankId);
        Task<List<ReceiptReferenceDTO>> GetIssuedDetails(string con, Int64 _BankId);
        bool SaveChequesIssued(ChequesOnHandDTO chequesOnHanddto, string con);

        #region ChequesInBank
        Task<List<ReceiptReferenceDTO>> GetChequesInBankData(string con, long depositedBankid);
        Task<List<ReceiptReferenceDTO>> GetClearedReturnedCheques(string con, string BrsFromDate, string BrsTodate, long depositedBankid);
        bool SaveChequesInBank(ChequesOnHandDTO ChequesOnHanddto, string ConnectionString);
        #endregion
    }
}
