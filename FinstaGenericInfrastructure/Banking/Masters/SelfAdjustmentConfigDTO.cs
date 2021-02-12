using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
    public class SelfAdjustmentConfigDTO
    {
        public Int64 pRecordid { get; set; }
        public string pTransdate { get; set; }
        public string pCompnayname { get; set; }
        public string pBranchname { get; set; }
        public Int64 pFdaccountid { get; set; }
        public Int64 pMemberid { get; set; }
        public string pGroupcodeticketno { get; set; }
        public string pChitpersonname { get; set; }
        public string pPaymenttype { get; set; }
        public string pBankname { get; set; }
        public string pBankbranch { get; set; }
        public string pBankaccountno { get; set; }
        public string pIfsccode { get; set; }
        public string pMemberName { get; set; }
        public string pFdAccountno { get; set; }

    }
    public class SchemeTypeDTO
    {
        public object pSchemeid { get; set; }
        public object pSchemeName { get; set; }
    }
    public class MembersDTO
    {
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pMobileno { get; set; }
        public object pContactid { get; set; }
    }
    public class FdAccountDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountnumber { get; set; }
        public object pMembername { get; set; }

    }
    public class CompanyNamesDTO
    {
        public object pCompanyid { get; set; }
        public object pcompanyname { get; set; }

    }
    public class BranchNamesDTO
    {
        public object pCompanyid { get; set; }
        public object pBranchid { get; set; }
        public object pBranchname { get; set; }

    }
    public class SelfBankDetailsDTO
    {
        public object pBankname { get; set; }
        public object pAccountno { get; set; }
        public object pBranhname { get; set; }
        public object pIfsccode { get; set; }

    }
    public class ChitBranchDetails
    {
        public long pBranchId { get; set; }
        public object pBranchname { get; set; }
        public object pVchRegion { get; set; }
        public object pVchZone { get; set; }
    }
}
