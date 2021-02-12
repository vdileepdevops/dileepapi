using System;
using System.Collections.Generic;
using System.Text;

namespace FinstaInfrastructure.Banking.Transactions
{
    public class CommissionPaymentDTO :CommonDTO
    {
        
        public string ppaymentid { get; set; }
        public string pCommissionpaymentDate { get; set; }
        
        public object pagentid { get; set; }
        public string pagentname { get; set; }

        public object asondate { get; set; }

        public string psurname { get; set; }
        public long pcontactid { get; set; }
        public string pdob { get; set; }
        public string pgender { get; set; }
        public long pmobileno { get; set; }

        public object pTotalpaymentamount { get; set; }
        public string pNarration { get; set; }

        public string pmodofPayment { get; set; }
        public string ptranstype { get; set; }
        public object pintrestpaidmonth { get; set; }
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
        public object pfinancialservice { get; set; }
        public object preferencenodcard { get; set; }


        public object pBankbranchname { get; set; }
        public object pChequeno { get; set; }

        
        public object pReferenceno { get; set; }
        public object pUpiId { get; set; }
        public object pDebitcard { get; set; }
        public object pFinancialservice { get; set; }
        public List<CommissionPaymentDetailsDTO> pcommissionpaymentlist { set; get; }
        public List<CommissionPaymentAgentViewDTO> pagentlist { set; get; }

    }
    public class CommissionPaymentAgentViewDTO
    {
        public object pagentname { get; set; }
        public object pmembername { get; set; }
        public object pfdaccountno { get; set; }
        public object psalespersonname { get; set; }
        public object pcommsssionvalue { get; set; }
        public object pdebitaccountid { get; set; }
        public object ptransdate { get; set; }
        public object pfdname { get; set; }
        public object ptenortype { get; set; }
        public object ptenor { get; set; }
        public object pdepositamount { get; set; }
        public object pinteresttype { get; set; }
        public object pinterestrate { get; set; }
        public object pmaturityamount { get; set; }
        public object pinterestpayable { get; set; }
        public object pdepositdate { get; set; }
        public object pmaturitydate { get; set; }

    }
    public class CommissionPaymentSaveDTO
    {
        public object pvoucherid { get; set; }
    }
    public class CommissionPaymentDetailsDTO
    {
        public object pPaymentstatus { get; set; }
        public object pdebitaccountid { get; set; }
        public long pAgentid { get; set; }
        public string pAgentname { get; set; }
        public string pSchemeAccountno { get; set; }
        public string pMembername { get; set; }
        public long pcommissionaymentid { get; set; }
        public object pDepositamount { get; set; }
        public object pMaturityamount { get; set; }
        public object pInterestrate { get; set; }
        public object pCommissionamount { get; set; }
        public object pTdsamount { get; set; }
        public object ptotalamount { get; set; }

        public object ptransdate { get; set; }

        public object pTenor { get; set; }
        public object pCommissionvalue { get; set; }


        public object pvoucherno { get; set; }
        public object ppaymentdate { get; set; }
        public object pmodeofpayment { get; set; }
        public object ppaybankname { get; set; }
        public object ppaybankbranchname { get; set; }
        public object ppaychequeno { get; set; }
        public object pbankingtranstype { get; set; }
    }
    public class AgentBankDetailsDTO
    {
        public object pAgentName { get; set; }
        public object pBankName { get; set; }
        public object pBankAccountNo { get; set; }
        public object pBankIfsccode { get; set; }
        public object pBankBranch { get; set; }
        
    }
}
