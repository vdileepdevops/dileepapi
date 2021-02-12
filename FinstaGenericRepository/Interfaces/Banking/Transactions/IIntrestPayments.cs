using System;
using System.Collections.Generic;
using System.Text;
using FinstaInfrastructure.Banking.Transactions;

namespace FinstaRepository.Interfaces.Banking.Transactions
{
    public interface IIntrestPayments
    {
        List<IntrestPaymentDTO> GetSchemename(string Connectionstring);
        List<IntrestPaymentDTO> GetCompany(string Connectionstring);
        List<IntrestPaymentDTO> GetBranchName(string companyname,string Connectionstring);

      
        List<IntrestPaymentDetailsDTO> GetMemberPaymenthistory(long schemeid,string paymenttype, string companyname, string branchname,string forthemonth, string Connectionstring);

        List<IntrestPaymentDetailsDTO> GetShowInterestPaymentReport(long schemeid, string fdaccountno, string paymenttype, string companyname, string branchname, string  pdatecheked, string frommonthof, string tomonthof, string type, string Connectionstring);

        List<IntrestPaymentDetailsDTO> GetShowInterestpaymentdetailsforview(string Connectionstring);

        bool SaveInterestPayment(IntrestPaymentDTO _IntrestPaymentDTO, string ConnectionString, out string Paymentid);

        bool RunInterestPaymentFunction(string ConnectionString);

    }
}
