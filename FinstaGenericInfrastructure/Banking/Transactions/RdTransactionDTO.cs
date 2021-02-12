using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{

    public class RdTransactionDataEdit : RdMemberandSchemeSave
    {
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
    public class RDTransactionDTO : RDdetailsFromScheme
    {
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
    }
    public class RdMembersandContactDetails
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
    public class RdNameandCode
    {
        public object pRdname { get; set; }
        public object pRdcode { get; set; }
        public object pRdnameCode { get; set; }
        public object pRdCalculationmode { get; set; }
        public long pRdConfigId { get; set; }
        public long pRdDetailsRecordid { get; set; }
    }
    public class RdComfigurationIdandName
    {
        public long pRdAccountId { get; set; }
        public object pRdaccountNo { get; set; }
    }
    public class RDMemberNomineeDetails
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

    public class RdSchemeData
    {
        public object pRdCalculationmode { get; set; }
        public decimal? pMinInstallmentAmount { get; set; }
        public decimal? pMaxInstallmentAmount { get; set; }
        public object pInvestmentPeriodFrom { get; set; }
        public object pInvestmentPeriodTo { get; set; }
        public object pInterestPayOut { get; set; }
        public object pInterestType { get; set; }
        public decimal? pInterestRateFrom { get; set; }
        public decimal? pInterestRateTo { get; set; }
        public decimal? pMultiplesof { get; set; }
        public decimal? pValuefor100 { get; set; }
        public string pCaltype { get; set; }

        public List<RDTenureModes> pTenureModesList { get; set; }
        public object pRdtabletenure { get; set; }
        public object pRdtablematurityamount { get; set; }
        public object pRdtabledepositamount { get; set; }
        public object pRdtableinterestamount { get; set; }
        public object pRdtablepayindenomination { get; set; }
        public object pRdtabletenuremode { get; set; }
        public string pinstallmentpayin { get; set; }
    }
    public class RdConfigartionSchemesDTO
    {
        public long precordid { set; get; }
        public long pMembertypeid { get; set; }
        public object pMembertype { get; set; }
        public object pApplicanttype { get; set; }
        public object pRdcalucationmode { get; set; }
        public decimal? pMininstalmentamount { get; set; }
        public decimal? pMaxinstalmentamount { get; set; }
        public object pInstalmentpayin { get; set; }
        public object pInvestmentperiodfrom { get; set; }
        public object pInvestmentperiodto { get; set; }
        public object pInterestpayuot { get; set; }
        public object pInteresttype { get; set; }
        public object pCompoundInteresttype { get; set; }
        public decimal? pInterestratefrom { get; set; }
        public decimal? pInterestrateto { get; set; }
        public decimal? pValueper100 { get; set; }
        public decimal? pTenure { get; set; }
        public object pTenuremode { get; set; }
        public decimal? pPayindenomination { get; set; }
        public decimal? pInterestamount { get; set; }
        public decimal? pDepositamount { get; set; }
        public decimal? pMaturityamount { get; set; }
        public object pTypeofOperation { set; get; }

    }

    public class RdJointandNomineeSave : RdMembersandContactDetails
    {
        public object pAccountype { get; set; }
        public long pRdAccountId { get; set; }
        public object pRdaccountNo { get; set; }
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
        public List<RdMembersandContactDetails> RdMembersandContactDetailsList { get; set; }
        public List<RDMemberNomineeDetails> RDMemberNomineeDetailsList { get; set; }
        public long pCreatedby { get; set; }

        public bool pIsjointMembersapplicableorNot { get; set; }
        public bool pIsNomineesApplicableorNot { get; set; }
    }

    public class RdTransSaveDTO
    {

        public long pRdaccountid { set; get; }
        public object pRdaccountno { set; get; }
        public object pTransdate { set; get; }
        public long pMembertypeid { set; get; }
        public object pMembertype { set; get; }
        public long pMemberid { set; get; }
        public object pApplicanttype { set; get; }
        public object pMembercode { set; get; }
        public object pMembername { set; get; }
        public long pContactid { set; get; }
        public object pContacttype { set; get; }
        public object pContactreferenceid { set; get; }
        public long pRdconfigid { set; get; }
        public object pRdname { set; get; }
        public object pRdinstalmentpayin { set; get; }
        public long pTenor { set; get; }
        public decimal? pInstalmentamount { set; get; }
        public object pInteresttype { set; get; }
        public object pCompoundinteresttype { set; get; }
        public decimal? pInterestrate { set; get; }
        public decimal? pDepositamount { set; get; }
        public decimal? pMaturityamount { set; get; }
        public decimal? pInterestpayable { set; get; }
        public string pDeposidate { set; get; }
        public string pMaturitydate { set; get; }
        public bool pIsJointMembersapplicable { get; set; }
        public bool pIsinterestdepositinsaving { get; set; }
        public bool pIsautorenew { get; set; }
        public bool pRenewonlyprinciple { get; set; }
        public bool pRenewonlyprincipleinterest { get; set; }
        public bool pIsjointapplicable { get; set; }
        public bool pIsreferralapplicable { get; set; }
        public object pBondprintstatus { set; get; }
        public object pAccountstatus { set; get; }
        public object pTokenno { set; get; }
        public long pCreatedby { get; set; }

    }

    public class RdTransIdandName
    {
        public long pRdAccountId { get; set; }
        public object pRdaccountNo { get; set; }
    }
    public class RdTransactionReferrals
    {
        public long pRdAccountId { get; set; }
        public object pRdaccountNo { get; set; }
        public long pReferralId { get; set; }
        public object pReferralName { get; set; }
        public object pSalesPersonName { get; set; }
        public decimal? pCommisionValue { get; set; }
        public long pCreatedby { get; set; }
        public object pTypeofOperation { get; set; }
        public bool pIsReferralsapplicable { get; set; }
        public object pCommissionType { get; set; }
        public object pContactId { get; set; }
        public string pReferralCode { get; set; }
        public object pAdvocateName { get; set; }
    }

    public class RdMemberandSchemeSave : RDTransactionDTO
    {
        public string pTransDate { get; set; }
        public object pMemberName { get; set; }
        public object pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public object pContactrefid { get; set; }
        public object pContacttype { get; set; }
        public long pCreatedby { get; set; }
        public object pRdAccountNo { get; set; }
        public bool pBondPrintStatus { get; set; }
        public object pAccountStatus { get; set; }
        public object pTokenNo { get; set; }
        public bool pIsJointMembersapplicable { get; set; }
        public bool pIsReferralsapplicable { get; set; }
        public bool pIsNomineesapplicable { get; set; }
        public object pTypeofOperation { get; set; }
        public long pRdAccountId { get; set; }
        public long pChitbranchId { get; set; }
        public object pChitbranchname { get; set; }
        public object pTenortype { get; set; }
    }

    public class RDTenureModes
    {
        public object pTenurename { get; set; }
        public int pSortorder { get; set; }
        public object pTenurenature { get; set; }
        public object pPayinduration { get; set; }
    }
    public class RdTransactinTenuresofTable
    {
        public long pInterestTenure { get; set; }
    }

    public class RdTransactinInstallmentsAmountofTable
    {
        public decimal? pInstallmentAmount { get; set; }
    }

    public class RdTransactionMainGridData
    {
        public long pRdAccountId { get; set; }
        public object pRdaccountNo { get; set; }
        public long pMembertypeId { get; set; }
        public object pMemberType { get; set; }
        public object pMemberTypeCode { get; set; }
        public object pApplicantType { get; set; }
        public object pRdname { get; set; }
        public object pMemberName { get; set; }
        public decimal? pInstallmentAmount { get; set; }
        public decimal? pMaturityAmount { get; set; }
        public string pDepositDate { get; set; }
        public string pMaturityDate { get; set; }
        public object pReceiptStatus { get; set; }
        public object pTenure { get; set; }
        public object pInterestPayout { get; set; }
    }

    public class RdInterestRateValidation
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
    public class RDFiMemberContactDetails
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
    public class RdInterestPayout
    {
        public object pInterestPayOut { get; set; }
    }
    public class RDdetailsFromScheme : RdNameandCode
    {
        public List<RdInterestPayout> RdInterestPayoutList { get; set; }
        public decimal? pRdAmount { get; set; }
        public decimal? pMinInstallmentAmount { get; set; }
        public decimal? pMaxInstallmentAmount { get; set; }
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

        public decimal? pinstallmentAmount { get; set; }
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

    public class RDMatuerityamount
    {
        public decimal? pMatueritytAmount { get; set; }
        public object pInterestamount { get; set; }
    }

    public class RdTransactinInterestAmountofTable
    {
        public decimal? pInterestAmount { get; set; }
    }
    public class RdTransactinMaturityAmountofTable
    {
        public decimal? pMaturityAmount { get; set; }
        public decimal? pDepositamount { get; set; }
    }
    public class RDInstallmentchart
    {
        public object prdaccountno { get; set; }
        public object pinstalmentno { get; set; }
        public object pinstalmentdate { get; set; }
        public object pinstalmentamount { get; set; }
    }
}
