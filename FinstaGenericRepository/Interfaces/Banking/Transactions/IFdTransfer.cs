using FinstaInfrastructure.Banking.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IFdTransfer
    {
       Task< List<FdschemeNameandCode>>  GetFdSchemes( string Connectionstring);
        List<FDDetailsDTO> GetFdToDetails(string Branchname,string Membercode, string Connectionstring);
        List<FDDetailsDTO> GetFdFromDetails( string Connectionstring);
        List<FDDetailsByIdDTO> GetFromFdDetailsByid(string FdAccountNo, string Connectionstring);
        List<FDDetailsByIdDTO> GetToFdDetailsByid(string FdAccountNo, string Connectionstring);
        bool SaveFdTransfer(Fdtransfersave _Fdtransfersave, string Connectionstring);

        List<FDMemberDetailsDTO> GetMemberDetails(string BranchName,string Connectionstring);


    }
}
