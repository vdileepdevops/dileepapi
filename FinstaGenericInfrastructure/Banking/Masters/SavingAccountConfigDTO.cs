using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{
   public class SavingAccountConfigDTO:CommonDTO
    {
        public Int64 pSavingAccountid { set; get; }
        public string pSavingAccountname { set; get; }
        public SavingAccountNameandCodeDTO SavingAccountNameandCodelist { set; get; }
        public List<SavingAccountConfigDetailsDTO> savingAccountConfiglist { set; get; }
        public List<DocumentlistDto> identificationdocumentsList { get; set; }
        public LoanFacilityDTO LoanFacilityList { get; set; }
        public ReferralCommissionDTO ReferralCommissionList { get; set; }
        public List<DocumentsMasterDTO> getidentificationdocumentsList { get; set; }

      
        
    }

    public class SavingAccountNameandCodeDTO:CommonDTO
    {
        public Int64 pSavingAccountid { set; get; }
        public string pSavingAccountname { set; get; }
        public string pSavingAccountcode { set; get; }
        public string pCompanycode { get; set; }
        public string pBranchcode { get; set; }
        public string pSeries { get; set; }
        public int pSerieslength { get; set; }
        public string pSavingaccnamecode { set; get; }
        public bool pStatus { set; get; }
    }
    public class SavingAccountConfigDetailsDTO:CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public Int64 pMembertypeid { set; get; }
        public string pMembertype { set; get; }
        public string pApplicanttype { set; get; }
        public Int64 pSavingConfigid { set; get; }
        public string pSavingAccname { set; get; }
        public decimal? pMinopenamount { get; set; }
        public decimal? pMinbalance { get; set; }
        public string pInterestpayout { set; get; }
        public decimal? pInterestrate { set; get; }
        public bool pIspenaltyapplicableonminbal { set; get; }
        public string pPenaltycaltype { set; get; }
        public decimal? pPenaltyvalue { set; get; }
        public bool pIssavingspayinapplicable { set; get; }
        public string pSavingspayinmode { set; get; }
        public decimal? pSavingmindepositamount { set; get; }
        public decimal? pSavingmaxdepositamount{set;get;}
        public bool pIswithdrawallimitapplicable { set; get; }
        public string pWithdrawallimitpayout { set; get; }
        public decimal? pMaxwithdrawllimit { set; get; }
        public string ptypeofoperation { set; get; }
    }
    public class LoanFacilityDTO:CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public Int64 pSavingConfigid { set; get; }
        public string pSavingAccname { set; get; }
        public bool pIsloanfacilityapplicable { set; get; }
        public decimal? pEligiblepercentage { set; get; }
        public decimal? pAgeperiod { set; get; }
        public string pAgeperiodtype { set; get; }

    }
    public class ReferralCommissionDTO:CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public Int64 pSavingConfigid { set; get; }
        public string pSavingAccname { set; get; }
        public string pReferralcommissioncalfield { set; get; }
        public string pReferralcommissiontype { set; get; }
        public decimal? pCommissionValue { set; get; }
        public bool pIsreferralcommissionapplicable { set; get; }
        public bool pIstdsapplicable { set; get; }
        public string pTdsaccountid { set; get; }
        public string ptdssection { set; get; }
        public decimal? pTdspercentage { set; get; }

    }
    public class IdentificationDocumentsDTO:CommonDTO
    {

        public Int64 pSavingConfigid { set; get; }
        public string pSavingAccname { set; get; }
        public string ptypeofoperation { set; get; }
        public List<DocumentlistDto> identificationdocumentsList { get; set; }
    }
    public class SavingschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
}
