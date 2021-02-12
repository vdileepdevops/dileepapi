using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
   public  interface IMemberType
    {
        bool SaveMemberType(MemberTypeDTO MemberTypeDTO, string ConnectionString);
        bool UpdateMemberType(MemberTypeDTO MemberTypeDTO, string ConnectionString);
        bool DeleteMemberType(int MemberID, string ConnectionString);
        MemberschemeandcodeCount GetMemberNameCount(Int64 memberid, string MemberType, string MemberTypeCode, string ConnectionString);
        Task<List<MemberTypeDTO>> GetMemberTypeDetails(string ConnectionString);
        Task<List<MemberTypeDTO>> GetSavingMemberTypeDetails(string ConnectionString);
        Task<List<MemberTypeDTO>> GetShareMemberTypeDetails(string ConnectionString);
        Task<List<MemberTypeDTO>> BindMemberType(string ConnectionString);
        int GetMemberTypeCount(string MemberType, string ConnectionString);
        int GetMembercodeCount(string MemberTypeCode, string ConnectionString);
    }
}
