using FinstaInfrastructure.Common;
using FinstaInfrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class IntrestPaymentDTO : CommonDTO
    {
        public string ppaymentid { get; set; }
        public string pInterestPaymentDate { get; set; }
        public object pCode { get; set; }
        public object pBranchname { get; set; }
        public object pCompanyname { get; set; }
        public object pSchemeId { get; set; }
        public object pSchemename { get; set; }

        public string pAdjustmenttype { get; set; }
        public object pForthemonth { get; set; }

        public object pTotalpaymentamount { get; set; }
        public string pNarration { get; set; }

        public string pmodofPayment { get; set; }
        public string ptranstype { get; set; }
        public object pintrestpaidmonth { get; set; }
        public string pBankname { get; set; }
        public object pBankbranchname { get; set; }
      //  public object pChequeno { get; set; }
        


        public object pbankid { get; set; }
        public string pbankname { get; set; }
        public object pbranchname { get; set; }
        public object pchequeno { get; set; }

        public string ptypeofpayment { get; set; }
        public object preferencenoonline { get; set; }
        public object pUpiname { get; set; }
        public object pUpiid { get; set; }

        public object pdebitcard { get; set; }
        public object pfinancialservice { get; set; }
        public object preferencenodcard { get; set; }

        public List<IntrestPaymentDetailsDTO> pintrestpaymentlist { set; get; }
    }
    public class InterestPaymentSaveDTO
    {
        public object pvoucherid { get; set; }
    }
    public class IntrestPaymentDetailsDTO
    {
        public object pPaymentstatus { get; set; }
        public object pInterestpaymentid { get; set; }
        public object pSchemename { get; set; }
        public object pInterestpaidmonth { get; set; }
        public object pMembername { get; set; }
        public object pFdaccountno { get; set; }
        public object pIntrestamount { get; set; }
        public object pTdsamount { get; set; }
        public object ptotalamount { get; set; }
        public object pPaymenttype { get; set; }
        public object pdebitaccountid { get; set; }

        public object pbankname { get; set; }
        public object pbankbranch { get; set; }
        public object pdepositeamount { get; set; }
        public object pinterestrate { get; set; }
        public object pmaturiryamount { get; set; }
        public object pinterestpayable { get; set; }
        public object pdepositedate { get; set; }
        public object pmaturitydate { get; set; }

        public object pvoucherno { get; set; }
        public object ppaymentdate { get; set; }
        public object pmodeofpayment { get; set; }
        public object ppaybankname { get; set; }
        public object ppaybankbranchname { get; set; }
        public object ppaychequeno { get; set; }

    }










}
