using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.Interfaces.Settings
{
   public interface ICompany
    {
        bool SaveCompanyDetails(CompanyDTO CompanyDTO, string connectionstring);
        CompanyDTO getCompanyDetails(string ConnectionString);
    }
}
