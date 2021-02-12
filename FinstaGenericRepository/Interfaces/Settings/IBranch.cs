using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.Interfaces.Settings
{
    public interface IBranch
    {
        bool SaveBranchDetails(BranchDTO BranchDTO, string connectionstring);
        BranchDTO getBranchDetails(string ConnectionString);
        string checkbranchnameDuplicates(string branchname, string branchcode, int branchid, string connectionstring);
    }
}
