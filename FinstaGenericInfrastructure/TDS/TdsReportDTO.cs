using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.TDS
{
   public class TdsReportDTO
    {
        public object pParentId { get; set; }
        public object pParentAccountName { get; set; }
        public object pVoucherNo { get; set; }
        public object pAccountId { get; set; }
        public object pAccountName { get; set; }
        public object pPanNo { get; set; }
        public object pAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pActualTdsAmount { get; set; }
        public object pBalance { get; set; }
        public object pLoginId { get; set; }
        public object pTransDate { get; set; }
        public object pTdsSection { get; set; }
        public object pSectionName { get; set; }
        public object pPaidAmount { get; set; }
        public object pJvNo { get; set; }
    }
}
