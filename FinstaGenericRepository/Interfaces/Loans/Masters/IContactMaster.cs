using FinstaInfrastructure.Loans.Masters;
using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Loans.Transactions;


namespace FinstaRepository.Interfaces.Loans.Masters
{
    public interface IContactMaster
    {
        // bool Saveaddresstype(string addressname);
        #region SaveContact
        bool Savecontact(ContactMasterDTO contact,string ConnectionString);
        #endregion

        #region ViewContact
        ContactMasterDTO ViewContact(string referenceid,string ConnectionString);
        #endregion

        #region GetContactdetails
        List<ContactMasterDTO> GetContactdetails(string ConnectionString,string Type);
        #endregion

        #region UpdateContact
        bool UpdateContact(ContactMasterDTO contact,string ConnectionString);
        #endregion

        #region DeleteContact
        bool DeleteContact(ContactMasterDTO contact, string ConnectionString);
        #endregion

        #region ContactAddressTypes       
        bool SaveAddressType(contactAddressDTO addressname,string ConnectionString);

        List<contactAddressDTO> GetAddressType(string contactype,string ConnectionString);

        int checkInsertAddressTypeDuplicates(string addresstype, string contactype, string connectionstring);
        #endregion

        #region ContactEnterpriseTypes     
        bool SaveEnterpriseType(EnterpriseTypeDTO Enterprisetype,string ConnectionString);

        List<EnterpriseTypeDTO> GetEnterpriseType(string ConnectionString);

        int checkInsertEnterpriseTypeDuplicates(string enterprisetype, string connectionstring);
        #endregion

        #region ContactBusinessTypes
        bool SaveBusinessTypes(BusinessTypeDTO BusinessTypes,string ConnectionString);

        List<BusinessTypeDTO> GetBusinessTypes(string ConnectionString);

        int checkInsertBusinessTypesDuplicates(string businesstype, string connectionstring);
        #endregion

        #region Personcount
        int GetPersoncount(ContactMasterDTO ContactDto, string ConnectionString);
        #endregion
        List<ContactViewDTO> GetContactView(string ViewName, string ConnectionString);
        //List<FirstinformationDTO> GetLoansBasedOnContactRefID(string contactrefID, string ConnectionString);
    }

}
