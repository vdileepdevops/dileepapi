using FinstaInfrastructure.TDS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.TDS
{
   public interface IChallana
    {
        Task<List<ChallanaDTO>> GetChallanaDetails(string Connectionstring, string FromDate, string ToDate, string SectionName, string CompanyType, string PanType);
        bool SaveChallanaEntry(ChallanaEntryDTO _ChallanaEntryDTO, string ConnectionString);
        List<ChallanaNoDTO> GetChallanaNumbers(string connectionstring);
        List<ChallanaDetailsDTO> GetChallanaEntryDetails(string connectionstring, string ChallanaNO);
        bool SaveChallanaPayment(ChallanaPaymentDTO _ChallanaPaymentDTO, string ConnectionString, out string Paymentid);
        List<ChallanaNoDTO> GetChallanaPaymentNumbers(string connectionstring);
        GetChallanaPaymentsDTO GetChallanaPaymentDetails(string connectionstring, string ChallanaNO);
        bool SaveCinEntry(CinEntryDTO _CinEntryDTO, string ConnectionString);
        Task<List<CinEntryReportDTO>> GetCinEntryReportByChallanaNo(string Connectionstring, string ChallanaNO);
        Task<List<CinEntryReportDTO>> GetCinEntryReportsBetweenDates(string Connectionstring, string FromDate, string ToDate);
        List<ChallanaNoDTO> GetCinEntryChallanaNumbers(string connectionstring);
        ChallanaPaymentReportDTO GetChallanaPaymentReport(string connectionstring, string ChallanaNO);
    }
}
