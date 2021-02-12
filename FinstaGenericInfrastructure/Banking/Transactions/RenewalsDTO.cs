using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class RenewalsDTO
    {
    }
    public class SchemeTypeDTO
    {
        public object pSchemeid { get; set; }
        public object pSchemeName { get; set; }
    }
    public class FDAccountsDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pMemberid { get; set; }

    }
    public class FDAccountDetailsDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pMemberid { get; set; }
        public object pContactreferenceid { get; set; }
        public object pFdname { get; set; }
        public object pTenor { get; set; }
        public object pTenortype { get; set; }
        public object pInterestrate { get; set; }
        public object pDepositamount { get; set; }
        public object pMaturityamount { get; set; }
        public object pDepositdate { get; set; }
        public object pMaturitydate { get; set; }

    }

}
