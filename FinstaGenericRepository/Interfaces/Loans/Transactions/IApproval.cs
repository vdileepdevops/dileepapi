using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Loans.Transactions
{
    public interface IApproval
    {
        #region Saveapprovedapplications
        bool Saveapprovedapplications(ApprovalDTO ApprovalList, string Connectionstring);
        #endregion

        #region  GetApplicantSureitytypes
        List<ApprovalDTO> Getapprovedapplications(string Strapplicationid, string ConnectionString);
        #endregion

        #region  ViewApplications
        List<ViewapplicationsDTO> ViewApplications(string ViewType, string ConnectionString);
        #endregion

        #region  GetLoanwisecharges
        List<LoanwisechargeDTO> GetLoanwisecharges(string Loanname,decimal Amount,decimal tenor, string applicanttype, string Loanpayin, string ConnectionString, string tranddate, Int32 schemeid);
        #endregion

        #region  ViewApplicationsbyid
        List<ViewapplicationsDTO> ViewApplicationsbyid(string applicationid, string ConnectionString);
        #endregion

        #region GetcashflowStatements
        List<CashflowDTO> GetSavingdetails(string applicationid, string ConnectionString);
        List<CashflowDTO> GetExstingloandetails(string applicationid, string ConnectionString);
        #endregion

    }
}
