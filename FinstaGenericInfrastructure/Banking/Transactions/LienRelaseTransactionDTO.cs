using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class LienRelasegetmemberDTO
    {
        public object pMemberid { set; get; }
        public object pMembercode { set; get; }
        public object pMembername { set; get; }
        public object pContactnumber { set; get; }
        public object pFdaccountno { set; get;}

    }
    public class LienRelaseBranchrDTO
    {
        public object pBranchname { set; get; }
        public object pBranchcode { set; get; }
       

    }

    public class LienReleasememberfdDTO
    {
        public object pMembername { set; get; }
        public object pTenor { set; get; }
        public object pLienamount { set; get; }
        public object pCompanybranch { set; get; }       
        public object pLienadjuestto { set; get; }
        public object pDepositamount{ set; get; }
        public object pLienid { set; get; }
        public object pLiendate { set; get; }
        public object pLienrealsedate { set; get; }
        public object pFdaccountno { get; set; }
    }

    public class LienreleaseDTO
                
    {
        public object pLienid { set; get; }
        public object pLienrealsedate { set; get; }
        public object pStatusid { set; get; }
        public object ptypeofoperation { set; get; }
        public object pCreatedby { set; get; }

    }
    public class LienreleaseSaveDTO

    {
        public List<LienreleaseDTO> ListLienreleaseDTO { get; set; }

    }



    public class LienReleaseviewDTO
    {
      
        public List<LienReleasememberfdDTO> LienReleaselist { set; get; }



    }


}
