using FinstaInfrastructure.Banking.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Banking.Masters
{
    public interface ILienEntry
    {

        bool SaveLienentry(LienEntryDTO _lienentryDTO, string connectionstring);
        
        Task<List<GetfdraccountnoDTO>> Getfddetails(long Memberid, string chitbranchname, string type, string connectionstring);
        Task<List<FiMemberContactDetails>> GetMemberDetails(string chitbranchname, string connectionstring);

        GetmemberfddetailsDTO Getmemberfddetails(long Memberid, string Fdraccountno, string connectionstring);

        List<LienEntryViewDTO> Lienviewdata(string connectionstring);
        List<LienEntryDTO> GetLiendata(string Fdraccountno,string connectionstring);
        bool DeleteLienentry(long  Lienid, string connectionstring);

        LienEntryDetailsForEdit GetLienentryforEdit(long lienid,string connectionstring);
        Task<List<ChitBranchDetails>> GetFDBranchDetails(string Connectionstring);




    }
}
