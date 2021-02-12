using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Masters
{   
    public class InsuranceMemberBind
    {
        public string pMemberCodeandName { get; set; }
        public long pMemberId { get; set; }
        public long Contactid { get; set; }
        public string pContactrefid { get; set; }
    }
    public class Viewmemberdetails
    {       
        public string pAccountholdername { get; set; }
        public long pMemberId { get; set; }
        public string pContactrefid { get; set; }
        public string pMemberrefcode { get; set; }
        public string pJoiningDate { get; set; }
        public string pMembertype { get; set; }
        public string pDateofbirth { get; set; }
        public string pAge { get; set; }
        public string pNomineeName { get; set; }
        public string pNomimneeRelation { get; set; }      
    }
    public class InsuranceschemeDetails
    {      
        public long InsuranceschemeId { get; set; }
        public string InsuranceschemeCode { get; set; }
        public decimal? Claimamount { get; set; }
        public string ClaimType { get; set; }
        public decimal? Premiumamount { get; set; }
        public string PremiumPayin { get; set; }
        public string InsuranceschemeDuration { get; set; }
        public bool IspremiumRefund { get; set; }
       
    }
    public class InsuranceMemberNomineeDetails 
    {
        public string pNomineeName { get; set; }
        public string pNomimneeRelation { get; set; }
       // public long pMemberId { get; set; }
        public long Contactid { get; set; }
        public string pContactrefid { get; set; }
       // public long pMemberID { get; set; }
        public string pMemberrefcode { get; set; }       
        public string ContactNo { get; set; }
        public string IdProofname { get; set; }
        public string Idproofpath { get; set; }
        public string IdproofReferenceNo { get; set; }
        public long pRecordid { get; set; }
        public string ptypeofoperation { get; set; }
        public long pCreatedby { get; set; }
        public bool pStatus { get; set; }
        public bool Isprimarynominee { get; set; }
        public int pAge { get; set; }
        public string Dateofbirth { get; set; }
        public string IdproofType { get; set; }
    }
    public class InsuranceMembersave
    {
        public string pInsuranceType { get; set; }
        public string pTransdate { get; set; }
        public string pMembertype { get; set; }
        public long pMembertypeId { get; set; }
        public string pMemberCodeandName { get; set; }
        public long pMemberId { get; set; }
        public string pSchemeName { get; set; }
        public long pSchemeId { get; set; }
        public string pPolicystartdate { get; set; }
        public string pPolicyenddate { get; set; }
        public string pPolicycoveragePeriod { get; set; }
        public List<InsuranceMemberNomineeDetails> _InsuranceMemberNomineeDetailsListSave { get; set; }
        public long pCreatedby { get; set; }
        public string ptypeofoperation { get; set; }
        public long pApplicanttypeId { get; set; }
        public string pApplicanttype { get; set; }
        public decimal? pPremiumamount { get; set; }
    }
    public class InsuranceMembersDataforMainGrid
    {
        public long pRecordid { get; set; }
        public long pMemberId { get; set; }
        public string pMembername { get; set; }
        public string pMembercode { get; set; }
        public decimal? Premiumamount { get; set; }       
        public string pPolicystartdate { get; set; }
        public string pPolicyenddate { get; set; }
        public string pPolicycoveragePeriod { get; set; }
        public string pNomineeName { get; set; }
        public string pMemberCodeandName { get; set; }
    }
    public class GetInsuranceMemberDataforEdit 
    {
        public long pRecordid { get; set; }
        public string pInsuranceType { get; set; }
        public string pTransdate { get; set; }
        public string pMembertype { get; set; }
        public long pMembertypeId { get; set; }
        public string pMemberCodeandName { get; set; }
        public long pMemberId { get; set; }
        public string pSchemeName { get; set; }
        public long pSchemeId { get; set; }
        public string pPolicystartdate { get; set; }
        public string pPolicyenddate { get; set; }
        public string pPolicycoveragePeriod { get; set; }
        public long Contactid { get; set; }
        public string pContactrefid { get; set; }
        public List<InsuranceMemberNomineeDetails> _InsuranceMemberNomineeDetailsEditList { get; set; }
        public InsuranceschemeDetails _InsuranceschemeDetailsEdit { get; set; }
        public string pApplicanttype { get; set; }
    }
    public class InsuranceSchemes
    {
        public long pInsurenceconfigid { get; set; }
        public string pInsurencename { get; set; }
    }

    public class Applicanttypesdata
    {       
        public string pApplicanttype { get; set; }
    }

}
