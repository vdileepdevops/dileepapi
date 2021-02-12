using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class MaturityPaymentDTO
    {

    }
    public class MemberTypesDTO
    {
        public object pMembertypeId { get; set; }
        public object pMemberType { get; set; }
    }
    public class MemberIdsDTO
    {
        public object pContactreferenceid { get; set; }
        public object pMembertypeids { get; set; }

    }
    public class DepositIdsDTO
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pContactno { get; set; }


    }
    public class MaturitybondsSave
    {
        public object pMaturitybondid { get; set; }
        public object pMemberid { get; set; }
        public object pTranstype { get; set; }
        public object pTranstypeid { get; set; }
        public object pTransdate { get; set; }
        public object pMatureamount { get; set; }
        public object pPreinterestrate { get; set; }
        public object pInterestpayble { get; set; }
        public object pAgentcommssionvalue { get; set; }
        public object pAgentcommssionPayable { get; set; }
        public object pDamages { get; set; }
        public object pInterestpaid { get; set; }
        public object pCommissionpaid { get; set; }
        public object pJv_id { get; set; }
        public object pMaturityType { get; set; }
        public object pNarration { get; set; }
        public object pNetpayble { get; set; }
        public object pStatus { get; set; }
        public object ptypeofoperation { get; set; }
        public object ptdsamount { get; set; }
        public object psuspenceamount { get; set; }

    }
    public class PreMaturedetailsDTO
    {
        public object Pnumid { get; set; }
        public object pBranch { get; set; }
        public object pMember { get; set; }
        public object pBondamount { get; set; }
        public object pBondDate { get; set; }
        public object pMatureDate { get; set; }
        public object pPrematureDate { get; set; }
        public object pPeriod { get; set; }
        public object pPredays { get; set; }
        public object pPeriodmode { get; set; }
        public object pRateofinterest { get; set; }
        public object pActualrateinterest { get; set; }
        public object pInterestpaidamt { get; set; }
        public object pCalcinterestamt { get; set; }
        public object pSubtotal { get; set; }
        public object pNetpay { get; set; }
        public object pPromotorsalary { get; set; }
        public object pTdsPayble { get; set; }
        public object pIntrestPayble { get; set; }
        public object pCommissionPaid { get; set; }
        public object pSuspenceAmount { get; set; }


    }
    public class MaturityBondViewDTO
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
        public object pFdname { get; set; }

    }
    public class MaturityPaymentsDTO
    {
        public object pMaturitypaymentid { get; set; }
        public object pTransType { get; set; }
        public object pTransTypeid { get; set; }
        public string pMaturityjvdate { get; set; }
        public decimal pPaidAmount { get; set; }
        public decimal pOutstandingAmount { get; set; }
        public decimal pLateFeeAmount { get; set; }
        
        public int pJvid { get; set; }
        public object pVoucherid { get; set; }
        public long pAccountno { get; set; }
    }
    public class MaturityPaymentSaveDTO
    {
        public string pMaturitypaymentdate { get; set; }
        public string ppaymentid { get; set; }
        public decimal ptotalpaidamount { get; set; }
        public string pnarration { get; set; }
        public string pmodofpayment { get; set; }
        public string pbankname { get; set; }
        public string pbranchname { get; set; }
        public string ptranstype { get; set; }
        public string pCardNumber { get; set; }
        public string pUpiname { get; set; }
        public string pUpiid { get; set; }
        public string ptypeofpayment { get; set; }
        public string pChequenumber { get; set; }
        public object pchequedate { get; set; }
        public long pbankid { get; set; }
        public int pCreatedby { get; set; }
        public object pStatusname { get; set; }
        public long pMemberid { get; set; }
        public string pMembername { get; set; }
        public string ptypeofoperation { get; set; }
        public object pPaymentType { get; set; }
        public object pNarration { get; set; }
        public object pStatus { get; set; }
        public List<MaturityPaymentsDTO> MaturityPaymentsList { get; set; }
    }
    public class MaturityMembersDTO
    {
        public object pMemberid { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pMobileno { get; set; }
        public object pContactid { get; set; }
        public object pContactrefid { get; set; }
    }
    public class MaturityBondstList
    {
        public object pFdaccountid { get; set; }
        public object pFdaccountno { get; set; }
        public object pTransdate { get; set; }
        public object pMatureAmount { get; set; }
        public object pPreinterestrate { get; set; }
        public object pInterestpayble { get; set; }
        public object pAgentcommssionvalue { get; set; }
        public object pAgentcommssionpayable { get; set; }
        public object pInterespaid { get; set; }
        public object pCommissionpaid { get; set; }
        public object pNetPayable { get; set; }
        public object pDepositamount { get; set; }
        public object pAccountno { get; set; }
        public object pMaturityType { get; set; }
        public object pDamages { get; set; }
        public object pLatefeedays { get; set; }
        public object pInterestAmountPerday { get; set; }
        public object pLateFeeAmount { get; set; }
        public object pPayamount { get; set; }
        public object pPaid_Amount { get; set; }
        public object pPending_Amount { get; set; }

    }
    public class MaturityPaymentDetailsViewDTO
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
    }
    public class MaturityRenewalSaveDTO
    {
        public object pPaymentType { set; get; }
        public object pnarration { get; set; }
        public object pStatus { get; set; }
        public List<MaturityPaymentsDTO> MaturityPaymentsList { get; set; }
        public FdMemberandSchemeSave _FdMemberandSchemeSave { get; set; }
    }
    public class LienEntryViewDTO
    {
        public long pLienid { set; get; }
        public string pLiendate { set; get; }
        public string pMembercode { set; get; }
        public string pFdaccountno { set; get; }

        public decimal pLienamount { set; get; }

        public decimal pLiencount { set; get; }
        public string pCompanyname { set; get; }
        public string pCompanybranch { set; get; }
        public string pLienadjuestto { set; get; }

        public string pReceipttype { set; get; }
        public string pLienstatus { set; get; }
        public bool pstatus { set; get; }
        public string pMembername { set; get; }
        public string pDepositdate { set; get; }
        public decimal pDepositamount { set; get; }

        public Int64 pCreatedby { set; get; }

        public string ptypeofoperation { set; get; }

    }

}
