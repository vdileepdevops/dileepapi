using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Loans.Transactions
{
   public interface IReceipts
    {
        #region  GetLoannames
        List<LoannamesDTO> GetLoannames(string ConnectionString);
        #endregion

        #region  GetApplicantionid
        List<EmiReceiptsDTO> GetApplicantionid(string loanname, string ConnectionString,string formname);
        #endregion

        #region  GetParticulars
        List<ParticularsDTO> GetParticulars(string Loanid, string transdate, string ConnectionString, string formname);
        #endregion

        #region  ViewParticularsDetails
        List<ViewParticularsDTO> ViewParticularsDetails(string loanid, string transdate, string ConnectionString, string todate, string duestype);
        #endregion

        #region  GetTransactions
        List<OutstandingbalDTO> GetTransactions(string loanid, string ConnectionString);
        #endregion

        #region  GetLoandetails
        List<LoandetailsDTO> GetLoandetails(string loanid, string ConnectionString);
        #endregion

        #region  SaveEmiReceipt
        bool SaveEmiReceipt(SaveEmireceiptsDTO SaveEmireceiptslist, string ConnectionString, out string OUTReceiptid);
        #endregion

        #region  Viewtodayreceipts
        List<ViewtodayreceiptsDTO> Viewtodayreceipts(string fromdate, string todate, string ConnectionString,string formname);
        #endregion
    }
}
