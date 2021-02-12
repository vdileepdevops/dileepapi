using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Loans.Transactions
{
    public interface IDisbursement
    {
        Task<DisbursementDTO> GetApprovedApplicationsByID(string vchapplicationid, string ConnectionString);
        bool SaveLoanDisbursement(DisbursementDTO _DisbursementDTO, string ConnectionString,out string paymentId);
        Task<DisbursementViewDTO> GetDisbursementViewData(string ConnectionString);
        Task<EmiChartReportDTO> GetEmiChartReport(string vchapplicationid, string ConnectionString);
        Task<DisbursementReportDTO> GetDisbursedReportDetails(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString);
        Task<List<DisbursementReportDuesDetailsDTO>> GetDisbursedReportDuesDetails(DisbursementReportDTO _DisbursementReportDTO, string ConnectionString);
        Task<List<EmiChartViewDTO>> GetEmiChartViewData(string ConnectionString);

        Task<DisbursementTrendDTO> GetDisbursementTrendReport(string monthname, string ConnectionString);
        Task<DisbursementTrendDTO> GetCollectionandDuesTrendReport(string monthname, string type, string ConnectionString);
    }
}
