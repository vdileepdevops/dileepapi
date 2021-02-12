using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;

namespace FinstaInfrastructure.Settings
{
    public class EmployeeDTO : PersonalDetailsDTO
    {
        public string pContactName { get; set; }
        public Int64 pContactId { get; set; }
        public Int64 pEmployeeId { get; set; }
        public string pEmployeeName { get; set; }
        public string pEmployeeSurName { get; set; }
        public string pEmployeeTitleName { get; set; }
        
        // KYC Details
        public List<documentstoreDTO> pListEmpKYC { get; set; }
      //  public List<documentstoreDTO> pListDocStore { get; set; }
        // Employee Details
        public decimal? pEmploymentBasicSalary { get; set; }
        public decimal? pEmploymentAllowanceORvda { get; set; }
        public string pEmploymentDesignation { get; set; }
        public Int64? pEmploymentRoleId { get; set; }
        public string pEmploymentRoleName { get; set; }
        public string pEmploymentJoiningDate { get; set; }
        public decimal? pEmploymentCTC { get; set; }       
        // Family Details
        public List<familyDetailsDTO> pListFamilyDetails { get; set; }       
        // Bank Details
        public List<EmployeeBankDetails> pListEmpBankDetails { get; set; }
        // Main Employee Grid Data
        public string pEmployeeContactNo { get; set; }
        public string pEmployeeContactEmail { get; set; }
        public string pContactRefNo { get; set; }
        public string pMainTransactionType { get; set; }
    }   
    public class EmployeeBankDetails : BankDetailsDTO
    {
        // Bank Details   
        public Int64 pBankRecordid { get; set; }     
        public Int64 pEmployeeID { get; set; }
    }

    public class EmployeeRole : CommonDTO
    {
        public Int64 pEmployeeRoleID { get; set; }
        public string pEmployeeRoleName { get; set; }
    }

    public class PersonalDetailsDTO : CommonDTO
    {
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

    public class familyDetailsDTO
    {
        public object pfamilyrecordid { get; set; }
        public object pEmployeeid { get; set; }
        public Int64 pTotalnoofmembers { get; set; }
        public string pContactpersonname { get; set; }
        public string pRelationwithemployee { get; set; }
        public string pContactnumber { get; set; }
        public string pdateofbirth { set; get; }
        public string pdob { set; get; }
        public string ptypeofoperation { get; set; }
        public object precordid { set; get; }
        public object relationshipid { set; get; }
        public string relationshipname { set; get; }
        public string pname { set; get; }
        public object page { set; get; }
        public object pgender { set; get; }
        public object pmaritialstatus { set; get; }
        public object qualificationid { set; get; }
        public object qualificationname { set; get; }
        public object poccupation { set; get; }
        public object pphoneno { set; get; }
    }
    public class ContactEmployeeDTO : PersonalDetailsDTO
    {
        public object precordid { set; get; }
        public object pContactName { get; set; }
        public object pContactId { get; set; }
        public object pEmployeeId { get; set; }
        public object pEmployeeName { get; set; }
        public object pEmployeeSurName { get; set; }
        public object pEmployeeTitleName { get; set; }
        public object mdesignationname { set; get; }

        // KYC Details
        public List<documentstoreDTO> pListEmpKYC { get; set; }
        //  public List<documentstoreDTO> pListDocStore { get; set; }
        // Employee Details
        public object pEmploymentBasicSalary { get; set; }
        public object pEmploymentAllowanceORvda { get; set; }
        public object pEmploymentDesignation { get; set; }
        public object pEmploymentRoleId { get; set; }
        public object pEmploymentRoleName { get; set; }
        public string pEmploymentJoiningDate { get; set; }
        public object pEmploymentCTC { get; set; }
        // Family Details
        public List<familyDetailsDTO> pListFamilyDetails { get; set; }
        public List<familyDetailsDTO> plstemployess { set; get; }
        // Bank Details
        public List<EmployeeBankDetails> pListEmpBankDetails { get; set; }
        // Main Employee Grid Data
        public object pEmployeeContactNo { get; set; }
        public object pEmployeeContactEmail { get; set; }
        public object pContactRefNo { get; set; }
        public object pMainTransactionType { get; set; }
        public object pdesignation { get; set; }
        public object pCountryId { get; set; }
        public object pCountry { get; set; }
        public object mdesignationid { get; set; }
        public object pdisciplinaryactions { get; set; }
        public object pextracurricularactivities { get; set; }
        public object pemployeecode { get; set; }
    }
    //public class RelationShipDTO : CommonDTO
    //{
    //    public object relationshipid { get; set; }
    //    public object relationshipname { get; set; }
    //    public object status { get; set; }
    //}
    public class QualificationDTO : CommonDTO
    {
        public object qualificationid { get; set; }
        public object qualificationname { get; set; }
    }
    public class RelationShipDTO : CommonDTO
    {
        public object relationshipid { get; set; }
        public object relationshipname { get; set; }
        public object status { get; set; }
    }
}
