using FinstaInfrastructure.Loans.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{


    public class SavingsTransactionDataEdit : SavingAccountTransactionSave
    {
       
        public List<jointdetails> JointMembersandContactDetailsList { get; set; }
        public List<NomineeDetails> MemberNomineeDetailsList { get; set; }
        public Referrals MemberReferalDetails { get; set; }
       
    }
    public class SavingAccountTransactionDTO
    {
        public long pSavingaccountid { get; set; }
        public string pSavingaccountno { get; set; }
        public SavingAccountTransactionSave SavingAccountTransactionlist { set; get; }
        public List<jointdetails> JointMembersandContactDetailsList { set; get; }
        public List<NomineeDetails> MemberNomineeDetailsList { set; get; }
        public Referrals MemberReferalDetails { set; get; }
    }
    public class SavingAccountConfigBind
    {
        public long pSavingconfigid { get; set; }
        public string pSavingaccname { get; set; }
        public string pSavingaccnamecode { get; set; }
    }

    public class SavingAccountConfigDetailsBind
    {
        public string pSavingaccnamecode { get; set; }
        public decimal? pMinopenamount { get; set; }
        public string pInterestpayout { get; set; }
        public bool pIssavingspayinapplicable { get; set; }
        public string pSavingspayinmode { get; set; }
        public decimal? pSavingmindepositamount { get; set; }
        public decimal? pSavingmaxdepositamount { get; set; }
        public bool pIswithdrawallimitapplicable { get; set; }
        public decimal? pMaxwithdrawallimit { get; set; }
        public decimal? pMinmaintainbalance { get; set; }
        
    }
    public class ContactDetails
    {
        public long pContactid { get; set; }
        public string pContacttype { get; set; }
        public string pContactreferenceid { get; set; }
        public string pContactno { get; set; }
    }
    public class MemberDetails
    {
        public long pMemberid { get; set; }
        public string pMembercode { get; set; }
        public string pMembername { get; set; }
        public string pContacttype { get; set; }
    }
    public class SavingAccountTransactionIdAndNo
    {
        public long pSavingaccountid { get; set; }
        public string pSavingaccountno { get; set; }
    }
    public class SavingAccountTransactionSave
    {
        public long pSavingaccountid { get; set; }
        public string pSavingaccountno { get; set; }
        public string pTransdate { get; set; }
        public long pMembertypeid { get; set; }
        public string pMembertype { get; set; }
        public long pMemberid { get; set; }
        public string pApplicanttype { get; set; }
        public string pMembercode { get; set; }
        public string pMembername { get; set; }
        public long pContactid { get; set; }
        public string pContacttype { get; set; }
        public string pContactreferenceid { get; set; }
        public long pSavingconfigid { get; set; }
        public string pSavingaccname { get; set; }
        public decimal? pSavingsamount { get; set; }
        public string pSavingsamountpayin { get; set; }
        public string pInterestpayout { get; set; }
        public string pInterestcompound { get; set; }
        public decimal? pMinsavingamount { get; set; }
        public bool pIsjointapplicable { get; set; }
        public bool pIsreferralapplicable { get; set; }
        public string pBondprintstatus { get; set; }
        public string pAccountstatus { get; set; }
        public string pTokenno { get; set; }
        public long pStatusid { get; set; }
        public long pCreatedby { get; set; }
        public string ptypeofoperation { get; set; }
        public bool pIsNomineesapplicable { get; set; }
    }

    public class JointMembers
    {
        public long pRecordid { set; get; }
        public long pSavingaccountid { set; get; }
        public string pSavingaccountno { set; get; }
        public long pMemberid { set; get; }
        public string pMembercode { set; get; }
        public string pMembername { set; get; }
        public long pContactid { set; get; }
        public string pContacttype { set; get; }
        public string pContactreferenceid { set; get; }
        public long pStatusid { get; set; }
        public long pCreatedby { get; set; }
        public string ptypeofoperation { get; set; }
    }
    public class JointmemberAndNomineeSave
    {
        public List<JointMembers> JointMembersList { get; set; }
        public List<ApplicationPersonalNomineeDTO> PersonalNomineeList { get; set; }
      
        public long pCreatedby { set; get; }
        public string pvchapplicationid { set; get; }
        public long papplicationid { set; get; }
        public bool pIsjointapplicable { set; get; }
    }

    public class ReferralSave
    {
        public long pRecordid { set; get; }
        public long pSavingaccountid { set; get; }
        public string pSavingaccountno { set; get; }
        public long pReferralid { set; get; }
        public string pReferralname { set; get; }
        public string pSalespersonname { set; get; }
        public decimal? pCommsssionvalue { set; get; }
        public long pStatusid { get; set; }
        public long pCreatedby { get; set; }
        public bool pIsreferralapplicable { set; get; }
        public string ptypeofoperation { get; set; }
    }
    public class SavingAccountTransactionMainGrid
    {
        public long pSavingaccountid { set; get; }
        public string pSavingaccountno { set; get; }
        public string pMembertype { get; set; }
        public string pApplicanttype { get; set; }
        public string pTransdate { get; set; }
        public string pContacttype { get; set; }
        public string pMembername { get; set; }
        public string pSavingaccname { get; set; }
    }
    }
