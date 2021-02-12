using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;

namespace FinstaInfrastructure.Banking.Masters
{
    public class FIMemberDTO : CommonDTO
    {
        public FiMemberContactDetails _FiMemberContactDetailsDTO { get; set; }
        public FIMemberPersonalInformationDTO _FIMemberPersonalInformationDTO { get; set; }
        public FIMemberKYCDocumentsDTO _FIMemberKYCDocumentsDTO { get; set; }
        public List<FIMemberReferencesDTO> lobjAppReferences { get; set; }
        public FiMemberReferralDTO _FiMemberReferralDTO { get; set; }
        public string pMemberReferenceid { get; set; }
        public long pMemberId { set; get; }       
        public bool ispersonaldetailsapplicable { get; set; }
    }

    public class FIMeberRefIdAndID
    {
        public string pMemberReferenceId { get; set; }
        public long pMemberId { get; set; }
        public string pDob { get; set; }
    }

    public class FIMembersaveReferences : CommonDTO
    {
        public List<FIMemberReferencesDTO> lobjAppReferences { get; set; }
        public string pMemberReferenceId { get; set; }
        public long pMemberId { get; set; }
        public bool pIsreferencesapplicable { get; set; }
    }
    public class FiMemberContactDetails : CommonDTO
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
    }
    public class FiMemberReferralDTO : CommonDTO
    {
        private string name = "Referrals";
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
        public bool pIsReferralApplicableorNot { set; get; }
        public bool IsReferralCommisionpaidforthisLoan { set; get; }
        public long pRefRecordId { get; set; }           
        public decimal? pCommisionPayoutAmountorPercentile { get; set; }
        public bool pIsTdsapplicable { get; set; }
        public string pTdstype { get; set; }
        public string pTdsSection { get; set; }
        public decimal? pTdsPercentage { get; set; }
        public decimal? pTdsamount { get; set; }
        public string pCommisionPayoutType { get; set; }
        public string pTDSAccountId { get; set; }
        public string pMemberReferenceId { get; set; }
        public long pMemberId { set; get; }
    }
    public class FIMemberReferencesDTO : CommonDTO
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
        public long pMemberId { get; set; }
        public string pMemberReferenceId { get; set; }
        public string pApplicanttype { get; set; }
        public string pRefFirstname { get; set; }
        public string pRefLastname { get; set; }
        public decimal? pRefcontactNo { get; set; }
        public decimal? pRefalternatecontactNo { get; set; }
        public string pRefEmailID { get; set; }
        public string pRefAlternateEmailId { get; set; }        
    }
    public class FIMemberKYCDocumentsDTO : CommonDTO
    {
        public long pApplicationid { set; get; }
        public string pVchapplicationid { set; get; }
        public Boolean pisKYCapplicable { get; set; }       
        public List<documentstoreDTO> documentstorelist { get; set; }
    }

    //public class documentstoreDTO : KYCDocumentsDTO
    //{
    //    private string name = "KYC DocumentsDTO Details";
    //    public string SectionName
    //    {
    //        get
    //        {
    //            return name;
    //        }
    //        set
    //        {
    //            name = value;
    //        }
    //    }
    //    public string pVerificationoperation { set; get; }

    //    public string psubsectionname { set; get; }
    //    public string IsVerified { set; get; }
    //    public Int64 pDocstoreId { get; set; }
    //    public Int64 pContactId { get; set; }
    //    public Int64 pLoanId { get; set; }
    //    public Int64 pApplicationNo { get; set; }
    //    public string pName { get; set; }
    //    public string pContactType { get; set; }
    //    //public string ptypeofoperation { get; set; }
    //    public string pTransactionType { get; set; } // For CRUD Operation Status
    //    public string pApplicanttype { get; set; }

    //    public string pContactreferenceid { get; set; }
    //    public Boolean pisapplicable { get; set; }
    //    public Int64 pCreatedby { get; set; }


    //}
    
    public class FIMemberPersonalInformationDTO : CommonDTO
    {
        public string pMemberReferenceId { get; set; }
        public long pMemberId { get; set; }
        public Boolean pIspersonaldetailsapplicable { get; set; }
        public List<FIMemberPersonalDetailsDTO> _FIMemberPersonalDetailsList { get; set; }
        public List<FIMemberEmployeementDTO> _FIMemberEmployeementList { get; set; }
        public List<FIMemberPersonalFamilyDTO> _FIMemberPersonalFamilyList { get; set; }
        public List<FIMemberPersonalNomineeDTO> _FIMemberPersonalNomineeList { get; set; }
        public List<FIMemberPersonalBankDTO> _FIMemberPersonalBankList { get; set; }
        public List<FIMemberPersonalIncomeDTO> _FIMemberPersonalIncomeList { get; set; }
        public List<FIMemberPersonalOtherIncomeDTO> _FIMemberPersonalOtherIncomeList { get; set; }
        public List<FIMemberPersonalEducationDTO> _FIMemberPersonalEducationList { get; set; }
        public long pContactId { get; set; }
    }   
    public class FIMemberEmployeementDTO : CommonDTO
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
    public class FIMemberPersonalDetailsDTO : CommonDTO
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
    public class FIMemberPersonalFamilyDTO : CommonDTO
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
    public class FIMemberPersonalNomineeDTO : CommonDTO
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
    public class FIMemberPersonalBankDTO : BankDetailsDTO
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
    public class FIMemberPersonalIncomeDTO : CommonDTO
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
    public class FIMemberPersonalOtherIncomeDTO : CommonDTO
    {
        public bool pisapplicable { set; get; }
        public long precordid { set; get; }
        public long pcontactid { set; get; }
        public string papplicanttype { set; get; }

        public string pcontactreferenceid { set; get; }
        public string psourcename { set; get; }
        public decimal pgrossannual { set; get; }
        
    }
    public class FIMemberPersonalEducationDTO : CommonDTO
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
    public class FIMembertypeDTO
    {
        public long pMembertypeId { get; set; }
        public string pMemberType { get; set; }
        public string pMemberTypeCode { get; set; }
    }
    public class FIApplicantTypeDTO
    {        
        public string pApplicantType { get; set; }
    }
    public class FIMemberandApplicantList
    {
        public List< FIMembertypeDTO> _FIMembertypeDTOList { get; set; }
        public List<FIApplicantTypeDTO> _FIApplicantTypeDTO { get; set; }
    }
    public class ContactDetailsDTO
    {
        public object pContactId { get; set; }
        public object pContactName { get; set; }
        public object pReferenceId { get; set; }
        public object pTitleName { get; set; }
        public object pBusinessEntitycontactNo { get; set; }
        public object pBusinessEntityEmailId { get; set; }
        public object pContactimagepath { get; set; }
        public object pFatherName { get; set; }
        public object pRoleid { get; set; }
        public object pRolename { get; set; }
    }
}
