using FinstaInfrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Transactions
{
    public  class VerificationDTO
    {
        public TeleVerificationDTO TeleVerificationDTO { set; get; }
        public FieldverificationDTO FieldverificationDTO { set; get; }
        public FIverificationDTO FIverificationDTO { set; get; }
    }  
    public class TeleVerificationDTO:CommonDTO
    {       
        public Int64 papplicationid { set; get; }
        public string pvchapplicationid { set; get; }
        public Int64 pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pApplicantName { set; get; }
        public string pLoantype { set; get; }
        public string pLoanName { set; get; }
        public string pDateofApplication { set; get; }
        public string pDateofDisbursement { set; get; }
        public Int64 Pconctano { set; get; }
        public Int64 precordid { set; get; }
        public string pverificationdate { set; get; }
        public string pverificationtime { set; get; }
        public Int64 pinvestigationexecutiveid { set; get; }
        public string pinvestigationexecutivename { set; get; }          
        public string pteleverifierscomments { set; get; }
        public string pteleverifiersrating { set; get; }
        public CustomerAvailabilityDTO CustomerAvailabilityDTO { set; get; }
        public spoketoDTO spoketoDTO { set; get; }
        public string pVerifierName { set; get; }
    }
    public class CustomerAvailabilityDTO
    {
        public bool pcustomeravailability { set; get; }
        public string pcontacttype { set; get; }
    }
    public class spoketoDTO
    {
        public string pspoketo { set; get; }
        public spoketoOtherDTO spoketoOtherDTO { set; get; }
    }
    public class spoketoOtherDTO
    {
        public string pnameoftheperson { set; get; }
        public string prelationshipwithapplicant { set; get; }
    }

    public class VerificationDetailsDTO
    {
        public Int64 papplicationid { set; get; }
        public string pvchapplicationid { set; get; }
        public Int64 pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string PApplicantName { set; get; }
        public Int64 pbusinessentitycontactno { set; get; }
        public bool pisteleverification { set; get; }
        public bool pisphysicalverification { set; get; }
        public bool pisdocumentverified { set; get; }
        public string  pLoantype { set; get; }
        public string pLoanName { set; get; }
        public string pdateofapplication { set; get; }
        public string pteleverificationdate { set; get; }
        public string paddressverificationdate { set; get; }
        public string pdocumentverificationdate { set; get; }
        public decimal pLoanAmount { set; get; }
        public decimal pinstalmentAmount { set; get; }


    }
    public class AddressType
    {
       
        public string pAddresType { set; get; }
    }
    public class ProffType
    {
        public string pdocumentgroupname { set; get; }
        public string pProfname { set; get; }
    }
    public class FieldverificationDTO:CommonDTO
    {
        public Int64 papplicationid { set; get; }
        public string pvchapplicationid { set; get; }
        public Int64 pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }
        public string pDateofApplication { set; get; }
        public string pDateofDisbursement { set; get; }
        public string pApplicantName { set; get; }
        public string pLoantype { set; get; }
        public string pLoanName { set; get; }
        public string Pcontacttype { set; get; }
        public Int64 Pconctano { set; get; }
        public Int64 precordid { set; get; }
        public string pverificationdate { set; get; }
        public string pverificationtime { set; get; }
        public Int64 pInvestigationexecutiveid { set; get; }
        public string pInvestigationexecutivename { set; get; }
        public string paddresstype { set; get; }
      //  public bool pisaddressconfirmed { set; get; }
        public AddressconfirmedDTO AddressconfirmedDTO { set; get; }
        public List<AddressType> lstaddrestype { set; get; }
        public List<ProffType> lstProffType { set; get; }
        public FieldVerifiersobservationDTO FieldVerifiersobservationDTO { set; get; }
        public List<FieldVerifyneighbourcheckDTO> FieldVerifyneighbourcheckDTO { set; get; }
        public string pFieldVerifiersComments { set; get; }
        public string pVerifierName { set; get; }

        public string pFieldVerifiersRating { set; get; }
    }
    public class AddressconfirmedDTO
    {
        public bool pisaddressconfirmed { set; get; }
        public string pUploaddocumentname { set; get; }
        public string puploadlocationdocpath { set; get; }
        public string plongitude { set; get; }
        public string pLatitude { set; get; }
        public int pNoofyearsatpresentaddress { set; get; }
        public string pHouseownership { set; get; }
        public string pPersonmet { set; get; }
        public string pPersonname { set; get; }
        public string pRelationshipwithapplicant { set; get; }
        public string pDateofbirth { set; get; }
        public int pAge { set; get; }
        public string pMaritalStatus { set; get; }
        public int pTotalnoofmembersinfamily { set; get; }
        public int pEarningmembers { set; get; }
        public int pChildren { set; get; }
        public string pEmploymentorbusinessdetails { set; get; }
        public decimal? pMonthlyincome { set; get; }
        public string pFieldVerifiersComments { set; get; }
        public string pFieldVerifiersRating { set; get; }
        public List<FielddocumentverificationDTO> lstFielddocumentverificationDTO { set; get; }
    }

    public class FielddocumentverificationDTO
    {
        public Int64 Precordid { set; get; }
        public String Pdocprooftype { set; get; }
        public String Pdocproofname { set; get; }
        public bool pisdocumentverified { set; get; }
        public string ptypeofoperation { set; get; }

    }
    public class FieldVerifiersobservationDTO
    {
        public Int64 Precordid { set; get; }
        public string pAddressLocalityDescription { set; get; }
        public string pAccessability { set; get; }
        public string pTypeofAccomodation { set; get; }
        public string pApproxArea { set; get; }
        public bool pVisiblePoliticalAffiliation { set; get; }
        public string pAffiliationRemarks { set; get; }
        public string pAddressFurnishing { set; get; }
        public string pVisibleAssets { set; get; }
        public string pCustomerCooperation { set; get; }
        public bool pCustomerAvailability { set; get; }
        public string pFieldVerifiersComments { set; get; }
        public string pFieldVerifiersRating { set; get; }
    }
    public class FieldVerifyneighbourcheckDTO
    {
        public Int64 Precordid { set; get; }
        public string pNameoftheNeighbour { set; get; }
        public string pAddressofNeighbour { set; get; }
        public bool pisApplicantstayhere { set; get; }
        public string pHouseOwnership { set; get; }
        public string papplicantisstayingsince { set; get; }
        public string papplicantisstayingMontsOrYears { set; get; }

        public string pFieldVerifiersComments { set; get; }
        public string pFieldVerifiersRating { set; get; }
        public string pTypeofoperation { set; get; }

    }
    public class FIverificationDTO:CommonDTO
    {
        
        public Int64 papplicationid { set; get; }
        public string pvchapplicationid { set; get; }
        public Int64 pcontactid { set; get; }
        public string pcontactreferenceid { set; get; }      
        public string pverificationdate { set; get; }
        public string pverificationtime { set; get; }
        public List<FIverificationDetailsDTO> lstFIverificationDetailsDTO { set; get; }
    }

    public class FIverificationDetailsDTO
    {
        public Int64 precordid { set; get; }    
        public string pVerifiedsectionname { set; get; }
        public string pVerifiedsubsectionname { set; get; }
        public string pContacttype { set; get; }
        public string pVerificationstatus { set; get; }
        public string pFIVerifierscomments { set; get; }
        public string pFIVerifiersrating { set; get; }
      
    }

    #region Verification-Loan-Specific-Classes

    public class ApplicationLoanSpecificDTOinVerification
    {
        public string pLoantype { set; get; }
        public string pVchapplicationid { get; set; }
        public Int64 pApplicantid { set; get; }
        public Int64 pApplicationid { get; set; }
        public string pContactreferenceid { get; set; }
        public int pCreatedby { get; set; }
        public List<VehicleLoanDTOVerification> lstVehicleLoanDTO { set; get; }
        public GoldLoanDTOVerification lstGoldLoanDTO { set; get; }
        public EducationLoanDTOVerification EducationLoanDTO { set; get; }
        public List<LoanagainstDepositDTOVerification> lstLoanagainstDepositDTO { set; get; }
        public List< HomeLoanDTOVerification> _HomeLoanDTOLst { set; get; }
        public BalanceTransferDTOVerification BalanceTransferDTO { set; get; }
        public BusinessLoanDTOVerification BusinessLoanDTO { set; get; }
        public ConsumerLoanDTOVerification ConsumerLoanDTO { set; get; }
    }

    #region Consumer Loan

    public class ConsumerLoanDTOVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public decimal? ptotalproductcost { set; get; }
        public decimal? pdownpayment { set; get; }
        public decimal? pbalanceamount { set; get; }
        public List<ConsumerLoanDetailsDTOVerification> lstConsumerLoanDetailsDTO { set; get; }
    }
    public class ConsumerLoanDetailsDTOVerification : CommonSectionsandSubsectionNames
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

    #region Business-Loan
    public class BusinessLoanDTOVerification : CommonSectionsandSubsectionNames
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
        public List<BusinessfinancialperformanceDTOVerification> lstBusinessfinancialperformanceDTO { set; get; }
        public List<BusinesscredittrendpurchasesDTOVerification> lstBusinesscredittrendpurchasesDTO { set; get; }
        public List<BusinesscredittrendsalesDTOVerification> lstBusinesscredittrendsalesDTO { set; get; }
        public List<BusinessstockpositionDTOVerification> lstBusinessstockpositionDTO { set; get; }
        public List<BusinesscostofprojectDTOVerification> lstBusinesscostofprojectDTO { set; get; }
        public BusinessancillaryunitaddressdetailsDTOVerification BusinessancillaryunitaddressdetailsDTO { set; get; }
        public List<BusinessloanturnoverandprofitorlossVerification> lstBusinessloanturnoverandprofitorloss { set; get; }
        public List<BusinessloanassociateconcerndetailsVerification> lstBusinessloanassociateconcerndetails { set; get; }

    }
    public class BusinessfinancialperformanceDTOVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public decimal? pturnoveramount { set; get; }
        public decimal? pnetprofitamount { set; get; }
        public string pUploadfilename { set; get; }
        public string pbalancesheetdocpath { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinesscredittrendpurchasesDTOVerification : CommonSectionsandSubsectionNames
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
    public class BusinesscredittrendsalesDTOVerification : CommonSectionsandSubsectionNames
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
    public class BusinessstockpositionDTOVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public string pfinacialyear { set; get; }
        public decimal? pmaxstockcarried { set; get; }
        public decimal? pminstockcarried { set; get; }
        public decimal? pavgtotalstockcarried { set; get; }
        public string pTypeofoperation { set; get; }
    }
    public class BusinesscostofprojectDTOVerification : CommonSectionsandSubsectionNames
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
    public class BusinessancillaryunitaddressdetailsDTOVerification : ContactAddressDTO
    {
        public Int64 pRecordid { set; get; }
    }
    public class BusinessloanturnoverandprofitorlossVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public string pturnoveryear { set; get; }
        public decimal? pturnoveramount { set; get; }
        public decimal? pturnoverprofit { set; get; }
        public string pTypeofoperation { set; get; }

    }
    public class BusinessloanassociateconcerndetailsVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public string pnameofassociateconcern { set; get; }
        public string pnatureofassociation { set; get; }
        public string pnatureofactivity { set; get; }
        public string pitemstradedormanufactured { set; get; }
        public string pTypeofoperation { set; get; }
    }


    #endregion

    #region Balance-Transfer

    public class BalanceTransferDTOVerification : CommonSectionsandSubsectionNames
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

    #endregion

    #region Home Loan

    public class HomeLoanDTOVerification : CommonSectionsandSubsectionNames
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
        public BalanceTransferDTOVerification BalanceTransferDTO { set; get; }
    }

    #endregion

    #region Loan Against Deposit

    public class LoanagainstDepositDTOVerification : CommonSectionsandSubsectionNames
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


    #endregion

    #region Education Loan

    public class EducationLoanDTOVerification : CommonSectionsandSubsectionNames
    {
        public string pNameoftheinstitution { set; get; }
        public string pNameofProposedcourse { set; get; }
        public string pselectionoftheinstitute { set; get; }
        public string pRankingofinstitution { set; get; }
        public string pDurationofCourse { set; get; }
        public string pDateofCommencement { set; get; }
        public string pseatsecured { set; get; }
        public List<EducationInstutiteAddressDTOVerification> lstEducationInstutiteAddressDTO { set; get; }
        public List<EducationQualifcationDTOVerification> lstEducationQualifcationDTO { set; get; }
        public List<EducationLoanFeeDetailsDTOVerification> lstEducationLoanFeeDetailsDTO { set; get; }
        public List<EducationLoanyearwiseFeedetailsDTOVerification> lstEducationLoanyearwiseFeedetailsDTO { set; get; }
    }

    public class EducationInstutiteAddressDTOVerification : CommonSectionsandSubsectionNames
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
    public class EducationQualifcationDTOVerification : CommonSectionsandSubsectionNames
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
    public class EducationLoanFeeDetailsDTOVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public decimal? ptotalfundrequirement { set; get; }
        public string pnonrepayablescholarship { set; get; }
        public string prepayablescholarship { set; get; }
        public decimal? pfundsavailablefromfamily { set; get; }
    }
    public class EducationLoanyearwiseFeedetailsDTOVerification : CommonSectionsandSubsectionNames
    {
        public Int64 pRecordid { set; get; }
        public string pyear { set; get; }
        public string pqualification { set; get; }
        public decimal? pfee { set; get; }
        public string pTypeofoperation { set; get; }
    }

    #endregion

    #region Gold Loan

    public class GoldLoanDTOVerification 
    {
        public Int64 pRecordid { set; get; }
        public decimal? pTotalAppraisedValue { set; get; }
        public string pAppraisalDate { set; get; }
        public string pAppraisorName { set; get; }
        public List<GoldLoanDetailsDTOVerification> lstGoldLoanDetailsDTO { set; get; }
    }

    public class GoldLoanDetailsDTOVerification : CommonSectionsandSubsectionNames
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


    #endregion

    #region Vehicle Loan

    public class VehicleLoanDTOVerification : CommonSectionsandSubsectionNames
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


    #endregion

    public class CommonSectionsandSubsectionNames
    {
        public string SectionName { get; set; }
        public string psubsectionname { get; set; }
        public string IsVerified { get; set; }
    }

    #endregion
}
