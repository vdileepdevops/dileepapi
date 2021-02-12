using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Reports
{
    public class LAReportsDTO : CommonDTO
    {
        public object pCashflowSno { get; set; }

        public object pperticulars { get; set; }

        public object pcashamtsummary { get; set; }
        public object pmembername { get; set; }

        public object pmemberid { get; set; }

        public object pcompanyname { get; set; }
        public object pbranch { get; set; }
        public object pSchemename { get; set; }
        public object pSchemeId { get; set; }
        public object pFdAccNo { get; set; }
        public object pFdAccNoMembername { get; set; }

        public object pContactIdsummary { get; set; }
        public object pAgentnamesummary { get; set; }
        public object pAmountsummary { get; set; }

        public object pPointsummary { get; set; }

        public List<MaturityIntimationDTO> pmaturityintimationlist { set; get; }
        public List<AgentPointsDetailsDTO> lstAgentpontsdetails { get; set; }
        public List<CashFlowDetailsDTO> lstCashflowdetails { get; set; }

    }
    public class TargetReportDTO
    {
        public object preceiptamountsummary { get; set; }
        public object pprematureamoutsummary { get; set; }
        public object pbranchsummary { get; set; }
        public object pachivedsummary { get; set; }
        public object pTargetmonth { get; set; }
        public object pAccountno { get; set; }
        public object pAccountname { get; set; }
        public object pReceiptamount { get; set; }
        public object pPreMaturityamount { get; set; }
        public object pbranch { get; set; }
        public object pReceiptDate { get; set; }

        public object pClearDate { get; set; }

        public object pReceipttype { get; set; }
       

    }
    public class CashFlowPerticularDetailsDTO
    {
        public object pTransdate { get; set; }
        public object pAccountname { get; set; }
        public object pParentaccountname { get; set; }
        public object pCreditamt { get; set; }
        public object pDebitamount { get; set; }
        public object pPerticulars { get; set; }
        public object pTransactionno { get; set; }
      

    }
    public class CashFlowDetailsDTO
    {
        public object pMonthname { get; set; }
        public object pFdaccountno { get; set; }
        public object pChitbranchname { get; set; }
        public object pMembername { get; set; }
        public object pSchemename { get; set; }
        public object pTenure { get; set; }
        public object pInterestpayout { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestpayable { get; set; }
        public object pMaturityamount { get; set; }
        public object pMonthlyint { get; set; }
        public object pDepositDate { get; set; }
        public object pMaturityDate { get; set; }
        public object pSquareyard { get; set; }
        public object pCaltype { get; set; }
        public object pInterestrate { get; set; }
        
    }
    public class AgentPointsDetailsDTO
        {
            public object pReceiptFromDate { get; set; }
            public object pReceiptToDate { get; set; }
            public object pChequeFromDate { get; set; }
            public object pChequeToDate { get; set; }


            public object pContactid { get; set; }
            public object pAgentname { get; set; }
            public object pReceiptamount { get; set; }
            public object pClearDate { get; set; }
            public object pFdaccountno { get; set; }
            public object pPonts { get; set; }
            public object pMembername { get; set; }
            public object pAccountname { get; set; }
            public object pBranch { get; set; }
            public object pReceiptDate { get; set; }
            public object pReceipttype { get; set; }
        }
       
    public class MaturityIntimationDTO
    {
        public object pmembername { get; set; }
        public object pfdaccountno { get; set; }

        public object pschemename { get; set; }
        public object pbranchname { get; set; }
        public object pfdname { get; set; }
        public object ptenortype { get; set; }
        public object ptenor { get; set; }
        public object pdepositamount { get; set; }
        public object pinterestrate { get; set; }
        public object pmaturityamount { get; set; }
        public object pdepositdate { get; set; }
        public object pmaturitydate { get; set; }

    }

    public class LienReleaseDTO
    {
        public object pliendate { get; set; }
        public object plienreleasedate { get; set; }
        public object plienamount { get; set; }
        public object pmembername { get; set; }
        public object pfdaccountno { get; set; }

        public object pschemename { get; set; }
        public object pbranchname { get; set; }
        public object pfdname { get; set; }
        public object ptenortype { get; set; }
        public object ptenor { get; set; }
        public object pdepositamount { get; set; }
        public object pinterestrate { get; set; }
        public object pmaturityamount { get; set; }
        public object pdepositdate { get; set; }
        public object pmaturitydate { get; set; }

    }

    public class SelfAdjustmentDTO
    {
        public object pmembername { get; set; }
        public object pfdaccountno { get; set; }

        public object pschemename { get; set; }
        public object pbranchname { get; set; }
        public object pfdname { get; set; }
        public object ptenortype { get; set; }
        public object ptenor { get; set; }
        public object pdepositamount { get; set; }
        public object pinterestrate { get; set; }
        public object pmaturityamount { get; set; }
        public object pdepositdate { get; set; }
        public object pmaturitydate { get; set; }

    }

    public class MaturityTrendDTO
    {

        public object pschemename { get; set; }
        public object pCount { get; set; }
        public object pcurrentmonth { get; set; }
        public object pcurrentmonth1 { get; set; }
        public object pcurrentmonth2 { get; set; }
        public object pcurrentmonth3 { get; set; }
        public object pcurrentmonth4 { get; set; }
        public object pcurrentmonth5 { get; set; }
        public object pcurrentmonth6 { get; set; }
        public object pcurrentmonth7 { get; set; }
        public object pcurrentmonth8 { get; set; }
        public object pcurrentmonth9 { get; set; }
        public object pcurrentmonth10 { get; set; }
        public object pcurrentmonth11 { get; set; }
        public object pcurrentmonth12 { get; set; }



    }
    public class MaturityTrendDetailsDTO
    {
        public object pSchemename1 { get; set; }
        public object pFdAccountNo { get; set; }
        public object pTransdate { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pTenor { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pMaturitydate { get; set; }
    }

    public class InterestPaymentTrendDTO
    {

        public object pschemename { get; set; }
        public object pCount { get; set; }
        public object pcurrentmonth { get; set; }
        public object pcurrentmonth1 { get; set; }
        public object pcurrentmonth2 { get; set; }
        public object pcurrentmonth3 { get; set; }
        public object pcurrentmonth4 { get; set; }
        public object pcurrentmonth5 { get; set; }
        public object pcurrentmonth6 { get; set; }
        public object pcurrentmonth7 { get; set; }
        public object pcurrentmonth8 { get; set; }
        public object pcurrentmonth9 { get; set; }
        public object pcurrentmonth10 { get; set; }
        public object pcurrentmonth11 { get; set; }
        public object pcurrentmonth12 { get; set; }



    }

    public class InterestPaymentTrendDetailsDTO
    {
        public object pSchemename1 { get; set; }
        public object pMonth { get; set; }
        public object pFdAccountNo { get; set; }
        public object pTransdate { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pTenor { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pMaturitydate { get; set; }

        public object pInterestamount { get; set; }
        public object pTdsamount { get; set; }
        public object pTotalinterestamount { get; set; }
    }

    public class PreMaturityDetailsDTO
    {

        public object pSchemename { get; set; }
        public object pFdAccountNo { get; set; }
        public object pTransdate { get; set; }
        public object pMembername { get; set; }
        public object pTenor { get; set; }
        public object pDepositamount { get; set; }
        public object pDepositdate { get; set; }
        public object pInterestrate { get; set; }

        public object pInterestPayable { get; set; }

        public object pMaturityamount { get; set; }
        public object pMaturitydate { get; set; }

        public object pChitbranchname { get; set; }
        public object pTranstype { get; set; }
        public object pMaturityJVDate { get; set; }
        public object pMaturityPaymentDate { get; set; }
        public object pPaidAmount { get; set; }

        public object pPreMaturityDate { get; set; }
        public object pPreMaturityAmt { get; set; }


    }

    public class MemberwiseReceiptsDTO
    {
        public object pReceiptVoucherno { get; set; }

        public object pChequeStatus { get; set; }
        public object pFDReceiptDate { get; set; }

        public object pReceiptDate { get; set; }
        public object pDeposittype { get; set; }
        public object pInstallmentamount { get; set; }
        public object pReceivedamount { get; set; }
        public object pModeofReceipt { get; set; }

        public object pStatus { get; set; }

        public object pSchemename { get; set; }
        
        public object pFdAccountNo { get; set; }
        public object pMembername { get; set; }
        public object pDepositamount { get; set; }
        public object pAgentName { get; set; }
        public object pCommissionValue { get; set; }
        public object pDepositdate { get; set; }

       
    }
    public class InterestreportDTO
    {
        public object pMemberid { get; set; }
        public object pMembername { get; set; }
        public object pAdvanceacno { get; set; }
        public object pInterestamount { get; set; }
        public object pInterestPayable { get; set; }
        public object pTdsamount { get; set; }
        public object pPancard { get; set; }
        public object pMonth { get; set; }

    }
    public class PrintMaturityTrendDetailsDTO
    {
        public object pMonth { get; set; }
        public object pSchemename1 { get; set; }
        public object pFdAccountNo { get; set; }
        public object pTransdate { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pTenor { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pMaturitydate { get; set; }
    }

    public class PrintInterestPaymentTrendDetailsDTO
    {
        public object pSchemename1 { get; set; }
        public object pMonth { get; set; }
        public object pFdAccountNo { get; set; }
        public object pTransdate { get; set; }
        public object pMembercode { get; set; }
        public object pMembername { get; set; }
        public object pTenor { get; set; }
        public object pDepositamount { get; set; }
        public object pInterestrate { get; set; }
        public object pMaturityamount { get; set; }
        public object pMaturitydate { get; set; }

        public object pInterestamount { get; set; }
        public object pTdsamount { get; set; }
        public object pTotalinterestamount { get; set; }
    }
    public class ApplicationFormDTO
    {
        public Int64 pFdaccountid { get; set; }
        public string pFdAccountNo { get; set; }
        public string pMobileNo { get; set; }
    }
    public class ApplicationFormDetailsDTO
    {
        public string pFdAccountNO { get; set; }
        public string pTitleName { get; set; }
        public string pMemberName { get; set; }
        public string pAge { get; set; }
        public string pDob { get; set; }
        public string pGender { get; set; }
        public string pMobileNo { get; set; }
        public string pMailId { get; set; }
        public string pMemberAddress { get; set; }
        public string pBranchName { get; set; }
        public string pNominee { get; set; }
        public string pReceiptDate { get; set; }
        public string pBankName { get; set; }
        public string pModeOfPayment { get; set; }
        public string pReceiptNo { get; set; }
        public string pBankBranchName { get; set; }
        public string pCompanyName { get; set; }
        public string pCompanyAddress { get; set; }
    }


}

