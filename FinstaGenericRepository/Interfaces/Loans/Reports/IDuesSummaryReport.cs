using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;

namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface IDuesSummaryReport
    {
        Task<List< DuesSummaryReportDTO>> GetDuesSummaryReport(string Connectionstring, string FromDate, string ToDate, int recordid, string fieldname, string fieldtype,string duestype);
    }
}
