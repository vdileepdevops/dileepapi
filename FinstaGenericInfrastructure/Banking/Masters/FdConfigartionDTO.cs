using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Masters;

namespace FinstaInfrastructure.Banking.Masters
{
   public class FdConfigartionDTO: CommonDTO
    {
        public FdNameAndCodeDTO FdNameAndCodeDTO { set; get; }
        public List<FDConfigartionDetailsDTO> lstFDConfigartionDetailsDTO { set; get; }
        public List<DepositCalculationTablesDTO> lstDepositCalculationTables { get; set; }
        public FDloanfacilityDetailsDTO FDloanfacilityDetailsDTO { set; get; }
        public FDReferralCommissionDTO FDReferralCommissionDTO { set; get; }
    }

    public class IdentificationDocumentsDto
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pFdconfigid { get; set; }
        public string pFdname { get; set; }
        public string pFdcode { get; set; }
        public string pFdnamecode { get; set; }

        public List<DocumentsMasterDTO> identificationdocumentsList { get; set; }
      //  public List<DocumentlistDto> identificationdocumentsList { get; set; }

    }
    //public class pIdentificationDocumentsDTO : CommonDTO

    //{
    //    public string pContactType { get; set; }
    //    public long pDocumentId { get; set; }
    //    public string pDocumentName { get; set; }
    //    public long pDocumentgroupId { set; get; }
    //    public long pDocumentGroupId { get; set; }
    //    public bool pDocumentMandatory { set; get; }
    //    public bool pDocumentRequired { set; get; }
    //    public long pLoantypeId { set; get; }
    //    public long pLoanId { set; get; }
    //    public string pDocumentGroup { get; set; }
    //}
    public class FdNameAndCodeDTO
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pFdconfigid { get; set; }
        public string pFdname { get; set; }
        public string pFdcode { get; set; }
        public string pCompanycode { get; set; }
        public string pBranchcode { get; set; }
        public string pSeries { get; set; }
        public Int64 pSerieslength { get; set; }
        public string pFdnamecode { get; set; }
        public bool pStatus { set; get; }
    }
    public class FdConfigDeatails
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pFdconfigid { get; set; }
        public string pFdname { get; set; }
        public string pFdcode { get; set; }
        public string pFdnamecode { get; set; }


        public List<FDConfigartionDetailsDTO> lstFDConfigartionDetailsDTO { set; get; }
      //  public List<DepositCalculationTablesDTO> lstDepositCalculationTables { get; set; }

    }
    public class FDConfigartionDetailsDTO
    {
        public Int64 precordid { set; get; }
        public Int64 pMembertypeid { get; set; }
        public Int64 pFdconfigid { get; set; }

        public string pMembertype { get; set; }
        public string pApplicanttype { get; set; }
        public string pFDcalucationmode { get; set; }
        public decimal? pMininstalmentamount { get; set; }
        public decimal? pMaxinstalmentamount { get; set; }
        public string pInstalmentpayin { get; set; }
        public string pInvestmentperiodfrom { get; set; }
        public string pInvestmentperiodto { get; set; }
        public string pInteresttype { get; set; }
        public string pInterestCompunding { get; set; }
        public string pInterestpayuot { get; set; }
        public decimal? pInterestratefrom { get; set; }
        public decimal? pInterestrateto { get; set; }
        public decimal? pValueper100 { get; set; }
        public int ptenure { set; get; }
        public string ptenuremode { set; get; }
        public decimal? pinterestamount { get; set; }
        public decimal? pdepositamount { get; set; }
        public decimal? pmaturityamount { get; set; }
        public decimal? pPayindenomination { get; set; }
        public string pTypeofOperation { set; get; }

        public Boolean pIsreferralcommissionapplicable { get; set; }
        public string pReferralcommissiontype { get; set; }
        public decimal? pCommissionValue { get; set; }
        public Boolean pIstdsapplicable { get; set; }
        public string pTdsaccountid { get; set; }
        public string pTdssection { get; set; }
        public decimal? pTdspercentage { get; set; }

        public decimal? pMultiplesofamount { get; set; }
        public decimal? pRatepersquareyard { get; set; }
        public string pCaltype { get; set; }

    }
    public class FDloanfacilityDetailsDTO
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pFdconfigid { get; set; }
        public string pFdname { get; set; }
        public string pFdcode { get; set; }
        public string pFdnamecode { get; set; }

        public Int64 precordid { set; get; }
        public Boolean pIsloanfacilityapplicable { get; set; }
        public decimal? pEligiblepercentage { get; set; }
      //  public Boolean pIsloanageperiod { get; set; }
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

        public List<FixedDepositPrematurityInterestPercentages> _FixedDepositPrematurityInterestPercentages { get; set; }
    }


    public class FixedDepositPrematurityInterestPercentages
    {
        public long pRecordid { get; set; }
        public decimal? pminprematuritypercentage { get; set; }
        public decimal? pmaxprematuritypercentage { get; set; }
        public string pprematurityperiodtype { get; set; }
        public string pTypeofOperation { set; get; }
        public decimal? pPercentage { get; set; }

    }


    public class FDReferralCommissionDTO
    {
        public Int64 pCreatedby { set; get; }
        public Int64 pFdconfigid { get; set; }
        public string pFdname { get; set; }
        public string pFdcode { get; set; }
        public string pFdnamecode { get; set; }

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
    public class FDViewDTO
    {
        public Int64 pFDconfigid { get; set; }
        public string pFDname { get; set; }
        public string pFdnamecode { get; set; }
        public bool pstatus { set; get; }
    }
    public class FdschemeandcodeCount
    {
        public int pSchemeCount { get; set; }
        public int pSchemeCodeCount { get; set; }
    }
}
