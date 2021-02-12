using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
   public  class LienEntryDTO
    {
        public long pLienid { set; get; }
        public string pLiendate { set; get; }
        public string  pMembercode { set; get; }
        public string  pFdaccountno { set; get; }

        public decimal pLienamount { set; get; }

        public decimal pLiencount { set; get; }
        public string  pCompanyname { set; get; }
        public string  pCompanybranch { set; get; }
        public string  pLienadjuestto { set; get; }

        public string pReceipttype { set; get; }
        public string pLienstatus { set; get; }
        public bool pstatus { set; get; }

        public Int64 pCreatedby { set; get; }

        public string ptypeofoperation { set; get; }
               
    }

    public class GetfdraccountnoDTO
    {

        public string pFdaccountno { set; get; }
        public long pFdacctid { set; get; }
        public string pMembername { set; get; }
        public long pMemberid { set; get; }

    }

    public class GetmemberfddetailsDTO
    { 
        public object pMembername { set; get; }
        public object pDepositamount { set; get; } 

        public object pTenor { set; get; }

        public object pTransdate { set; get; }

        public object pFdaccountno { set; get; }
        public object pMemberid { set; get; }
        public object pTenortype { set; get; }
        public object pBalance { set; get; }
    }
    public class LienEntryDetailsForEdit
    {
        public object pLienid { set; get; }
        public object pLienDate{ set; get; }
        public object pMemberId { set; get; }
        public object pMemberCode { set; get; }
        public object pFdAccountid { set; get; }
        public object pFdaccountno { set; get; }
        public object pLienamount { set; get; }
        public object pTobranch { set; get; }
        public object pBranchname { set; get; }
        public object pLienadjuestto { set; get; }
    }

    public class LienEntryViewDTO
    {
        public long pLienid { set; get; }
        public string pLiendate { set; get; }
        public string pMembercode { set; get; }
        public string pFdaccountno { set; get; }

        public decimal pLienamount { set; get; }

        public decimal pLiencount { set; get; }
        public string pCompanyname { set; get; }
        public string pCompanybranch { set; get; }
        public string pLienadjuestto { set; get; }

        public string pReceipttype { set; get; }
        public string pLienstatus { set; get; }
        public bool pstatus { set; get; }
        public string pMembername { set; get; }
        public string pDepositdate { set; get; }
        public decimal pDepositamount { set; get; }

        public Int64 pCreatedby { set; get; }

        public string ptypeofoperation { set; get; }

    }


}
