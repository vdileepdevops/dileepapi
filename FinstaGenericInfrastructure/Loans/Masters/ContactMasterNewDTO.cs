using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Masters
{
    public class ContactMasterNewDTO : ContactDTO
    {
        public long pContactId { get; set; }
        public Boolean pcontactexistingstatus { get; set; }
        public string pContactType { get; set; }
        public string pDob { get; set; }
        public string pGender { get; set; }
        public string pGenderCode { get; set; }
        //public string pFatherName { get; set; }
        public string pSpouseName { get; set; }
        public List<contactAddressNewDTO> pAddressList { get; set; }
        public List<conatactdetaisNewDTO> pcontactdetailslist { get; set; }
        public List<EmailidsNewDTO> pEmailidsList { get; set; }
        public List<ContactBunsineePersonDto> pbusinessList { get; set; }
        public List<EnterpriseTypeNewDTO> pEnterpriseTypelist { get; set; }
        public List<BusinessTypeNewDTO> pBusinesstypelist { get; set; }
        public List<documentstoreDTO> documentstorelist { get; set; }
        public List<referralbankdetailsDTO> referralbankdetailslist { get; set; }
        public string pBusinesscontactreferenceid { get; set; }
        public string pBusinesscontactName { get; set; }
        public Int16 pAge { get; set; }
        public List<BusinessContactNewDTO> pBusinessContactlist { get; set; }
        public string pBusinesstype { get; set; }
        public string pEnterpriseType { get; set; }
        public string pBusinessEntityEmailid { get; set; }
        public string pBusinessEntityContactno { get; set; }
        public object pEmailId2 { get; set; }
        public object pAlternativeNo { get; set; }
    }
    public class ContactViewNewDTO
    {
        public long pContactdId { set; get; }
        public string pContactType { get; set; }
        public string pRefNo { get; set; }
        public string pContactName { get; set; }
        public string pFatherName { get; set; }
        public string pContactNumber { get; set; }
        public string pContactEmail { get; set; }
        public string pImagePath { get; set; }
        public object pImage { get; set; }

        public string pAddresDetails { get; set; }
        public bool pissupplier { get; set; }
        public bool pisadvocate { get; set; }
        public bool pisemployee { get; set; }
        public bool pisreferral { get; set; }
        public object subscribercount { get; set; }
        public object pGender { get; set; }
        public string pContactimagepath { get; set; }

    }
    public class ContactBunsineePersonDto
    {
        public object precordid { set; get; }
        public object pContactId { set; get; }
        public object pContactName { set; get; }
        public object designationid { set; get; }
        public object designationname { set; get; }
        public object pBusinessPriority { set; get; }
        public object pstatus { set; get; }
        public object ptypeofoperation { set; get; }
    }
    public class contactAddressNewDTO : CommonDTO

    {
        public long pRecordId { set; get; }
        public string pContactType { get; set; }
        public string pAddressType { get; set; }
        public string pAddress1 { get; set; }
        public string pAddress2 { get; set; }
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
        public object pAddressPriority { get; set; }
        //public string ptypeofoperation { get; set; }
        public string pAddressDetails { get; set; }
        public object plongitude { get; set; }
        public object platitude { get; set; }
        public Boolean pPriorityCon { get; set; }
    }
    public class conatactdetaisNewDTO : CommonDTO
    {
        public string pContactNumber { get; set; }
        public string pPriority { get; set; }
    }

    public class EmailidsNewDTO : CommonDTO
    {
        public long pRecordId { set; get; }
        public string pEmailId { get; set; }
        public string pPriority { get; set; }
        public string pContactNumber { get; set; }
        public string pContactName { get; set; }
    }

    public class EnterpriseTypeNewDTO : CommonDTO
    {
        public string pEnterpriseType { get; set; }

    }

    public class BusinessTypeNewDTO : CommonDTO
    {
        public string pBusinesstype { get; set; }

    }

    public class BusinessContactNewDTO : CommonDTO
    {
        public string PBusinessContactRefNo { set; get; }
        public string PBusinessContactName { set; get; }
        public string PBusinessContactNo { set; get; }
    }
    public class SubscriberContactDTO : CommonDTO
    {
        public object subscriberid { get; set; }
        public object subscribersignedate { get; set; }
        public object contactid { get; set; }
        public object contacttype { get; set; }
        public object contactreferenceid { get; set; }
        public object contactname { get; set; }
        public object contactmobilenumber { get; set; }
        public object contactemailid { get; set; }
        public object contactaddress { get; set; }
        public object contactimagepath { get; set; }
        public object subscriberchitgroup { get; set; }
        public object subscriberchitgroupid { get; set; }
        public object subscriberticketno { get; set; }

    }
    public class ReferralDTO : CommonDTO
    {
        public object samebranchcode { get; set; }
        public object precordid { set; get; }
        public object preferralcode { set; get; }
        public object pContactId { set; get; }
        public bool pIsPanNoAvailable { set; get; }
        public object pPanNumber { set; get; }
        public object introducedname { set; get; }
        public object introducedid { set; get; }
        public object ptdsSectionName { set; get; }
        public object ptdsSectionID { set; get; }
        public object pDocStorePath { set; get; }
        public object pFilename { set; get; }
        public string pipaddress { get; set; }
    }
    public class IntroducedDTO : CommonDTO
    {
        public object introducedid { get; set; }
        public object introducedcode { get; set; }
        public object introducedname { get; set; }
        public object introducedmobilenumber { get; set; }
        public object introducedemailid { get; set; }
    }
    public class SupplierDTO
    {
        public object pContactId { set; get; }
        public bool pIsSupplier { set; get; }
    }
    public class AdvocateDTO
    {
        public object pContactId { set; get; }
        public bool pIsAdvocate { set; get; }


    }
    public class RelationShipNewDTO 
    {
        public object relationshipid { get; set; }
        public object relationshipname { get; set; }
        public object status { get; set; }
    }

    //public class SubscriberContactDTO : CommonDTO
    //{
    //    public object subscriberid { get; set; }
    //    public object subscribersignedate { get; set; }
    //    public object contactid { get; set; }
    //    public object contacttype { get; set; }
    //    public object contactreferenceid { get; set; }
    //    public object contactname { get; set; }
    //    public object contactmobilenumber { get; set; }
    //    public object contactemailid { get; set; }
    //    public object contactaddress { get; set; }
    //    public object contactimagepath { get; set; }
    //    public object subscriberchitgroup { get; set; }
    //    public object subscriberchitgroupid { get; set; }
    //    public object subscriberticketno { get; set; }

    //}
}
