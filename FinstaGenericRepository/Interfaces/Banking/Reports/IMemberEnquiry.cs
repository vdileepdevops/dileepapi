using FinstaInfrastructure.Banking.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
namespace FinstaRepository.Interfaces.Banking.Reports
{
    public interface IMemberEnquiry
    {
        List<MemberDetailsDTO> GetMemberDetails(string Connectionstring);
        Task<List<MemberTransactionDTO>> GetMemberTransactions(long Memberid, string Connectionstring);
        List<MemberReceiptDTO> GetMemberReceiptDetails(string FdAccountNo, string Connectionstring);
        List<MemberNomineeDetailsDTO> GetMemberNomineeDetails(string FdAccountNo, string Connectionstring);
        List<MemberInterestPaymentDTO> GetMemberInterestPaymentDetails(string FdAccountNo, string Connectionstring);
        List<MemberPromoterSalaryDTO> GetMemberPromoterSalaryDetails(string FdAccountNo, string Connectionstring);
        List<MemberLiensDTO> GetMemberLiensDetails(string FdAccountNo, string Connectionstring);
        List<MemberMaturityBondDTO> GetMemberMaturityBondsDetails(string FdAccountNo, string Connectionstring);
        List<MaturityPaymentsDTO> GetMemberMaturityPaymentsDetails(string FdAccountNo, string Connectionstring);


        #region Member Enquiry Details Report ...
        List<MemberEnquiryDTO> GetMemberEnquiryDetailsReport(string FdAccountNo, string ConnectionString);
        #endregion
    }
}
