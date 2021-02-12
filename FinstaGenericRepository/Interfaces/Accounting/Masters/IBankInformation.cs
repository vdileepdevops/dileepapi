using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Accounting;

namespace FinstaRepository.Interfaces.Accounting.Masters
{
    public interface IBankInformation
    {
        bool SaveBankInformation(BankInformationDTO lstBankInformation, string con);
        List<BankUPI> GetBankUPIDetails(string con);
        List<BankInformationDTO> GetBAnkDetails(string con);
        List<BankInformationDTO> GetBankNames(string con);
        //Int64 GenerateBookId(string con);
        bool SaveChequeManagement(BankInformationDTO lstChequemanagement, string con);
        List<ChequemanagementDTO> ViewChequeManagementDetails(string con);
        List<BankInformationDTO> ViewBankInformationDetails(string con);
        BankInformationDTO ViewBankInformation(long precordid, string con);
        bool DeleteBankInformation(BankInformationDTO lstChequemanagement, string con);
        long GetExistingChequeCount(string con, long bankId, long chqFromNo, long chqToNo);
       List<string> GetCheckDuplicateDebitCardNo(string con, BankInformationDTO lstBankInformation);
        long GetCheckDuplicateUPIId(string con, string uPIId);
    }
}
