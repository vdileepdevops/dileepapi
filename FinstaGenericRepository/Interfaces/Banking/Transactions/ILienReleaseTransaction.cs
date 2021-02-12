using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
   public  interface ILienReleaseTransaction
    {
        List<LienRelaseBranchrDTO> GetBranches(string connectionstring);
        List<LienRelasegetmemberDTO> LienReleasemembercode(string Branchname, string connectionstring);

        Task<List<LienRelasegetmemberDTO>> Getlienreleasefd(string  Membercode,string BranchName, string connectionstring);

        Task<List<LienReleasememberfdDTO>>   GetLienrelasedata(string Membercode, string Fdraccountno, string LienDate, string connectionstring);

        bool SaveLienreleaseentry(LienreleaseSaveDTO _LienreleasesaveDTO, string connectionstring);

        bool DeleteLienreleaseentry(long Lienid, string connectionstring);

        LienReleaseviewDTO Lienreleaseviewdata(string connectionstring);




    }
}
