using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface ICommissionPayment
    {
        List<CommissionPaymentDTO> GetAgentDetails(string Connectionstring);
        List<CommissionPaymentAgentViewDTO> GetAgentContactDetails(long agentid, string Connectionstring);

        List<CommissionPaymentDetailsDTO> ShowPromoterSalaryDetails(long agentid, string asondate, string Connectionstring);
        List<CommissionPaymentDetailsDTO> ShowPromoterSalaryReport(long agentid, string frommonthof, string tomonthof, string type, string pdatecheked, string Connectionstring);

        bool SaveCommisionPayment(CommissionPaymentDTO _CommissionPaymentDTO, string ConnectionString, out string Paymentid);

        List<CommissionPaymentDetailsDTO> GetViewCommisionpaymentdetails(string Connectionstring);
        List<AgentBankDetailsDTO> GetAgentBankDetails(long agentid, string Connectionstring);
    }
}
