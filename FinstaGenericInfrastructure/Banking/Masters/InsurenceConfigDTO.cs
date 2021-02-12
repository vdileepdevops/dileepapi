using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
    public class InsurenceConfigDTO : CommonDTO
    {
        public List<InsurenceConfigartionDetailsDTO> lstInsurenceConfigartionDetails { set; get; }
        public Int64 pInsurenceconfigid { get; set; }
        public string pInsurencename { get; set; }
        public string pInsurencenamecode { get; set; }
    }
    public class InsurenceNameAndCodeDTO : CommonDTO
    {
        public Int64 pInsurenceconfigid { get; set; }
        public string pInsurencename { get; set; }
        public string pInsurencecode { get; set; }
        public string pCompanycode { get; set; }
        public string pBranchcode { get; set; }
        public string pSeries { get; set; }
        public Int64 pSerieslength { get; set; }
        public string pInsurencenamecode { get; set; }
        public bool pStatus { set; get; }
    }

    public class InsuranceschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
    public class InsurenceConfigartionDetailsDTO
    {
        public Int64 precordid { set; get; }
        public Int64 pMembertypeid { get; set; }
        public string pMembertype { get; set; }
        public string pApplicanttype { get; set; }
        public Int64 pAgefrom { get; set; }
        public Int64 pAgeto { get; set; }
        public string pInsuranceduration { get; set; }
        public decimal? pPremiumamountpayable { get; set; }
        public string pPremiumpayin { get; set; }
        public string pInsuranceclaimpayoutevent { get; set; }
        public decimal? pInsuranceclaimamount { get; set; }
        public Boolean pPremiumrefund { get; set; }
        public string pTypeofOperation { get; set; }
    }
    public class insurenceReferralCommissionDTO:CommonDTO
    {
        public Int64 pInsurenceconfigid { get; set; }
        public string pInsurencename { get; set; }
        public string pInsurencenamecode { get; set; }
        public Int64 precordid { set; get; }
        public Boolean pIsreferralcommissionapplicable { get; set; }
        public string pReferralcommissiontype { get; set; }
        public decimal? pCommissionValue { get; set; }
        public Boolean pIstdsapplicable { get; set; }
        public string pTdsaccountid { get; set; }
        public string pTdssection { get; set; }
        public decimal? pTdspercentage { get; set; }
        public string pTypeofOperation { set; get; }
    }
    public class InsurenceConfigViewDTO
    {
        public Int64 pInsurenceconfigid { get; set; }
        public string pInsurencename { get; set; }
        public string pInsurencenamecode { get; set; }
        public bool pstatus { set; get; }
    }


}
