using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Transactions;
using FinstaInfrastructure;
using FinstaInfrastructure.Settings;
using System.Threading.Tasks;
namespace FinstaRepository.Interfaces.Loans.Masters
{
    public interface IContactMasterNew
    { 
        // bool Saveaddresstype(string addressname);
        #region SaveContact
        bool Savecontact(ContactMasterNewDTO contact, out string contactid, string ConnectionString);
        #endregion

        #region ViewContact
        ContactMasterNewDTO ViewContact(string referenceid, string ConnectionString);
        #endregion

        #region GetContactdetails
        List<ContactMasterNewDTO> GetContactdetails(string ConnectionString, string Type);
        #endregion

        #region UpdateContact
        bool UpdateContact(ContactMasterNewDTO contact, out string contactId, string ConnectionString);
        #endregion

        List<DesignationDTO> GetDesignations(string con);

        #region DeleteContact
        bool DeleteContact(ContactMasterNewDTO contact, string ConnectionString);
        #endregion

        #region ContactAddressTypes       
        bool SaveAddressType(contactAddressNewDTO addressname, string ConnectionString);
        List<contactAddressNewDTO> GetAddressType(string contactype, string ConnectionString);
        int checkInsertAddressTypeDuplicates(string addresstype, string contactype, string connectionstring);
        #endregion

        #region ContactEnterpriseTypes     
        bool SaveEnterpriseType(EnterpriseTypeNewDTO Enterprisetype, string ConnectionString);

        List<EnterpriseTypeNewDTO> GetEnterpriseType(string ConnectionString);

        int checkInsertEnterpriseTypeDuplicates(string enterprisetype, string connectionstring);
        #endregion

        #region ContactBusinessTypes
        bool SaveBusinessTypes(BusinessTypeNewDTO BusinessTypes, string ConnectionString);

        List<BusinessTypeNewDTO> GetBusinessTypes(string ConnectionString);

        int checkInsertBusinessTypesDuplicates(string businesstype, string connectionstring);
        #endregion

        #region Personcount
        int GetPersoncount(ContactMasterNewDTO ContactDto, string ConnectionString);
        #endregion
        int GetContactCount(string ViewName, string searchby, string ConnectionString);
        List<ContactViewNewDTO> GetContactView(string ViewName, string endindex, string searchby, string ConnectionString);
        List<ContactViewNewDTO> GetContactViewdata(string ViewName, string ConnectionString);
        List<ContactMasterNewDTO> GetContactdetailsByMobileNo(string con, string pMobileNo);

         List<SubscriberContactDTO> GetSubContactdetails(string con, string type);
         ContactViewNewDTO GetContactViewbyid(string con, string refid);
         bool SaveContactEmployee(string con, ContactEmployeeDTO employeeDTO);
        Task<ContactEmployeeDTO> GetEmployeedeatils(string con, string refernceid);
       // Task<List<RelationShipDTO>> getRelationShip(string con);
         Task<ReferralDTO> GetReferraldeatils(string con, string refernceid);
         Task<List<QualificationDTO>> ViewQualificationDetails(string con);
         Task<List<IntroducedDTO>> GetInterducedDetails(string con);
        Task<List<RelationShipNewDTO>> getRelationShip(string con);
        bool SaveContactReferral(string con, ReferralDTO referralDTO);
         bool SaveContactSupplier(string con, SupplierDTO supplierDTO);
         bool SaveContactAdvocate(string con, AdvocateDTO advocateDTO);
        Int32 CheckDocumentExist(Int32 DocumentId, string ReferenceNo, string connectionstring);
        List<SubscriberContactDTO> GetContactsList(string ConnectionString);
        //List<FirstinformationDTO> GetLoansBasedOnContactRefID(string contactrefID, string ConnectionString);
    }
}
