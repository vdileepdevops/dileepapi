using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;
using System.Threading.Tasks;


namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IBondsPreview
    {
        Task<List<GetPreviewDetailsDTO>> GetBondsDetails(string ConnectionString);
        Task<BondspreviewMain> GetBondsPreviewDetails(string fdaccountnos, string ConnectionString);

        bool SaveBondsPrint(Bonds_PrintDTO ObjBondsprint, string Connectionstring);



    }
}
