using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{
    public class BankDetailsDTO : CommonDTO
    {
        // public string ptypeofoperation { get; set; }
        public string pBankAccountname { get; set; }
        public string pBankName { get; set; }
        public string pBankAccountNo { get; set; }
        public string pBankifscCode { get; set; }
        public string pBankBranch { get; set; }
        public Boolean pIsprimaryAccount { get; set; }
        public object pBankId { get; set; }
        public object precordid { set; get; }
        //  public string pTransactionType { get; set; } // For CRUD Operation Status
    }
}
