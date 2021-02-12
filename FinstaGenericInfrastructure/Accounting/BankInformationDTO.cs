using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;
using FinstaInfrastructure.Loans.Masters;

namespace FinstaInfrastructure.Accounting
{
    public class BankInformationDTO : CommonDTO
    {      

        public List<BankInformationAddressDTO> lstBankInformationAddressDTO { get; set; }
        public List<BankdebitcarddtlsDTO> lstBankdebitcarddtlsDTO { get; set; }
        public List<BankUPI> lstBankUPI { get; set; }
        public List<ChequemanagementDTO> lstChequemanagementDTO { get; set; }
        public List<ChequesDTO> lstCheques { get; set; }
        public Int64 pRecordid { get; set; }
        public string pBankname { get; set; }
        public string pBankbranch { get; set; }
        public string pAccountname { get; set; }
        public string pAcctountype { get; set; }
        public string pAccountnumber { get; set; }
        public string pBankdate { get; set; }
        public Int64 pOverdraft { get; set; }
        public Int64 pAccountid { get; set; }
        public string pIfsccode { get; set; }
        public string pSwiftccode { get; set; }
        public Int64 pOpeningBalance { get; set; }
        public Boolean pIsdebitcardapplicable { get; set; }
        public Boolean pIsupiapplicable { get; set; }
        public string pOpeningBalanceType { get; set; }
        public string pChqegeneratestatus { get; set; }
        public string popeningjvno { get; set; }
    }
    public class BankInformationAddressDTO : ContactAddressDTO
    {
        public Int64 pRecordid { get; set; }
        public string pAddressType { get; set; }
        public long pBankId { get; set; }
    }
    public class ChequemanagementDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }    
        public Int64 pBankId { get; set; }
        public Int64 pNoofcheques { get; set; }
        public Int64 pChequefrom { get; set; }
        public Int64 pChequeto { get; set; }
        public Int64 pChqbookid { get; set; }
        public string pChqegeneratestatus { get; set; }
        public string pBankname { get; set; }
        public string pAccountnumber { get; set; }
    }
    public class ChequesDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public Int64 pChequenumber { get; set; }
        public Int64 pChqbookid { get; set; }
    }
    public class BankUPI : CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public string pUpiname { get; set; }
        public string pUpiid { get; set; }
        public long pBankId { get; set; }
    }
    public class BankdebitcarddtlsDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }
        public Int64 pCardNo { get; set; }
        public string pCardName { get; set; }
        public string pValidfrom { get; set; }
        public string pValidto { get; set; }
        public long pBankId { get; set; }
    }
}
