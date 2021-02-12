using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.TDS
{
   public class ChallanaDTO
    {
        public object pParentId { get; set; }
        public object pAccountId { get; set; }
        public object pTdsVoucherId { get; set; }
        public object pTransDate { get; set; }
        public object pAccountName { get; set; }
        public object pPanNo { get; set; }
        public object pAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pActualTdsAmount { get; set; }
        public object pBalance { get; set; }
        public object pTdsSection { get; set; }
        public object pSectionName { get; set; }
        public object pPaidAmount { get; set; }
        public object pCompanyType { get; set; }
        public object pPanType { get; set; }
    }
    public class ChallanaEntryDTO
    {
        public object pChallanaId { get; set; }
        public object pChallanaNo { get; set; }
        public string pFromDate { get; set; }
        public string pToDate { get; set; }
        public object pCompanyType { get; set; }
        public object pTdsType { get; set; }
        public object pTotalTdsAmount { get; set; }
        public object pActualTotalTdsAmount { get; set; }
        public object pTotalPaidAmount { get; set; }
        public object ptypeofoperation { get; set; }
        public object pCompanyId { get; set; }

        public List<ChallanaEntryDetailsDTO> _ChallanaEntryDetails { get; set; }
    }
    public class ChallanaEntryDetailsDTO
    {
        public object pChallanaId { get; set; }
        public object pCompanyType { get; set; }
        public object pParentId { get; set; }
       
       
        public object pAccountId { get; set; }
        
        public object pPanNo { get; set; }
        public object pAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pActualTdsAmount { get; set; }
        public object pBalance { get; set; }
        public object pPaidAmount { get; set; }
        
        public object pTdsVoucherId { get; set; }


        public object pStatus { get; set; }
    }
    public class ChallanaNoDTO
    {
        public object pChallanaId { get; set; }
        public object pChallanaNo { get; set; }
        public object pFromdate { get; set; }
        public object pTodate { get; set; }
        public string pSection { get; set; }
        
    }
    public class ChallanaDetailsDTO
    {
        public object pChallanaDetailsId { get; set; }
        public object pChallanaNo { get; set; }
        public object pTdsVoucherId { get; set; }
        public object pParentId { get; set; }
        public object pAccountId { get; set; }
        public object pAccountName { get; set; }

        public object pPanNo { get; set; }
        public object pAmount { get; set; }
        public object pTdsAmount { get; set; }
        public object pActualTdsAmount { get; set; }
        public object pBalance { get; set; }
        public object pPaidAmount { get; set; }
        public object pVoucherId { get; set; }


    }
    public class ChallanaPaymentDTO 
    {

        public string ppaymentid { get; set; }
        public string pCommissionpaymentDate { get; set; }
       
        public object pTotalpaymentamount { get; set; }
        public string pNarration { get; set; }

        public string ptranstype { get; set; }
       
        public string pBankname { get; set; }
        public object pbankid { get; set; }
        public string pbankname { get; set; }
        public object pbranchname { get; set; }
        public object pchequeno { get; set; }

        public string ptypeofpayment { get; set; }
        public object preferencenoonline { get; set; }
        public object pUpiname { get; set; }
        public object pUpiid { get; set; }

        public object pdebitcard { get; set; }
        
        public object preferencenodcard { get; set; }
        public object pBankbranchname { get; set; }
        public object pChequeno { get; set; }
        public object pReferenceno { get; set; }
        public object pUpiId { get; set; }
        public object pDebitcard { get; set; }
        public object pCreatedby { get; set; }


        public List<ChallanaPaymentDetailsDTO> pcommissionpaymentlist { set; get; }
       

    }
    public class ChallanaPaymentDetailsDTO
    {
       
        public object pdebitaccountid { get; set; }
        public long pChallanaDetailsId { get; set; }
        public object ptotalamount { get; set; }

    }
    public class ChallanaPaymentSaveDTO
    {
        public object pvoucherid { get; set; }
    }
    public class GetChallanaPaymentsDTO
    {
        public object pVoucherno { get; set; }
        public object pPaymentdate { get; set; }
        public object pBankname { get; set; }
        public object pChequenumber { get; set; }
        public List<ChallanaDetailsDTO> ChallanaPaymentList { get; set; }

    }

    public class CinEntryDTO
    {
        public object pChallanaId { get; set; }
        public object pVoucherId { get; set; }
        public object pReferenceNo { get; set; }
        public object pPaidDate { get; set; }
        public object pPaidBank { get; set; }
        public object pBsrCode { get; set; }
        public object pChallanaSNO { get; set; }
        public object pChallanaBank { get; set; }
        public object pChallanaDate { get; set; }
        public object ptypeofoperation { get; set; }
    }

    public class CinEntryReportDTO
    {
        public object pChallanaNo { get; set; }
        public object pPanNo { get; set; }
        public object pName { get; set; }
        public object pPaidDate { get; set; }
        public decimal pAmount { get; set; }
        public decimal pActualTdsAmount { get; set; }
        public object pChallanaDate { get; set; }
        public object pBsrCode { get; set; }
        public object pChallanaSNO { get; set; }
        public object pChallanaBank { get; set; }
        public object pReferenceNo { get; set; }        
        public object pPaidBank { get; set; }
        public object pFromdate { get; set; }
        public object pTodate { get; set; }
        public object pSection { get; set; }
        public decimal pTdsAmount { get; set; }

    }
    public class ChallanaPaymentReportDTO
    {
        public object pChallanaNo { get; set; }
        public object pTanNo { get; set; }
        public object pCompanyType { get; set; }
        public object pCompanyName { get; set; }
        public object pAddress { get; set; }
        public object pSection { get; set; }
        public decimal pActualTdsAmount { get; set; }
        public object pCurrentYear { get; set; }
        public object pNextYear { get; set; }
       
    }
}
