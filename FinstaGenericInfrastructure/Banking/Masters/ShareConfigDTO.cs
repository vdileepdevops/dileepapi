using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
   public class ShareConfigDTO
    {
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public string psharecode { set; get; }
        public string pcompanycode { set; get; }
        public string pbranchcode { set; get; }
        public string pseries { set; get; }
        public Int64 pserieslength { set; get; }
        public string psharenamecode { set; get; }
        public Int64 pCreatedby { set; get; }
        
      
    }
    public class ShareConfigDetails
    {
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public string psharecode { set; get; }
        public Int64 pCreatedby { set; get; }
        public List<ShareconfigDetailsDTO> lstShareconfigDetailsDTO { set; get; }

    }
    public class ShareconfigDetailsDTO
    {
        public Int64 precordid { set; get; }
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public Int64 pmembertypeid { set; get; }
        public string pmembertype { set; get; }
        public string papplicanttype { set; get; }
        public decimal? pfacevalue { set; get; }
        public Int64 pminshares { set; get; }
        public Int64 pmaxshares { set; get; }
        public bool pismultipleshares { set; get; }
        public Int64 pmultipleshares { set; get; }
        public bool pisdivedend { set; get; }
        public string pdivedendpayout { set; get; }
        public bool psharetransfer { set; get; }
        public string pTypeofOperation { set; get; }
    }
    public class ShareconfigReferralDTO
    {
        public Int64 precordid { set; get; }
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public string psharecode { set; get; }
        public bool pisreferralcommissionapplicable { set; get; }
        public string preferralcommissiontype { set; get; }
        public decimal? pcommissionValue { set; get; }
        public bool pistdsapplicable { set; get; }
        public string ptdsaccountid { set; get; }
        public string ptdssection { set; get; }
        public Int64 ptdspercentage { set; get; }
        public Int64 pCreatedby { set; get; }
    }

    public class ShareviewDTO
    {
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public string psharecode { set; get; }
        public string psharenamecode { set; get; }
        public string pstatus { set; get; }
    }

    public class ShareschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
}
