using System;
using System.Collections.Generic;
using FinstaInfrastructure.Accounting;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Accounting.Masters
{
   public interface IAccountMaster
    {
        Task<List<AccountsTreeDTO>> GetAccountTree(string ConnectionString);
        Task<List<AccountsTreeDTO>> AccountTreeSearch(string ConnectionString, string searchterm);
        List<AccountTreeDTO> GetAccountTreeDetails(string ConnectionString);
        bool SaveAccountMaster(AccountCreationDTO accountcreate, string connectionstring);
        int checkAccountnameDuplicates(string Accountname,string AccountType,int Parentid, string connectionstring);
        Task<List<AccountTreeNewDTO>> GetAccountTreeNewDetails(string ConnectionString);
        List<TdsSectionNewDTO> getTdsSectionNo(string ConnectionString);
        bool SaveAccountHeads(AccountCreationNewDTO accountcreate, string connectionstring);
    }
}
