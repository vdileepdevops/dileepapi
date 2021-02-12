using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;

namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface ISanctionLetter
    {
        Task<SanctionLetter> GetSanctionLetterData(string ConnectionString,string VchapplicationID);
        Task <List<SanctionLetter>> GetSanctionLetterMainData(string ConnectionString, string Letterstatus);
        bool Savesanctionletter(SanctionLetter _SanctionLetter, string Connectionstring);
        Task<List<SanctionLetterCounts>> GetSanctionLettersCount(string ConnectionString);
    }
}
