using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Accounting;
using FinstaInfrastructure.Common;
using FinstaInfrastructure.Loans.Transactions;

namespace FinstaInfrastructure.Settings.Users
{
    public class ContactDataDTO
    {
        public long pGuratorCount { set; get; }
        public long pLoansCount { set; get; }
        public long pCoApplicantCount { set; get; }

        public object pPartyStatus { set; get; }
        public object pEmployeeStatus { set; get; }
        public object pReferralStatus { set; get; }
        public object pAdvacateStatus { set; get; }

        public List<ContactBankDetaisDTO> lstContactBankDetaisDTO { set; get; }
        public List<KycDocDTO> lstKycDocDTO { set; get; }
        public List<ContactPersonalDetailsDTO> contactpersonaldetailslist { set; get; }
        public List<ContactNomineeDetailsDTO> contactnomineedetailslist { set; get; }
        public ContactViewDTO ContactViewDTO { set; get; }
        public List<FirstinformationDTO> applicantdetailslist { get; set; }
        public List<FirstinformationDTO> gurantordetailslist { get; set; }
        public List<FirstinformationDTO> coapplicantdetailslist { get; set; }
        public List<AccountReportsDTO> partydetailslist { get; set; }
        public RefferalDetailsDTO referraldetails { get; set; }
        public List<FirstinformationDTO> referralloansdetailslist { get; set; }
        public List<FirstinformationDTO> advacatedetailslist { get; set; }
        public EmployeeDetailsDTO employeedetailslist { get; set; }
        public List<CicScoreDetailsDTO> cicdetailslist { get; set; }

    }

    public class CicScoreDetailsDTO
    {
        public decimal pcicscorepercentage { get; set; }

        public object pciccompanyName { get; set; }
        public decimal pcicscore { get; set; }
        public decimal pcicmaxscore { get; set; }       
    }

    public class RefferalDetailsDTO
    {
       
        public decimal pbusinessamount { get; set; }
        public decimal pcommissionpaidamount { get; set; }

        public decimal pcommissindueamount { get; set; }
        public decimal ploansamount { get; set; }

        public decimal pfdamount { get; set; }
        public decimal prdamount { get; set; }

        public decimal psdamount { get; set; }
        public object pStatusName { get; set; }
    }

    public class EmployeeDetailsDTO
    {

        public object prolename { get; set; }
        public object pdesignation { get; set; }

        public object pdateofjoining { get; set; }
        public decimal pbasicsalary { get; set; }

        public decimal pallowanceorvariablepay { get; set; }
        public decimal ptotalcosttocompany { get; set; }

        public object pStatusName { get; set; }
    }

    public class ContactViewDTO
    {
        public long pContactdId { set; get; }
        public object pContactType { get; set; }
        public object pRefNo { get; set; }
        public object pContactName { get; set; }
        public object pFatherName { get; set; }
        public object pContactNumber { get; set; }
        public object pContactEmail { get; set; }
        public object pImagePath { get; set; }
        public List<string> pImage { get; set; }
        public object pAddresDetails { get; set; }
        public object pnetannualincome { get; set; }
        public object potherincome { get; set; }
        public object pContactDate { get; set; }
    }
    public class ContactBankDetaisDTO : BankDetailsDTO
    {
        public long pRecordid { set; get; }
    }

    public class ContactPersonalDetailsDTO
    {
        public object presidentialstatus { get; set; }
        public object pmaritalstatus { get; set; }

        public object pplaceofbirth { get; set; }

        public object pcountryofbirth { get; set; }
        public object pnationality { get; set; }

        public object pminoritycommunity { get; set; }

        public object pbusinessactivity { get; set; }
        public object pestablishmentdate { get; set; }
        public object pcommencementdate { get; set; }
        public object pcinnumber { get; set; }
        public object pgstinno { get; set; }

    }
    public class ContactNomineeDetailsDTO
    {
        public object pnomineename { get; set; }
        public object prelationship { get; set; }

        public object pdateofbirth { get; set; }

        public object pcontactno { get; set; }

    }

    public class KycDocDTO : KYCDocumentsDTO
    {


        public Int64 pDocstoreId { get; set; }
        public Int64 pContactId { get; set; }
        public Int64 pLoanId { get; set; }
        public Int64 pApplicationNo { get; set; }
        public object pName { get; set; }
        public object pContactType { get; set; }

        public object pTransactionType { get; set; } // For CRUD Operation Status
        public object pApplicanttype { get; set; }
        public Boolean pisapplicable { get; set; }

    }
}
