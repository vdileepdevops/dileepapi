using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Loans.Reports;

namespace FinstaRepository.Interfaces.Loans.Reports
{
    public interface IDeliveryOrderLetter
    {
        Task<List<DeliveryOrderLetterDTO>> GetDeliveryOrderLetterMainData(string ConnectionString, string Letterstatus);

        bool Savedeliveryorderletter(DeliveryOrderLetterDTO __DeliveryOrderLetter, string Connectionstring);

        Task<DeliveryOrderLetterDTO> GetDeliveryOrderLetterData(string ConnectionString, string VchapplicationID);

        Task<List<DeliveryOrdersCount>> GetDeliveryOrdersCount(string ConnectionString);
    }
}
