using FinstaInfrastructure.Common;
using FinstaInfrastructure.Loans.Masters;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Transactions
{
    public class ApplicationDTO : CommonDTO
    {
        public FirstinformationDTO FirstinformationDTO { get; set; }
        public ApplicationLoanDetailsDTO ApplicationLoanDetailsDTO { get; set; }
        public ApplicationApplicantandOthersDTO ApplicationApplicantandOthersDTO { get; set; }
        public ApplicationKYCDocumentsDTO ApplicationKYCDocumentsDTO { get; set; }
        public ApplicationPersonalInformationDTO ApplicationPersonal { get; set; }
        public ApplicationSecurityandCollateralDTO ApplicationSecurityandCollateralDTO { get; set; }
        public ApplicationExistingLoansDTO ApplicationExistingLoansDTO { get; set; }
        public ApplicationLoanSpecificDTO ApplicationLoanSpecificDTO { get; set; }
        public ApplicationLoanReferencesDTO ApplicationLoanReferencesDTO { get; set; }
        public ApplicationLoanSpecificReferralDTO ApplicationLoanSpecificReferralDTO { get; set; }
        public ApplicationExistingLoanDetailsDTO ApplicationExistingLoanDetailsDTO { get; set; }

    }
    public partial class FIDocumentViewDTO : CommonDTO
    {
        public string pverificationdate { set; get; }
        public string pverificationtime { set; get; }
        public List<contactPersonalAddressDTO> lstcontactPersonalAddressDTO { set; get; }
        public List<ApplicationContactPersonalDetailsDTO> lstApplicationContactPersonalDetailsDTO { set; get; }
        public List<FirstinformationDTO> FirstinformationDTO { get; set; }
        public ApplicationKYCDocumentsDTO ApplicationKYCDocumentsDTO { get; set; }
        public ApplicationPersonalInformationDTO ApplicationPersonal { get; set; }
        public ApplicationSecurityandCollateralDTO ApplicationSecurityandCollateralDTO { get; set; }
        public List<ApplicationExistingLoanDetailsDTO> ApplicationExistingLoanDetailsDTO { get; set; }
        public ApplicationReferencesDTO ApplicationReferencesDTO { get; set; }
        public ApplicationLoanSpecificDTOinVerification _ApplicationLoanSpecificDTOinVerification { get; set; }
        public string pVerificationstatus { set; get; }
        public string pFIVerifierscomments { set; get; }
        public string pFIVerifiersrating { set; get; }
    }
    public class contactPersonalAddressDTO

    {
        private string name = "Personal Address Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string psubsectionname { set; get; }

        public string pVerificationoperation { set; get; }
        public Int64 pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string IsVerified { set; get; }
        public long pRecordId { set; get; }
        public string pContactType { get; set; }
        public string pAddressType { get; set; }
        public string pAddress1 { get; set; }
        public string pAddress2 { get; set; }
        public string papplicanttype { set; get; }
        public string pState { get; set; }
        public Int64 pStateId { get; set; }
        public string pDistrict { get; set; }
        public Int64 pDistrictId { get; set; }
        public string pCity { get; set; }
        public int pCityId { get; set; }
        public string pCountry { get; set; }
        public Int64 pCountryId { get; set; }
        public Int64 pPinCode { get; set; }
        public string pPriority { get; set; }
        public string ptypeofoperation { get; set; }
        public string pAddressDetails { get; set; }
    }
    public class FirstinformationDTO : CommonDTO
    {
        public string pstatementstatus;
        private string name = "Loan Application Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string psubsectionname { set; get; }

        public string pVerificationoperation { set; get; }

        public string IsVerified { set; get; }
        public List<CreditScoreDetailsDTO> lstCreditscoreDetailsDTO { get; set; }
        public List<KYCDcumentsDetailsDTO> lstKYCDcumentsDetailsDTO { get; set; }
        public List<SurityApplicantsDTO> lstsurityapplicantsDTO { get; set; }
        public List<InstalmentsgenerationDTO> lstInstalmentsgenerationDTO { get; set; }

        public Int64 papplicationid { set; get; }
        public string pVchapplicationid { get; set; }
        public string pDateofapplication { get; set; }
        public string pDateofDisbursement { set; get; }
        public Int64 pApplicantid { get; set; }
        public string pContactreferenceid { get; set; }
        public string pApplicantname { get; set; }
        public string pContacttype { get; set; }
        public Int64 pLoantypeid { get; set; }
        public string pLoantype { get; set; }
        public Int64 pLoanid { get; set; }
        public string pLoanname { get; set; }
        public string pApplicanttype { get; set; }
        public Boolean pIsschemesapplicable { get; set; }
        public string pSchemename { get; set; }
        public string pSchemecode { get; set; }
        public decimal? pAmountrequested { get; set; }
        public string pPurposeofloan { get; set; }
        public string pLoanpayin { get; set; }
        public string pInteresttype { get; set; }
        public decimal? pRateofinterest { get; set; }
        public Int64 pTenureofloan { get; set; }
        public string pTenuretype { get; set; }
        public string pLoaninstalmentpaymentmode { get; set; }

        public int pPartprinciplepaidinterval { get; set; }
        public decimal? pInstalmentamount { get; set; }
        public string pVchapplicantstatus { get; set; }
        public Boolean pIssurietypersonsapplicable { get; set; }
        public Boolean pIsKYCapplicable { get; set; }
        public Boolean pIspersonaldetailsapplicable { get; set; }
        public Boolean pIssecurityandcolletralapplicable { get; set; }
        public Boolean pIsexistingloansapplicable { get; set; }
        public Boolean pIsreferencesapplicable { get; set; }
        public Boolean pIsreferralapplicable { get; set; }
        public string pReferralname { get; set; }
        public string preferralcontactrefid { get; set; }

        public string pSalespersonname { get; set; }
        public string psalespersoncontactrefid { get; set; }
        public int pschemeid { get; set; }
        public string pFIVerifierscomments { set; get; }
        public string pFIVerifiersrating { set; get; }
        public string pverificationdate { set; get; }
        public string pverificationtime { set; get; }
        public string pVerifierName { set; get; }
        public string papprovaldate { set; get; }
        public string pfirstdisbursementdate { set; get; }
        public string ploancloseddate { set; get; }
        public string pstatusname { set; get; }
        public string pNextEmiDate { set; get; }
        public decimal pOverdueAmount { set; get; }
        public string plastdisbursementdate { get; set; }
        public decimal ptotaldisburseamount { get; set; }
        public string ploanstartddate { get; set; }

        public decimal pinstalmentprinciple { get; set; }
        public decimal pinstalmentinterest { get; set; }
        public decimal pactualpenalty { get; set; }
        public decimal ppaidprinciple { get; set; }
        public decimal ppaidinterest { get; set; }
        public decimal ppaidpenalty { get; set; }
        public decimal pwaiveofpenalty { get; set; }
        public decimal pprincipledue { get; set; }
        public decimal pinterestdue { get; set; }
        public decimal ppenaltydue { get; set; }
        public decimal pfutureprincipledue { get; set; }
        public decimal pfutureinterestdue { get; set; }
        public decimal ptotaldueamount { get; set; }
        public decimal ptotalpaidamount { get; set; }
    }
    public class GetLoandetailsDTO
    {
        public int pschemeid { get; set; }
        public string pSchemename { get; set; }
        public string pLoanpayin { get; set; }
        public string pSchemecode { get; set; }
        public string pInteresttype { get; set; }
        public decimal? pRateofinterest { get; set; }
        public decimal? pMinloanamount { get; set; }
        public decimal? pMaxloanamount { get; set; }
        public decimal? pTenorfrom { get; set; }
        public decimal? pTenorto { get; set; }
        public string pLoaninstalmentpaymentmode { get; set; }
        public string pLoaninstalmentpaymentmodecode { get; set; }
    }
    public class AcknowledgementDTO
    {
        public Int64 pLoanid { set; get; }
        public string pLoanname { set; get; }
        public Int64 pApplicantid { set; get; }
        public string pVchapplicationid { set; get; }
        public string pApplicantname { set; get; }
        public decimal? pLoanamount { set; get; }
        public string pMobileno { set; get; }
        public string pEmail { set; get; }
        public string pSentto { get; set; }
        public string pSentstatus { get; set; }
        public DateTime pTransDate { get; set; }
        public string pApplicationdate { get; set; }
        public string psentthrough { get; set; }
        public Int64 pCreatedby { set; get; }

        public string pInteresttype { get; set; }
        public decimal pInterestRate { get; set; }
        public long pTenureofloan { get; set; }
        public string pTenuretype { get; set; }
        public string pTitlename { get; set; }

    }

    public class ApplicationApplicantandOthersDTO : CommonDTO
    {
        public Int64 pApplicationid { get; set; }
        public string pvchapplicationid { get; set; }
        public Boolean pissurietypersonsapplicable { get; set; }
        public List<SurityApplicantsDTO> lstsurityapplicantsDTO { get; set; }

    }
    public class SurityApplicantsDTO : CommonDTO
    {
        public Int64 pRecordid { get; set; }

        public string pContactreferenceid { get; set; }
        public Int64 pContactid { get; set; }
        public string pSurityapplicanttype { get; set; }
        public string pContactname { get; set; }
        public string psuritycontactno { get; set; }

    }
    public class ApplicationKYCDocumentsDTO : CommonDTO
    {
        public long pApplicationid { set; get; }
        public string pVchapplicationid { set; get; }
        public Boolean pisKYCapplicable { get; set; }

        public List<CreditScoreDetailsDTO> lstCreditscoreDetailsDTO { get; set; }
        public List<documentstoreDTO> documentstorelist { get; set; }
    }
    public class CreditScoreDetailsDTO : CommonDTO
    {
        private string name = "Credit Score Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string pVerificationoperation { set; get; }
        public string psubsectionname { set; get; }

        public string IsVerified { set; get; }
        public Int64 pRecordid { get; set; }
        public Int64 pContactid { get; set; }
        public string pContactreferenceid { get; set; }
        public string pApplicanttype { get; set; }
        public Boolean pIscreditscoreapplicable { get; set; }
        public string pCiccompany { get; set; }
        public Int64 pCreditscore { get; set; }
        public Int64 pMaxcreditscore { get; set; }
        public string pCreditscorefilepath { get; set; }
        public string pFilename { get; set; }
        public Boolean pisapplicable { get; set; }
        public string pApplicantname { get; set; }

    }

    public class KYCDcumentsDetailsDTO : CommonDTO
    {
        //public Int64 pRecordid { get; set; }
        //public Int64 pContactid { get; set; }
        //public string pContactreferenceid { get; set; }
        //public string pApplicantype { get; set; }
        //public Boolean pIskycdocapplicable { get; set; }
        //public string pIdprooftype { get; set; }
        //public string pIdproofname { get; set; }
        //public string pIdreferencenumber { get; set; }
        //public string pProoffilepath { get; set; }
        //public string pUploadfilename { get; set; }




    }
    public class ApplicationLoanDetailsDTO
    {

    }

    #region SecurityandCollateral
    public class ApplicationSecurityandCollateralDTO : CommonDTO
    {
        private string name = "Security and Collateral";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string IsVerified { set; get; }
        public string psubsectionname { set; get; }
        public long pApplicationid { set; get; }
        public string pVchapplicationid { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }

        public Boolean pissecurityandcolletralapplicable { get; set; }


        public Boolean pIsimmovablepropertyapplicable { get; set; }
        public Boolean pismovablepropertyapplicable { get; set; }
        public Boolean pissecuritychequesapplicable { get; set; }
        public Boolean pisdepositslienapplicable { get; set; }
        public Boolean pissecuritylienapplicable { get; set; }
        public List<ApplicationSecurityandCollateralImMovablePropertyDetails> ImMovablePropertyDetailsList { get; set; }
        public List<ApplicationSecurityandCollateralMovablePropertyDetails> MovablePropertyDetailsList { get; set; }
        public List<ApplicationSecurityandCollateralSecuritycheques> SecuritychequesList { get; set; }
        public List<ApplicationSecurityandCollateralDepositsasLien> DepositsasLienList { get; set; }
        public List<ApplicationSecurityandCollateralOtherPropertyorsecurityDetails> otherPropertyorsecurityDetailsList { get; set; }
    }
    public class ApplicationSecurityandCollateralImMovablePropertyDetails : CommonDTO
    {
        private string name = "Security and Collateral";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string IsVerified { set; get; }
        public string psubsectionname { set; get; }
        public string pContactTYpe { set; get; }
        public bool pIsapplicable { set; get; }
        public long pRecordid { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }
        public string pTypeofproperty { set; get; }
        public string pTitledeed { set; get; }
        public string pDeeddate { set; get; }
        public string pPropertyownername { set; get; }
        public string pAddressofproperty { set; get; }
        public decimal? pEstimatedmarketvalue { set; get; }
        public string pPropertydocpath { set; get; }
        public string pPropertydocpathname { set; get; }


    }
    public class ApplicationSecurityandCollateralMovablePropertyDetails : CommonDTO
    {
        private string name = "Security and Collateral";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string IsVerified { set; get; }
        public string psubsectionname { set; get; }
        public string pContactTYpe { set; get; }
        public bool pIsapplicable { set; get; }
        public long pRecordid { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }
        public string PTypeofvehicle { set; get; }
        public string pVehicleownername { set; get; }
        public string pVehiclemodelandmake { set; get; }
        public string pRegistrationno { set; get; }
        public decimal? pEstimatedmarketvalue { set; get; }
        public string pVehicledocpath { set; get; }
        public string pVehicledocpathname { set; get; }
    }


    public class ApplicationSecurityandCollateralSecuritycheques : CommonDTO
    {
        public bool pIsapplicable { set; get; }
        public long pRecordid { set; get; }
        public string pApplicanttype { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }
        public string pTypeofsecurity { set; get; }
        public string pBankname { set; get; }
        public string pIfsccode { set; get; }
        public string pAccountno { set; get; }
        public string pChequeno { set; get; }
        public string pSecuritychequesdocpath { set; get; }
        public string pSecuritychequesdocpathname { set; get; }
    }
    public class ApplicationSecurityandCollateralDepositsasLien : CommonDTO
    {
        public bool pIsapplicable { set; get; }
        public long pRecordid { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }
        public string pDepositin { set; get; }
        public string pTypeofdeposit { set; get; }
        public string pDepositorbank { set; get; }
        public string pDepositaccountno { set; get; }
        public decimal? pDepositamount { set; get; }
        public decimal? pRateofinterest { set; get; }
        public string pDepositdate { set; get; }
        public string pTenureofdeposit { set; get; }
        public string pDeposittenuretype { set; get; }
        public string pMaturitydate { set; get; }
        public string pDepositdocpath { set; get; }
        public string pDepositdocpathname { set; get; }
    }
    public class ApplicationSecurityandCollateralOtherPropertyorsecurityDetails : CommonDTO
    {
        public bool pIsapplicable { set; get; }
        public long pRecordid { set; get; }
        public long pContactid { set; get; }
        public string pContactreferenceid { set; get; }
        public string pNameofthesecurity { set; get; }
        public decimal? pEstimatedvalue { set; get; }
        public string pSecuritydocpath { set; get; }
        public string pSecuritydocpathname { set; get; }
    }
    #endregion

    public class ApplicationExistingLoansDTO
    {

    }
    public class ApplicationLoanSpecificDTO
    {
        public string pLoantype { set; get; }
        public string pVchapplicationid { get; set; }
        public Int64 pApplicantid { set; get; }
        public Int64 pApplicationid { get; set; }
        public string pContactreferenceid { get; set; }
        public int pCreatedby { get; set; }
        public List<VehicleLoanDTO> lstVehicleLoanDTO { set; get; }
        public GoldLoanDTO lstGoldLoanDTO { set; get; }
        public EducationLoanDTO EducationLoanDTO { set; get; }
        public List<LoanagainstDepositDTO> lstLoanagainstDepositDTO { set; get; }
        public HomeLoanDTO HomeLoanDTO { set; get; }
        public BalanceTransferDTO BalanceTransferDTO { set; get; }
        public BusinessLoanDTO BusinessLoanDTO { set; get; }
        public ConsumerLoanDTO ConsumerLoanDTO { set; get; }
    }

    public class ApplicationLoanSpecificReferralDTO
    {

    }
    #region lOANSDETAILS
    public class VehicleLoanDTO
    {

        public string pShowroomName { set; get; }
        public string pVehicleManufacturer { set; get; }
        public string pVehicleModel { set; get; }
        public decimal? pActualcostofVehicle { set; get; }
        public decimal? pDownPayment { set; get; }
        public decimal? pOnroadprice { set; get; }
        public decimal? pRequestedLoanAmount { set; get; }
        public string pEngineNo { set; get; }
        public string pChassisNo { set; get; }
        public string pRegistrationNo { set; get; }
        public string pYearofMake { set; get; }
        public string pAnyotherRemarks { set; get; }
    }
    public class GoldLoanDTO
    {
        public Int64 pRecordid { set; get; }
        public decimal? pTotalAppraisedValue { set; get; }
        public string pAppraisalDate { set; get; }
        public string pAppraisorName { set; get; }
        public List<GoldLoanDetailsDTO> lstGoldLoanDetailsDTO { set; get; }
    }
    public class GoldLoanDetailsDTO
    {
        public Int64 pRecordid { set; get; }
        public string pGoldArticleType { set; get; }
        public string pDetailsofGoldArticle { set; get; }
        public string pCarat { set; get; }
        public decimal? pGrossweight { set; get; }
        public decimal? pNetWeight { set; get; }
        public decimal? pAppraisedValue { set; get; }
        public string pobservations { set; get; }
        public string pUploadfilename { set; get; }
        public string pGoldArticlePath { set; get; }
        public string pTypeofoperation { set; get; }
    }



    public class EducationLoanDTO
    {
        public string pNameoftheinstitution { set; get; }
        public string pNameofProposedcourse { set; get; }
        public string pselectionoftheinstitute { set; get; }
        public string pRankingofinstitution { set; get; }
        public string pDurationofCourse { set; get; }
        public string pDateofCommencement { set; get; }
        public string pseatsecured { set; get; }
        public List<EducationInstutiteAddressDTO> lstEducationInstutiteAddressDTO { set; get; }
        public List<EducationQualifcationDTO> lstEducationQualifcationDTO { set; get; }
        public List<EducationLoanFeeDetailsDTO> lstEducationLoanFeeDetailsDTO { set; get; }
        public List<EducationLoanyearwiseFeedetailsDTO> lstEducationLoanyearwiseFeedetailsDTO { set; get; }
    }
    public class EducationInstutiteAddressDTO
    {
        public string pAddress1 { set; get; }
        public string pAddress2 { set; get; }
        public string pCity { set; get; }
        public Int32 pStateid { set; get; }
        public string pState { set; get; }

        public Int32 pDistrictid { set; get; }
        public string pDistrict { set; get; }
        public Int32 pCountryid { set; get; }
        public string pCountry { set; get; }
        public string pPincode { set; get; }
    }
    public class EducationQualifcationDTO
    {
        public Int64 pRecordid { set; get; }
        public string pqualification { set; get; }
        public string pinstitute { set; get; }
        public string pyearofpassing { get; set; }
        public string pnoofattempts { get; set; }
        public string pmarkspercentage { get; set; }
        public string pgrade { get; set; }
        public bool pisscholarshipsapplicable { get; set; }
        public string pscholarshiporprize { get; set; }
        public string pscholarshipname { get; set; }
        public string pTypeofoperation { get; set; }
    }
    public class EducationLoanFeeDetailsDTO
    {
        public decimal? ptotalfundrequirement { set; get; }
        public string pnonrepayablescholarship { set; get; }
        public string prepayablescholarship { set; get; }
        public decimal? pfundsavailablefromfamily { set; get; }
    }
    public class EducationLoanyearwiseFeedetailsDTO
    {
        public Int64 pRecordid { set; get; }
        public string pyear { set; get; }
        public string pqualification { set; get; }
        public decimal? pfee { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class LoanagainstDepositDTO

    {
        public Int64 pRecordid { set; get; }

        public string pdeposittype { set; get; }
        public string pbankcreditfacility { set; get; }
        public string pdepositaccountnumber { set; get; }
        public decimal? pdepositamount { set; get; }
        public decimal? pdepositinterestpercentage { set; get; }
        public string pdepositdate { set; get; }
        public string pdeposittenure { set; get; }
        public string pUploadfilename { set; get; }
        public string pdepositdocpath { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class HomeLoanDTO
    {
        public Int64 pRecordid { set; get; }
        public decimal? pinitialpayment { set; get; }
        public string ppropertylocation { set; get; }
        public string ppropertyownershiptype { set; get; }
        public string ppropertytype { get; set; }
        public string ppurpose { set; get; }
        public string ppropertystatus { set; get; }
        public string paddress1 { set; get; }
        public string paddress2 { set; get; }
        public string pcity { set; get; }
        public string pstate { set; get; }
        public string pdistrict { set; get; }
        public Int64 pDistrictId { set; get; }
        public Int64 pStateId { set; get; }
        public Int64 pCountryId { set; get; }
        public string pcountry { set; get; }
        public string ppincode { set; get; }
        public string pbuildertieup { set; get; }
        public string pprojectname { set; get; }
        public string pownername { set; get; }
        public string pselleraddress { set; get; }
        public string pbuildingname { set; get; }
        public string pblockname { set; get; }
        public decimal? pbuiltupareain { set; get; }
        public decimal? pplotarea { set; get; }
        public decimal? pundividedshare { set; get; }
        public decimal? pplintharea { set; get; }
        public string pbookingdate { set; get; }
        public string pcompletiondate { set; get; }
        public string poccupancycertificatedate { set; get; }
        public decimal? pactualcost { set; get; }
        public decimal? psaleagreementvalue { set; get; }
        public decimal? pstampdutycharges { set; get; }
        public decimal? potheramenitiescharges { set; get; }
        public decimal? potherincidentalexpenditure { set; get; }
        public decimal? ptotalvalueofproperty { set; get; }
        public string pageofbuilding { set; get; }
        public decimal? poriginalcostofproperty { set; get; }
        public decimal? pestimatedvalueofrepairs { set; get; }
        public decimal? pamountalreadyspent { set; get; }
        public decimal? potherborrowings { set; get; }
        public decimal? ptotalvalue { set; get; }
        public BalanceTransferDTO BalanceTransferDTO { set; get; }
    }
    public class BalanceTransferDTO
    {
        public Int64 pRecordid { set; get; }
        public string pbankorcreditfacilityname { set; get; }
        public string ploandate { set; get; }
        public string ploanaccountno { set; get; }
        public decimal? ploanamount { set; get; }
        public string poutstandingdate { set; get; }
        public decimal? pinstallmentamount { set; get; }
        public string ploanpayin { set; get; }
        public Int64 ptotaltenureofloan { set; get; }
        public Int64 pbalancetenureofloan { set; get; }
        public string ploansanctiondocpath { set; get; }
        public string pemichartdocpath { set; get; }
    }
    public class BusinessLoanDTO
    {
        public Int64 pRecordid { set; get; }
        public string pdescriptionoftheactivity { set; get; }
        public bool pisfinancialperformanceapplicable { set; get; }
        public bool piscredittrendforpurchasesapplicable { set; get; }
        public bool piscredittrendforsalesapplicable { set; get; }
        public bool pisstockpositionapplicable { set; get; }
        public bool piscostofprojectapplicable { set; get; }
        public bool pisancillaryunit { set; get; }
        public bool passociateconcernsexist { set; get; }
        public List<BusinessfinancialperformanceDTO> lstBusinessfinancialperformanceDTO { set; get; }
        public List<BusinesscredittrendpurchasesDTO> lstBusinesscredittrendpurchasesDTO { set; get; }
        public List<BusinesscredittrendsalesDTO> lstBusinesscredittrendsalesDTO { set; get; }
        public List<BusinessstockpositionDTO> lstBusinessstockpositionDTO { set; get; }
        public List<BusinesscostofprojectDTO> lstBusinesscostofprojectDTO { set; get; }
        public BusinessancillaryunitaddressdetailsDTO BusinessancillaryunitaddressdetailsDTO { set; get; }
        public List<Businessloanturnoverandprofitorloss> lstBusinessloanturnoverandprofitorloss { set; get; }
        public List<Businessloanassociateconcerndetails> lstBusinessloanassociateconcerndetails { set; get; }

    }
    public class BusinessfinancialperformanceDTO
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public decimal? pturnoveramount { set; get; }
        public decimal? pnetprofitamount { set; get; }
        public string pUploadfilename { set; get; }
        public string pbalancesheetdocpath { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinesscredittrendpurchasesDTO
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public string psuppliername { set; get; }
        public string paddress { set; get; }
        public string pcontactno { set; get; }
        public decimal? pmaxcreditreceived { set; get; }
        public decimal? pmincreditreceived { set; get; }
        public decimal? pavgtotalcreditreceived { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinesscredittrendsalesDTO
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public string pcustomername { set; get; }
        public string paddress { set; get; }
        public string pcontactno { set; get; }
        public decimal? pmaxcreditgiven { set; get; }
        public decimal? pmincreditgiven { set; get; }
        public decimal? ptotalcreditsales { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinessstockpositionDTO
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public decimal? pmaxstockcarried { set; get; }
        public decimal? pminstockcarried { set; get; }
        public decimal? pavgtotalstockcarried { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinesscostofprojectDTO
    {
        public Int64 pRecordid { set; get; }
        public decimal? pcostoflandincludingdevelopment { set; get; }
        public decimal? pbuildingandothercivilworks { set; get; }
        public decimal? pplantandmachinery { set; get; }
        public decimal? pequipmenttools { set; get; }
        public decimal? ptestingequipment { set; get; }
        public decimal? pmiscfixedassets { set; get; }
        public decimal? perectionorinstallationcharges { set; get; }
        public decimal? ppreliminaryorpreoperativeexpenses { set; get; }
        public decimal? pprovisionforcontingencies { set; get; }
        public decimal? pmarginforworkingcapital { set; get; }
    }
    public class BusinessancillaryunitaddressdetailsDTO : ContactAddressDTO
    {
        public Int64 pRecordid { set; get; }
    }
    public class Businessloanturnoverandprofitorloss
    {
        public Int64 pRecordid { set; get; }
        public string pturnoveryear { set; get; }
        public decimal? pturnoveramount { set; get; }
        public decimal? pturnoverprofit { set; get; }
        public string pTypeofoperation { set; get; }

    }
    public class Businessloanassociateconcerndetails
    {
        public Int64 pRecordid { set; get; }
        public string pnameofassociateconcern { set; get; }
        public string pnatureofassociation { set; get; }
        public string pnatureofactivity { set; get; }
        public string pitemstradedormanufactured { set; get; }
        public string pTypeofoperation { set; get; }
    }
    #endregion

    #region locationmaster
    public class PropertylocationDTO : CommonDTO
    {
        public long plocationid { set; get; }
        public string plocationname { set; get; }
    }
    #endregion

    #region propertyownershiptype
    public class PropertyownershiptypeDTO : CommonDTO
    {
        public long pRecordid { set; get; }
        public string pownershipname { set; get; }
    }
    #endregion

    #region propertytype
    public class propertytypeDTO : CommonDTO
    {
        public long pRecordid { set; get; }
        public string ppropertytype { set; get; }
    }
    #endregion

    #region purpose
    public class purposeDTO : CommonDTO
    {
        public long pRecordid { set; get; }
        public string ppurpose { set; get; }
    }

    #endregion

    #region propertystatus
    public class propertystatusDTO : CommonDTO
    {
        public long pRecordid { set; get; }
        public string propertystatus { set; get; }
    }
    #endregion

    #region PersonalDetails
    public class ApplicationPersonalInformationDTO : CommonDTO
    {
        public long papplicationid { set; get; }
        public string pvchapplicationid { set; get; }

        public Boolean pIspersonaldetailsapplicable { get; set; }

        public List<ApplicationPersonalDetailsDTO> PersonalDetailsList { get; set; }
        public List<ApplicationPersonalEmployeementDTO> PersonalEmployeementList { get; set; }
        public List<ApplicationPersonalFamilyDTO> PersonalFamilyList { get; set; }
        public List<ApplicationPersonalNomineeDTO> PersonalNomineeList { get; set; }
        public List<ApplicationPersonalBankDTO> PersonalBankList { get; set; }
        public List<ApplicationPersonalIncomeDTO> PersonalIncomeList { get; set; }
        public List<ApplicationPersonalOtherIncomeDTO> PersonalOtherIncomeList { get; set; }
        public List<ApplicationPersonalEducationDTO> PersonalEducationList { get; set; }
        public List<BusinessDetailsDTO> BusinessDetailsDTOList { set; get; }
        public List<businessfinancialdetailsDTO> businessfinancialdetailsDTOList { set; get; }
        public List<PersonalInformationSectionsDTO> PersonalInformationSectionsDTOList { set; get; }

        public long pGuardianId { get; set; }
        public string pGuardianReferenceId { get; set; }
        public string pTypeofapplicant { get; set; }

    }

    public class PersonalInformationSectionsDTO : CommonDTO
    {
        public long pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string papplicanttype { set; get; }
        public Boolean pIsemplymentdetailsapplicable { get; set; }
        public Boolean pIspersonalbirthdetailsapplicable { get; set; }
        public Boolean pIsfamilydetailsapplicable { get; set; }
        public Boolean pIsnomineedetailsapplicable { get; set; }
        public Boolean pIsbankdetailsapplicable { get; set; }
        public Boolean pIsincomedetailsapplicable { get; set; }
        public Boolean pIseducationdetailsapplicable { get; set; }
        public Boolean pisbusinessactivitydetailsapplicable { get; set; }
        public Boolean pisfinancialperformanceapplicable { get; set; }
    }

    public class BusinessDetailsDTO : CommonDTO
    {
        private string name = "Business Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string pcontacttype { set; get; }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string papplicanttype { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pbusinessactivity { set; get; }
        public string pestablishmentdate { set; get; }
        public string pcommencementdate { set; get; }
        public string pcinnumber { set; get; }
        public string pgstinno { set; get; }

    }
    public class businessfinancialdetailsDTO : CommonDTO
    {
        private string name = "Business Financial Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string pcontacttype { set; get; }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string papplicanttype { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pfinancialyear { set; get; }
        public decimal? pturnoveramount { get; set; }
        public decimal? pnetprofitamount { get; set; }
        public string pdocbalancesheetpath { set; get; }

    }
    public class ApplicationPersonalEmployeementDTO : CommonDTO
    {
        private string name = "Employement Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string papplicanttype { set; get; }

        public string IsVerified { set; get; }
        public string psubsectionname { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        //  public string papplicanttype { set; get; }
        public bool pisapplicable { set; get; }
        public string pemploymenttype { set; get; }
        public string pnameoftheorganization { set; get; }
        public string pEnterpriseType { set; get; }
        public string pemploymentrole { set; get; }
        public string pofficeaddress { set; get; }
        public string pofficephoneno { set; get; }

        public string preportingto { set; get; }
        public decimal pemployeeexp { set; get; }
        public string pemployeeexptype { set; get; }
        public decimal ptotalworkexp { set; get; }
        public string pdateofestablishment { set; get; }
        public string pdateofcommencement { set; get; }
        public string pgstinno { set; get; }
        public string pcinno { set; get; }
        public string pdinno { set; get; }
        public string ptradelicenseno { set; get; }
    }
    public class EmployeeRoleDTO : CommonDTO
    {
        public long pemploymentroleid { set; get; }
        public string pemploymentrole { set; get; }
    }
    public class ApplicationPersonalDetailsDTO : CommonDTO
    {
        private string name = "Personal Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string papplicanttype { set; get; }

        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public string pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string presidentialstatus { set; get; }
        public string pmaritalstatus { set; get; }
        public string pplaceofbirth { set; get; }
        public string pcountryofbirth { set; get; }
        public string pnationality { set; get; }
        public string pminoritycommunity { set; get; }
    }
    public class ApplicationContactPersonalDetailsDTO
    {
        private string name = "Personal KYC Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string papplicanttype { set; get; }

        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public string pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string presidentialstatus { set; get; }
        public string pmaritalstatus { set; get; }
        public string pplaceofbirth { set; get; }
        public string pcountryofbirth { set; get; }
        public string pnationality { set; get; }
        public string pminoritycommunity { set; get; }
        public string pContactType { get; set; }
        public string pDob { get; set; }
        public string pGender { get; set; }
        public string pGenderCode { get; set; }
        public string pFatherName { get; set; }
        public string pSpouseName { get; set; }
        public string pBusinesscontactreferenceid { get; set; }
        public string pBusinesscontactName { get; set; }
        public int pAge { get; set; }
        public string pBusinesstype { get; set; }
        public string pEnterpriseType { get; set; }
        public string pBusinessEntityEmailid { get; set; }
        public string pBusinessEntityContactno { get; set; }
        public Int64 pContactId { get; set; }
        public string pTitleName { get; set; }
        public string pName { get; set; }
        public string pSurName { get; set; }
        public string pBusinessEntitycontactNo { get; set; }
        public string pBusinessEntityEmailId { get; set; }
        public string pContactimagepath { get; set; }
        public string pContactName { get; set; }
        public string pReferenceId { get; set; }
        public string pPhoto { get; set; }

    }
    public class ApplicationPersonalFamilyDTO : CommonDTO
    {
        private string name = "Personal Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string papplicanttype { set; get; }

        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }

        public string pcontactreferenceid { set; get; }
        public int ptotalnoofmembers { set; get; }
        public int pnoofearningmembers { set; get; }
        public string pfamilytype { set; get; }
        public int pnoofboyschild { set; get; }
        public int pnoofgirlchild { set; get; }
        public string phouseownership { set; get; }
    }

    public class ApplicationPersonalNomineeDTO : CommonDTO
    {
        private string name = "Nominee Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string papplicanttype { set; get; }
        public bool pisprimarynominee { set; get; }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
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
    }
    public class ApplicationPersonalBankDTO : BankDetailsDTO
    {
        private string name = "Bank Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string psubsectionname { set; get; }
        public string papplicanttype { set; get; }

        public string IsVerified { set; get; }
        public string pContacttype { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
    }
    public class ApplicationPersonalIncomeDTO : CommonDTO
    {
        private string name = "Income Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public string papplicanttype { set; get; }

        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string pcontacttype { set; get; }
        public string pcontactreferenceid { set; get; }
        public decimal pgrossannualincome { set; get; }
        public decimal pnetannualincome { set; get; }
        public decimal paverageannualexpenses { set; get; }
        public decimal pMonthlySavings { set; get; }
    }
    public class ApplicationPersonalOtherIncomeDTO : CommonDTO
    {
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string papplicanttype { set; get; }

        public string pcontactreferenceid { set; get; }
        public string psourcename { set; get; }
        public decimal pgrossannual { set; get; }
    }
    public class ApplicationPersonalEducationDTO : CommonDTO
    {
        private string name = "Education Details";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string pcontacttype { set; get; }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public string papplicanttype { set; get; }
        public long pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pqualification { set; get; }
        public string pnameofthecourseorprofession { set; get; }
        public string poccupation { set; get; }
    }
    #endregion

    public class ExistingLoanDetailsDTO : CommonDTO
    {
        public Boolean pIsexistingloansapplicable { get; set; }
        public string pVchapplicationid { get; set; }
        public List<ApplicationExistingLoanDetailsDTO> lstApplicationExistingLoanDetails { get; set; }
    }
    public class ApplicationExistingLoanDetailsDTO : CommonDTO
    {
        private string name = "Existing Loan";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public Int64 pRecordid { get; set; }
        public Int64 pApplicationid { get; set; }
        public Int64 pApplicantid { get; set; }
        public string pContactType { set; get; }
        public Int64 pContactid { get; set; }
        public string pContactreferenceid { get; set; }
        public string pTypeofLender { get; set; }
        public string pbankorcreditfacilityname { get; set; }
        public string pLoanno { get; set; }
        public string pLoanname { get; set; }
        public string pLoansanctiondate { get; set; }
        public Int64 pTenureofloan { get; set; }
        public Int64 premainingTenureofloan { get; set; }
        public decimal? pRateofinterest { get; set; }
        public string pLoanpayin { get; set; }
        public decimal pInstalmentamount { get; set; }
        public decimal? ploanamount { get; set; }
        public decimal pPrincipleoutstanding { get; set; }
        public string pLoanSanctionDocumentpath { get; set; }
        public string pLoanSanctionDocumentfilename { get; set; }
        public string pEmichartDocumentpath { get; set; }
        public string pEmichartDocumentfilename { get; set; }
        public bool pisbalancetransferapplicable { get; set; }

    }
    #region EducationLoanMasters
    public class EducationalQualificationDTO : CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public string peducationalqualification { set; get; }

    }
    public class EducationalFeeYearDTO : CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public string pYear { set; get; }

    }
    public class EducationalFeeQualificationDTO : CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public string peducationalfeequalification { set; get; }

    }
    public class ApplicantIdsDTO
    {
        public Int64 pApplicantID { set; get; }
        public string pApplicationID { set; get; }
    }
    #endregion


    #region ApplicationReferences
    public class ApplicationLoanReferencesDTO : CommonDTO
    {
        private string name = "Reference";
        public string SectionName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string psubsectionname { set; get; }
        public string IsVerified { set; get; }
        public long pRefRecordId { get; set; }
        public long papplicationId { get; set; }
        public string pvchapplicationId { get; set; }
        public string pApplicanttype { get; set; }
        public string pRefFirstname { get; set; }
        public string pRefLastname { get; set; }
        public decimal? pRefcontactNo { get; set; }
        public decimal? pRefalternatecontactNo { get; set; }
        public string pRefEmailID { get; set; }
        public string pRefAlternateEmailId { get; set; }
    }

    public class ApplicationReferencesDTO : CommonDTO
    {


        public Boolean pIsreferencesapplicable { get; set; }
        public List<ApplicationLoanReferencesDTO> LobjAppReferences { get; set; }
    }

    #endregion

    #region Consumer Loan Masters

    public class ProducttypeDTO : CommonDTO
    {
        public Int64 pRecordid { set; get; }
        public string pproducttype { set; get; }
    }
    public class ConsumerLoanDTO
    {
        public Int64 pRecordid { set; get; }
        public decimal? ptotalproductcost { set; get; }
        public decimal? pdownpayment { set; get; }
        public decimal? pbalanceamount { set; get; }
        public List<ConsumerLoanDetailsDTO> lstConsumerLoanDetailsDTO { set; get; }
    }
    public class ConsumerLoanDetailsDTO
    {
        public Int64 pRecordid { set; get; }
        public string pproducttype { set; get; }
        public string pproductname { set; get; }
        public string pmanufacturer { set; get; }
        public string pproductmodel { set; get; }
        public Int64 pquantity { set; get; }
        public decimal? pcostofproduct { set; get; }
        public decimal? pinsurancecostoftheproduct { set; get; }
        public decimal? pothercost { set; get; }
        public decimal? ptotalcostofproduct { set; get; }
        public bool piswarrantyapplicable { set; get; }
        public string pperiod { set; get; }
        public string pperiodtype { set; get; }
        public string pTypeofoperation { set; get; }
    }
    #endregion

}
