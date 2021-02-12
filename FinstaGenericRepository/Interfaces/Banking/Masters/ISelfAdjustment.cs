using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface ISelfAdjustment
    {
        Task<List<CompanyNamesDTO>> GetCompanyname(string ConnectionString);
        Task<List<BranchNamesDTO>> GetBranchName(string Companyname, string ConnectionString);
        Task<List<SchemeTypeDTO>> GetSchemeType(string BranchName, string ConnectionString);
        Task<List<MembersDTO>> GetMembers(string branchname, Int64 fdconfigid, string ConnectionString);
        Task<List<FdAccountDTO>> GetFdAcnumbers(string branchname, int memberid, Int64 fdconfigid, string ConnectionString);
        Task<List<SelfBankDetailsDTO>> GetBankDetails(int Contactid, string ConnectionString);
        bool SaveSelforAdjustment(SelfAdjustmentConfigDTO Selfadjustment, string Connectionstring);
        Task<List<SelfAdjustmentConfigDTO>> ViewSelfAdjustmendtetails(string ConnectionString);
        Task<List<SelfAdjustmentConfigDTO>> GetSelfAdjustmendtetailsByid(int memberid, int fdid, string ConnectionString);
        Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring);
    }
}
