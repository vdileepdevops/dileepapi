using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;

namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface IDisbursmentLetter
    {
        Task<List<DisbursmentLetterDTO>> GetDisbursalLetterMainData(string ConnectionString, string Letterstatus);
        Task<List<DisbursalLetterCount>> GetDisbursementLettersCount(string ConnectionString);
        bool SavedisbursalLetter(DisbursmentLetterDTO _DisbursmentLetterDTO, string Connectionstring);
        Task<DisbursmentLetterDTO> GetDisbursalLetterData(string Connectionstring, string vchapplicationId, string Voucherno);
    }
}
