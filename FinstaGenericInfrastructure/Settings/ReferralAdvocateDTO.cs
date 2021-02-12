using FinstaInfrastructure.Common;
using System;
using System.Collections.Generic;

namespace FinstaInfrastructure.Settings
{
    public class ReferralAdvocateDTO : ContactDTO
    {
        public List<documentstoreDTO> documentstorelist { get; set; }
        public List<referralbankdetailsDTO> referralbankdetailslist { get; set; }
        public referraltaxdetailsDTO referraltaxdetailslist { get; set; }
        //public string pContactName { get; set; }
        public Int64 pReferralId { get; set; }
        public string pAdvocateName { get; set; }
        public string pReferralName { get; set; }
        public Int64 pDocumentId { get; set; }
        public string pDocumentName { get; set; }
        public Int64 pRecordid { get; set; }
        public string pContactReferanceId { get; set; }
        public string pReferralCode { get; set; }
    }
    public class Upload
    {
        public string pDocStorePath { get; set; }
        public string pDocFileType { get; set; }
       // public IFormFile pUploadFile { get; set; }
    }
    public class DeleteDTO : CommonDTO
    {
        public Int64 pReferralId { get; set; }
    }
    public class documentstoreDTO : KYCDocumentsDTO
    {
        private string name = "KYC DocumentsDTO Details";
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
        public Int64 pDocstoreId { get; set; }
        public Int64 pContactId { get; set; }
        public Int64 pLoanId { get; set; }
        public Int64 pApplicationNo { get; set; }
        public string pName { get; set; }
        public string pContactType { get; set; }
        //public string ptypeofoperation { get; set; }
        public string pTransactionType { get; set; } // For CRUD Operation Status
        public string pApplicanttype { get; set; }

        public string pContactreferenceid { get; set; }
        public Boolean pisapplicable { get; set; }
        public Int64 pCreatedby { get; set; }


    }
    public class referralbankdetailsDTO : BankDetailsDTO
    {
        public Int64 pRefbankId { get; set; }
        public Int64 pReferralId { get; set; }
        public object pContactbankId { get; set; }
     
    }
    public class GstTyeDTO : CommonDTO
    {
        public string pGstType { get; set; }
        public Int64 pRecordid { get; set; }
    }
    public class TdsSectionDTO : CommonDTO
    {
        public bool istdsapplicable;

        public Int64 pRecordid { get; set; }
        public string pTdsSection { get; set; }
        public decimal pTdsPercentage { get; set; }
    }
    public class referraltaxdetailsDTO : TaxDetailsDTO
    {
        public Int64 pReferralId { get; set; }
        public Int64 pRefTaxId { get; set; }

    }
}