using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinstaRepository.Interfaces.Loans.Transactions
{
   public  interface IVerification
    {
       Task<bool> SaveVerficationDetails(TeleVerificationDTO TeleVerificationDTO,string ConnectonString);
        Task<TeleVerificationDTO> GetVerficationDetails(string strapplicatonid,string ConnectonString);
        Task<bool> SaveFieldverification(FieldverificationDTO FieldverificationDTO, string ConnectonString);
        Task<List<VerificationDetailsDTO>> GetAllApplicantVerificationDetails(string ConnectonString);
        Task<FieldverificationDTO> GetFieldVerificationDetails(string strapplicatonid, string ConnectonString);
        Task<bool> SaveFIverification(FIDocumentViewDTO FIDocumentViewDTO, string ConnectonString);
        Task<AddressconfirmedDTO> GetDetailsOfApplicant(string ContactRefID, string ConnectonString);

        Task<ApplicationLoanSpecificDTOinVerification> GetApplicantLoanSpecificDetailsinVerification(string strapplicatonid, string ConnectonString);
    }
}
