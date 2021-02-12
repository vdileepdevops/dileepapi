using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
   public interface IShareConfig
    {
        //bool SaveShareConfig(ShareConfigDTO ShareConfigDTO, string ConnectionString);
        //ShareConfigDTO GetShareConfigurationDetails(Int64 ShareconfigID,string ShareName, string ConnectionString);
        Task<List<ShareviewDTO>> GetShareview(string ConnectionString);
        bool DeleteShareConfiguration(Int64 ShareconfigID, string ShareName, string ConnectionString);
        ShareschemeandcodeCount GetShareNameCodeCount(Int64 shareid, string ShareName, string ShareCode, string ConnectionString);
        int GetShareNameCount(string ShareName, string ConnectionString);
        int GetShareCodeCount(string ShareCode, string ConnectionString);
        bool SaveShareNameANdcode(ShareConfigDTO ShareConfigDTO, string ConnectionString);
        Task<ShareConfigDTO> GetShareNameANdcode(string ShareName,string ShareCode, string ConnectionString);
        bool SaveShareConfigDetails(ShareConfigDetails ShareConfigDetails, string ConnectionString);
        Task<ShareConfigDetails> GetShareConfigDetails(string ShareName, string ShareCode, string ConnectionString);
        bool SaveShareConfigReferral(ShareconfigReferralDTO ShareconfigReferralDTO, string ConnectionString);
        Task<ShareconfigReferralDTO> GetShareConfigReferralDetails(string ShareName, string ShareCode, string ConnectionString);
    }
}
