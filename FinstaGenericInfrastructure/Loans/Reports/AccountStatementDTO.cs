using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Reports
{
    public class AccountStatementDTO
    {
        public string pName { get; set; }
        public string pFathername { get; set; }
        public string pAddress { get; set; }
        public string pMobileno { get; set; }
        public string pEmailid { get; set; }
        public string pAadharno { get; set; }
        public string pPanNo { get; set; }

        public string pLoanno { get; set; }
        public decimal  pInstalmentamount { get; set; }
        public decimal  pLoanamount { get; set; }
        public string pFirstinstalmentdate { get; set; }
        public string pLoantype { get; set; }
        public string pLastinstalmentdate { get; set; }
        public string pTenure { get; set; }
        public string pLoanpayin { get; set; }
        public string pCurrentStatus { get; set; }
        public string pInterest { get; set; }
        public string pLoanclosedDate { get; set; }
        public string pDisbursedDate { get; set; }
        public List<Transactiondetails> pTransactionListDetails { get; set; }
        public List<documentstoreDTO> pDocumentstoredetails { get; set; }
        public long pApplicationid { get; set; }
        public string pVchapplicationid { get; set; }
        public string pLoanname { get; set; }
        public string pTitlename { get; set; }
        public string pContactImagePath { get; set; }
        public string ploaninstalmentpaymentmode { get; set; }
    }

    public class Transactiondetails
    {
        public string pTransDate { get; set; }
        public string pTransno { get; set; }
        public string pParticulars { get; set; }
        public string pDebitorCredit { get; set; }        
        public decimal pBalance { get; set; }
        public decimal pDisburseamount { get; set; }
        public decimal pDebitamount { get; set; }
        public decimal pCreditamount { get; set; }

        public string pBalanceCreditorDebit { get; set; }

    }
}
