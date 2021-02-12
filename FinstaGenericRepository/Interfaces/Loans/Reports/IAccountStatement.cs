using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;

namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface IAccountStatement
    {
        Task<AccountStatementDTO> GetAccountstatementReport(string ConnectionString, string VchapplicationID);
    }
}
