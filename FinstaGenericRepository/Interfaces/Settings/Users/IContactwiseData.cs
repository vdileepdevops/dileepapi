using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings.Users;
using FinstaInfrastructure.Loans.Transactions;

namespace FinstaRepository.Interfaces.Settings.Users
{
   public interface IContactwiseData
    {
        Task<ContactDataDTO> GetContactData(string ContactRefID, string ConnectionString);
        Task<ContactDataDTO> GetContactDataDetails(string loaddataType, string ContactRefID, string ConnectionString);


    }
}
