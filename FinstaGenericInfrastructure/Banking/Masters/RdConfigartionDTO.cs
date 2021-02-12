using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Masters;

namespace FinstaInfrastructure.Banking.Masters
{
    public class RdConfigartionDTO : CommonDTO
    {
        public List<RdConfigartionDetailsDTO> lstRdConfigartionDetails { get; set; }
        public List<DepositCalculationTablesDTO> lstDepositCalculationTables { get; set; }
        public RdloanfacilityDetailsDTO RdloanfacilityDetailsDTO { set; get; }
        public RdReferralCommissionDTO RdReferralCommissionDTO { set; get; }

    }
    public class IdentificationDocumentsDt
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdcode { get; set; }
        public string pRdnamecode { get; set; }

        public List<DocumentsMasterDTO> identificationdocumentsList { get; set; }
        //  public List<DocumentlistDto> identificationdocumentsList { get; set; }

    }
    public class RdNameAndCodeDTO : CommonDTO
    {
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdcode { get; set; }
        public string pCompanycode { get; set; }
        public string pBranchcode { get; set; }
        public string pSeries { get; set; }
        public Int64 pSerieslength { get; set; }
        public string pRdnamecode { get; set; }
        public Int64 pDuplicatecount { get; set; }
    }
    public class RdConfigartionDetails : CommonDTO
    {
        public List<RdConfigartionDetailsDTO> lstRdConfigartionDetails { get; set; }
        //public List<DepositCalculationTablesDTO> lstDepositCalculationTables { get; set; }
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdnamecode { get; set; }

    }
    public class RdConfigartionDetailsDTO 
    {
        public Int64 precordid { set; get; }
        public Int64 pMembertypeid { get; set; }
        public string pMembertype { get; set; }
        public string pApplicanttype { get; set; }
        public string pRdcalucationmode { get; set; }
        public decimal? pMininstalmentamount { get; set; }
        public decimal? pMaxinstalmentamount { get; set; }
        public string pInstalmentpayin { get; set; }
        public string pInvestmentperiodfrom { get; set; }
        public string pInvestmentperiodto { get; set; }
        public string pInterestpayuot { get; set; }
        public string pInteresttype { get; set; }
        public string pCompoundInteresttype { get; set; }
        public decimal? pInterestratefrom { get; set; }
        public decimal? pInterestrateto { get; set; }
        public decimal? pValueper100 { get; set; }
        public decimal? pTenure { get; set; }
        public string pTenuremode { get; set; }
        public decimal? pPayindenomination { get; set; }
        public Boolean pIsreferralcommissionapplicable { get; set; }
        public string pReferralcommissiontype { get; set; }
        public decimal? pCommissionValue { get; set; }
        public Boolean pIstdsapplicable { get; set; }
        public string pTdsaccountid { get; set; }
        public string pTdssection { get; set; }
        public decimal? pTdspercentage { get; set; }

        public decimal? pMultiplesofamount { get; set; }
        public decimal? pInterestamount { get; set; }
        public decimal? pDepositamount { get; set; }
        public decimal? pMaturityamount { get; set; }
        public string pTypeofOperation { set; get; }

    }
    public class DepositCalculationTablesDTO
    {
        public Int64 precordid { set; get; }
        public string pDeposittype { get; set; }
        public Int64 pDepositconfigid { get; set; }
        public string pDepositname { get; set; }
        public Int64 pMembertypeid { get; set; }
        public string pMembertype { get; set; }
        public string pApplicanttype { get; set; }
        public decimal? pTenure { get; set; }
        public string pTenuremode { get; set; }
        public decimal? pPayindenomination { get; set; }
        public string pInterestcaltype { get; set; }
        public decimal? pInterestamount { get; set; }
        public decimal? pDepositamount { get; set; }
        public decimal? pMaturityamount { get; set; }
        public string pTypeofOperation { set; get; }

    }
    public class RdloanfacilityDetailsDTO : CommonDTO
    {
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdnamecode { get; set; }
        public Int64 precordid { set; get; }
        public Boolean pIsloanfacilityapplicable { get; set; }
        public decimal? pEligiblepercentage { get; set; }
        public Boolean pIsloanageperiod { get; set; }
        public decimal? pAgeperiod { get; set; }
        public string pAgeperiodtype { get; set; }
        public Boolean pIsprematuretylockingperiod { get; set; }
        public decimal? pPrematuretyageperiod { get; set; }
        public string pPrematuretyageperiodtype { get; set; }
        public Boolean pIslatefeepayble { get; set; }
        public string pLatefeepaybletype { get; set; }
        public decimal? pLatefeepayblevalue { get; set; }
        public Int64 pLatefeeapplicablefrom { get; set; }
        public string pLatefeeapplicabletype { get; set; }
        public string pTypeofOperation { set; get; }
        public List<RecurringDepositPrematurityInterestPercentages> _RecurringDepositPrematurityInterestPercentages { get; set; }
    }
    public class RecurringDepositPrematurityInterestPercentages
    {
        public long pRecordid { get; set; }
        public decimal? pminprematuritypercentage { get; set; }
        public decimal? pmaxprematuritypercentage { get; set; }
        public string pprematurityperiodtype { get; set; }
        public string pTypeofOperation { set; get; }
        public decimal? pPercentage { get; set; }

    }
    public class RdReferralCommissionDTO : CommonDTO
    {
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdnamecode { get; set; }
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
    public class RdViewDTO
    {
        public Int64 pRdconfigid { get; set; }
        public string pRdname { get; set; }
        public string pRdnamecode { get; set; }
        public bool pstatus { set; get; }
    }


    public class RdschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
}
