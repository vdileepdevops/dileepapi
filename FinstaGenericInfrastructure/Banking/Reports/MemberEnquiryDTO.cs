using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Reports
{
    public class MemberEnquiryDTO
    {
        public string pContactImagePath { get; set; }

        public List<MemberDetailsDTO> LstMemberDetails { get; set; }
        public List<MemberBankDetailsDTO> LstMemberBankDetails { get; set; }
        public List<MemberTransactionDetailsDTO> LstMemberTransactionDetails { get; set; }
        public List<MemberReceiptDTO> LstMemberReceiptDetails { get; set; }
        public List<MemberNomineeDetailsDTO> LstMemberNomieeDetails { get; set; }
        public List<MemberInterestPaymentDTO> LstMemberInterestPaymentDetails { get; set; }
        public List<MemberPromoterSalaryDTO> LstMemberPromoterSalarytDetails { get; set; }
        public List<MemberLiensDTO> LstMemberLientDetails { get; set; }
        public List<MemberMaturityBondDTO> LstMemberMaturityBondDetails { get; set; }
        public List<MaturityPaymentsDTO> LstMemberMaturityPaymentsDetails { get; set; }

    }
    public class MemberDetailsDTO
    {
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pMobileno { get; set; }
        public object pAddress { get; set; }
        public object pFathername { get; set; }
    }
    public class MemberTransactionDTO
    {
        public string pContactImagePath { get; set; }
        public object pImage { get; set; }
        public List<MemberBankDetailsDTO> LstMemberBankDetails { get; set; }
        public List<MemberTransactionDetailsDTO> LstMemberTransactionDetails { get; set; }
    }
    public class MemberBankDetailsDTO
    {
        public object pBankname { get; set; }
        public object pBranch { get; set; }
        public object pAccountno { get; set; }
        public object pIfsccode { get; set; }

    }
    public class MemberTransactionDetailsDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMembername { get; set; }
        public object pContacttype { get; set; }
        public object pApplicanttype { get; set; }
        public object pSchemeid { get; set; }
        public object pSchemename { get; set; }
        public object pChitbranchid { get; set; }
        public object pChitbranchname { get; set; }
        public object pDepositamount { get; set; }
        public object pTenure { get; set; }
        public object pInteresttype { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pInterestpayable { get; set; }
        public object pFdcalculationmode { get; set; }
        public object pInterestpayout { get; set; }
        public object pPaidamount { get; set; }
        public object pReceivedamount { get; set; }
        public object pBalanceamount { get; set; }
        public object pPendingchequeamount { get; set; }
        public object pTransactiondate { get; set; }
        public object pDepositdate { get; set; }
        public object pMaturitydate { get; set; }

    }
    public class MemberReceiptDTO
    {
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pFdaccountno { get; set; }
        public object pFdreceipttdate { get; set; }
        public object pAdvanceamount { get; set; }
        public object pReceivedamount { get; set; }
        public object pModeofreceipt { get; set; }
        public object pReceiptno { get; set; }
        public object pChequestatus { get; set; }


    }
    public class MemberNomineeDetailsDTO
    {
        public object pNomineename { get; set; }
        public object pContactno { get; set; }
        public object pRelationship { get; set; }
        public object pIdproofname { get; set; }
        public object pReferencenumber { get; set; }
        public object pProportion { get; set; }

    }
    public class MemberInterestPaymentDTO
    {
        public object pFdaccountno { get; set; }
        public object pSchemeName { get; set; }
        public object pPaymentType { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pTotalAmount { get; set; }
        public object pInterestpaidmonth { get; set; }
        public object pPaymentdate { get; set; }
        public object pPaymentid { get; set; }

    }
    public class MemberPromoterSalaryDTO
    {
        public object pTransdate { get; set; }
        public object pCommissiontransdate { get; set; }
        public object pMembername { get; set; }
        public object pFdaccountno { get; set; }
        public object pCommissionpaymentid { get; set; }
        public object pCommssionAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pTotalAmount { get; set; }
        public object pDepositamount { get; set; }
        public object pTenure { get; set; }
        public object pAgentname { get; set; }
        public object pCommsssionvalue { get; set; }
        public object pPaymentid { get; set; }
        public object pPaymentdate { get; set; }


    }
    public class MemberLiensDTO
    {
        public object pLiendate { get; set; }
        public object pMembername { get; set; }
        public object pDepositdate { get; set; }
        public object pDepositAmount { get; set; }
        public object pFdaccountno { get; set; }
        public object pCompanybranch { get; set; }
        public object pLienAdjustTo { get; set; }
        public object pLienAmount { get; set; }
        public object pLienstatus { get; set; }
        public object pTenure { get; set; }
        public object pInterestrate { get; set; }
        public object pInterestpayable { get; set; }
        public object pInteresttype { get; set; }


    }
    public class MemberMaturityBondDTO
    {
        public object pMembername { get; set; }
        public object pTranstype { get; set; }
        public object pFdaccountno { get; set; }
        public object pTransdate { get; set; }
        public object pMatureamount { get; set; }
        public object pInterestpayble { get; set; }
        public object pNetpayable { get; set; }
        public object pMaturitytype { get; set; }
        public object pDepositamount { get; set; }
        public object pTenure { get; set; }
        public object pSchemeName { get; set; }
        public object pAgentCommssionPayable { get; set; }
        public object pDamages { get; set; }
        public object pInteresPaid { get; set; }

    }
    public class MaturityPaymentsDTO
    {
        public object pMembername { get; set; }
        public object pMembercode { get; set; }
        public object pFdaccountno { get; set; }
        public object pDepositdate { get; set; }
        public object pDepositamount { get; set; }
        public object pTenure { get; set; }
        public object pMaturityamount { get; set; }
        public object pInterestpayout { get; set; }
        public object pPaymentType { get; set; }
        public object pMaturitypaymentdate { get; set; }
        public object pPaidAmount { get; set; }
        public object pVoucherid { get; set; }
        public object pPaymentdate { get; set; }
        public object pModeofpayment { get; set; }
    }
}
