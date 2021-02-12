using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Masters;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class ShareApplicationDTO
    {
        public Int64 pshareapplicationid { set; get; }
        public string pShareAccountNo { set; get; }
        public string pApplicanttype { set; get; }
        public Int64 pmembertypeid { set; get; }
        public string pmembertype { set; get; }
        public Int64 pmemberid { set; get; }
        public string pmembercode { set; get; }
        public string pmembername { set; get; }
        public Int64 pcontactid { set; get; }
        public Int64 pshareconfigid { set; get; }
        public string psharename { set; get; }
        public string preferenceno { set; get; }
        public decimal? pfacevalue { set; get; }
        public Int64 pnoofsharesissued { set; get; }
        public Int64 pdistinctivefrom { set; get; }
        public Int64 pdistinctiveto { set; get; }
        public decimal? ptotalamount { set; get; }
        public string pTransdate { set; get; }
        public string pshareissuedate { set; get; }
        //public string pprintstatus { set; get; }
        public bool pismemberfeeapplicable { set; get; }
        public Int64 pCreatedby { set; get; }
        public long pShareaccountid { get; set; }
        public object pReceiptStatus { get; set; }
    }
    public class shareMembersDetails
    {
        public object pMemberName { get; set; }
        public object pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public object pContactrefid { get; set; }
        public object pContacttype { get; set; }
        public object pTypeofOperation { get; set; }
        public long precordid { get; set; }
        public object pContactnumber { set; get; }

    }
    public class shareconfigDetails
    {
        public long pShareconfigid { get; set; }
        public object pSharename{ set; get; }
        public object pApplicanttype { set; get; }

        public object pFacevalue { set; get; }
        public object pMinshare { set; get; }
        public object pMaxshare { set; get; }    



    }
    public class savejointandnomiee
    {
        public string paccounttype { get; set; }
        public long pAccountId { get; set; }
        public string paccountNo { get; set; }
        public List<jointdetails> JointDetailsList { get; set; }
        public List<NomineeDetails>  NomineeDetailsList { get; set; }
        public long pCreatedby { get; set; }
        public bool pIsjointapplicableorNot { get; set; }
        public bool pIsNomineesApplicableorNot { get; set; }
    }
    public class jointdetails
    {
        public string pMemberName { get; set; }
        public string pMemberCode { get; set; }
        public long pMemberId { get; set; }
        public long pContactid { get; set; }
        public string pContactrefid { get; set; }
        public string pContacttype { get; set; }
        public string pTypeofOperation { get; set; }
        public long precordid { get; set; }
        public string pContactnumber { set; get; }

    }
    public class NomineeDetails
    {
        public bool pisprimarynominee { set; get; }
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
        public string ptypeofoperation { set; get; }
        public decimal? pPercentage { get; set; }
        public string pMemberrefcode { get; set; }
        public bool pStatus { get; set; }
        public int pAge { get; set; }
    }

    public class Referrals
    {
        public string paccounttype { get; set; }
        public long pAccountId { get; set; }
        public string paccountNo { get; set; }
        public long pReferralId { get; set; }
        public string pReferralCode { get; set; }
        public long pContactId   { get; set; }
        public string pReferralname { get; set; }
        public long pEmployeeidId { get; set; }
        public string pSalesPersonName { get; set; }
        public decimal? pCommisionValue { get; set; }
        public string pCommissionType { get; set; }
        public string pTypeofOperation { get; set; }
        public long pCreatedby { get; set; }
        public bool pIsReferralsapplicable { get; set; }
        public long preferaldetailsid { get; set; }
    }
    public class shareComfigurationIdandName
    {
        public long pshareapplicationid { get; set; }
        public object pShareAccountNo { get; set; }
    }
}
