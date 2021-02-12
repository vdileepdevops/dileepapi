using FinstaInfrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Loans.Masters
{
    public class ContactMasterDTO : ContactDTO
    {
        public long pContactId { get; set; }
        public Boolean pcontactexistingstatus { get; set; }
        public string pContactType { get; set; }
        public string pDob { get; set; }
        public string pGender { get; set; }
        public string pGenderCode { get; set; }
        public string pFatherName { get; set; }
        public string pSpouseName { get; set; }
        public List<contactAddressDTO> pAddressList { get; set; }
        public List<conatactdetaisDTO> pcontactdetailslist { get; set; }
        public List<EmailidsDTO> pEmailidsList { get; set; }
        public List<EnterpriseTypeDTO> pEnterpriseTypelist { get; set; }
        public List<BusinessTypeDTO> pBusinesstypelist { get; set; }
        public string pBusinesscontactreferenceid { get; set; }
        public string pBusinesscontactName { get; set; }
        public Int16 pAge { get; set; }
        public List<BusinessContactDTO> pBusinessContactlist { get; set; }
        public string pBusinesstype { get; set; }
        public string pEnterpriseType { get; set; }
        public string pBusinessEntityEmailid { get; set; }
        public string pBusinessEntityContactno { get; set; }
    }
    public class ContactViewDTO
    {
        public long pContactdId { set; get; }
        public string pContactType { get; set; }
        public string pRefNo { get; set; }
        public string pContactName { get; set; }
        public string pFatherName { get; set; }
        public string pContactNumber { get; set; }
        public string pContactEmail { get; set; }
        public string pImagePath { get; set; }
        public List<string> pImage { get; set; }

        public string pAddresDetails { get; set; }

    }
    public class contactAddressDTO : CommonDTO

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
        public string ptypeofoperation { get; set; }
        public string pAddressDetails { get; set; }
    }
    public class conatactdetaisDTO : CommonDTO
    {
        public string pContactNumber { get; set; }
        public string pPriority { get; set; }
    }

    public class EmailidsDTO : CommonDTO
    {
        public long pRecordId { set; get; }
        public string pEmailId { get; set; }
        public string pPriority { get; set; }
        public string pContactNumber { get; set; }
        public string pContactName { get; set; }
    }

    public class EnterpriseTypeDTO : CommonDTO
    {
        public string pEnterpriseType { get; set; }

    }

    public class BusinessTypeDTO : CommonDTO
    {
        public string pBusinesstype { get; set; }

    }

    public class BusinessContactDTO : CommonDTO
    {
        public string PBusinessContactRefNo { set; get; }
        public string PBusinessContactName { set; get; }
        public string PBusinessContactNo { set; get; }
    }
}
