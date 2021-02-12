using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class FdTransferDTO
    {
    }
    public class FdschemeNameandCode
    {
        public object pFdname { get; set; }
        public object pFdcode { get; set; }
        public object pFdnameCode { get; set; }
        public object pFdCalculationmode { get; set; }
        public long pFdConfigId { get; set; }        
    }
    public class Fdtransfersave
    {
        public long pFromMemberId { get; set; }
        public long pToMemberId { get; set; }
        public long pFromAccountId { get; set; }
        public long pToAccountId { get; set; }
        public string pTransferdate { get; set; }
        public decimal? pTransferamount { get; set; }
        public long pJvid { get; set; }
        public bool pStatus { get; set; }
    }
}
