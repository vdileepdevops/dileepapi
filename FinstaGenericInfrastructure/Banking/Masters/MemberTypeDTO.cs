using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
   public class MemberTypeDTO
    {
        public Int64 pmembertypeid { set; get; }
        public string pmembertype { set; get; }
        public string pmembertypecode { set; get; }
        public string pcompanycode { set; get; }
        public string pbranchcode { set; get; }
        public string pseries { set; get; }
        public Int64 pserieslength { set; get; }
        public string pmembercode { set; get; }
        public bool pissharesissueapplicable { set; get; }

        public bool pisaccounttypecreationapplicable { set; get; }

        public bool pismembershipfeeapplicable { set; get; }
        public bool pstatus { set; get; }

        public Int64 pCreatedby { set; get; }
    }

    public class MemberschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
}
