using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{


    public class FdTransactionDataEdit : FdMemberandSchemeSave
    {
        public List<FdMembersandContactDetails> FdMembersandContactDetailsList { get; set; }
        public List<FDMemberNomineeDetails> FDMemberNomineeDetailsList { get; set; }


        public List<jointdetails> JointMembersandContactDetailsList { get; set; }
        public List<NomineeDetails> MemberNomineeDetailsList { get; set; }
        public Referrals MemberReferalDetails { get; set; }
        public object pReferralId { get; set; }
        public object pAdvocateName { get; set; }
        public object pSalesPersonName { get; set; }
        public decimal? pCommisionValue { get; set; }
        public object pCommissionType { get; set; }
        public object pReferralCode { get; set; }
        public object pReferralContactId { get; set; }
    }


    public class FdMemberandSchemeSave : FDTransactionDTO
    {
        public string pTransDate { get; set; }
        public object pMemberName { get; set; }
        public object pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public object pContactrefid { get; set; }
        public object pContacttype { get; set; }
        public long pCreatedby { get; set; }
        public object pFdAccountNo { get; set; }
        public bool pBondPrintStatus { get; set; }
        public object pAccountStatus { get; set; }
        public object pTokenNo { get; set; }
        public bool pIsJointMembersapplicable { get; set; }
        public bool pIsReferralsapplicable { get; set; }
        public bool pIsNomineesapplicable { get; set; }
        public object pTypeofOperation { get; set; }
        public long pFdAccountId { get; set; }
        public long pChitbranchId { get; set; }
        public object pChitbranchname { get; set; }

    }

    public class FdJointandNomineeSave : FdMembersandContactDetails
    {
        public long pFdAccountId { get; set; }
        public object pFdaccountNo { get; set; }
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
        public List<FdMembersandContactDetails> FdMembersandContactDetailsList { get; set; }
        public List<FDMemberNomineeDetails> FDMemberNomineeDetailsList { get; set; }
        public long pCreatedby { get; set; }

        public bool pIsjointMembersapplicableorNot { get; set; }
        public bool pIsNomineesApplicableorNot { get; set; }
    }


    public class FDTransactionDTO : FDdetailsFromScheme
    {
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
    }

    public class FdMembersandContactDetails
    {
        public object pMemberName { get; set; }
        public object pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public object pContactrefid { get; set; }
        public object pContacttype { get; set; }
        public object pTypeofOperation { get; set; }
        public long precordid { get; set; }
        public object pContactnumber { set; get; }

    }
    public class FDMemberNomineeDetails
    {
        public bool pisprimarynominee { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string pContacttype { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pnomineename { set; get; }
        public string prelationship { set; get; }
        public string pdateofbirth { set; get; }
        public string pcontactno { set; get; }
        public string pidprooftype { set; get; }
        public string pidproofname { set; get; }
        public string preferencenumber { set; get; }
        public string pdocidproofpath { set; get; }
        public string ptypeofoperation { set; get; }
        public decimal? pPercentage { get; set; }
        public string pMemberrefcode { get; set; }
        public bool pStatus { get; set; }
        public int pAge { get; set; }
    }
    public class FdNameandCode
    {
        public object pFdname { get; set; }
        public object pFdcode { get; set; }
        public object pFdnameCode { get; set; }
        public object pFdCalculationmode { get; set; }
        public long pFdConfigId { get; set; }
        public long pFdDetailsRecordid { get; set; }
    }
    public class FDdetailsFromScheme : FdNameandCode
    {
        public List<FdInterestPayout> FdInterestPayoutList { get; set; }
        public decimal? pFdAmount { get; set; }
        public decimal? pMinDepositAmount { get; set; }
        public decimal? pMaxdepositAmount { get; set; }
        public object pInvestmentPeriodFrom { get; set; }
        public object pInvestmentPeriodTo { get; set; }
        public object pInterestType { get; set; }
        public decimal? pInterestRateFrom { get; set; }
        public decimal? pInterestRateTo { get; set; }
        public decimal? pInterestOrValueForHundred { get; set; }
        public object pInterestTenureMode { get; set; }
        public long pInterestTenure { get; set; }
        public decimal? pInterestAmount { get; set; }
        public decimal? pDepositAmount { get; set; }
        public decimal? pMaturityAmount { get; set; }
        public decimal? pPayinDenomination { get; set; }
        public object pInterestPayOut { get; set; }
        public object pCompoundInterestType { get; set; }
        public decimal? pInterestRate { get; set; }
        public string pDepositDate { get; set; }
        public string pMaturityDate { get; set; }
        public bool pIsinterestDepositinSaving { get; set; }
        public bool pIsAutoRenew { get; set; }
        public bool pIsRenewOnlyPrinciple { get; set; }
        public bool pIsRenewOnlyPrincipleandInterest { get; set; }
        public object pValueORInterestratelabel { get; set; }
        public decimal? pMultiplesof { get; set; }
        public bool pIsinterestDepositinBank { get; set; }
        public object pReferralcommisiontype { get; set; }
        public decimal? pReferralCommsionvalue { get; set; }
        public decimal? pSquareyard { get; set; }
        public string pCaltype { get; set; }
    }
    public class FdInterestPayout
    {
        public object pInterestPayOut { get; set; }
    }
    public class FdComfigurationIdandName
    {
        public long pFdAccountId { get; set; }
        public object pFdaccountNo { get; set; }
    }
    public class FdTransactionReferrals
    {
        public long pFdAccountId { get; set; }
        public object pFdaccountNo { get; set; }
        public long pReferralId { get; set; }
        public object pAdvocateName { get; set; }
        public object pSalesPersonName { get; set; }
        public long pCreatedby { get; set; }
        public decimal? pCommisionValue { get; set; }
        public object pTypeofOperation { get; set; }
        public bool pIsReferralsapplicable { get; set; }
        public object pCommissionType { get; set; }
        public object pContactId { get; set; }
        public string pReferralCode { get; set; }

    }

    public class FdTransactionMainGridData
    {
        public long pFdAccountId { get; set; }
        public object pFdaccountNo { get; set; }
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
        public object pFdname { get; set; }
        public object pMemberName { get; set; }
        public decimal? pDepositAmount { get; set; }
        public decimal? pMaturityAmount { get; set; }
        public string pDepositDate { get; set; }
        public string pMaturityDate { get; set; }
        public object pReceiptStatus { get; set; }
        public object pChitbranchname { get; set; }
        public object pTenure { get; set; }
        public object pInterestPayout { get; set; }
    }

    public class FdTransactinTenuresofTable
    {
        public long pInterestTenure { get; set; }
    }
    public class FdTransactinDepositAmountofTable
    {
        public decimal? pDepositAmount { get; set; }
    }

    public class FdTransactinMaturityAmountofTable
    {
        public decimal? pMaturityAmount { get; set; }
    }

    public class FdTransactinInterestAmountofTable
    {
        public decimal? pInterestAmount { get; set; }
    }

   

    public class ChitBranchDetails
    {
        public long pBranchId { get; set; }
        public object pBranchname { get; set; }
        public object pVchRegion { get; set; }
        public object pVchZone { get; set; }
    }

    public class MaturityamountfromDetails
    {
        public object pInterestTenureMode { get; set; }
        public long pInterestTenure { get; set; }
        public decimal? pDepositAmount { get; set; }
        public object pInterestPayOut { get; set; }
        public object pCompoundInterestType { get; set; }
        public decimal? pInterestRate { get; set; }
    }
    public class Matuerityamount
    {
        public decimal? pMatueritytAmount { get; set; }
        public object pInterestamount { get; set; }
    }


    public class FdSchemeData
    {
        public object pFdCalculationmode { get; set; }
        public decimal? pMinDepositAmount { get; set; }
        public decimal? pMaxdepositAmount { get; set; }
        public object pInvestmentPeriodFrom { get; set; }
        public object pInvestmentPeriodTo { get; set; }
        public object pInterestPayOut { get; set; }
        public object pInterestType { get; set; }
        public decimal? pInterestRateFrom { get; set; }
        public decimal? pInterestRateTo { get; set; }
        public decimal? pMultiplesof { get; set; }
        public decimal? pValuefor100 { get; set; }
        public string pCaltype { get; set; }

        public List<TenureModes> pTenureModesList { get; set; }

        public object pFdtabletenure { get; set; }
        public object pFdtablematurityamount { get; set; }
        public object pFdtabledepositamount { get; set; }
        public object pFdtableinterestamount { get; set; }
        public object pFdtablepayindenomination { get; set; }
        public object pFdtabletenuremode { get; set; }
    }

    public class TenureModes
    {
        public object pTenurename { get; set; }
        public int pSortorder { get; set; }
    }

    public class FdInterestRateValidation
    {
        public int pTenureCount { get; set; }
        public decimal? pMinInterestRate { get; set; }
        public decimal? pMaxInterestRate { get; set; }
        public object pReferralcommisiontype { get; set; }
        public decimal? pReferralCommisionvalue { get; set; }
        public decimal? pRatePerSquareYard { get; set; }
        public decimal? pValuefor100 { get; set; }
        public string pCaltype { get; set; }
    }

    public class FiMemberContactDetails
    {
        public string pContacttype { get; set; }
        public string pContactName { get; set; }
        public string pMemberType { get; set; }
        public long pMembertypeId { get; set; }
        public string pMemberStatus { get; set; }
        public string pContactReferenceId { get; set; }
        public long pContactId { get; set; }
        public string pMemberReferenceId { get; set; }
        public string pEmailId { get; set; }
        public string pContactNo { get; set; }
        public long pMemberId { get; set; }
        public string pApplicantType { get; set; }
        public string pFIMemberDate { get; set; }
        public bool pStatus { get; set; }
        public bool pIsreferencesapplicable { get; set; }
        public string ptypeofoperation { get; set; }
    }
}

