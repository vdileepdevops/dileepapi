using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Common
{
    public class TaxDetailsDTO : CommonDTO
    {
        public Boolean pIstdsApplicable { get; set; }
        public string ptdsSectionName { get; set; }
        public Boolean pIsgstApplicable { get; set; }
        public string pStateName { get; set; }
        public string pGstType { get; set; }
        public string pGstNo { get; set; }
    }
}
