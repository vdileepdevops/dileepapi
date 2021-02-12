using FinstaInfrastructure.TDS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.TDS
{
   public interface ITdsReport
    {
        Task<List<TdsReportDTO>> GetTdsReportDetails(string Connectionstring, string FromDate, string ToDate, string SectionName);
    }
}
