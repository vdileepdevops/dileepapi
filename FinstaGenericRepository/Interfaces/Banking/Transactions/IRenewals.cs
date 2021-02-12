using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;


namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IRenewals
    {
        Task<List<SchemeTypeDTO>> GetSchemeType(string ConnectionString);
        Task<List<FDAccountsDTO>> GetFdaccounts(string ConnectionString);
        Task<List<FDAccountDetailsDTO>> GetFdAccountDetails(int fdid, string ConnectionString);
    }
}
